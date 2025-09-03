namespace ProtocolTests.Protocol.QActions.QAction.CSharpSLProtocolFillArray
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpSLProtocolFillArray;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpSLProtocolFillArray();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArray_Valid()
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
        public void QAction_CSharpSLProtocolFillArray_HardCodedPid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "HardCodedPid",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.HardCodedPid(null, null, null, "2000", "103"),
                    Error.HardCodedPid(null, null, null, "2000", "103"),
                    Error.HardCodedPid(null, null, null, "2000", "103"),

                    Error.HardCodedPid(null, null, null, "2000", "103"),
                    Error.HardCodedPid(null, null, null, "2000", "103"),
                    Error.HardCodedPid(null, null, null, "2000", "103"),

                    Error.HardCodedPid(null, null, null, "2000", "103"),
                    Error.HardCodedPid(null, null, null, "2000", "103"),
                    Error.HardCodedPid(null, null, null, "2000", "103"),
                    Error.HardCodedPid(null, null, null, "2000", "103"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArray_NonExistingParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingParam",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingParam(null, null, null, "1000", "103"),
                    Error.NonExistingParam(null, null, null, "1100", "103"),
                    Error.NonExistingParam(null, null, null, "1200", "103"),
                    Error.HardCodedPid(null, null, null, "1000", "103"),
                    Error.HardCodedPid(null, null, null, "1100", "103"),
                    Error.HardCodedPid(null, null, null, "1200", "103"),

                    Error.NonExistingParam(null, null, null, "2000", "103"),
                    Error.NonExistingParam(null, null, null, "2100", "103"),
                    Error.NonExistingParam(null, null, null, "2200", "103"),
                    Error.HardCodedPid(null, null, null, "2000", "103"),
                    Error.HardCodedPid(null, null, null, "2100", "103"),
                    Error.HardCodedPid(null, null, null, "2200", "103"),

                    Error.NonExistingParam(null, null, null, "3000", "103"),
                    Error.NonExistingParam(null, null, null, "3100", "103"),
                    Error.NonExistingParam(null, null, null, "3200", "103"),
                    Error.HardCodedPid(null, null, null, "3000", "103"),
                    Error.HardCodedPid(null, null, null, "3100", "103"),
                    Error.HardCodedPid(null, null, null, "3200", "103"),
                    
		            // TODO: Not yet covered
                    //Error.NonExistingParam(null, null, null, "4000", "103"),
                    //Error.NonExistingParam(null, null, null, "4200", "103"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArray_ParamMissingHistorySet()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ParamMissingHistorySet",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ParamMissingHistorySet(null, null, null, "1002"),
                    Error.ParamMissingHistorySet(null, null, null, "1003"),

                    Error.ParamMissingHistorySet(null, null, null, "1102"),
                    Error.ParamMissingHistorySet(null, null, null, "1103"),

                    Error.ParamMissingHistorySet(null, null, null, "1002"),
                    Error.ParamMissingHistorySet(null, null, null, "1003"),

                    Error.ParamMissingHistorySet(null, null, null, "1102"),
                    Error.ParamMissingHistorySet(null, null, null, "1103"),

                    // Not yet covered
                    //Error.ParamMissingHistorySet(null, null, null, "1002"),
                    //Error.ParamMissingHistorySet(null, null, null, "1003"),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    //[TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CSharpSLProtocolFillArray();

        [TestMethod]
        public void Protocol_CSharpSLProtocolFillArray_ParamMissingHistorySet()
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
        public void QAction_CSharpSLProtocolFillArray_NonExistingParam()
        {
            // Create ErrorMessage
            var message = Error.NonExistingParam(null, null, null, "1", "2");

            var expected = new ValidationResult()
            {
                ErrorId = 1,
                FullId = "3.9.1",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Method 'SLProtocol.FillArray' references a non-existing 'table' with PID '1'. QAction ID '2'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArray_ParamMissingHistorySet()
        {
            // Create ErrorMessage
            var message = Error.ParamMissingHistorySet(null, null, null, "1");

            var expected = new ValidationResult()
            {
                ErrorId = 2,
                FullId = "3.9.2",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "SLProtocol.FillArray overload with 'DateTime? timeInfo' argument requires 'Param@historySet=true'. column PID '1'.",
                HowToFix = "",
                HasCodeFix = true,
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
                ErrorId = 3,
                FullId = "3.9.3",
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
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CSharpSLProtocolFillArray();

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArray_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArray_CheckId() => Generic.CheckId(check, CheckId.CSharpSLProtocolFillArray);
    }
}