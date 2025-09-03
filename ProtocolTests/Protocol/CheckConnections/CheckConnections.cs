namespace ProtocolTests.Protocol.CheckConnections
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.CheckConnections;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckConnections();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckConnections_Valid()
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
        public void Protocol_CheckConnections_Valid_BasicNames()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_BasicNames",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnections_Valid_ExtendedNames()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_ExtendedNames",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnections_Valid_SshNames()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_SshNames",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Protocol_CheckConnections_DuplicateConnectionName()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateConnectionName",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicateConnectionName(null, null, null, "SNMP Connection").WithSubResults(
                        Error.DuplicateConnectionName_Sub(null, null, null, "SNMP Connection", "0"),
                        Error.DuplicateConnectionName_Sub(null, null, null, "SNMP Connection", "3")),

                    Error.DuplicateConnectionName(null, null, null, "HTTP Connection").WithSubResults(
                        Error.DuplicateConnectionName_Sub(null, null, null, "HTTP Connection", "1"),
                        Error.DuplicateConnectionName_Sub(null, null, null, "HTTP Connection", "2")),

                    Error.DuplicateConnectionName(null, null, null, "IP Connection - Test").WithSubResults(
                        Error.DuplicateConnectionName_Sub(null, null, null, "IP Connection - Test", "4"),
                        Error.DuplicateConnectionName_Sub(null, null, null, "IP Connection - Test", "5"),
                        Error.DuplicateConnectionName_Sub(null, null, null, "IP Connection - Test", "6"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnections_InvalidCombinationOfSyntax1And2()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidCombinationOfSyntax1And2",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidCombinationOfSyntax1And2(null, null, null),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnections_InvalidConnectionCount()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidConnectionCount",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidConnectionCount(null, null, null, "2", "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnections_InvalidConnectionName()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidConnectionName",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidConnectionName(null, null, null, "Polling", "snmp", "0"),
                    Error.InvalidConnectionName(null, null, null, "Events", "smart-serial", "1"),
                    Error.InvalidConnectionName(null, null, null, "BLA", "snmp", "2"),
                    Error.InvalidConnectionName(null, null, null, "SNMP Connection - XXX", "serial", "3"),
                    Error.InvalidConnectionName(null, null, null, "IP Connection", "snmp", "4"),
                    Error.InvalidConnectionName(null, null, null, "- 0", "snmp", "5"),
                    Error.InvalidConnectionName(null, null, null, "XXX - 0", "snmp", "6"),
                    Error.InvalidConnectionName(null, null, null, "SNMP Connection - ", "snmp", "7"),
                    Error.InvalidConnectionName(null, null, null, "IP Connection - SSH", "serial", "9"),
                    //Error.InvalidConnectionName(null, null, null, "SNMP Connection", "snmp", "8"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnections_MismatchingNames()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MismatchingNames",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MismatchingNames(null, null, null, "1", "'IP Connection' vs 'IP Connection - Events'", true).WithSubResults(
                        Error.MismatchingNames(null, null, null, "1", "'IP Connection - Events'", false),
                        Error.MismatchingNames(null, null, null, "1", "'IP Connection'", false)),
                    Error.MismatchingNames(null, null, null, "2", "'SNMP Connection - BLA' vs 'SNMP Connection - Testing'", true).WithSubResults(
                        Error.MismatchingNames(null, null, null, "2", "'SNMP Connection - Testing'", false),
                        Error.MismatchingNames(null, null, null, "2", "'SNMP Connection - BLA'", false)),
                    Error.MismatchingNames(null, null, null, "3", "'HTTP Connection' vs 'HTTP Connection - Web Stuff'", true).WithSubResults(
                        Error.MismatchingNames(null, null, null, "3", "'HTTP Connection - Web Stuff'", false),
                        Error.MismatchingNames(null, null, null, "3", "'HTTP Connection'", false))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnections_MismatchingNamesUndefined1()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MismatchingNamesUndefined1",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MismatchingNames(null, null, null, "1", "'IP Connection' vs ''", true).WithSubResults(
                        Error.MismatchingNames(null, null, null, "1", "''", false),
                        Error.MismatchingNames(null, null, null, "1", "'IP Connection'", false)),
                    Error.MismatchingNames(null, null, null, "2", "'SNMP Connection - BLA' vs ''", true).WithSubResults(
                        Error.MismatchingNames(null, null, null, "2", "''", false),
                        Error.MismatchingNames(null, null, null, "2", "'SNMP Connection - BLA'", false)),
                    Error.MismatchingNames(null, null, null, "3", "'HTTP Connection' vs ''", true).WithSubResults(
                        Error.MismatchingNames(null, null, null, "3", "''", false),
                        Error.MismatchingNames(null, null, null, "3", "'HTTP Connection'", false))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnections_MismatchingNamesUndefined2()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MismatchingNamesUndefined2",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MismatchingNames(null, null, null, "1", "'' vs 'IP Connection - Events'", true).WithSubResults(
                        Error.MismatchingNames(null, null, null, "1", "'IP Connection - Events'", false),
                        Error.MismatchingNames(null, null, null, "1", "''", false)),
                    Error.MismatchingNames(null, null, null, "2", "'' vs 'SNMP Connection - Testing'", true).WithSubResults(
                        Error.MismatchingNames(null, null, null, "2", "'SNMP Connection - Testing'", false),
                        Error.MismatchingNames(null, null, null, "2", "''", false)),
                    Error.MismatchingNames(null, null, null, "3", "'' vs 'HTTP Connection - Web Stuff'", true).WithSubResults(
                        Error.MismatchingNames(null, null, null, "3", "'HTTP Connection - Web Stuff'", false),
                        Error.MismatchingNames(null, null, null, "3", "''", false)),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckConnections_UnrecommendedSyntax2()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedSyntax2",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedSyntax2(null, null, null),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckConnections();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckTypeTag_Valid_Multiple()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "Valid_Multiple",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckTypeTag_Valid()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckTypeTag_Valid_SnmpV2ToSnmp()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "Valid_SnmpV2ToSnmp",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckTypeTag_Valid_Syntax1To2()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "Valid_Syntax1To2",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckTypeTag_Valid_Syntax1To3()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "Valid_Syntax1To3",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckTypeTag_Valid_Syntax2To1()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "Valid_Syntax2To1",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckTypeTag_Valid_Syntax3To1()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "Valid_Syntax3To1",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Protocol_CheckTypeTag_ConnectionAdded()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "ConnectionAdded",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.ConnectionAdded(null, null, "smart-serial", "2", ""),
                    ErrorCompare.ConnectionAdded(null, null, "serial", "3", "Serial Name"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckTypeTag_ConnectionAddedFromNothing()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "ConnectionAddedFromNothing",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.ConnectionAdded(null, null, "http", "1", "SomeHTTPConnection"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckTypeTag_ConnectionsOrderChanged()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "ConnectionsOrderChanged",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.ConnectionsOrderChanged(null, null, "0:snmp, 1:http, 2:smart-serial, 3:serial", "0:snmp, 1:smart-serial, 2:http, 3:serial"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckTypeTag_ConnectionTypeChanged_Multiple()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "ConnectionTypeChanged_Multiple",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.ConnectionTypeChanged(null, null, "serial", "0", "", "snmp"),
                    ErrorCompare.ConnectionTypeChanged(null, null, "serial", "1", "serialToSnmpV2", "snmpv2"),
                    ErrorCompare.ConnectionTypeChanged(null, null, "smart-serial", "2", "smartSerialToSnmpV3", "snmpv3"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckTypeTag_ConnectionTypeChanged()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "ConnectionTypeChanged",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.ConnectionTypeChanged(null, null, "snmp", "0", "", "snmpv2"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckConnections();

        [TestMethod]
        public void Protocol_CheckConnections_MismatchingNames()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MismatchingNames",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        #region Validate Errors

        [TestMethod]
        public void Protocol_CheckConnections_DuplicateConnectionName()
        {
            // Create ErrorMessage
            var message = Error.DuplicateConnectionName(null, null, null, "myConnection");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "1.23.3",
                Category = Category.Protocol,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Duplicated Connection name 'myConnection'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckConnections_DuplicateConnectionName_Sub()
        {
            // Create ErrorMessage
            var message = Error.DuplicateConnectionName_Sub(null, null, null, "myConnection", "1");

            var expected = new ValidationResult
            {
                ErrorId = 4,
                FullId = "1.23.4",
                Category = Category.Protocol,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Duplicated Connection name 'myConnection'. Connection IDs '1'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckConnections_InvalidCombinationOfSyntax1And2()
        {
            // Create ErrorMessage
            var message = Error.InvalidCombinationOfSyntax1And2(null, null, null);

            var expected = new ValidationResult
            {
                ErrorId = 6,
                FullId = "1.23.6",
                Category = Category.Protocol,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Connections can not be defined simultaneously via 'Protocol/Type' and 'Protocol/Connections'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckConnections_InvalidConnectionCount()
        {
            // Create ErrorMessage
            var message = Error.InvalidConnectionCount(null, null, null, "0", "1");

            var expected = new ValidationResult
            {
                ErrorId = 5,
                FullId = "1.23.5",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Connection count in 'Protocol/Type' tag '0' does not match with PortSettings count '1'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckConnections_InvalidConnectionName()
        {
            // Create ErrorMessage
            var message = Error.InvalidConnectionName(null, null, null, "myName", "snmp", "1");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "1.23.2",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Invalid connection name 'myName' for a 'snmp' connection. Connection ID '1'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckConnections_MismatchingNames()
        {
            // Create ErrorMessage
            var message = Error.MismatchingNames(null, null, null, "0", "'myConnectionName' vs 'myConnectionOtherName'", false);

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "1.23.1",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Connection 0 has mismatching names: 'myConnectionName' vs 'myConnectionOtherName'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckConnections_UnrecommendedSyntax2()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedSyntax2(null, null, null);

            var expected = new ValidationResult
            {
                ErrorId = 7,
                FullId = "1.23.7",
                Category = Category.Protocol,
                Severity = Severity.Minor,
                Certainty = Certainty.Uncertain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Unrecommended use of the 'Protocol/Connections' syntax.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        #endregion

        #region Compare Errors

        [TestMethod]
        public void Protocol_CheckConnections_ConnectionAdded()
        {
            // Create ErrorMessage
            var message = ErrorCompare.ConnectionAdded(null, null, "snmp", "0", "myNewSnmp");

            var expected = new ValidationResult
            {
                ErrorId = 10,
                FullId = "1.23.10",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = String.Empty,
                Description = "snmp Connection '0' with name 'myNewSnmp' was added.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckConnections_ConnectionsOrderChanged()
        {
            // Create ErrorMessage
            var message = ErrorCompare.ConnectionsOrderChanged(null, null, "0:snmp, 1:serial", "0:serial, 1:snmp");

            var expected = new ValidationResult
            {
                ErrorId = 8,
                FullId = "1.23.8",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = String.Empty,
                Description = "Order of connections changed from '0:snmp, 1:serial' to '0:serial, 1:snmp'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckConnections_ConnectionTypeChanged()
        {
            // Create ErrorMessage
            var message = ErrorCompare.ConnectionTypeChanged(null, null, "snmp", "0", "myConnection", "serial");

            var expected = new ValidationResult
            {
                ErrorId = 9,
                FullId = "1.23.9",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = String.Empty,
                Description = "snmp Connection '0' with name 'myConnection' was changed into 'serial'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckConnections();

        [TestMethod]
        public void Protocol_CheckConnections_CheckCategory() => Generic.CheckCategory(check, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckConnections_CheckId() => Generic.CheckId(check, CheckId.CheckConnections);
    }
}