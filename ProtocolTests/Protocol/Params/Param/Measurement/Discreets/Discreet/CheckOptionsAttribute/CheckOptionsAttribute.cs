namespace ProtocolTests.Protocol.Params.Param.Measurement.Discreets.Discreet.CheckOptionsAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Discreets.Discreet.CheckOptionsAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckOptionsAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckOptionsAttribute_Valid()
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
        public void Param_CheckOptionsAttribute_EmptyConfirmOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyConfirmOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MisconfiguredConfirmOptions(null, null, null, "199").WithSubResults(
                        Error.EmptyConfirmOption(null, null, null, "Random", "199"),
                        Error.EmptyConfirmOption(null, null, null, "Delete all", "199"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_MisconfiguredConfirmOptions()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MisconfiguredConfirmOptions",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MisconfiguredConfirmOptions(null, null, null, "199").WithSubResults(
                        Error.MissingConfirmOption(null, null, null, "Delete", "199"),
                        Error.EmptyConfirmOption(null, null, null, "Delete selected item(s)", "199"),
                        Error.UntrimmedConfirmOption(null, null, null, "Delete all", "199", "All rows will be deleted permanently.    "))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_MissingConfirmOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingConfirmOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MisconfiguredConfirmOptions(null, null, null, "199").WithSubResults(
                        Error.MissingConfirmOption(null, null, null, "Delete all", "199"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_UntrimmedConfirmOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedConfirmOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MisconfiguredConfirmOptions(null, null, null, "199").WithSubResults(
                        Error.UntrimmedConfirmOption(null, null, null, "Random", "199", " My untrimmed confirm message on normal action.   "),

                        Error.UntrimmedConfirmOption(null, null, null, "Delete all", "199", " My untrimmed confirm message on critical action   "),
                        Error.UntrimmedConfirmOption(null, null, null, "Delete with options before", "199", " My untrimmed confirm message on critical action with other options before   "),
                        Error.UntrimmedConfirmOption(null, null, null, "Delete with options after", "199", " My untrimmed confirm message on critical action with other options after   "))
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly IRoot codeFix = new CheckOptionsAttribute();

        [TestMethod]
        public void Param_CheckDisplayTag_InvalidPagebuttonCaption()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedConfirmOption",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckOptionsAttribute();

        [TestMethod]
        public void Param_CheckOptionsAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckOptionsAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckOptionsAttribute);
    }
}