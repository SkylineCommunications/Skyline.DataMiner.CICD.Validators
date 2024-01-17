namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Actions.Action.On.CheckIdAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckIdAttribute, Category.Action)]
    internal class CheckIdAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var action in context.EachActionWithValidId())
            {
                if (action.On?.Id == null)
                {
                    continue;
                }

                (GenericStatus status, string rawValue, _) = GenericTests.CheckBasics(action.On.Id, isRequired: false);

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttibute(this, action.On.Id, action.On.Id, action.Id.RawValue));
                    continue;
                }

                List<IValidationResult> invalidResults = new List<IValidationResult>();
                List<IValidationResult> untrimmedResults = new List<IValidationResult>();

                string[] onIds = rawValue.Split(';');
                foreach (string onId in onIds)
                {
                    string onIdTrimmed = onId.Trim();

                    // Invalid
                    if (!UInt32.TryParse(onIdTrimmed, out _) || !GenericTests.IsPlainNumbers(onIdTrimmed))
                    {
                        invalidResults.Add(Error.InvalidValue(this, action.On.Id, action.On.Id, onId, action.Id.RawValue));
                        continue;
                    }

                    // Non-Existing Item
                    switch (action.On.Value)
                    {
                        case EnumActionOn.Command:
                            if (!context.ProtocolModel.TryGetObjectByKey<ICommandsCommand>(Mappings.CommandsById, onIdTrimmed, out _))
                            {
                                results.Add(Error.NonExistingId(this, action.On.Id, action.On.Id, "Command", onId, action.Id.RawValue));
                                continue;
                            }
                            break;
                        case EnumActionOn.Group:
                            if (!context.ProtocolModel.TryGetObjectByKey<IGroupsGroup>(Mappings.GroupsById, onIdTrimmed, out _))
                            {
                                results.Add(Error.NonExistingId(this, action.On.Id, action.On.Id, "Group", onId, action.Id.RawValue));
                                continue;
                            }
                            break;
                        case EnumActionOn.Pair:
                            if (!context.ProtocolModel.TryGetObjectByKey<IPairsPair>(Mappings.PairsById, onIdTrimmed, out _))
                            {
                                results.Add(Error.NonExistingId(this, action.On.Id, action.On.Id, "Pair", onId, action.Id.RawValue));
                                continue;
                            }
                            break;
                        case EnumActionOn.Parameter:
                            if (!context.ProtocolModel.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, onIdTrimmed, out _))
                            {
                                results.Add(Error.NonExistingId(this, action.On.Id, action.On.Id, "Param", onId, action.Id.RawValue));
                                continue;
                            }
                            break;
                        case EnumActionOn.Response:
                            if (!context.ProtocolModel.TryGetObjectByKey<IResponsesResponse>(Mappings.ResponsesById, onIdTrimmed, out _))
                            {
                                results.Add(Error.NonExistingId(this, action.On.Id, action.On.Id, "Response", onId, action.Id.RawValue));
                                continue;
                            }
                            break;
                        case EnumActionOn.Timer:
                            if (!context.ProtocolModel.TryGetObjectByKey<ITimersTimer>(Mappings.TimersById, onIdTrimmed, out _))
                            {
                                results.Add(Error.NonExistingId(this, action.On.Id, action.On.Id, "Timer", onId, action.Id.RawValue));
                                continue;
                            }
                            break;
                    }

                    // Untrimmed (not via status bit-flag because this is checked on id level and NOT on attribute level)
                    if (onId != onIdTrimmed)
                    {
                        untrimmedResults.Add(Error.UntrimmedValueInAttribute(this, action, action.On.Id, onId, action.Id.RawValue));
                        continue;
                    }
                }

                Helper.AddResultOrGroupedResults(results, invalidResults, generateSummaryResult: x => Error.InvalidValue(this, action.On.Id, action.On.Id, rawValue, action.Id.RawValue).WithSubResults(x));

                Helper.AddResultOrGroupedResults(results, untrimmedResults, generateSummaryResult: x => Error.UntrimmedValueInAttribute(this, action, action.On.Id, rawValue, action.Id.RawValue).WithSubResults(x));
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedValueInAttribute:

                    if (!(context.Result.ReferenceNode is IActionsAction readAction))
                    {
                        result.Message = "context.Result.Node is not of type IActionsAction.";
                        break;
                    }

                    var editAction = context.Protocol.Actions.Get(readAction);
                    if (editAction == null)
                    {
                        result.Message = "Could not find matching action in edit model.";
                        break;
                    }

                    string onId = readAction.On?.Id?.RawValue;
                    if (onId == null)
                    {
                        result.Message = "readAction.On?.Id?.RawValue == null";
                        break;
                    }

                    string[] onIds = onId.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < onIds.Length; i++)
                    {
                        onIds[i] = onIds[i].Trim();
                    }

                    editAction.On.Id.Value = String.Join(";", onIds);
                    result.Success = true;

                    break;
                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }
}