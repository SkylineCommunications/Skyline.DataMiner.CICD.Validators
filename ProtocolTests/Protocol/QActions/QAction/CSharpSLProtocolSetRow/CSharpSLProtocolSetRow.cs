namespace ProtocolTests.Protocol.QActions.QAction.CSharpSLProtocolSetRow
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpSLProtocolSetRow;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpSLProtocolSetRow();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpSLProtocolSetRow_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void QAction_CSharpSLProtocolSetRow_HardCodedPid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "HardCodedPid",
                ExpectedResults = new List<IValidationResult>
                {
		            // By Key
                    Error.HardCodedPid(null, null, null, "1000", "102"),
                    Error.HardCodedPid(null, null, null, "1000", "102"),
                    Error.HardCodedPid(null, null, null, "1000", "102"),
                    Error.HardCodedPid(null, null, null, "1000", "102"),
                    
		            // By RowPosition
                    Error.HardCodedPid(null, null, null, "1000", "102"),
                    Error.HardCodedPid(null, null, null, "1000", "102"),
                    Error.HardCodedPid(null, null, null, "1000", "102"),
                    Error.HardCodedPid(null, null, null, "1000", "102"),

                    // Constant in a different location
                    Error.HardCodedPid(null, null, null, "1000", "102"),

                    // Constant in the same method
                    Error.HardCodedPid(null, null, null, "1000", "102"),
                    
		            // Constant as global property
                    Error.HardCodedPid(null, null, null, "1000", "102"),

                    // Unchanged variable inside the same method
                    Error.HardCodedPid(null, null, null, "1000", "102"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolSetRow_NonExistingParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingParam",
                ExpectedResults = new List<IValidationResult>
                {
                    // Doesn't cover the standalone param message yet

                    Error.NonExistingParam(null, null, null, "2000", "102"),
                    Error.NonExistingParam(null, null, null, "2100", "102"),
                    Error.NonExistingParam(null, null, null, "2200", "102"),

                    Error.NonExistingParam(null, null, null, "3000", "102"),
                    Error.NonExistingParam(null, null, null, "3100", "102"),
                    Error.NonExistingParam(null, null, null, "3200", "102"),
                    Error.NonExistingParam(null, null, null, "3300", "102"),
                    Error.NonExistingParam(null, null, null, "3400", "102"),

                    // Cause all tests are using hard-coded PIDs
                    Error.HardCodedPid(null, null, null, "100", "102"),
                    Error.HardCodedPid(null, null, null, "2000", "102"),
                    Error.HardCodedPid(null, null, null, "2100", "102"),
                    Error.HardCodedPid(null, null, null, "2200", "102"),

                    Error.HardCodedPid(null, null, null, "3000", "102"),
                    Error.HardCodedPid(null, null, null, "3100", "102"),
                    Error.HardCodedPid(null, null, null, "3200", "102"),
                    Error.HardCodedPid(null, null, null, "3300", "102"),
                    Error.HardCodedPid(null, null, null, "3400", "102"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolSetRow_ParamMissingHistorySet()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ParamMissingHistorySet",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.HardCodedPid(null, null, null, "1000", "102"),
                    Error.HardCodedPid(null, null, null, "1000", "102"), // Calculated one
                    Error.HardCodedPid(null, null, null, "1000", "102"), // Constant from another class

                    Error.ParamMissingHistorySet(null, null, null, "1002"),
                    Error.ParamMissingHistorySet(null, null, null, "1003"),

                    // ValueType isn't covered yet
                    //Error.ParamMissingHistorySet(null, null, null, "1002"), // Not yet covered
                    //Error.ParamMissingHistorySet(null, null, null, "1003"), // Not yet covered

                    Error.ParamMissingHistorySet(null, null, null, "1002"),
                    Error.ParamMissingHistorySet(null, null, null, "1003"),

                    Error.ParamMissingHistorySet(null, null, null, "1002"),
                    Error.ParamMissingHistorySet(null, null, null, "1003"),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    //[TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CSharpSLProtocolSetRow();

        [TestMethod]
        public void QAction_CSharpSLProtocolSetRow_ParamMissingHistorySet()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "ParamMissingHistorySet",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void QAction_CSharpSLProtocolSetRow_NonExistingParam()
        {
            // Create ErrorMessage
            var message = Error.NonExistingParam(null, null, null, "1", "2");

            var expected = new ValidationResult()
            {
                ErrorId = 1,
                FullId = "3.8.1",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Method 'SLProtocol.SetRow' references a non-existing 'table' with PID '1'. QAction ID '2'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArray_HardCodedPid()
        {
            // Create ErrorMessage
            var message = Error.HardCodedPid(null, null, null, "1", "2");

            var expected = new ValidationResult()
            {
                ErrorId = 2,
                FullId = "3.8.2",
                Category = Category.QAction,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Unrecommended use of magic number '1', use 'Parameter' class instead. QAction ID '2'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolSetRow_ParamMissingHistorySet()
        {
            // Create ErrorMessage
            var message = Error.ParamMissingHistorySet(null, null, null, "1");

            var expected = new ValidationResult()
            {
                ErrorId = 3,
                FullId = "3.8.3",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "SLProtocol.SetRow overload with 'ValueType timeInfo' argument requires 'Param@historySet=true'. column PID '1'.",
                HowToFix = "",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CSharpSLProtocolSetRow();

        [TestMethod]
        public void QAction_CSharpSLProtocolSetRow_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpSLProtocolSetRow_CheckId() => Generic.CheckId(check, CheckId.CSharpSLProtocolSetRow);
    }
}