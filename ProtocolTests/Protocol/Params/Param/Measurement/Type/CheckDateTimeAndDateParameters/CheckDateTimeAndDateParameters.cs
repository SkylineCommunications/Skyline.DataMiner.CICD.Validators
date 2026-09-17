namespace ProtocolTests.Protocol.Params.Param.Measurement.Type.CheckDateTimeAndDateParameters
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Type.CheckDateTimeAndDateParameters;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckDateTimeAndDateParameters();

        [TestMethod]
        public void Param_CheckDateTimeAndDateParameters_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>(),
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDateTimeAndDateParameters_InvalidInterpreteDecimals()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidInterpreteDecimals",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidInterpreteDecimals(null, null, null, "1001"),
                },
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDateTimeAndDateParameters_InvalidDisplayDecimals()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidDisplayDecimals",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidDisplayDecimals(null, null, null, "1001"),
                },
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDateTimeAndDateParameters_MissingDisableHeaderSum()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingDisableHeaderSum",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingDisableHeaderSum(null, null, null, "1001", "1000"),
                },
            };

            Generic.Validate(test, data);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckDateTimeAndDateParameters();

        [TestMethod]
        public void Param_CheckDateTimeAndDateParameters_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckDateTimeAndDateParameters_CheckId() => Generic.CheckId(root, CheckId.CheckDateTimeAndDateParameters);
    }
}
