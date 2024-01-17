namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Name.CheckNameTag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckNameTag, Category.Param)]
    internal class CheckNameTag : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            Dictionary<string, List<IParamsParam>> paramsPerNames = new Dictionary<string, List<IParamsParam>>(StringComparer.OrdinalIgnoreCase);
            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                var paramName = param.Name;
                (GenericStatus status, _, string nameValue) = GenericTests.CheckBasics(paramName, isRequired: true);

                // Missing
                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingTag(this, param, paramName, param.Id.RawValue));
                    continue;
                }

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyTag(this, param, paramName, param.Id.RawValue));
                    continue;
                }

                // Reserved Names
                if (ParamHelper.IsRestrictedParamName(param))
                {
                    results.Add(Error.RestrictedName(this, param, paramName, param.Id.RawValue, paramName.RawValue));
                    continue;
                }

                // Invalid Chars
                char[] forbiddenChars = Helper.CheckInvalidChars(nameValue, ParamHelper.RestrictedParamNameChars).ToArray();
                if (forbiddenChars.Length > 0)
                {
                    results.Add(Error.InvalidChars(this, param, paramName, paramName.RawValue, String.Join(" ", forbiddenChars)));
                    continue;
                }

                // RTDisplay Required
                if (param.IsContextMenu())
                {
                    IValidationResult rtDisplayError = Error.RTDisplayExpectedOnContextMenu(this, param, paramName, param.Id.RawValue);
                    context.CrossData.RtDisplay.AddParam(param, rtDisplayError);
                }
                else if (param.IsQActionFeedback())
                {
                    IValidationResult rtDisplayError = Error.RTDisplayExpectedOnQActionFeedback(this, param, paramName, param.Id.RawValue);
                    context.CrossData.RtDisplay.AddParam(param, rtDisplayError);
                }

                // Duplicate Preparations
                if (!paramsPerNames.ContainsKey(nameValue))
                {
                    paramsPerNames.Add(nameValue, new List<IParamsParam>());
                }

                paramsPerNames[nameValue].Add(param);

                // Unrecommended Leading Chars
                char[] unrecommendedStartChars = ParamHelper.GetParamNameUnrecommendedStartChars(nameValue).ToArray();
                if (unrecommendedStartChars.Length > 0)
                {
                    results.Add(Error.UnrecommendedStartChars(this, param, paramName, paramName.TagName, paramName.RawValue, String.Join(", ", unrecommendedStartChars)));
                    continue;
                }

                // Unrecommended Chars
                object[] unrecommendedChars = ParamHelper.GetParamNameUnrecommendedChars(nameValue).ToArray();
                if (unrecommendedChars.Length > 0)
                {
                    results.Add(Error.UnrecommendedChars(this, param, paramName, paramName.TagName, paramName.RawValue, String.Join(" ", unrecommendedChars)));
                    continue;
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedTag(this, param, paramName, param.Id.RawValue, paramName.RawValue));
                    continue;
                }
            }

            // Duplicate Names
            foreach (var kvp in paramsPerNames)
            {
                string paramName = kvp.Key;
                List<IParamsParam> sameNameParams = kvp.Value;

                if (ParamHelper.IsValidParamAssociation(sameNameParams))
                {
                    continue;
                }

                // Generate Error (More than 2 params or wrong combination of 2 params)
                IValidationResult duplicatedValue = Error.DuplicatedValue(this, null, null, paramName, String.Join(", ", sameNameParams.Select(x => x.Id?.RawValue)));
                for (int i = 0; i < sameNameParams.Count; i++)
                {
                    IValidationResult subResult = Error.DuplicatedValue(this, sameNameParams[i], sameNameParams[i].Name, sameNameParams[i].Name.RawValue, sameNameParams[i].Id?.RawValue);
                    duplicatedValue.WithSubResults(subResult);
                }

                results.Add(duplicatedValue);
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol?.Params == null)
            {
                result.Message = "No Params found!";
                return result;
            }

            var readNode = (IParamsParam)context.Result.ReferenceNode;
            var editNode = context.Protocol.Params.Get(readNode);

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedTag:
                    editNode.Name.Value = readNode.Name.Value.Trim();
                    result.Success = true;
                    break;

                case ErrorIds.InvalidChars:
                    editNode.Name.Value = ParamHelper.ReplaceParamNameInvalidChars(readNode.Name.RawValue);
                    result.Success = true;
                    break;

                case ErrorIds.UnrecommendedChars:
                    editNode.Name.Value = ParamHelper.ReplaceParamNameUnrecommendedChars(readNode.Name.RawValue);
                    result.Success = true;
                    break;

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var relations = context.NewProtocolModel.RelationManager;
            foreach ((IParamsParam oldParam, IParamsParam newParam) in context.EachMatchingParam())
            {
                string newPid = newParam?.Id?.RawValue;

                if (newParam == null)
                {
                    continue;
                }

                if (!newParam.TryGetTable(relations, out IParamsParam table))
                {
                    continue;
                }

                uint? tablePid = table?.Id?.Value;
                if (tablePid == null)
                {
                    continue;
                }

                if (table.IsLoggerTable())
                {
                    var oldName = oldParam?.Name?.Value;
                    var newName = newParam?.Name?.Value;
                    if (!String.Equals(oldName, newName, StringComparison.Ordinal))
                    {
                        results.Add(ErrorCompare.LoggerTableColumnNameChanged(newParam, newParam, oldName, newPid, table.Id?.RawValue, newName));
                    }
                }
            }

            return results;
        }
    }
}