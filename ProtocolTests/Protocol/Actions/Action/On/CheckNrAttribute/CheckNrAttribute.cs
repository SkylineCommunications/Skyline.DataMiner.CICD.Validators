namespace ProtocolTests.Protocol.Actions.Action.On.CheckNrAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Actions.Action.On.CheckNrAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckNrAttribute();

        #region Valid Checks

        [TestMethod]
        public void Action_CheckNrAttribute_Valid()
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
        public void Action_CheckNrAttribute_EmptyAttibute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttibute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttibute(null, null, null, "1"),
                    Error.EmptyAttibute(null, null, null, "2"),
                    Error.EmptyAttibute(null, null, null, "3"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Currently not expecting any result")]
        public void Action_CheckNrAttribute_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    // There is currently no such thing as invalid value for this attribute as any string is authorized.
                    // Note that this should only cover invalid values according to xsd/ProtocolModel.
                    // Further parsing is done via the CheckActionTypes check
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckNrAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, " 0;1", "1"),
                    Error.UntrimmedAttribute(null, null, null, "0;1  ", "2"),
                    Error.UntrimmedAttribute(null, null, null, "  0;1 ", "3"),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckNrAttribute();

        [TestMethod]
        public void Action_CheckOnTag_UntrimmedAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedAttribute",
            };

            Generic.Fix(check, data);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckNrAttribute();

        [TestMethod]
        public void Action_CheckNrAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Action);

        [TestMethod]
        public void Action_CheckNrAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckNrAttribute);
    }
}