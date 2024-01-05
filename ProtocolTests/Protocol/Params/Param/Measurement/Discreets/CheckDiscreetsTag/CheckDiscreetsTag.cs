namespace ProtocolTests.Protocol.Params.Param.Measurement.Discreets.CheckDiscreetsTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Discreets.CheckDiscreetsTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckDiscreetsTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckDiscreetsTag_Valid()
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
        public void Param_CheckDiscreetsTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null, "discreet", "1"),
                    Error.MissingTag(null, null, null, "button", "2"),
                    Error.MissingTag(null, null, null, "pagebutton", "3"),
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
        public void Param_CheckDiscreetsTag_MissingTag()
        {
            // Create ErrorMessage
            var message = Error.MissingTag(null, null, null, "button", "0");

            string description = "Missing 'Measurement/Discreets' tag for 'button' Param with ID '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckDiscreetsTag();

        [TestMethod]
        public void Param_CheckDiscreetsTag_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckDiscreetsTag_CheckId() => Generic.CheckId(check, CheckId.CheckDiscreetsTag);
    }
}