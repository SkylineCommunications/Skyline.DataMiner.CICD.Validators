namespace ProtocolTests.Protocol.Params.Param.Name.CheckColumnNames
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Name.CheckColumnNames;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckColumnNames();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckColumnNames_Valid()
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
        public void Param_CheckColumnNames_MissingTableNameAsPrefix()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTableNameAsPrefix",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTableNameAsPrefix(null, null, null, "TableSyntax1", "Instance1", "101").WithExtraData(ExtraData.TableName, "TableSyntax1"),
                    Error.MissingTableNameAsPrefix(null, null, null, "TableSyntax2", "Instance2", "201").WithExtraData(ExtraData.TableName, "TableSyntax2"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckColumnNames_MissingTableNameAsPrefixes()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTableNameAsPrefixes",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTableNameAsPrefixes(null, null, null, "TableSyntax1", "100").WithSubResults(
                        Error.MissingTableNameAsPrefix(null, null, null, "TableSyntax1", "Instance_1", "101").WithExtraData(ExtraData.TableName, "TableSyntax1"),
                        Error.MissingTableNameAsPrefix(null, null, null, "TableSyntax1", "Column2_1", "102").WithExtraData(ExtraData.TableName, "TableSyntax1"),
                        Error.MissingTableNameAsPrefix(null, null, null, "TableSyntax1", "Column3_1", "103").WithExtraData(ExtraData.TableName, "TableSyntax1"),
                        Error.MissingTableNameAsPrefix(null, null, null, "TableSyntax1", "Column3_1", "153").WithExtraData(ExtraData.TableName, "TableSyntax1")),

                    Error.MissingTableNameAsPrefixes(null, null, null, "TableSyntax2", "200").WithSubResults(
                        Error.MissingTableNameAsPrefix(null, null, null, "TableSyntax2", "Instance_2", "201").WithExtraData(ExtraData.TableName, "TableSyntax2"),
                        Error.MissingTableNameAsPrefix(null, null, null, "TableSyntax2", "Column2_2", "202").WithExtraData(ExtraData.TableName, "TableSyntax3"),
                        Error.MissingTableNameAsPrefix(null, null, null, "TableSyntax2", "Column3_2", "203").WithExtraData(ExtraData.TableName, "TableSyntax3"),
                        Error.MissingTableNameAsPrefix(null, null, null, "TableSyntax2", "Column3_2", "253").WithExtraData(ExtraData.TableName, "TableSyntax3")),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckColumnNames();

        [TestMethod]
        public void Param_CheckColumnNames_MissingTableNameAsPrefix()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingTableNameAsPrefix",
            };

            Generic.Fix(check, data);
        }

        [TestMethod]
        public void Param_CheckColumnNames_MissingTableNameAsPrefixes()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingTableNameAsPrefixes",
            };

            Generic.Fix(check, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Param_CheckColumnNames_MissingTableNameAsPrefix()
        {
            // Create ErrorMessage
            var message = Error.MissingTableNameAsPrefix(null, null, null, "Inputs", "Instance", "102");

            string description = "Missing table name 'Inputs' in front of column name 'Instance'. Column PID '102'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckColumnNames_MissingTableNameAsPrefixes()
        {
            // Create ErrorMessage
            var message = Error.MissingTableNameAsPrefixes(null, null, null, "Inputs", "100");

            string description = "Missing table name 'Inputs' in front of column names. Table PID '100'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckColumnNames();

        [TestMethod]
        public void Param_CheckColumnNames_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckColumnNames_CheckId() => Generic.CheckId(check, CheckId.CheckColumnNames);
    }
}