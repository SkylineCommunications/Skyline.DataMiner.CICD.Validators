namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Actions.Action.CheckActionTypes
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    internal static class Error
    {
        public static IValidationResult IncompatibleTypeVsOnTag(IValidate test, IReadable referenceNode, IReadable positionNode, string actionType, string actionOn, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckActionTypes,
                ErrorId = ErrorIds.IncompatibleTypeVsOnTag,
                FullId = "6.7.1",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Incompatible '{0}' value '{1}' with '{2}' value '{3}'. {4} {5} '{6}'.", "Action/Type", actionType, "Action/On", actionOn, "Action", "ID", actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "Only the following combination of 'Action/On' and 'Action/Type' are supported." + Environment.NewLine + "- command" + Environment.NewLine + "    - crc" + Environment.NewLine + "    - length" + Environment.NewLine + "    - make" + Environment.NewLine + "    - replace" + Environment.NewLine + "    - replace data" + Environment.NewLine + "    - stuffing" + Environment.NewLine + "- group" + Environment.NewLine + "    - add to execute" + Environment.NewLine + "    - execute" + Environment.NewLine + "    - execute next" + Environment.NewLine + "    - execute one" + Environment.NewLine + "    - execute one top" + Environment.NewLine + "    - execute one now" + Environment.NewLine + "    - force execute" + Environment.NewLine + "    - set" + Environment.NewLine + "    - set with wait" + Environment.NewLine + "- pair" + Environment.NewLine + "    - set next" + Environment.NewLine + "    - timeout" + Environment.NewLine + "- parameter" + Environment.NewLine + "    - aggregate" + Environment.NewLine + "    - append" + Environment.NewLine + "    - append data" + Environment.NewLine + "    - change lenght" + Environment.NewLine + "    - clear" + Environment.NewLine + "    - clear on display" + Environment.NewLine + "    - copy" + Environment.NewLine + "    - copy reverse" + Environment.NewLine + "    - go" + Environment.NewLine + "    - increment" + Environment.NewLine + "    - multiply" + Environment.NewLine + "    - normalize" + Environment.NewLine + "    - pow" + Environment.NewLine + "    - read" + Environment.NewLine + "    - replace data" + Environment.NewLine + "    - reverse" + Environment.NewLine + "    - run actions" + Environment.NewLine + "    - save" + Environment.NewLine + "    - set" + Environment.NewLine + "    - set and get with wait" + Environment.NewLine + "    - set info" + Environment.NewLine + "    - set with wait" + Environment.NewLine + "- protocol" + Environment.NewLine + "    - close" + Environment.NewLine + "    - lock/unlock" + Environment.NewLine + "    - merge" + Environment.NewLine + "    - open" + Environment.NewLine + "    - priority lock/unlock" + Environment.NewLine + "    - read file" + Environment.NewLine + "    - sleep" + Environment.NewLine + "    - stop current group" + Environment.NewLine + "    - swap column" + Environment.NewLine + "    - wmi" + Environment.NewLine + "- response" + Environment.NewLine + "    - clear" + Environment.NewLine + "    - clear length info" + Environment.NewLine + "    - crc" + Environment.NewLine + "    - length" + Environment.NewLine + "    - read" + Environment.NewLine + "    - read stuffing" + Environment.NewLine + "    - replace" + Environment.NewLine + "    - replace data" + Environment.NewLine + "    - stuffing" + Environment.NewLine + "- timer" + Environment.NewLine + "    - restart timer" + Environment.NewLine + "    - start" + Environment.NewLine + "    - stop" + Environment.NewLine + "    - reschedule",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult MissingOnIdAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string actionType, string actionOn, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckActionTypes,
                ErrorId = ErrorIds.MissingOnIdAttribute,
                FullId = "6.7.2",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing attribute '{0}' due to '{1}' '{2}' and '{3}' '{4}'. {5} {6} '{7}'.", "On@id", "Action/Type", actionType, "Action/On", actionOn, "Action", "ID", actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/On@id' attribute is mandatory in following cases:" + Environment.NewLine + "- On timer: always." + Environment.NewLine + "- On pair:" + Environment.NewLine + "    - timeout." + Environment.NewLine + "- On group:" + Environment.NewLine + "    - All execute flavors: 'add to execute'/'execute'/'execute next'/'execute one'/'execute one top'/'execute one now'/'force execute'.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult MissingTypeIdAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string actionType, string actionOn, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckActionTypes,
                ErrorId = ErrorIds.MissingTypeIdAttribute,
                FullId = "6.7.3",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing attribute '{0}' due to '{1}' '{2}' and '{3}' '{4}'. {5} {6} '{7}'.", "Type@id", "Action/Type", actionType, "Action/On", actionOn, "Action", "ID", actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/Type@id' attribute is mandatory in following cases:" + Environment.NewLine + "- On pair:" + Environment.NewLine + "    - timeout: PID of the parameter that holds the timeout value (in ms).",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult MissingOnNrAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string actionType, string actionOn, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckActionTypes,
                ErrorId = ErrorIds.MissingOnNrAttribute,
                FullId = "6.7.4",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing attribute '{0}' due to '{1}' '{2}' and '{3}' '{4}'. {5} {6} '{7}'.", "On@nr", "Action/Type", actionType, "Action/On", actionOn, "Action", "ID", actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/On@nr' attribute is mandatory in following cases:" + Environment.NewLine + "- On pair:" + Environment.NewLine + "    - set next: the 1-based position(s) of the pair(s) in the group on which the next value needs to be set.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult NonExistingParamRefInTypeIdAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string pid, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckActionTypes,
                ErrorId = ErrorIds.NonExistingParamRefInTypeIdAttribute,
                FullId = "6.7.5",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Attribute '{0}' references a non-existing '{1}' with {2} '{3}'. {4} {5} '{6}'.", "Type@id", "Param", "ID", pid, "Action", "ID", actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/Type@id' attribute should refer to existing parameter ID in following cases:" + Environment.NewLine + "- On pair:" + Environment.NewLine + "    - timeout: ID of the parameter that holds the timeout value (in ms)." + Environment.NewLine + "    - set next: (optional) ID of the parameter containing the 'time to wait after pair' value (in ms).",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult MissingTypeIdOrTypeValueAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string actionType, string actionOn, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckActionTypes,
                ErrorId = ErrorIds.MissingTypeIdOrTypeValueAttribute,
                FullId = "6.7.6",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing attribute '{0}' due to '{1}' '{2}' and '{3}' '{4}'. {5} {6} '{7}'.", "Type@id or Type@value", "Action/Type", actionType, "Action/On", actionOn, "Action", "ID", actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "Following cases require either a 'Action/Type@id' or a 'Action/Type@value' attribute:" + Environment.NewLine + "- On pair:" + Environment.NewLine + "    - set next: define the 'time to wait after pair' value (in ms) either by referencing the ID of a parameter containing the (dynamic) value via 'Type@id' or by hard coding the value via the 'Type@value' attribute.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult ExcessiveTypeIdOrTypeValueAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string actionType, string actionOn, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckActionTypes,
                ErrorId = ErrorIds.ExcessiveTypeIdOrTypeValueAttribute,
                FullId = "6.7.7",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Excessive attribute '{0}' due to '{1}' '{2}' and '{3}' '{4}'. {5} {6} '{7}'.", "Type@id or Type@value", "Action/Type", actionType, "Action/On", actionOn, "Action", "ID", actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "Following cases require either a 'Action/Type@id' or a 'Action/Type@value' attribute (one or the other, not both):" + Environment.NewLine + "- On pair:" + Environment.NewLine + "    - set next: define the 'time to wait after pair' value (in ms) either by referencing the ID of a parameter containing the (dynamic) value via 'Type@id' or by hard coding the value via the 'Type@value' attribute.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult NonExistingRefToPairOnTimeoutSetNext(IValidate test, IReadable referenceNode, IReadable positionNode, string pairPosition, string groupId, string actionId, string triggerId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckActionTypes,
                ErrorId = ErrorIds.NonExistingRefToPairOnTimeoutSetNext,
                FullId = "6.7.8",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Attribute 'On@nr' references a non-existing 'Pair' with 1-based position '{0}' in Group '{1}'. Action ID '{2}' triggered by Trigger '{3}'.", pairPosition, groupId, actionId, triggerId),
                HowToFix = "",
                ExampleCode = "",
                Details = "For pair timeout actions, the 'Action/On@nr' attribute should contain the 1-based position(s) of pair(s) in related group(s).",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult NonExistingConnectionRefInTypeNrAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string connectionId, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckActionTypes,
                ErrorId = ErrorIds.NonExistingConnectionRefInTypeNrAttribute,
                FullId = "6.7.9",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Attribute '{0}' references a non-existing '{1}' with {2} '{3}'. {4} {5} '{6}'.", "Type@nr", "Connection", "ID", connectionId, "Action", "ID", actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/Type@nr' attribute should refer to an existing connection ID (0-based) in following cases:" + Environment.NewLine + "- On group:" + Environment.NewLine + "    - 'set' / 'set with wait' : allowing to set multiple SNMP parameters in one go. The refered connection should be of type SNMP (snmp, snmpV2 or snmpV3).",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UnsupportedConnectionTypeDueTo(IValidate test, IReadable referenceNode, IReadable positionNode, string actionType, string actionOn, string connectionId, string connectionType, string actionId, string optionalPrefix)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckActionTypes,
                ErrorId = ErrorIds.UnsupportedConnectionTypeDueTo,
                FullId = "6.7.10",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("{5}'Type@nr' attribute in action of type '{0}' on '{1}' references Connection '{2}' with wrong type '{3}'. Action ID '{4}'.", actionType, actionOn, connectionId, connectionType, actionId, optionalPrefix),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/Type@nr' attribute should refer to the ID (0-based) of an existing connection in following cases:" + Environment.NewLine + "- On group:" + Environment.NewLine + "    - 'set' / 'set with wait' : allowing to set multiple SNMP parameters in one go. The refered connection should be of type SNMP (snmp, snmpV2 or snmpV3).",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UnsupportedGroupContentDueTo(IValidate test, IReadable referenceNode, IReadable positionNode, string actionType, string actionOn, string groupId, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckActionTypes,
                ErrorId = ErrorIds.UnsupportedGroupContentDueTo,
                FullId = "6.7.11",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Attribute 'On@id' in action of type '{0}' on '{1}' references Group '{2}' which is missing 'Content/Param' tag(s). Action ID '{3}'.", actionType, actionOn, groupId, actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/On@id' attribute should refer to the ID of an existing Group in following cases:" + Environment.NewLine + "- On group:" + Environment.NewLine + "    - 'set' / 'set with wait' : allowing to set multiple SNMP parameters in one go. The group content should refer to SNMP parameters of type 'write'.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UnsupportedGroupParamType(IValidate test, IReadable referenceNode, IReadable positionNode, string actionType, string actionOn, string groupId, string paramId, string paramType, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckActionTypes,
                ErrorId = ErrorIds.UnsupportedGroupParamType,
                FullId = "6.7.12",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Attribute 'On@id' in action of type '{0}' on '{1}' references Group '{2}' which references Param '{3}' with unsupported 'Param/Type' '{4}'. Action ID '{5}'.", actionType, actionOn, groupId, paramId, paramType, actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/On@id' attribute should refer to the ID of an existing Group in following cases:" + Environment.NewLine + "- On group:" + Environment.NewLine + "    - 'set' / 'set with wait' : allowing to set multiple SNMP parameters in one go. The group content should refer to SNMP parameters Param/Type 'write' and SNMP/Enabled 'true'.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UnsupportedGroupParamWithoutSnmp(IValidate test, IReadable referenceNode, IReadable positionNode, string actionType, string actionOn, string groupId, string paramId, string snmpEnabledValue, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckActionTypes,
                ErrorId = ErrorIds.UnsupportedGroupParamWithoutSnmp,
                FullId = "6.7.13",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Attribute 'On@id' in action of type '{0}' on '{1}' references Group '{2}' which references Param '{3}' with unsupported 'SNMP/Enabled' '{4}'. Action ID '{5}'.", actionType, actionOn, groupId, paramId, snmpEnabledValue, actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/On@id' attribute should refer to the ID of an existing Group in following cases:" + Environment.NewLine + "- On group:" + Environment.NewLine + "    - 'set' / 'set with wait' : allowing to set multiple SNMP parameters in one go. The group content should refer to SNMP parameters Param/Type 'write' and SNMP/Enabled 'true'.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UnsupportedAttributeOnNr(IValidate test, IReadable referenceNode, IReadable positionNode, string actionType, string actionOn, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckActionTypes,
                ErrorId = ErrorIds.UnsupportedAttributeOnNr,
                FullId = "6.7.100",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Unsupported attribute '{0}' in combination with '{1}' '{2}' and  '{3}' '{4}'. {5} {6} '{7}'.", "Action/On@nr", "Action/Type", actionType, "Action/On", actionOn, "Action", "ID", actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/On@nr' attribute only makes sense in following 'Action/On' and 'Action/Type' scenarios:" + Environment.NewLine + "- On param:" + Environment.NewLine + "    - reverse :  Semicolon (;) separated list of 0-based position(s) of the parameter in the command/response." + Environment.NewLine + "- On pair:" + Environment.NewLine + "    - set next :  Semicolon (;) separated list of 1-based position(s) of the pair(s) in the group.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint IncompatibleTypeVsOnTag = 1;
        public const uint MissingOnIdAttribute = 2;
        public const uint MissingTypeIdAttribute = 3;
        public const uint MissingOnNrAttribute = 4;
        public const uint NonExistingParamRefInTypeIdAttribute = 5;
        public const uint MissingTypeIdOrTypeValueAttribute = 6;
        public const uint ExcessiveTypeIdOrTypeValueAttribute = 7;
        public const uint NonExistingRefToPairOnTimeoutSetNext = 8;
        public const uint NonExistingConnectionRefInTypeNrAttribute = 9;
        public const uint UnsupportedConnectionTypeDueTo = 10;
        public const uint UnsupportedGroupContentDueTo = 11;
        public const uint UnsupportedGroupParamType = 12;
        public const uint UnsupportedGroupParamWithoutSnmp = 13;
        public const uint UnsupportedAttributeOnNr = 100;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckActionTypes = 7;
    }
}