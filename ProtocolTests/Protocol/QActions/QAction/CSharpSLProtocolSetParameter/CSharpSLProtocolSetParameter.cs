namespace ProtocolTests.Protocol.QActions.QAction.CSharpSLProtocolSetParameter
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpSLProtocolSetParameter;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpSLProtocolSetParameter();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpSLProtocolSetParameter_Valid()
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
        public void QAction_CSharpSLProtocolSetParameter_HardCodedPid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "HardCodedPid",
                ExpectedResults = new List<IValidationResult>
                {
                    // QAction 100: SetParameter
                    Error.HardCodedPid(null, null, null, "100", "100"),
                    Error.HardCodedPid(null, null, null, "100", "100"),
                    Error.HardCodedPid(null, null, null, "200", "100"),
                    Error.HardCodedPid(null, null, null, "201", "100"),

                    Error.HardCodedPid(null, null, null, "100", "100"),

                    Error.HardCodedPid(null, null, null, "100", "100"),
                    
                    // QAction 101: SetParameter_HistorySet
                    Error.HardCodedPid(null, null, null, "100", "101"),
                    Error.HardCodedPid(null, null, null, "100", "101"),

                    Error.HardCodedPid(null, null, null, "100", "101"),
                    Error.HardCodedPid(null, null, null, "100", "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolSetParameter_NonExistingParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingParam",
                ExpectedResults = new List<IValidationResult>
                {
                    // QAction 100: SetParameter
		            // Hard-coded
                    Error.HardCodedPid(null, null, null, "9", "100"),
                    Error.NonExistingParam(null, null, null, "9", "100"),
                    Error.HardCodedPid(null, null, null, "19", "100"),
                    Error.NonExistingParam(null, null, null, "19", "100"),
                    Error.HardCodedPid(null, null, null, "-29", "100"),
                    Error.NonExistingParam(null, null, null, "-29", "100"),
                    
		            // Variables
                    Error.HardCodedPid(null, null, null, "99", "100"),
                    Error.HardCodedPid(null, null, null, "199", "100"),
                    Error.NonExistingParam(null, null, null, "99", "100"),
                    Error.NonExistingParam(null, null, null, "199", "100"),
                    
		            // Wrappers (not yet covered)
                    //Error.NonExistingParam(null, null, null, "999", "100"),
                    //Error.NonExistingParam(null, null, null, "1099", "100"),
                    //Error.NonExistingParam(null, null, null, "1199", "100"),
                    //Error.NonExistingParam(null, null, null, "1299", "100"),

                    
                    // QAction 101: SetParameter_HistorySet
		            // Hard-coded
                    Error.HardCodedPid(null, null, null, "9", "101"),
                    Error.NonExistingParam(null, null, null, "9", "101"),
                    Error.HardCodedPid(null, null, null, "19", "101"),
                    Error.NonExistingParam(null, null, null, "19", "101"),
                    Error.HardCodedPid(null, null, null, "-29", "101"),
                    Error.NonExistingParam(null, null, null, "-29", "101"),
                    
		            // Variables
                    Error.HardCodedPid(null, null, null, "99", "101"),
                    Error.HardCodedPid(null, null, null, "199", "101"),
                    Error.NonExistingParam(null, null, null, "99", "101"),
                    Error.NonExistingParam(null, null, null, "199", "101"),
                    
		            // Wrappers (not yet covered)
                    //Error.NonExistingParam(null, null, null, "999", "101"),
                    //Error.NonExistingParam(null, null, null, "1099", "101"),
                    //Error.NonExistingParam(null, null, null, "1199", "101"),
                    //Error.NonExistingParam(null, null, null, "1299", "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolSetParameter_NonExistingParam_NoParamsTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingParam_NoParamsTag",
                ExpectedResults = new List<IValidationResult>
                {
                    // QAction 100: SetParameter
		            // Hard-coded
                    Error.HardCodedPid(null, null, null, "9", "100"),
                    Error.HardCodedPid(null, null, null, "19", "100"),
                    Error.HardCodedPid(null, null, null, "-29", "100"),
                    Error.NonExistingParam(null, null, null, "9", "100"),
                    Error.NonExistingParam(null, null, null, "19", "100"),
                    Error.NonExistingParam(null, null, null, "-29", "100"),
                    
		            // Variables
                    Error.HardCodedPid(null, null, null, "99", "100"),
                    Error.HardCodedPid(null, null, null, "199", "100"),
                    Error.NonExistingParam(null, null, null, "99", "100"),
                    Error.NonExistingParam(null, null, null, "199", "100"),
                    
		            // Wrappers (not yet covered)
                    //Error.NonExistingParam(null, null, null, "999", "100"),
                    //Error.NonExistingParam(null, null, null, "1099", "100"),
                    //Error.NonExistingParam(null, null, null, "1199", "100"),
                    //Error.NonExistingParam(null, null, null, "1299", "100"),

                    
                    // QAction 101: SetParameter_HistorySet
		            // Hard-coded
                    Error.HardCodedPid(null, null, null, "9", "101"),
                    Error.HardCodedPid(null, null, null, "19", "101"),
                    Error.HardCodedPid(null, null, null, "-29", "101"),
                    Error.NonExistingParam(null, null, null, "9", "101"),
                    Error.NonExistingParam(null, null, null, "19", "101"),
                    Error.NonExistingParam(null, null, null, "-29", "101"),
                    
		            // Variables
                    Error.HardCodedPid(null, null, null, "99", "101"),
                    Error.HardCodedPid(null, null, null, "199", "101"),
                    Error.NonExistingParam(null, null, null, "99", "101"),
                    Error.NonExistingParam(null, null, null, "199", "101"),
                    
		            // Wrappers (not yet covered)
                    //Error.NonExistingParam(null, null, null, "999", "101"),
                    //Error.NonExistingParam(null, null, null, "1099", "101"),
                    //Error.NonExistingParam(null, null, null, "1199", "101"),
                    //Error.NonExistingParam(null, null, null, "1299", "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolSetParameter_ParamMissingHistorySet()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ParamMissingHistorySet",
                ExpectedResults = new List<IValidationResult>
                {
                    // Hard-coded
                    Error.HardCodedPid(null, null, null, "200", "101"),
                    Error.HardCodedPid(null, null, null, "201", "101"),
                    Error.ParamMissingHistorySet(null, null, null, "200"),
                    Error.ParamMissingHistorySet(null, null, null, "201"),
                    
                    // Variables
                    Error.HardCodedPid(null, null, null, "200", "101"),
                    Error.HardCodedPid(null, null, null, "201", "101"),
                    Error.ParamMissingHistorySet(null, null, null, "200"),
                    //Error.ParamMissingHistorySet(null, null, null, "201"),
                    Error.HardCodedPid(null, null, null, "200", "101"),
                    Error.HardCodedPid(null, null, null, "201", "101"),
                    Error.ParamMissingHistorySet(null, null, null, "200"),
                    //Error.ParamMissingHistorySet(null, null, null, "201"),
                    
                    // Parameter helper class
                    Error.ParamMissingHistorySet(null, null, null, "200"),
                    //Error.ParamMissingHistorySet(null, null, null, "201"),
                    Error.ParamMissingHistorySet(null, null, null, "200"),
                    //Error.ParamMissingHistorySet(null, null, null, "201"),
                    
                    // Wrappers: Not (yet) covered)
                    //Error.HardCodedPid(null, null, null, "200", "101"),
                    //Error.HardCodedPid(null, null, null, "201", "101"),
                    //Error.ParamMissingHistorySet(null, null, null, "200"),
                    //Error.ParamMissingHistorySet(null, null, null, "201"),
                    //Error.HardCodedPid(null, null, null, "200", "101"),
                    //Error.HardCodedPid(null, null, null, "201", "101"),
                    //Error.ParamMissingHistorySet(null, null, null, "200"),
                    //Error.ParamMissingHistorySet(null, null, null, "201"),
                    //Error.ParamMissingHistorySet(null, null, null, "200"),
                    //Error.ParamMissingHistorySet(null, null, null, "201"),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    //[TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CSharpSLProtocolSetParameter();

        [TestMethod]
        public void Protocol_CSharpSLProtocolSetParameter_ParamMissingHistorySet()
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
        public void QAction_CSharpSLProtocolSetParameter_NonExistingParam()
        {
            // Create ErrorMessage
            var message = Error.NonExistingParam(null, null, null, "1", "2");

            var expected = new ValidationResult()
            {
                ErrorId = 1,
                FullId = "3.7.1",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Method 'SLProtocol.SetParameter' references a non-existing 'Param' with ID '1'. QAction ID '2'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolSetParameter_HardCodedPid()
        {
            // Create ErrorMessage
            var message = Error.HardCodedPid(null, null, null, "1", "2");

            var expected = new ValidationResult()
            {
                ErrorId = 2,
                FullId = "3.7.2",
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
        public void QAction_CSharpSLProtocolSetParameter_ParamMissingHistorySet()
        {
            // Create ErrorMessage
            var message = Error.ParamMissingHistorySet(null, null, null, "1");

            var expected = new ValidationResult()
            {
                ErrorId = 3,
                FullId = "3.7.3",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "SLProtocol.SetParameter overload with 'ValueType timeInfo' argument requires 'Param@historySet=true'. Param ID '1'.",
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
        private readonly IRoot check = new CSharpSLProtocolSetParameter();

        [TestMethod]
        public void QAction_CSharpSLProtocolSetParameter_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpSLProtocolSetParameter_CheckId() => Generic.CheckId(check, CheckId.CSharpSLProtocolSetParameter);
    }
}