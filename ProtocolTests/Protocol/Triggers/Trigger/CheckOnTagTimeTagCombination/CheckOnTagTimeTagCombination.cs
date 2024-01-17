namespace ProtocolTests.Protocol.Triggers.Trigger.CheckOnTagTimeTagCombination
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Triggers.Trigger.CheckOnTagTimeTagCombination;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckOnTagTimeTagCombination();

        #region Valid Checks

        [TestMethod]
        public void Trigger_CheckOnTagTimeTagCombination_ValidAllCombination()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidAllCombination",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckOnTagTimeTagCombination_ValidNoDuplicate()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidNoDuplicate",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckOnTagTimeTagCombination_Valid()
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
        public void Trigger_CheckOnTagTimeTagCombination_InvalidOnTagTimeTagCombination()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidOnTagTimeTagCombination",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidOnTagTimeTagCombination(null, null, null, "parameter", "before", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckOnTagTimeTagCombination_DuplicateTrigger()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateTrigger",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicateTrigger(null, null, null, Certainty.Certain, "1, 2").WithSubResults(
                        Error.DuplicateTrigger(null, null, null, Certainty.Certain, "1"),
                        Error.DuplicateTrigger(null, null, null, Certainty.Certain, "2")),
                    Error.DuplicateTrigger(null, null, null, Certainty.Certain, "10, 11").WithSubResults(
                        Error.DuplicateTrigger(null, null, null, Certainty.Certain, "10"),
                        Error.DuplicateTrigger(null, null, null, Certainty.Certain, "11")),
                    Error.DuplicateTrigger(null, null, null, Certainty.Certain, "20, 21").WithSubResults(
                        Error.DuplicateTrigger(null, null, null, Certainty.Certain, "20"),
                        Error.DuplicateTrigger(null, null, null, Certainty.Certain, "21")),
                    Error.DuplicateTrigger(null, null, null, Certainty.Certain, "100, 101, 102").WithSubResults(
                        Error.DuplicateTrigger(null, null, null, Certainty.Certain, "100"),
                        Error.DuplicateTrigger(null, null, null, Certainty.Certain, "101"),
                        Error.DuplicateTrigger(null, null, null, Certainty.Certain, "102")),
                    Error.DuplicateTrigger(null, null, null, Certainty.Uncertain, "200, 201, 202").WithSubResults(
                        Error.DuplicateTrigger(null, null, null, Certainty.Uncertain, "200"),
                        Error.DuplicateTrigger(null, null, null, Certainty.Uncertain, "201"),
                        Error.DuplicateTrigger(null, null, null, Certainty.Uncertain, "202"))
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckOnTagTimeTagCombination();

        [TestMethod]
        public void Trigger_CheckOnTagTimeTagCombination_CheckCategory() => Generic.CheckCategory(root, Category.Trigger);

        [TestMethod]
        public void Trigger_CheckOnTagTimeTagCombination_CheckId() => Generic.CheckId(root, CheckId.CheckOnTagTimeTagCombination);
    }
}