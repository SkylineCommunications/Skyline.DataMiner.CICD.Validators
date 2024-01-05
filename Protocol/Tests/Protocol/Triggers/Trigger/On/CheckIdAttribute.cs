namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Triggers.Trigger.On.CheckIdAttribute
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckIdAttribute, Category.Trigger)]
    internal class CheckIdAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (ITriggersTrigger trigger in context.EachTriggerWithValidId())
            {
                if (trigger.On?.Value == null)
                {
                    // Missing On || Unable to parse on value
                    continue;
                }

                bool isIdRequired;
                switch (trigger.On.Value)
                {
                    case EnumTriggerOn.Communication:
                    case EnumTriggerOn.Protocol:
                        isIdRequired = false;
                        break;

                    default:
                        isIdRequired = true;
                        break;
                }

                (GenericStatus status, string rawValue, string value) = Generic.GenericTests.CheckBasics(trigger.On.Id, isIdRequired);

                // Excessive
                if (!isIdRequired)
                {
                    if (!String.IsNullOrWhiteSpace(value))
                    {
                        string enumValue = EnumTriggerOnConverter.ConvertBack(trigger.On.Value.Value);
                        results.Add(Error.ExcessiveAttribute(this, trigger, trigger.On.Id, enumValue, trigger.Id.RawValue));
                    }

                    // Nothing needs to be checked if it isn't there.
                    continue;
                }

                // Missing
                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingAttribute(this, trigger, trigger.On, trigger.Id.RawValue));
                    continue;
                }

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, trigger, trigger.On.Id));
                    continue;
                }

                // Invalid
                if (!String.Equals(value, "0") && Helper.HasLeadingChar(value, '0'))
                {
                    results.Add(Error.LeadingZeros(this, trigger, trigger.On.Id, trigger.Id.RawValue, rawValue));
                    continue;
                }

                if (value.Split(';').Length > 1)
                {
                    results.Add(Error.MultipleIds(this, trigger, trigger.On.Id, trigger.Id.RawValue));
                    continue;
                }

                // Referenced Item
                if (String.Equals(value, "each", StringComparison.OrdinalIgnoreCase))
                {
                    bool hasAnyItem = false;
                    switch (trigger.On.Value)
                    {
                        case EnumTriggerOn.Command:
                            hasAnyItem = context.ProtocolModel.Protocol.Commands?.Count > 0;
                            break;

                        case EnumTriggerOn.Group:
                            hasAnyItem = context.ProtocolModel.Protocol.Groups?.Count > 0;
                            break;

                        case EnumTriggerOn.Pair:
                            hasAnyItem = context.ProtocolModel.Protocol.Pairs?.Count > 0;
                            break;

                        case EnumTriggerOn.Parameter:
                            hasAnyItem = context.ProtocolModel.Protocol.Params?.Count > 0;
                            break;

                        case EnumTriggerOn.Response:
                            hasAnyItem = context.ProtocolModel.Protocol.Responses?.Count > 0;
                            break;

                        case EnumTriggerOn.Session:
                            hasAnyItem = context.ProtocolModel.Protocol.HTTP?.Count > 0;
                            break;

                        case EnumTriggerOn.Timer:
                            hasAnyItem = context.ProtocolModel.Protocol.Timers?.Count > 0;
                            break;
                    }

                    if (!hasAnyItem)
                    {
                        string onTagValue = EnumTriggerOnConverter.ConvertBack(trigger.On.Value.Value);
                        results.Add(Error.NonExistingId(this, trigger, trigger.On.Id, onTagValue, rawValue, trigger.Id.RawValue));
                    }

                    continue;
                }

                (GenericStatus valueStatus, uint convertedValue) = Generic.GenericTests.CheckBasics<uint>(value);

                if (valueStatus.HasFlag(GenericStatus.Invalid))
                {
                    results.Add(Error.InvalidValue(this, trigger, trigger.Id, rawValue, "each, {uint}"));
                    continue;
                }

                bool success = false;
                bool isGeneralParam = false;
                switch (trigger.On.Value)
                {
                    case EnumTriggerOn.Command:
                        success = context.ProtocolModel.TryGetObjectByKey<ICommandsCommand>(Mappings.CommandsById, value, out _);
                        break;

                    case EnumTriggerOn.Group:
                        success = context.ProtocolModel.TryGetObjectByKey<IGroupsGroup>(Mappings.GroupsById, value, out _);
                        break;

                    case EnumTriggerOn.Pair:
                        success = context.ProtocolModel.TryGetObjectByKey<IPairsPair>(Mappings.PairsById, value, out _);
                        break;

                    case EnumTriggerOn.Parameter:
                        success = context.ProtocolModel.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, value, out _);
                        isGeneralParam = ParamHelper.IsGeneralParam(convertedValue);
                        break;

                    case EnumTriggerOn.Response:
                        success = context.ProtocolModel.TryGetObjectByKey<IResponsesResponse>(Mappings.ResponsesById, value, out _);
                        break;

                    case EnumTriggerOn.Session:
                        success = context.ProtocolModel.TryGetObjectByKey<IHTTPSession>(Mappings.SessionsById, value, out _);
                        break;

                    case EnumTriggerOn.Timer:
                        success = context.ProtocolModel.TryGetObjectByKey<ITimersTimer>(Mappings.TimersById, value, out _);
                        break;
                }

                if (!success && !isGeneralParam)
                {
                    string onTagValue = EnumTriggerOnConverter.ConvertBack(trigger.On.Value.Value);
                    results.Add(Error.NonExistingId(this, trigger, trigger.On.Id, onTagValue, rawValue, trigger.Id.RawValue));
                    continue;
                }

                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedAttribute(this, trigger, trigger.On.Id, rawValue));
                    continue;
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol?.Triggers == null)
            {
                result.Message = "No Triggers tag found!";
                return result;
            }

            var readNode = (ITriggersTrigger)context.Result.ReferenceNode;
            var editNode = context.Protocol.Triggers.Get(readNode);

            switch (context.Result.ErrorId)
            {
                case ErrorIds.ExcessiveAttribute:
                    editNode.On.Id = null;
                    result.Success = true;
                    break;

                case ErrorIds.UntrimmedAttribute:
                    editNode.On.Id.Value = readNode.On.Id.Value;
                    result.Success = true;
                    break;

                case ErrorIds.LeadingZeros:
                    editNode.On.Id.Value = readNode.On.Id.Value.TrimStart('0');
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