namespace ProtocolTests.Protocol.SNMP.CheckIncludepagesAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.SNMP.CheckIncludepagesAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckIncludepagesAttribute();

        [TestMethod]
        public void Protocol_CheckIncludepagesAttribute_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckIncludepagesAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckIncludepagesAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckIncludepagesAttribute_InvalidAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "abc", "true; false"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckIncludepagesAttribute_InvalidAttributeCasing()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAttributeCasing",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "True", "true; false"),
                }
            };

            Generic.Validate(test, data);
        }
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckIncludepagesAttribute();

        [TestMethod]
        public void Protocol_CheckIncludepagesAttribute_MissingAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingAttribute",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Protocol_CheckIncludepagesAttribute_InvalidAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "InvalidAttribute",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Protocol_CheckIncludepagesAttribute_InvalidAttributeCasing()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "InvalidAttributeCasing",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Protocol_CheckIncludepagesAttribute_EmptyAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "EmptyAttribute",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckIncludepagesAttribute();

        [TestMethod]
        public void Protocol_CheckIncludepagesAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckIncludepagesAttriubte_CheckId() => Generic.CheckId(root, CheckId.CheckIncludepagesAttribute);
    }
}