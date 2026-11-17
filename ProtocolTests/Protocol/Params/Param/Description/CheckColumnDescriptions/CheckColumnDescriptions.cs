namespace ProtocolTests.Protocol.Params.Param.Description.CheckColumnDescriptions
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Description.CheckColumnDescriptions;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckColumnDescriptions();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckColumnDescriptions_Valid()
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
        public void Param_CheckColumnDescriptions_MissingTableDescriptionAsSuffix()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTableDescriptionAsSuffix",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTableDescriptionAsSuffix(null, null, null, "Table Syntax 1", "Instance 1", "101").WithExtraData(ExtraData.TableDescription, "Table Syntax 1"),
                    Error.MissingTableDescriptionAsSuffix(null, null, null, "Table Syntax 2", "Instance 2", "201").WithExtraData(ExtraData.TableDescription, "Table Syntax 2"),
                    Error.MissingTableDescriptionAsSuffix(null, null, null, "Table Syntax 3", "Instance 3(Table Syntax 3)", "301").WithExtraData(ExtraData.TableDescription, "Table Syntax 3"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckColumnDescriptions_MissingTableDescriptionAsSuffixes()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTableDescriptionAsSuffixes",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTableDescriptionAsSuffixes(null, null, null, "Table Syntax 1", "100").WithSubResults(
                        Error.MissingTableDescriptionAsSuffix(null, null, null, "Table Syntax 1", "Instance 1", "101").WithExtraData(ExtraData.TableDescription, "Table Syntax 1"),
                        Error.MissingTableDescriptionAsSuffix(null, null, null, "Table Syntax 1", "Column 2", "102").WithExtraData(ExtraData.TableDescription, "Table Syntax 1"),
                        Error.MissingTableDescriptionAsSuffix(null, null, null, "Table Syntax 1", "Column 3", "103").WithExtraData(ExtraData.TableDescription, "Table Syntax 1"),
                        Error.MissingTableDescriptionAsSuffix(null, null, null, "Table Syntax 1", "Column 3", "153").WithExtraData(ExtraData.TableDescription, "Table Syntax 1")),

                    Error.MissingTableDescriptionAsSuffixes(null, null, null, "Table Syntax 2", "200").WithSubResults(
                        Error.MissingTableDescriptionAsSuffix(null, null, null, "Table Syntax 2", "Instance 2", "201").WithExtraData(ExtraData.TableDescription, "Table Syntax 2"),
                        Error.MissingTableDescriptionAsSuffix(null, null, null, "Table Syntax 2", "Column 2", "202").WithExtraData(ExtraData.TableDescription, "Table Syntax 2"),
                        Error.MissingTableDescriptionAsSuffix(null, null, null, "Table Syntax 2", "Column 3", "203").WithExtraData(ExtraData.TableDescription, "Table Syntax 2"),
                        Error.MissingTableDescriptionAsSuffix(null, null, null, "Table Syntax 2", "Column 3", "253").WithExtraData(ExtraData.TableDescription, "Table Syntax 2")),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckColumnDescriptions();

        [TestMethod]
        public void Param_CheckColumnDescriptions_MissingTableDescriptionAsSuffix()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingTableDescriptionAsSuffix",
            };

            Generic.Fix(check, data);
        }

        [TestMethod]
        public void Param_CheckColumnDescriptions_MissingTableDescriptionAsSuffixes()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingTableDescriptionAsSuffixes",
            };

            Generic.Fix(check, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Param_CheckColumnDescriptions_MissingTableDescriptionAsSuffix()
        {
            // Create ErrorMessage
            var message = Error.MissingTableDescriptionAsSuffix(null, null, null, "Inputs", "Instance", "102");

            string description = "Missing table description 'Inputs' in round brackets at the end of column description 'Instance'. Column PID '102'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckColumnDescriptions_MissingTableDescriptionAsSuffixes()
        {
            // Create ErrorMessage
            var message = Error.MissingTableDescriptionAsSuffixes(null, null, null, "Inputs", "100");

            string description = "Missing table description 'Inputs' in round brackets at the end of column descriptions. Table PID '100'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckColumnDescriptions();

        [TestMethod]
        public void Param_CheckColumnDescriptions_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckColumnDescriptions_CheckId() => Generic.CheckId(check, CheckId.CheckColumnDescriptions);
    }
}
