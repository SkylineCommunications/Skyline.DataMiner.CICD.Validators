namespace ProtocolTests.Protocol.Groups.Group.Content.Param.CheckParamTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Groups.Group.Content.Param.CheckParamTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckParamTag();

        #region Valid Checks

        [TestMethod]
        public void Group_CheckParamTag_Valid()
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
        public void Group_CheckParamTag_EmptyParamTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyParamTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyParamTag(null, null, null, "1"),
                    Error.EmptyParamTag(null, null, null, "2"),
                    Error.EmptyParamTag(null, null, null, "3"),
                    Error.EmptyParamTag(null, null, null, "3")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckParamTag_InvalidParamSuffix()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidParamSuffix",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidParamSuffix(null, null, null, "typo", "1000"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckParamTag_InvalidParamTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidParamTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidParamTag(null, null, null, "test1", "1"),
                    Error.InvalidParamTag(null, null, null, "test2", "2"),
                    Error.InvalidParamTag(null, null, null, "test3", "3"),
                    Error.InvalidParamTag(null, null, null, "test4", "3"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckParamTag_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001", "1"),
                    Error.NonExistingId(null, null, null, "1002", "2"),
                    Error.NonExistingId(null, null, null, "1003", "3"),
                    Error.NonExistingId(null, null, null, "1004", "3")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckParamTag_NonExistingIdNoParamsTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdNoParamsTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001", "1"),
                    Error.NonExistingId(null, null, null, "1001", "2"),
                    Error.NonExistingId(null, null, null, "1002", "2")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckParamTag_ObsoleteSuffixTable()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ObsoleteSuffixTable",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ObsoleteSuffixTable(null, null, null, "1000"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckParamTag_SuffixRequiresMultiThreadedTimer()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "SuffixRequiresMultiThreadedTimer",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.SuffixRequiresMultiThreadedTimer(null, null, null, "single", "999"),
                    Error.SuffixRequiresMultiThreadedTimer(null, null, null, "instance", "999"),
                    Error.SuffixRequiresMultiThreadedTimer(null, null, null, "tablev2", "999"),
                    Error.SuffixRequiresMultiThreadedTimer(null, null, null, "getnext", "999"),

                    Error.SuffixRequiresMultiThreadedTimer(null, null, null, "single", "1000"),
                    Error.SuffixRequiresMultiThreadedTimer(null, null, null, "instance", "1001"),
                    Error.SuffixRequiresMultiThreadedTimer(null, null, null, "tablev2", "1002"),
                    Error.SuffixRequiresMultiThreadedTimer(null, null, null, "getnext", "1003"),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckParamTag();

        [TestMethod]
        public void Group_CheckParamTag_ObsoleteSuffixTable()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "ObsoleteSuffixTable",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Group_CheckParamTag_EmptyParamTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyParamTag(null, null, null, "0");

            string description = "Empty tag 'Content/Param' in Group '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Group_CheckParamTag_InvalidParamTag()
        {
            // Create ErrorMessage
            var message = Error.InvalidParamTag(null, null, null, "test", "1");

            string description = "Invalid value 'test' in tag 'Content/Param'. Group ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
        [TestMethod]

        public void Group_CheckParamTag_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "0", "1");

            string description = "Tag 'Content/Param' references a non-existing 'Param' with ID '0'. Group ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckParamTag();

        [TestMethod]
        public void Group_CheckParamTag_CheckCategory() => Generic.CheckCategory(check, Category.Group);

        [TestMethod]
        public void Group_CheckParamTag_CheckId() => Generic.CheckId(check, CheckId.CheckParamTag);
    }
}