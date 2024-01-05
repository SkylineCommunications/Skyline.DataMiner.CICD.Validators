namespace ProtocolTests.Protocol.Pairs.Pair.Content.CheckContentTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Pairs.Pair.Content.CheckContentTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckContentTag();

        #region Valid Checks

        [TestMethod]
        public void Pair_CheckContentTag_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Pair_CheckContentTag_ValidEach()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidEach",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Pair_CheckContentTag_MissingClearResponseRoutine()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingClearResponseRoutine",
                ExpectedResults = new List<IValidationResult>
                {
                    // 1 command / 2 responses
                    Error.MissingClearResponseRoutine(null, null, null, "120").WithSubResults(
                        Error.MissingClearResponseRoutine_Sub(null, null, null, "121", "122")),
                    Error.MissingClearResponseRoutine(null, null, null, "125").WithSubResults(
                        Error.MissingClearResponseRoutine_Sub(null, null, null, "125", "126")),
                    
                    // 1 command / 3 responses : Clear others individually
                    Error.MissingClearResponseRoutine(null, null, null, "130").WithSubResults(
                        Error.MissingClearResponseRoutine_Sub(null, null, null, "132,133", "131"),
                        Error.MissingClearResponseRoutine_Sub(null, null, null, "132", "133")),
                    
                    // 1 command / 3 responses : Clear others in bulk
                    Error.MissingClearResponseRoutine(null, null, null, "135").WithSubResults(
                        Error.MissingClearResponseRoutine_Sub(null, null, null, "137,138", "136")),
                    
                    // 1 command / 3 responses : Clear itself
                    Error.MissingClearResponseRoutine(null, null, null, "140").WithSubResults(
                        Error.MissingClearResponseRoutine_Sub(null, null, null, "141", "141"),
                        Error.MissingClearResponseRoutine_Sub(null, null, null, "143", "143"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Pair_CheckContentTag_MissingClearResponseRoutineEach()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingClearResponseRoutineEach",
                ExpectedResults = new List<IValidationResult>
                {
                    // 1 command / 2 responses
                    Error.MissingClearResponseRoutine(null, null, null, "120").WithSubResults(
                        Error.MissingClearResponseRoutine_Sub(null, null, null, "122", "122")),
                    Error.MissingClearResponseRoutine(null, null, null, "125").WithSubResults(
                        Error.MissingClearResponseRoutine_Sub(null, null, null, "126", "126")),

                    // 1 command / 3 responses : Clear others individually
                    Error.MissingClearResponseRoutine(null, null, null, "130").WithSubResults(
                        Error.MissingClearResponseRoutine_Sub(null, null, null, "132,133", "131"),
                        Error.MissingClearResponseRoutine_Sub(null, null, null, "131", "133")),
                    
                    // 1 command / 3 responses : Clear others in bulk
                    Error.MissingClearResponseRoutine(null, null, null, "135").WithSubResults(
                        Error.MissingClearResponseRoutine_Sub(null, null, null, "137,138", "136")),
                    
                    // 1 command / 3 responses : Clear itself
                    Error.MissingClearResponseRoutine(null, null, null, "140").WithSubResults(
                        Error.MissingClearResponseRoutine_Sub(null, null, null, "141", "141"),
                        Error.MissingClearResponseRoutine_Sub(null, null, null, "143", "143"))
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Pair_CheckContentTag_MissingClearResponseRoutine()
        {
            // Create ErrorMessage
            var message = Error.MissingClearResponseRoutine(null, null, null, "0");

            string description = "Missing clear response routine for pair '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Pair_CheckContentTag_MissingClearResponseRoutineSub()
        {
            // Create ErrorMessage
            var message = Error.MissingClearResponseRoutine_Sub(null, null, null, "0", "1");

            string description = "Missing clear response '0' routine after response '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckContentTag();

        [TestMethod]
        public void Pair_CheckContentTag_CheckCategory() => Generic.CheckCategory(check, Category.Pair);

        [TestMethod]
        public void Pair_CheckContentTag_CheckId() => Generic.CheckId(check, CheckId.CheckContentTag);
    }
}