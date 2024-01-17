namespace ProtocolTests.Protocol.ParameterGroups.Group.CheckNameAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ParameterGroups.Group.CheckNameAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckNameAttribute();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckNameAttribute_Valid()
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
        public void Protocol_CheckNameAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null, "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckNameAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckNameAttribute_InvalidChars()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidChars",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidChars(null, null, null, "Invalid\\Char", "\\"),
                    Error.InvalidChars(null, null, null, " Invalid\\Char3 ", "\\"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckNameAttribute_LengthyValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "LengthyValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.LengthyValue(null, null, null, "RidiculousLongNameThatIsTooLongToHandle"),
                    Error.LengthyValue(null, null, null, " RidiculousLongNameThatIsTooLongToHandle3 "),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckNameAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "1", "TestName "),
                    Error.UntrimmedAttribute(null, null, null, "2", " TestName2 "),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckNameAttribute_DuplicatedValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicatedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicatedValue(null, null, null, "Name1", "1, 2").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "Name1", "1"),
                        Error.DuplicatedValue(null, null, null, "Name1", "2"))
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckNameAttribute();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckNameAttribute_Valid()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Protocol_CheckNameAttribute_DcfParameterGroupNameChanged()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "DcfParameterGroupNameChanged",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.DcfParameterGroupNameChanged(null, null, "1", "Stand Alone Input", "Rename"),
                    ErrorCompare.DcfParameterGroupNameChanged(null, null, "2", "Outputs", ""),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix test = new CheckNameAttribute();

        [TestMethod]
        public void Protocol_CheckNameAttribute_UntrimmedAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedAttribute",
            };

            Generic.Fix(test, data);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckNameAttribute();

        [TestMethod]
        public void Protocol_CheckNameAttribute_CheckCategory() => Generic.CheckCategory(root, Category.ParameterGroup);

        [TestMethod]
        public void Protocol_CheckNameAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckNameAttribute);
    }
}