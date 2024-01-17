namespace ProtocolTests.Protocol.Params.Param.Database.ColumnDefinition.CheckColumnDefinitionTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Database.ColumnDefinition.CheckColumnDefinitionTag;

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckColumnDefinitionTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckColumnDefinitionTag_Valid()
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
        public void Param_CheckColumnDefinitionTag_ChangedLoggerDataType()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "ChangedLoggerDataType",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.ChangedLoggerDataType(null, null, "VARCHAR(50)", "1", "VARCHAR(10)"),
                    ErrorCompare.ChangedLoggerDataType(null, null, "TEXT", "2", "VARCHAR(20)"),
                    ErrorCompare.ChangedLoggerDataType(null, null, "TEXT", "3", "DOUBLE"),
                    ErrorCompare.ChangedLoggerDataType(null, null, "VARCHAR(20)", "4", "SMALLINT"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckColumnDefinitionTag();

        [TestMethod]
        public void Param_CheckColumnDefinitionTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckColumnDefinitionTag_CheckId() => Generic.CheckId(root, CheckId.CheckColumnDefinitionTag);
    }
}