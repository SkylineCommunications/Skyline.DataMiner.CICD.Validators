namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Actions.Action.CheckActionTypes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;


    [Test(CheckId.CheckActionTypes, Category.Action)]
    internal class CheckActionTypes : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var action in context.EachActionWithValidId())
            {
                if (action.Type?.Value == null || action.On?.Value == null)
                {
                    // Invalid Type handled by check on Action/Type
                    // Invalid On handled by check on Action/On
                    continue;
                }

                var typeValue = action.Type.Value.Value;
                var onValue = action.On.Value.Value;

                ValidateHelper helper = new ValidateHelper(this, context, results, action, typeValue, onValue);
                helper.Validate();
            }

            return results;
        }

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {

        ////        default:
        ////            result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
        ////            break;
        ////    }

        ////    return result;
        ////}

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly ValidatorContext context;
        private readonly IProtocolModel model;
        private readonly RelationManager relationManager;
        private readonly List<IValidationResult> results;

        private readonly IActionsAction action;
        private readonly EnumActionType typeValue;
        private readonly EnumActionOn onValue;

        // Action/On attributes
        private bool isAttributeOnIdAllowed;
        private bool isAttributeOnNrAllowed;

        // Action/Type attributes
        private bool isAttributeTypeAllowedAllowed;
        private bool isAttributeTypeArgumentsAllowed;
        private bool isAttributeTypeEndOffsetAllowed;
        private bool isAttributeTypeIdAllowed;
        private bool isAttributeTypeNrAllowed;
        private bool isAttributeTypeOptionsAllowed;
        private bool isAttributeTypeRegexAllowed;
        private bool isAttributeTypeRescheduleAllowed;
        private bool isAttributeTypeReturnValueAllowed;
        private bool isAttributeTypeScaleAllowed;
        private bool isAttributeTypeScriptAllowed;
        private bool isAttributeTypeSequenceAllowed;
        private bool isAttributeTypeStartOffsetAllowed;
        private bool isAttributeTypeValueAllowed;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IActionsAction action, EnumActionType typeValue, EnumActionOn onValue)
        {
            this.test = test;
            this.context = context;
            this.model = context.ProtocolModel;
            this.relationManager = model.RelationManager;
            this.results = results;

            this.action = action;
            this.typeValue = typeValue;
            this.onValue = onValue;
        }

        public void Validate()
        {
            switch (onValue)
            {
                case EnumActionOn.Command:
                    ValidateOnCommand();
                    break;
                case EnumActionOn.Group:
                    ValidateOnGroup();
                    break;
                case EnumActionOn.Pair:
                    ValidateOnPair();
                    break;
                case EnumActionOn.Parameter:
                    ValidateOnParam();
                    break;
                case EnumActionOn.Protocol:
                    ValidateOnProtocol();
                    break;
                case EnumActionOn.Response:
                    ValidateOnResponse();
                    break;
                case EnumActionOn.Timer:
                    ValidateOnTimer();
                    break;
                default:
                    return;
            }

            ValidateUnsupportedAttribute();
        }

        private void ValidateOnCommand()
        {
            switch (typeValue)
            {
                case EnumActionType.Crc:
                    break;
                case EnumActionType.Length:
                    break;
                case EnumActionType.Make:
                    break;
                case EnumActionType.Replace:
                    break;
                case EnumActionType.ReplaceData:
                    break;
                case EnumActionType.Stuffing:
                    break;
                default:
                    results.Add(Error.IncompatibleTypeVsOnTag(test, action, action, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
                    return;
            }
        }

        #region Validate On Group
        private void ValidateOnGroup()
        {
            switch (typeValue)
            {
                case EnumActionType.AddToExecute:
                case EnumActionType.Execute:
                case EnumActionType.ExecuteNext:
                case EnumActionType.ExecuteOne:
                case EnumActionType.ExecuteOneTop:
                case EnumActionType.ExecuteOneNow:
                    // Nothing more to be checked here.
                    break;
                case EnumActionType.ForceExecute:
                    /*
                     * TODO: More checks required on 'force execute' actions ?
                     * - We may need to check that the action is triggered by a before/after command/response trigger
                     * - We may need to check that we force execute groups of type pair
                     *  ==> but we are unsure about the above 2 so will be for later on.
                     *  The above rules have been said/heard multiple times in the past.
                     *  However, as far as we can now see in the code & by testing, 'force execute' seem to work no matter what, meaning none of the below commented out checks are necessary.
                     * */

                    ////// Triggers
                    ////foreach (var trigger in action.GetTriggeringTriggers(relationManager))
                    ////{
                    ////    if (!(trigger.On?.Value == EnumTriggerOn.Command && trigger.Time?.Value == "after")
                    ////        && !(trigger.On?.Value == EnumTriggerOn.Response && trigger.Time?.Value == "before"))
                    ////    {
                    ////        // Should such action only be triggered afterCommand or beforeResponse ?
                    ////    }
                    ////}

                    ////// Groups
                    ////if (!String.IsNullOrWhiteSpace(action.On.Id?.Value))
                    ////{
                    ////    string[] onGroupIds = action.On.Id.Value.Split(';');
                    ////    foreach (var groupId in onGroupIds)
                    ////    {
                    ////        if (!model.TryGetObjectByKey<IGroupsGroup>(Mappings.GroupsById, groupId, out var group))
                    ////        {
                    ////            // NonExistingGroup already covered by 'Action/On@id' check.
                    ////            continue;
                    ////        }

                    ////        if (!group.GetContentPairs(relationManager).Any())
                    ////        {
                    ////            // Should only be on group polling pairs ?
                    ////        }
                    ////    }
                    ////}
                    break;
                case EnumActionType.Set:
                case EnumActionType.SetWithWait:
                    ValidateOnGroupSet();
                    break;
                default:
                    results.Add(Error.IncompatibleTypeVsOnTag(test, action, action, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
                    return;
            }

            /* Add here all checks that should apply no matter the (valid for timers) Action/Type */

            // Action/On@id attribute
            isAttributeOnIdAllowed = true;
            if (action.On.Id == null)
            {
                results.Add(Error.MissingOnIdAttribute(test, action, action.On, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
            }
        }

        private void ValidateOnGroupSet()
        {
            ValidateOnGroupSet_Group();
            ValidateOnGroupSet_Connection();
        }

        private void ValidateOnGroupSet_Group()
        {
            // Action/On@id
            isAttributeOnIdAllowed = true;
            if (String.IsNullOrWhiteSpace(action.On.Id?.Value))
            {
                // Missing or empty value already covered by 'Action/On@id' check.
                return;
            }

            string[] onGroupIds = action.On.Id.Value.Split(';');
            foreach (var groupId in onGroupIds)
            {
                // Sanity checks
                if (!model.TryGetObjectByKey<IGroupsGroup>(Mappings.GroupsById, groupId, out var group))
                {
                    // NonExistingGroup already covered by 'Action/On@id' check.
                    continue;
                }

                // Validate Params
                var parameters = group.GetContentParameters(relationManager).ToArray();
                foreach (var param in parameters)
                {
                    if (param.Type?.Value != EnumParamType.Write)
                    {
                        results.Add(Error.UnsupportedGroupParamType(test, param, param, action.Type.RawValue, action.On.RawValue, groupId, param.Id.RawValue, param.Type?.RawValue, action.Id.RawValue));
                    }

                    if (param.SNMP?.Enabled?.Value != true)
                    {
                        results.Add(Error.UnsupportedGroupParamWithoutSnmp(test, param, param, action.Type.RawValue, action.On.RawValue, groupId, param.Id.RawValue, param.SNMP?.Enabled?.RawValue, action.Id.RawValue));
                    }
                }

                // Validate Group
                if (!parameters.Any())
                {
                    results.Add(Error.UnsupportedGroupContentDueTo(test, action, action.On.Id, action.Type.RawValue, action.On.RawValue, groupId, action.Id.RawValue));
                }
            }
        }

        private void ValidateOnGroupSet_Connection()
        {
            uint connectionId = 0;
            string connectionIdString = "0";
            var connections = model.Protocol.GetConnections().ToArray();

            // Check if connection exists
            isAttributeTypeNrAllowed = true;
            bool isConnectionIdExplicit = action.Type.Nr?.Value != null;
            if (isConnectionIdExplicit)
            {
                connectionIdString = action.Type.Nr.Value;
                if (!UInt32.TryParse(connectionIdString, out connectionId) || connectionId >= connections.Length)
                {
                    results.Add(Error.NonExistingConnectionRefInTypeNrAttribute(test, action, action.Type.Nr, connectionIdString, action.Id.RawValue));
                    return;
                }
            }

            // Sanity check (will only serve when no connection at all cause other cases will be caught by the above break already)
            if (connectionId >= connections.Length)
            {
                return;
            }

            // Check if connection is an SNMP one
            var connectionType = connections[connectionId].Type;
            if (!IsSnmp(connectionType))
            {
                string connectionTypeString = EnumProtocolTypeConverter.ConvertBack(connectionType);
                string prefix = isConnectionIdExplicit ? String.Empty : "Default value for ";
                results.Add(Error.UnsupportedConnectionTypeDueTo(test, action, action.Type, action.Type.RawValue, action.On.RawValue, connectionIdString, connectionTypeString, action.Id.RawValue, prefix));
            }

            bool IsSnmp(EnumProtocolType? type)
            {
                return type == EnumProtocolType.Snmp || type == EnumProtocolType.Snmpv2 || type == EnumProtocolType.Snmpv3;
            }
        }
        #endregion

        #region Validate On Pair
        private void ValidateOnPair()
        {
            switch (typeValue)
            {
                case EnumActionType.SetNext:
                    ValidateOnPairSetNext();
                    break;
                case EnumActionType.Timeout:
                    ValidateOnPairTimeout();
                    break;
                default:
                    results.Add(Error.IncompatibleTypeVsOnTag(test, action, action, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
                    return;
            }
        }

        private void ValidateOnPairSetNext()
        {
            // Find related pairs
            var triggeringItems = ValidateOnPairSetNextFindPairs().ToArray();

            // On@nr
            isAttributeOnNrAllowed = true;
            if (action.On.Nr == null)
            {
                results.Add(Error.MissingOnNrAttribute(test, action, action.On, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
            }
            else if (!String.IsNullOrWhiteSpace(action.On.Nr.Value))
            {
                string[] nrParts = action.On.Nr.Value.Split(';');
                foreach (var pairPositionString in nrParts)
                {
                    if (!triggeringItems.Any())
                    {
                        results.Add(Error.NonExistingRefToPairOnTimeoutSetNext(test, action, action.On.Nr, pairPositionString, "NoGroup", action.Id.RawValue, "NoTrigger"));
                        continue;
                    }

                    bool isValidPairPosition = UInt32.TryParse(pairPositionString, out uint pairPosition);
                    foreach ((ITriggersTrigger trigger, IGroupsGroup group, IPairsPair[] groupPairs) in triggeringItems)
                    {
                        if (group == null)
                        {
                            results.Add(Error.NonExistingRefToPairOnTimeoutSetNext(test, action, action.On.Nr, pairPositionString, "NoGroup", action.Id.RawValue, trigger.Id.RawValue));
                            continue;
                        }

                        // 1-based pair position in group
                        if (!isValidPairPosition || pairPosition < 1 || pairPosition > groupPairs.Length)
                        {
                            results.Add(Error.NonExistingRefToPairOnTimeoutSetNext(test, action, action.On.Nr, pairPositionString, group.Id.RawValue, action.Id.RawValue, trigger.Id.RawValue));
                        }
                    }
                }
            }
            else
            {
                // Empty attribute handled by specific check on the attribute.
            }

            // Type@id && Type@value
            isAttributeTypeIdAllowed = true;
            isAttributeTypeValueAllowed = true;
            if (action.Type.Id == null && action.Type.ValueAttribute == null)
            {
                results.Add(Error.MissingTypeIdOrTypeValueAttribute(test, action, action.Type, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
            }
            else if (action.Type.Id != null && action.Type.ValueAttribute != null)
            {
                results.Add(Error.ExcessiveTypeIdOrTypeValueAttribute(test, action, action.Type, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
            }
            else if (action.Type.Id != null)
            {
                string pid = Convert.ToString(action.Type.Id.Value);
                if (!model.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, pid, out _))
                {
                    results.Add(Error.NonExistingParamRefInTypeIdAttribute(test, action, action.Type.Id, pid, action.Id.RawValue));
                }
            }
            else if (action.Type.ValueAttribute != null)
            {
                // TODO: check if value is a positive number (since this attribute is also used in scope of stuffing where it then requires one of those ugly strings)
                // ToConsider: maybe we could change if the number is not too big ?
            }
        }

        private IEnumerable<(ITriggersTrigger trigger, IGroupsGroup group, IPairsPair[] groupPairs)> ValidateOnPairSetNextFindPairs()
        {
            // Find out the triggers -> groups -> pairs based on trigger
            foreach (var trigger in action.GetTriggeringTriggers(relationManager))
            {
                // Sanity checks
                if (trigger.On?.Value != EnumTriggerOn.Group || trigger.On.Id?.Value == null)
                {
                    yield return (trigger, null, null);
                    continue;
                }

                // Find group(s)
                IGroupsGroup[] groups;
                if (trigger.On.Id.Value == "each")
                {
                    groups = context.EachGroupWithValidId().ToArray();
                }
                else if (model.TryGetObjectByKey<IGroupsGroup>(Mappings.GroupsById, trigger.On.Id.Value, out var group))
                {
                    groups = new IGroupsGroup[] { group };
                }
                else
                {
                    // This will be covered by check on Trigger/On@id
                    continue;
                }

                // Find pair(s)
                foreach (var group in groups)
                {
                    var pairs = group.GetContentPairs(relationManager).ToArray();
                    yield return (trigger, group, pairs);
                }
            }
        }

        private void ValidateOnPairTimeout()
        {
            // On@id
            isAttributeOnIdAllowed = true;
            if (action.On.Id == null)
            {
                results.Add(Error.MissingOnIdAttribute(test, action, action.On, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
            }
            else
            {
                // Empty attribute handled by specific check on the attribute.
                // Ref to NonExisting pair also handled by specific check on the attribute.
            }

            // Type@id
            isAttributeTypeIdAllowed = true;
            if (action.Type.Id == null)
            {
                results.Add(Error.MissingTypeIdAttribute(test, action, action.On, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
            }
            else if (action.Type.Id.Value == null)
            {
                // InvalidValue already covered by check on the specific attribute.
            }
            else if (!model.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, Convert.ToString(action.Type.Id.Value), out _))
            {
                string pid = Convert.ToString(action.Type.Id.Value);
                if (!model.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, pid, out _))
                {
                    results.Add(Error.NonExistingParamRefInTypeIdAttribute(test, action, action.Type.Id, pid, action.Id.RawValue));
                }
            }
            else
            {
                // TODO: check if parameter is read && numerical ?
            }
        }
        #endregion

        #region Validate On Param
        private void ValidateOnParam()
        {
            switch (typeValue)
            {
                case EnumActionType.Aggregate:
                    break;
                case EnumActionType.Append:
                    break;
                case EnumActionType.AppendData:
                    break;
                case EnumActionType.ChangeLength:
                    break;
                case EnumActionType.Clear:
                    break;
                case EnumActionType.ClearOnDisplay:
                    break;
                case EnumActionType.Copy:
                    break;
                case EnumActionType.CopyReverse:
                    break;
                case EnumActionType.Go:
                    break;
                case EnumActionType.Increment:
                    break;
                case EnumActionType.Multiply:
                    break;
                case EnumActionType.Normalize:
                    break;
                case EnumActionType.Pow:
                    break;
                case EnumActionType.Read:
                    break;
                case EnumActionType.ReplaceData:
                    break;
                case EnumActionType.Reverse:
                    ValidateOnParamReverse();
                    break;
                case EnumActionType.RunActions:
                    break;
                case EnumActionType.Save:
                    break;
                case EnumActionType.Set:
                    break;
                case EnumActionType.SetAndGetWithWait:
                    break;
                case EnumActionType.SetInfo:
                    break;
                case EnumActionType.SetWithWait:
                    break;
                default:
                    results.Add(Error.IncompatibleTypeVsOnTag(test, action, action, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
                    return;
            }
        }

        private void ValidateOnParamReverse()
        {
            // On@nr
            isAttributeOnNrAllowed = true;
        }
        #endregion

        private void ValidateOnProtocol()
        {
            switch (typeValue)
            {
                case EnumActionType.Close:
                    break;
                case EnumActionType.Lock:
                    break;
                case EnumActionType.Unlock:
                    break;
                case EnumActionType.Merge:
                    break;
                case EnumActionType.Open:
                    break;
                case EnumActionType.PriorityLock:
                    break;
                case EnumActionType.PriorityUnlock:
                    break;
                case EnumActionType.ReadFile:
                    break;
                case EnumActionType.Sleep:
                    break;
                case EnumActionType.StopCurrentGroup:
                    break;
                case EnumActionType.SwapColumn:
                    break;
                case EnumActionType.Wmi:
                    break;
                default:
                    results.Add(Error.IncompatibleTypeVsOnTag(test, action, action, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
                    return;
            }
        }

        private void ValidateOnResponse()
        {
            switch (typeValue)
            {
                case EnumActionType.Clear:
                    break;
                case EnumActionType.ClearLengthInfo:
                    break;
                case EnumActionType.Crc:
                    break;
                case EnumActionType.Length:
                    break;
                case EnumActionType.Read:
                    break;
                case EnumActionType.ReadStuffing:
                    break;
                case EnumActionType.Replace:
                    break;
                case EnumActionType.ReplaceData:
                    break;
                case EnumActionType.Stuffing:
                    break;
                default:
                    results.Add(Error.IncompatibleTypeVsOnTag(test, action, action, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
                    return;
            }
        }

        private void ValidateOnTimer()
        {
            switch (typeValue)
            {
                case EnumActionType.Reschedule:
                case EnumActionType.Start:
                case EnumActionType.Stop:
                    // For Reschedule/Start/Stop: nothing more to be validated than the On@id presence which is validated no matter the type (so after this switch-case)
                    break;
                case EnumActionType.RestartTimer:
                    // Type@reschedule attribute is optional and fully validated by the model/xsd so nothing more to validate here.
                    break;
                default:
                    results.Add(Error.IncompatibleTypeVsOnTag(test, action, action, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
                    return;
            }

            /* Add here all checks that should apply no matter the (valid for timers) Action/Type */

            // Action/On@id attribute
            isAttributeOnIdAllowed = true;
            if (action.On.Id == null)
            {
                results.Add(Error.MissingOnIdAttribute(test, action, action.On, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
            }
        }

        #region Unsupported Attributes
        private void ValidateUnsupportedAttribute()
        {
            // Action/On tag
            ValidateUnsupportedAttributeOnId();
            ValidateUnsupportedAttributeOnNr();

            // Action/TypeTag
            ValidateUnsupportedAttributeTypeAllowed();
            ValidateUnsupportedAttributeTypeArguments();
            ValidateUnsupportedAttributeTypeEndOffset();
            ValidateUnsupportedAttributeTypeId();
            ValidateUnsupportedAttributeTypeNr();
            ValidateUnsupportedAttributeTypeOptions();
            ValidateUnsupportedAttributeTypeRegex();
            ValidateUnsupportedAttributeTypeReschedule();
            ValidateUnsupportedAttributeTypeReturnValue();
            ValidateUnsupportedAttributeTypeScale();
            ValidateUnsupportedAttributeTypeScript();
            ValidateUnsupportedAttributeTypeSequence();
            ValidateUnsupportedAttributeTypeStartOffset();
            ValidateUnsupportedAttributeTypeValue();
        }

        private void ValidateUnsupportedAttributeOnId()
        {
            if (isAttributeOnIdAllowed || action.On.Id == null)
            {
                return;
            }

            // TODO: results.Add(Error.UnsupportedAttributeOnId(test, action, action.On.Id, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
        }

        private void ValidateUnsupportedAttributeOnNr()
        {
            if (isAttributeOnNrAllowed || action.On.Nr == null)
            {
                return;
            }

            results.Add(Error.UnsupportedAttributeOnNr(test, action, action.On.Nr, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
        }

        private void ValidateUnsupportedAttributeTypeAllowed()
        {
            if (isAttributeTypeAllowedAllowed || action.Type.Allowed == null)
            {
                return;
            }

            // TODO: results.Add(Error.UnsupportedAttributeTypeAllowed(test, action, action.Type.Id, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
        }

        private void ValidateUnsupportedAttributeTypeArguments()
        {
            if (isAttributeTypeArgumentsAllowed || action.Type.Arguments == null)
            {
                return;
            }

            // TODO: results.Add(Error.UnsupportedAttributeTypeArguments(test, action, action.Type.Id, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
        }

        private void ValidateUnsupportedAttributeTypeEndOffset()
        {
            if (isAttributeTypeEndOffsetAllowed || action.Type.Endoffset == null)
            {
                return;
            }

            // TODO: results.Add(Error.UnsupportedAttributeTypeEndOffset(test, action, action.Type.Id, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
        }

        private void ValidateUnsupportedAttributeTypeId()
        {
            if (isAttributeTypeIdAllowed || action.Type.Id == null)
            {
                return;
            }

            // TODO: results.Add(Error.UnsupportedAttributeTypeId(test, action, action.Type.Id, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
        }

        private void ValidateUnsupportedAttributeTypeNr()
        {
            if (isAttributeTypeNrAllowed || action.Type.Nr == null)
            {
                return;
            }

            // TODO: results.Add(Error.UnsupportedAttributeTypeNr(test, action, action.Type.Value, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
        }
        private void ValidateUnsupportedAttributeTypeOptions()
        {
            if (isAttributeTypeOptionsAllowed || action.Type.Options == null)
            {
                return;
            }

            // TODO: results.Add(Error.UnsupportedAttributeTypeOptions(test, action, action.Type.Id, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
        }

        private void ValidateUnsupportedAttributeTypeRegex()
        {
            if (isAttributeTypeRegexAllowed || action.Type.Regex == null)
            {
                return;
            }

            // TODO: results.Add(Error.UnsupportedAttributeTypeRegex(test, action, action.Type.Id, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
        }

        private void ValidateUnsupportedAttributeTypeReschedule()
        {
            if (isAttributeTypeRescheduleAllowed || action.Type.Reschedule == null)
            {
                return;
            }

            // TODO: results.Add(Error.UnsupportedAttributeTypeReschedule(test, action, action.Type.Id, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
        }

        private void ValidateUnsupportedAttributeTypeReturnValue()
        {
            if (isAttributeTypeReturnValueAllowed || action.Type.ReturnValue == null)
            {
                return;
            }

            // TODO: results.Add(Error.UnsupportedAttributeTypeReturnValue(test, action, action.Type.Id, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
        }

        private void ValidateUnsupportedAttributeTypeScale()
        {
            if (isAttributeTypeScaleAllowed || action.Type.Scale == null)
            {
                return;
            }

            // TODO: results.Add(Error.UnsupportedAttributeTypeScale(test, action, action.Type.Id, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
        }

        private void ValidateUnsupportedAttributeTypeScript()
        {
            if (isAttributeTypeScriptAllowed || action.Type.Script == null)
            {
                return;
            }

            // TODO: results.Add(Error.UnsupportedAttributeTypeScript(test, action, action.Type.Id, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
        }

        private void ValidateUnsupportedAttributeTypeSequence()
        {
            if (isAttributeTypeSequenceAllowed || action.Type.Sequence == null)
            {
                return;
            }

            // TODO: results.Add(Error.UnsupportedAttributeTypeSequence(test, action, action.Type.Id, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
        }

        private void ValidateUnsupportedAttributeTypeStartOffset()
        {
            if (isAttributeTypeStartOffsetAllowed || action.Type.Startoffset == null)
            {
                return;
            }

            // TODO: results.Add(Error.UnsupportedAttributeTypeStartOffset(test, action, action.Type.Id, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
        }


        private void ValidateUnsupportedAttributeTypeValue()
        {
            if (isAttributeTypeValueAllowed || action.Type.ValueAttribute == null)
            {
                return;
            }

            // TODO: results.Add(Error.UnsupportedAttributeTypeValue(test, action, action.Type.Value, action.Type.RawValue, action.On.RawValue, action.Id.RawValue));
        }
        #endregion
    }
}