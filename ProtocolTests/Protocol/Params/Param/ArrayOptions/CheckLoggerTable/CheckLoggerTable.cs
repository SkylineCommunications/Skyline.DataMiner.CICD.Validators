namespace ProtocolTests.Protocol.Params.Param.ArrayOptions.CheckLoggerTable
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckLoggerTable;

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckLoggerTable();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckLoggerTable_Valid()
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
        public void Param_CheckLoggerTable_RemovedLoggerColumn()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "RemovedLoggerColumn",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.RemovedLoggerColumn(null, null, "105", "100"),
                    ErrorCompare.RemovedLoggerColumn(null, null, "203", "200"),
                    ErrorCompare.RemovedLoggerColumn(null, null, "204", "200"),
                    ErrorCompare.RemovedLoggerColumn(null, null, "304", "300"),
                    ErrorCompare.RemovedLoggerColumn(null, null, "401", "400"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckLoggerTable();

        [TestMethod]
        public void Param_CheckLoggerTable_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckLoggerTable_CheckId() => Generic.CheckId(root, CheckId.CheckLoggerTable);
    }
}