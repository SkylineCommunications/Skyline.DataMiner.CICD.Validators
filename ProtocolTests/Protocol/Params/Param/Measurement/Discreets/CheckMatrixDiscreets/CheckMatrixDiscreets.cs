namespace ProtocolTests.Protocol.Params.Param.Measurement.Discreets.CheckMatrixDiscreets
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Discreets.CheckMatrixDiscreets;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckMatrixDiscreets();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckMatrixDiscreets_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckMatrixDiscreets_InvalidDiscreetCount()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidDiscreetCount",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidDiscreetCount(null, null, null, "49", "48", "3"),
                    Error.InvalidDiscreetCount(null, null, null, "49", "48", "4"),
                    Error.InvalidDiscreetCount(null, null, null, "0", "48", "5"),
                    Error.InvalidDiscreetCount(null, null, null, "0", "48", "6"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckMatrixDiscreets_MissingDiscreetValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingDiscreetValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingDiscreetValue(null, null, null, "48", "1"),
                    Error.MissingDiscreetValue(null, null, null, "16", "2"),
                    Error.MissingDiscreetValue(null, null, null, "6;7;13;31;32;45", "3"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckMatrixDiscreets_DiscreetsNotOneBased()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DiscreetsNotOneBased",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DiscreetsNotOneBased(null, null, null, "1"),
                    Error.DiscreetsNotOneBased(null, null, null, "2"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckMatrixDiscreets();

        [TestMethod]
        public void Param_CheckMatrixDiscreets_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckMatrixDiscreets_CheckId() => Generic.CheckId(root, CheckId.CheckMatrixDiscreets);
    }
}