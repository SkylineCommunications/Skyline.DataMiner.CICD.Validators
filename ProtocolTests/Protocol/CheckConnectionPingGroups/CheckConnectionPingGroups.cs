namespace ProtocolTests.Protocol.CheckConnectionPingGroups
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.CheckConnectionPingGroups;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckConnectionPingGroups();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_Valid()
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
        public void Protocol_CheckConnectionPingGroups_ValidSerialExplicitPingGroup()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidSerialExplicitPingGroup",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_ValidSerialExplicitPingPair()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidSerialExplicitPingPair",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_ValidSerialExplicitPingPairs()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidSerialExplicitPingPairs",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_ValidSerialImplicitPingPair()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidSerialImplicitPingPair",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_ValidSmartSerialNoPairWithResponse()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidSmartSerialNoPairWithResponse",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_ValidSnmpExplicitPingGroup()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidSnmpExplicitPingGroup",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_ValidSnmpImplicitPingGroup()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidSnmpImplicitPingGroup",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_ValidSnmpNoPolling()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidSnmpNoPolling",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_InvalidPingGroupType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidPingGroupType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidPingGroupType(null, null, null, "snmp", "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_MultiplePingPairsForConnection()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MultiplePingPairsForConnection",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MultiplePingPairsForConnection(null, null, null, "My Serial Main Connection", "serial", "0").WithSubResults(
                        Error.MultiplePingPairsForConnection_Sub(null, null, null, "0", "1"),
                        Error.MultiplePingPairsForConnection_Sub(null, null, null, "0", "2"),
                        Error.MultiplePingPairsForConnection_Sub(null, null, null, "0", "3"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_PingSerialPairHasNoResponse()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "PingSerialPairHasNoResponse",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.PingSerialPairHasNoResponse(null, null, null, "serial", "2"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_PingSerialPairHasNoResponseExplicitGroup()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "PingSerialPairHasNoResponseExplicitGroup",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.PingSerialPairHasNoResponse(null, null, null, "serial", "2"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_PingSerialPairHasNoResponseImplicit()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "PingSerialPairHasNoResponseImplicit",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.PingSerialPairHasNoResponse(null, null, null, "serial", "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_PingSmartSerialPairHasNoResponse()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "PingSmartSerialPairHasNoResponse",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.PingSerialPairHasNoResponse(null, null, null, "smart-serial", "2"),
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
        public void Protocol_CheckConnectionPingGroups_InvalidPingGroupType()
        {
            // Create ErrorMessage
            var message = Error.InvalidPingGroupType(null, null, null, "2", "3");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "1.26.1",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Ping group for '2' connection is not a '2' poll group. Group ID '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_MultiplePingPairsForConnection()
        {
            // Create ErrorMessage
            var message = Error.MultiplePingPairsForConnection(null, null, null, "2", "3", "4");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "1.26.3",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Multiple ping pairs for connection with name '2' and type '3'. Connection ID '4'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_MultiplePingPairsForConnection_Sub()
        {
            // Create ErrorMessage
            var message = Error.MultiplePingPairsForConnection_Sub(null, null, null, "2", "3");

            var expected = new ValidationResult
            {
                ErrorId = 4,
                FullId = "1.26.4",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Multiple ping pairs for connection '2'. Pair '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_PingSerialPairHasNoResponse()
        {
            // Create ErrorMessage
            var message = Error.PingSerialPairHasNoResponse(null, null, null, "serial", "1");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "1.26.2",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Ping pair for 'serial' connection contains no response. Pair ID '1'.",
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
        private readonly IRoot check = new CheckConnectionPingGroups();

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_CheckCategory() => Generic.CheckCategory(check, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckConnectionPingGroups_CheckId() => Generic.CheckId(check, CheckId.CheckConnectionPingGroups);
    }
}