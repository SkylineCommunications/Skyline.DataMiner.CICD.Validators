namespace ProtocolTests.Protocol.Actions.Action.CheckActionTypes
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Actions.Action.CheckActionTypes;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckActionTypes();

        #region Valid Checks

        [TestMethod]
        public void Action_CheckActionTypes_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_ValidOnCommand()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidOnCommand",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_ValidOnGroup()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidOnGroup",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_ValidOnPair()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidOnPair",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_ValidOnParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidOnParam",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_ValidOnProtocol()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidOnProtocol",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_ValidOnResponse()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidOnResponse",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_ValidOnTimer()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidOnTimer",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Action_CheckActionTypes_ExcessiveTypeIdOrTypeValueAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ExcessiveTypeIdOrTypeValueAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    // On Pair
                    Error.ExcessiveTypeIdOrTypeValueAttribute(null, null, null, "set next", "pair", "3000"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_IncompatibleTypeVsOnTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "IncompatibleTypeVsOnTag",
                ExpectedResults = new List<IValidationResult>
                {
                    // On Command
                    Error.IncompatibleTypeVsOnTag(null, null, null, "close", "command", "1000"),
                    
                    // On Group
                    Error.IncompatibleTypeVsOnTag(null, null, null, "close", "group", "2000"),
                    
                    // On Pair
                    Error.IncompatibleTypeVsOnTag(null, null, null, "close", "pair", "3000"),
                    
                    // On Param
                    Error.IncompatibleTypeVsOnTag(null, null, null, "close", "parameter", "4000"),
                    
                    // On Protocol
                    Error.IncompatibleTypeVsOnTag(null, null, null, "add to execute", "protocol", "5000"),
                    
                    // On Response
                    Error.IncompatibleTypeVsOnTag(null, null, null, "close", "response", "6000"),
                    
                    // On Timer
                    Error.IncompatibleTypeVsOnTag(null, null, null, "close", "timer", "7000"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_MissingOnIdAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingOnIdAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    // On Group
                    Error.MissingOnIdAttribute(null, null, null, "add to execute", "group", "2000"),
                    Error.MissingOnIdAttribute(null, null, null, "execute", "group", "2010"),
                    Error.MissingOnIdAttribute(null, null, null, "execute next", "group", "2020"),
                    Error.MissingOnIdAttribute(null, null, null, "execute one", "group", "2030"),
                    Error.MissingOnIdAttribute(null, null, null, "execute one top", "group", "2040"),
                    Error.MissingOnIdAttribute(null, null, null, "execute one now", "group", "2050"),
                    Error.MissingOnIdAttribute(null, null, null, "force execute", "group", "2060"),

                    Error.MissingOnIdAttribute(null, null, null, "set", "group", "2500"),
                    Error.MissingOnIdAttribute(null, null, null, "set with wait", "group", "2510"),

                    // On Pair
                    Error.MissingOnIdAttribute(null, null, null, "timeout", "pair", "3010"),

                    // On Timer
                    Error.MissingOnIdAttribute(null, null, null, "reschedule", "timer", "7000"),
                    Error.MissingOnIdAttribute(null, null, null, "restart timer", "timer", "7001"),
                    Error.MissingOnIdAttribute(null, null, null, "start", "timer", "7002"),
                    Error.MissingOnIdAttribute(null, null, null, "stop", "timer", "7003"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_MissingOnNrAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingOnNrAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    // On Pair
                    Error.MissingOnNrAttribute(null, null, null, "set next", "pair", "3000"),
                    Error.MissingOnNrAttribute(null, null, null, "set next", "pair", "3001"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_MissingTypeIdAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTypeIdAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    // On Pair
                    Error.MissingTypeIdAttribute(null, null, null, "timeout", "pair", "3010"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_MissingTypeIdOrTypeValueAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTypeIdOrTypeValueAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    // On Pair
                    Error.MissingTypeIdOrTypeValueAttribute(null, null, null, "set next", "pair", "3000"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_NonExistingConnectionRefInTypeNrAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingConnectionRefInTypeNrAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    // On Group
                    Error.NonExistingConnectionRefInTypeNrAttribute(null, null, null, "typo", "2500"),
                    Error.NonExistingConnectionRefInTypeNrAttribute(null, null, null, "-1", "2501"),
                    Error.NonExistingConnectionRefInTypeNrAttribute(null, null, null, "1", "2510"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_NonExistingParamRefInTypeIdAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingParamRefInTypeIdAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    // On Pair
                    Error.NonExistingParamRefInTypeIdAttribute(null, null, null, "1", "3001"),
                    Error.NonExistingParamRefInTypeIdAttribute(null, null, null, "1", "3010"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_NonExistingRefToPairOnTimeoutSetNext()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingRefToPairOnTimeoutSetNext",
                ExpectedResults = new List<IValidationResult>
                {
                    // On Pair Hard coded timeout time
                    Error.NonExistingRefToPairOnTimeoutSetNext(null, null, null, "a", "100", "3000", "101"),
                    Error.NonExistingRefToPairOnTimeoutSetNext(null, null, null, "5", "100", "3000", "101"),

                    // On Pair dynamic timeout time
                    Error.NonExistingRefToPairOnTimeoutSetNext(null, null, null, "b", "100", "3001", "101"),
                    Error.NonExistingRefToPairOnTimeoutSetNext(null, null, null, "0", "100", "3001", "101"),
                    Error.NonExistingRefToPairOnTimeoutSetNext(null, null, null, "-1", "100", "3001", "101"),
                    Error.NonExistingRefToPairOnTimeoutSetNext(null, null, null, "", "100", "3001", "101"),
                    Error.NonExistingRefToPairOnTimeoutSetNext(null, null, null, "10", "100", "3001", "101"),

                    // On Pair no Trigger
                    Error.NonExistingRefToPairOnTimeoutSetNext(null, null, null, "1", "NoGroup", "4000", "NoTrigger"),

                    // On Pair no Group
                    Error.NonExistingRefToPairOnTimeoutSetNext(null, null, null, "1", "NoGroup", "5000", "500"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_UnsupportedAttributeOnNr()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnsupportedAttributeOnNr",
                ExpectedResults = new List<IValidationResult>
                {
                    // On Command
                    Error.UnsupportedAttributeOnNr(null, null, null, "crc", "command", "1000"),
                    
                    // On Group
                    Error.UnsupportedAttributeOnNr(null, null, null, "add to execute", "group", "2000"),
                    
                    // On Pair
                    Error.UnsupportedAttributeOnNr(null, null, null, "timeout", "pair", "3010"),
                    
                    // On Parameter
                    Error.UnsupportedAttributeOnNr(null, null, null, "aggregate", "parameter", "4000"),
                    
                    // On Protocol
                    Error.UnsupportedAttributeOnNr(null, null, null, "close", "protocol", "5000"),
                    
                    // On Response
                    Error.UnsupportedAttributeOnNr(null, null, null, "clear", "response", "6000"),
                    
                    // On Timer
                    Error.UnsupportedAttributeOnNr(null, null, null, "reschedule", "timer", "7000"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_UnsupportedConnectionTypeDueTo()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnsupportedConnectionTypeDueTo",
                ExpectedResults = new List<IValidationResult>
                {
                    // On Group
                    Error.UnsupportedConnectionTypeDueTo(null, null, null, "set", "group", "0", "virtual", "2500", "Default value for "),
                    Error.UnsupportedConnectionTypeDueTo(null, null, null, "set", "group", "1", "http", "2501", ""),

                    Error.UnsupportedConnectionTypeDueTo(null, null, null, "set with wait", "group", "0", "virtual", "2510", ""),
                    Error.UnsupportedConnectionTypeDueTo(null, null, null, "set with wait", "group", "2", "serial", "2511", ""),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_UnsupportedGroupContentDueTo()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnsupportedGroupContentDueTo",
                ExpectedResults = new List<IValidationResult>
                {
                    // On Group
                    Error.UnsupportedGroupContentDueTo(null, null, null, "set", "group", "1", "2500"),

                    Error.UnsupportedGroupContentDueTo(null, null, null, "set with wait", "group", "2", "2510"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_UnsupportedGroupParamType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnsupportedGroupParamType",
                ExpectedResults = new List<IValidationResult>
                {
                    // On Group
                    Error.UnsupportedGroupParamType(null, null, null, "set", "group", "1", "150", "read", "2500"),
                    Error.UnsupportedGroupParamType(null, null, null, "set", "group", "1", "151", "write bit", "2500"),

                    Error.UnsupportedGroupParamType(null, null, null, "set with wait", "group", "2", "250", "dummy", "2510"),
                    Error.UnsupportedGroupParamType(null, null, null, "set with wait", "group", "2", "251", "read bit", "2510"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckActionTypes_UnsupportedGroupParamWithoutSnmp()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnsupportedGroupParamWithoutSnmp",
                ExpectedResults = new List<IValidationResult>
                {
                    // On Group
                    Error.UnsupportedGroupParamWithoutSnmp(null, null, null, "set", "group", "1", "150", "false", "2500"),
                    Error.UnsupportedGroupParamWithoutSnmp(null, null, null, "set", "group", "1", "151", "", "2500"),

                    Error.UnsupportedGroupParamWithoutSnmp(null, null, null, "set with wait", "group", "2", "250", "", "2510"),
                    Error.UnsupportedGroupParamWithoutSnmp(null, null, null, "set with wait", "group", "2", "251", "typo", "2510"),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Action_CheckActionTypes_IncompatibleTypeVsOnTag()
        {
            // Create ErrorMessage
            var message = Error.IncompatibleTypeVsOnTag(null, null, null, "actionType", "actionOn", "actionId");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "6.7.1",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Incompatible 'Action/Type' value 'actionType' with 'Action/On' value 'actionOn'. Action ID 'actionId'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckActionTypes();

        [TestMethod]
        public void Action_CheckActionTypes_CheckCategory() => Generic.CheckCategory(check, Category.Action);

        [TestMethod]
        public void Action_CheckActionTypes_CheckId() => Generic.CheckId(check, CheckId.CheckActionTypes);
    }
}