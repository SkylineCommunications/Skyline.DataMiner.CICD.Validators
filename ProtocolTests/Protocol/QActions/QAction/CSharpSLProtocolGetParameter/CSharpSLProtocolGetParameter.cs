namespace ProtocolTests.Protocol.QActions.QAction.CSharpSLProtocolGetParameter
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpSLProtocolGetParameter;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpSLProtocolGetParameter();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpSLProtocolGetParameter_Valid()
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
        public void QAction_CSharpSLProtocolGetParameter_HardCodedPid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "HardCodedPid",
                ExpectedResults = new List<IValidationResult>
                {
                    // Direct
                    Error.HardCodedPid(null, null, null, "100", "101"),
                    Error.HardCodedPid(null, null, null, "100", "101"),
                    Error.HardCodedPid(null, null, null, "100", "101"),
                    Error.HardCodedPid(null, null, null, "200", "101"),
                    Error.HardCodedPid(null, null, null, "300", "101"),
                    Error.HardCodedPid(null, null, null, "350", "101"),
                    
                    // Simple Math
                    Error.HardCodedPid(null, null, null, "100", "101"),
                    Error.HardCodedPid(null, null, null, "100", "101"),
                    Error.HardCodedPid(null, null, null, "100", "101"),
                    Error.HardCodedPid(null, null, null, "100", "101"),
                    
                    // Advanced Math
                    Error.HardCodedPid(null, null, null, "100", "101"),
                    Error.HardCodedPid(null, null, null, "100", "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolGetParameter_NonExistingParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingParam",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.HardCodedPid(null, null, null, "9", "101"),
                    Error.HardCodedPid(null, null, null, "19", "101"),
                    Error.HardCodedPid(null, null, null, "-29", "101"),

                    Error.HardCodedPid(null, null, null, "99", "101"),
                    Error.HardCodedPid(null, null, null, "199", "101"),

                    Error.NonExistingParam(null, null, null, "9", "101"),
                    Error.NonExistingParam(null, null, null, "19", "101"),
                    Error.NonExistingParam(null, null, null, "-29", "101"),

                    Error.NonExistingParam(null, null, null, "99", "101"),
                    Error.NonExistingParam(null, null, null, "199", "101"),

                    //Error.NonExistingParam(null, null, null, "999", "101"),   // Not (yet) covered
                    //Error.NonExistingParam(null, null, null, "1099", "101"),  // Not (yet) covered
                    //Error.NonExistingParam(null, null, null, "1199", "101"),  // Not (yet) covered
                    //Error.NonExistingParam(null, null, null, "1299", "101"),  // Not (yet) covered
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolGetParameter_NonExistingParam_NoParamsTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingParam_NoParamsTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.HardCodedPid(null, null, null, "9", "101"),
                    Error.HardCodedPid(null, null, null, "-29", "101"),
                    Error.HardCodedPid(null, null, null, "19", "101"),

                    Error.HardCodedPid(null, null, null, "99", "101"),
                    Error.HardCodedPid(null, null, null, "199", "101"),

                    Error.NonExistingParam(null, null, null, "9", "101"),
                    Error.NonExistingParam(null, null, null, "19", "101"),
                    Error.NonExistingParam(null, null, null, "-29", "101"),

                    Error.NonExistingParam(null, null, null, "99", "101"),
                    Error.NonExistingParam(null, null, null, "199", "101"),

                    //Error.NonExistingParam(null, null, null, "999", "101"),   // Not (yet) covered
                    //Error.NonExistingParam(null, null, null, "1099", "101"),  // Not (yet) covered
                    //Error.NonExistingParam(null, null, null, "1199", "101"),  // Not (yet) covered
                    //Error.NonExistingParam(null, null, null, "1299", "101"),  // Not (yet) covered
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
        public void QAction_CSharpSLProtocolGetParameter_NonExistingParam()
        {
            // Create ErrorMessage
            var message = Error.NonExistingParam(null, null, null, "1", "2");

            var expected = new ValidationResult()
            {
                ErrorId = 1,
                FullId = "3.6.1",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                Description = "Method 'SLProtocol.GetParameter' references a non-existing 'Param' with ID '1'. QAction ID '2'.",
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
        private readonly IRoot check = new CSharpSLProtocolGetParameter();

        [TestMethod]
        public void QAction_CSharpSLProtocolGetParameter_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpSLProtocolGetParameter_CheckId() => Generic.CheckId(check, CheckId.CSharpSLProtocolGetParameter);
    }
}