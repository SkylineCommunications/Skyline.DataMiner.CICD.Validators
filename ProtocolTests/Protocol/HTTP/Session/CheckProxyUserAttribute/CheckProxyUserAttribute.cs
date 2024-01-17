namespace ProtocolTests.Protocol.HTTP.Session.CheckProxyUserAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.HTTP.Session.CheckProxyUserAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckProxyUserAttribute();

        #region Valid Checks

        [TestMethod]
        public void HTTP_CheckProxyUserAttribute_Valid()
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
        public void HTTP_CheckProxyUserAttribute_NonExistingId()
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
        public void HTTP_CheckProxyUserAttribute_NonExistingIdNoParamsTag()
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
        public void HTTP_CheckProxyUserAttribute_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "0", "1");

            string description = "Attribute 'proxyUser' references a non-existing 'Param' with ID '0'. HTTP Session ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckProxyUserAttribute();

        [TestMethod]
        public void HTTP_CheckProxyUserAttribute_CheckCategory() => Generic.CheckCategory(check, Category.HTTP);

        [TestMethod]
        public void HTTP_CheckProxyUserAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckProxyUserAttribute);
    }
}