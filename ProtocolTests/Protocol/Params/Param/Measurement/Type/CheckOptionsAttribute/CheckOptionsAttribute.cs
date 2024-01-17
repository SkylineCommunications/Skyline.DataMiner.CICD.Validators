namespace ProtocolTests.Protocol.Params.Param.Measurement.Type.CheckOptionsAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Type.CheckOptionsAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckOptionsAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckOptionsAttribute_Valid()
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
        public void Param_CheckOptionsAttribute_ValidMatrix()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidMatrix",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_ValidTableSorting()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidTableSorting",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckOptionsAttribute_InvalidColumnDimensionsToOutputCount()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidColumnDimensionsToOutputCount",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidColumnDimensionsToOutputCount(null, null, null, "10000", "31", "32"),
                    Error.InvalidColumnDimensionsToOutputCount(null, null, null, "10001", "33", "32"),
                    Error.InvalidColumnDimensionsToOutputCount(null, null, null, "10002", "16", "32"),
                    Error.InvalidColumnDimensionsToOutputCount(null, null, null, "10003", "31", "32"),
                    Error.InvalidMatrixDimensionsToInputCount(null, null, null, "10003", "15", "16"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_InvalidConnectedMatrixPoints()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidConnectedMatrixPoints",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidConnectedMatrixPoints(null, null, null, "COMin 17", "minimum", "output", "10001"),
                    Error.InvalidConnectedMatrixPoints(null, null, null, "COMax 0", "maximum", "output", "10002"),
                    Error.InvalidConnectedMatrixPoints(null, null, null, "COMax 17", "maximum", "output", "10003"),
                    Error.InvalidConnectedMatrixPoints(null, null, null, "COMin 11 and COMax 10", "max smaller than min", "output", "10004"),
                    Error.InvalidConnectedMatrixPoints(null, null, null, "CIMin 33", "minimum", "input", "10011"),
                    Error.InvalidConnectedMatrixPoints(null, null, null, "CIMax 0", "maximum", "input", "10012"),
                    Error.InvalidConnectedMatrixPoints(null, null, null, "CIMax 33", "maximum", "input", "10013"),
                    Error.InvalidConnectedMatrixPoints(null, null, null, "CIMin 22 and CIMax 21", "max smaller than min", "input", "10014"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_InvalidMatrixDimensionsToInputCount()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidMatrixDimensionsToInputCount",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidMatrixDimensionsToInputCount(null, null, null, "10000", "15", "16"),
                    Error.InvalidMatrixDimensionsToInputCount(null, null, null, "10001", "17", "16"),
                    Error.InvalidMatrixDimensionsToInputCount(null, null, null, "10002", "32", "16"),
                    Error.InvalidMatrixDimensionsToInputCount(null, null, null, "10003", "15", "16"),
                    Error.InvalidColumnDimensionsToOutputCount(null, null, null, "10003", "31", "32"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_InvalidMatrixOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidMatrixOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidMatrixOption(null, null, null, "matrix", "10000"),
                    Error.InvalidMatrixOption(null, null, null, "matrix", "10001"),
                    Error.InvalidMatrixOption(null, null, null, "matrix", "10002"),
                    Error.InvalidMatrixOption(null, null, null, "matrix", "10003"),
                    Error.InvalidMatrixOption(null, null, null, "matrix", "10004"),
                    Error.InvalidMatrixOption(null, null, null, "matrix", "20000"),
                    Error.InvalidMatrixOption(null, null, null, "matrix", "20001"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null, "10000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_MissingMatrixOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingMatrixOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingMatrixOption(null, null, null, "matrix", "10000"),
                    Error.MissingMatrixOption(null, null, null, "matrix", "10001"),
                    Error.MissingMatrixOption(null, null, null, "matrix", "10002"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_MissingPriorityForSortedColumns()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingPriorityForSortedColumns",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingPriorityForSortedColumns(null, null, null, "1000"),
                    Error.MissingPriorityForSortedColumns(null, null, null, "2000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_MissingSortingOnDateTimeColumn()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingSortingOnDateTimeColumn",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingSortingOnDateTimeColumn(null, null, null, "1000", "1002"),

                    Error.MissingSortingOnDateTimeColumn(null, null, null, "1100", "1102, 1104").WithSubResults(
                        Error.MissingSortingOnDateTimeColumn(null, null, null, "1100", "1102"),
                        Error.MissingSortingOnDateTimeColumn(null, null, null, "1100", "1104")),

                    Error.MissingSortingOnDateTimeColumn(null, null, null, "1200", "1202, 1204").WithSubResults(
                        Error.MissingSortingOnDateTimeColumn(null, null, null, "1200", "1202"),
                        Error.MissingSortingOnDateTimeColumn(null, null, null, "1200", "1204")),

                    Error.MissingSortingOnDateTimeColumn(null, null, null, "10000", "10002"),

                    Error.MissingSortingOnDateTimeColumn(null, null, null, "10100", "10102, 10104").WithSubResults(
                        Error.MissingSortingOnDateTimeColumn(null, null, null, "10100", "10102"),
                        Error.MissingSortingOnDateTimeColumn(null, null, null, "10100", "10104")),

                    Error.MissingSortingOnDateTimeColumn(null, null, null, "10200", "10202, 10204").WithSubResults(
                        Error.MissingSortingOnDateTimeColumn(null, null, null, "10200", "10202"),
                        Error.MissingSortingOnDateTimeColumn(null, null, null, "10200", "10204"))
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void Param_CheckOptionsAttribute_ReferencedParamRTDisplayExpected()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ReferencedParamRTDisplayExpected",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ReferencedParamRTDisplayExpected(null, null, null, "1002", "1000"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckOptionsAttribute();

        [TestMethod]
        public void Param_CheckOptionsAttribute_MissingAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingAttribute",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_InvalidMatrixDimensionsToInputCount()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "InvalidMatrixDimensionsToInputCount",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_InvalidColumnDimensionsToOutputCount()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "InvalidColumnDimensionsToOutputCount",
            };

            Generic.Fix(codeFix, data);
        }

    }

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckOptionsAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckOptionsAttribute_Valid()
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
        public void Param_CheckOptionsAttribute_ColumnOrderChanged()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "ColumnOrderChanged",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.ColumnOrderChanged(null, null, "1001-1002-1003", "1000", "1001-1003-1002"),
                    ErrorCompare.ColumnOrderChanged(null, null, "2001-2002-2003", "2000", "2001-2002"),
                    ErrorCompare.ColumnOrderChanged(null, null, "3001-3002-3003", "3000", ""),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckOptionsAttribute();

        [TestMethod]
        public void Param_CheckOptionsAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckOptionsAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckOptionsAttribute);
    }
}