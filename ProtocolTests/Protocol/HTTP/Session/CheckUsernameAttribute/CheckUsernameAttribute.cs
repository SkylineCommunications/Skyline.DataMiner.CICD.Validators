namespace ProtocolTests.Protocol.HTTP.Session.CheckUsernameAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.HTTP.Session.CheckUsernameAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckUsernameAttribute();

        #region Valid Checks

        [TestMethod]
        public void HTTP_CheckUsernameAttribute_Valid()
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
        public void HTTP_CheckUsernameAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001", "1")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void HTTP_CheckUsernameAttribute_NonExistingIdNoParamsTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdNoParamsTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001", "1")
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
        public void HTTP_CheckUsernameAttribute_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "0", "1");

            string description = "Attribute 'userName' references a non-existing 'Param' with ID '0'. HTTP Session ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckUsernameAttribute();

        [TestMethod]
        public void HTTP_CheckUsernameAttribute_CheckCategory() => Generic.CheckCategory(check, Category.HTTP);

        [TestMethod]
        public void HTTP_CheckUsernameAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckUsernameAttribute);
    }
}