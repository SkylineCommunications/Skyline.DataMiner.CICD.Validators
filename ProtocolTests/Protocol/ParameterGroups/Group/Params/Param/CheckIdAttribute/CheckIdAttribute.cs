namespace ProtocolTests.Protocol.ParameterGroups.Group.Params.Param.CheckIdAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ParameterGroups.Group.Params.Param.CheckIdAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckIdAttribute();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckIdAttribute_Valid()
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
        public void Protocol_CheckIdAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "101", "1000"),

                    Error.NonExistingId(null, null, null, "101", "1001"),
                    Error.NonExistingId(null, null, null, "102", "1001"),

                    Error.NonExistingId(null, null, null, "101", "1002"),
                    Error.NonExistingId(null, null, null, "102", "1002"),

                    Error.NonExistingId(null, null, null, "1002", "1100"),

                    Error.NonExistingId(null, null, null, "101", "1200"),
                    Error.NonExistingId(null, null, null, "1002", "1200"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckIdAttribute_DuplicateParamInParameterGroup()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateParamInParameterGroup",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicateParamInParameterGroup(null, null, null, "10", "1").WithSubResults(
                        Error.DuplicateParamInParameterGroup(null, null, null, "10", "1"),
                        Error.DuplicateParamInParameterGroup(null, null, null, "10", "1"))
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckIdAttribute();

        [TestMethod]
        public void Protocol_CheckIdAttribute_CheckCategory() => Generic.CheckCategory(root, Category.ParameterGroup);

        [TestMethod]
        public void Protocol_CheckIdAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckIdAttribute);
    }
}