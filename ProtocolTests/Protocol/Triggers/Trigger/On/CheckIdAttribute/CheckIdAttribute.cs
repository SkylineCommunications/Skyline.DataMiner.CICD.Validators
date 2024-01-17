namespace ProtocolTests.Protocol.Triggers.Trigger.On.CheckIdAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Triggers.Trigger.On.CheckIdAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckIdAttribute();

        #region Valid Checks

        [TestMethod]
        public void Trigger_CheckIdAttribute_Valid()
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
        public void Trigger_CheckIdAttribute_MissingAttribute()
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
        public void Trigger_CheckIdAttribute_ExcessiveAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ExcessiveAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ExcessiveAttribute(null, null, null, "protocol", "1"),
                    Error.ExcessiveAttribute(null, null, null, "communication", "2"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckIdAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null),
                    Error.EmptyAttribute(null, null, null),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckIdAttribute_MultipleIds()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MultipleIds",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MultipleIds(null, null, null, "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckIdAttribute_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "a", "each, {uint}"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckIdAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "command", "1", "11"),
                    Error.NonExistingId(null, null, null, "command", "each", "12"),

                    Error.NonExistingId(null, null, null, "group", "1", "31"),
                    Error.NonExistingId(null, null, null, "group", "each", "32"),

                    Error.NonExistingId(null, null, null, "pair", "1", "41"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckIdAttribute_LeadingZeros()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "LeadingZeros",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.LeadingZeros(null, null, null, "1", "01"),
                    Error.LeadingZeros(null, null, null, "2", "002"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckIdAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, " 1"),
                    Error.UntrimmedAttribute(null, null, null, "2 "),
                    Error.UntrimmedAttribute(null, null, null, " 3 "),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckIdAttribute();

        [TestMethod]
        public void Trigger_CheckIdAttribute_ExcessiveAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "ExcessiveAttribute",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Trigger_CheckIdAttribute_LeadingZeros()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "LeadingZeros",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Trigger_CheckIdAttribute_UntrimmedAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedAttribute",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckIdAttribute();

        [TestMethod]
        public void Trigger_CheckIdAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Trigger);

        [TestMethod]
        public void Trigger_CheckIdAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckIdAttribute);
    }
}