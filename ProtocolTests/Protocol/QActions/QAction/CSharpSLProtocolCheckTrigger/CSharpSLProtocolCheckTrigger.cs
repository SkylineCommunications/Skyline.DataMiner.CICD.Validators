namespace ProtocolTests.Protocol.QActions.QAction.CSharpSLProtocolCheckTrigger
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpSLProtocolCheckTrigger;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpSLProtocolCheckTrigger();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpSLProtocolCheckTrigger_Valid()
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
        public void QAction_CSharpSLProtocolCheckTrigger_NonExistingTrigger()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingTrigger",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingTrigger(null, null, null, "10", "100"),
                    Error.NonExistingTrigger(null, null, null, "10", "100"),
                    Error.NonExistingTrigger(null, null, null, "11", "100"),
                    Error.NonExistingTrigger(null, null, null, "15", "100"),
                    Error.NonExistingTrigger(null, null, null, "14", "100"),
                    Error.NonExistingTrigger(null, null, null, "10", "100"),
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
        public void QAction_CSharpSLProtocolCheckTrigger_NonExistingTrigger()
        {
            // Create ErrorMessage
            var message = Error.NonExistingTrigger(null, null, null, "10", "100");

            string description = "Method 'SLProtocol.CheckTrigger' references a non-existing 'Trigger' with ID '10'. QAction ID '100'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CSharpSLProtocolCheckTrigger();

        [TestMethod]
        public void QAction_CSharpSLProtocolCheckTrigger_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpSLProtocolCheckTrigger_CheckId() => Generic.CheckId(check, CheckId.CSharpSLProtocolCheckTrigger);
    }
}