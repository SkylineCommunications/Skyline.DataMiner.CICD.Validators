namespace ProtocolTests.Protocol.Timers.Timer.Content.Group.CheckGroupTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Timers.Timer.Content.Group.CheckGroupTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckGroupTag();

        #region Valid Checks

        [TestMethod]
        public void Timer_CheckGroupTag_Valid()
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
        public void Timer_CheckGroupTag_NonExistingIdInGroup()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdInGroup",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingIdInGroup(null, null, null, "Group", "10", "1")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckGroupTag_EmptyGroupTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyGroupTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyGroupTag(null, null, null, "1")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckGroupTag_InvalidTableIdInGroupTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidTableIdInGroupTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidGroupTag(null, null, null, "col:10:12", "1"),
                    Error.InvalidGroupTag(null, null, null, "col:10:12", "2"),
                    Error.InvalidGroupTag(null, null, null, "col:3:4", "3")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckGroupTag_InvalidGroupTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidGroupTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidGroupTag(null, null, null, "col10:20", "1"),
                    Error.InvalidGroupTag(null, null, null, "col:5:20", "2").WithSubResults(
                        Error.NonExistingIdInGroup(null, null, null, "Column index Table '2000' (0-based)", "5", "2" ),
                        Error.NonExistingIdInGroup(null, null, null, "Group", "20", "2")),
                    Error.InvalidGroupTag(null, null, null, "col:5:20", "3"),
                    Error.InvalidGroupTag(null, null, null, "abc", "4")
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    [TestCategory("Attribute")]
    public class Attribute
    {
        private readonly IRoot check = new CheckGroupTag();

        [TestMethod]
        public void Timer_CheckGroupTag_CheckCategory() => Generic.CheckCategory(check, Category.Timer);

        [TestMethod]
        public void Timer_CheckGroupTag_CheckId() => Generic.CheckId(check, CheckId.CheckGroupTag);
    }
}