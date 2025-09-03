namespace ProtocolTests.Protocol.QActions.QAction.CSharpSLProtocolTriggerAction
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpSLProtocolTriggerAction;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpSLProtocolTriggerAction();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpSLProtocolTriggerAction_Valid()
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
        public void QAction_CSharpSLProtocolTriggerAction_NonExistingActionId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingActionId",
                ExpectedResults = new List<IValidationResult>
                {
		            // Hard-coded (1x)
                    Error.NonExistingActionId(null, null, null, "10", "101"),
                    Error.NonExistingActionId(null, null, null, "11", "101"),
                    Error.NonExistingActionId(null, null, null, "12", "101"),
                    
		            // Hard-coded Subtractions (2x)
                    Error.NonExistingActionId(null, null, null, "20", "101"),
                    Error.NonExistingActionId(null, null, null, "21", "101"),
                    Error.NonExistingActionId(null, null, null, "22", "101"),
                    
		            // Hard-coded additions (3x)
                    Error.NonExistingActionId(null, null, null, "30", "101"),
                    Error.NonExistingActionId(null, null, null, "31", "101"),
                    Error.NonExistingActionId(null, null, null, "32", "101"),

		            // Local variable (4x)
                    Error.NonExistingActionId(null, null, null, "40", "101"),
                    Error.NonExistingActionId(null, null, null, "41", "101"),
                    Error.NonExistingActionId(null, null, null, "42", "101"),

                    // Distant variable (5x)
                    Error.NonExistingActionId(null, null, null, "50", "101"),
                    Error.NonExistingActionId(null, null, null, "51", "101"),
                    Error.NonExistingActionId(null, null, null, "52", "101"),

                    // Method wrapper (6x)
                    //Error.NonExistingActionId(null, null, null, "60", "101"),   // Not yet covered
                    //Error.NonExistingActionId(null, null, null, "61", "101"),   // Not yet covered
                    //Error.NonExistingActionId(null, null, null, "62", "101"),   // Not yet covered

                    // Method wrapper (7x)
                    //Error.NonExistingActionId(null, null, null, "70", "101"),   // Not yet covered
                    //Error.NonExistingActionId(null, null, null, "71", "101"),   // Not yet covered
                    //Error.NonExistingActionId(null, null, null, "72", "101"),   // Not yet covered

                    // Method wrapper (8x)
                    //Error.NonExistingActionId(null, null, null, "80", "101"),   // Not yet covered
                    //Error.NonExistingActionId(null, null, null, "81", "101"),   // Not yet covered
                    //Error.NonExistingActionId(null, null, null, "82", "101"),   // Not yet covered
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolTriggerAction_NonExistingActionIdNoAction()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingActionId_NoAction",
                ExpectedResults = new List<IValidationResult>
                {
		            // Hard-coded (1x)
                    Error.NonExistingActionId(null, null, null, "10", "101"),
                    Error.NonExistingActionId(null, null, null, "11", "101"),
                    Error.NonExistingActionId(null, null, null, "12", "101"),
                    
		            // Hard-coded Subtractions (2x)
                    Error.NonExistingActionId(null, null, null, "20", "101"),
                    Error.NonExistingActionId(null, null, null, "21", "101"),
                    Error.NonExistingActionId(null, null, null, "22", "101"),
                    
		            // Hard-coded additions (3x)
                    Error.NonExistingActionId(null, null, null, "30", "101"),
                    Error.NonExistingActionId(null, null, null, "31", "101"),
                    Error.NonExistingActionId(null, null, null, "32", "101"),

		            // Local variable (4x)
                    Error.NonExistingActionId(null, null, null, "40", "101"),
                    Error.NonExistingActionId(null, null, null, "41", "101"),
                    Error.NonExistingActionId(null, null, null, "42", "101"),

                    // Distant variable (5x)
                    Error.NonExistingActionId(null, null, null, "50", "101"),
                    Error.NonExistingActionId(null, null, null, "51", "101"),
                    Error.NonExistingActionId(null, null, null, "52", "101"),

                    // Method wrapper (6x)
                    //Error.NonExistingActionId(null, null, null, "60", "101"),   // Not yet covered
                    //Error.NonExistingActionId(null, null, null, "61", "101"),   // Not yet covered
                    //Error.NonExistingActionId(null, null, null, "62", "101"),   // Not yet covered

                    // Method wrapper (7x)
                    //Error.NonExistingActionId(null, null, null, "70", "101"),   // Not yet covered
                    //Error.NonExistingActionId(null, null, null, "71", "101"),   // Not yet covered
                    //Error.NonExistingActionId(null, null, null, "72", "101"),   // Not yet covered

                    // Method wrapper (8x)
                    //Error.NonExistingActionId(null, null, null, "80", "101"),   // Not yet covered
                    //Error.NonExistingActionId(null, null, null, "81", "101"),   // Not yet covered
                    //Error.NonExistingActionId(null, null, null, "82", "101"),   // Not yet covered
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
        public void QAction_CSharpSLProtocolTriggerAction_NonExistingActionId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingActionId(null, null, null, "1", "2");

            var expected = new ValidationResult()
            {
                ErrorId = 1,
                FullId = "3.5.1",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                Description = "Method 'NotifyProtocol(221/*NT_RUN_ACTION*/, ...)' references a non-existing 'Action' with ID '1'. QAction ID '2'.",
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
        private readonly IRoot check = new CSharpSLProtocolTriggerAction();

        [TestMethod]
        public void QAction_CSharpSLProtocolTriggerAction_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpSLProtocolTriggerAction_CheckId() => Generic.CheckId(check, CheckId.CSharpSLProtocolTriggerAction);
    }
}