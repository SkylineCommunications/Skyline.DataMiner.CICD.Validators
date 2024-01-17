namespace ProtocolTests.Protocol.Params.Param.Message.CheckMessageTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Message.CheckMessageTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckMessageTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckMessageTag_Valid()
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
        public void Param_CheckMessageTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag",
                ExpectedResults = new List<IValidationResult>
                {
                    // No Confirm Pop-up
                    Error.MissingTag(null, null, null, "100").WithSubResults(
                        Error.MissingTag_Sub(null, null, null, "Reboot")),
                    Error.MissingTag(null, null, null, "101").WithSubResults(
                        Error.MissingTag_Sub(null, null, null, "Shutdown")),
                    Error.MissingTag(null, null, null, "102").WithSubResults(
                        Error.MissingTag_Sub(null, null, null, "Reboot"),
                        Error.MissingTag_Sub(null, null, null, "Shutdown")),
                    Error.MissingTag(null, null, null, "103").WithSubResults(
                        Error.MissingTag_Sub(null, null, null, "Reboot")),
                    
                    // Confirm Pop-up != always
                    Error.MissingTag(null, null, null, "1000").WithSubResults(
                        Error.MissingTag_Sub(null, null, null, "Reboot")),
                    Error.MissingTag(null, null, null, "1001").WithSubResults(
                        Error.MissingTag_Sub(null, null, null, "Reboot"))
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckMessageTag();

        [TestMethod]
        public void Param_CheckMessageTag_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckMessageTag_CheckId() => Generic.CheckId(check, CheckId.CheckMessageTag);
    }
}