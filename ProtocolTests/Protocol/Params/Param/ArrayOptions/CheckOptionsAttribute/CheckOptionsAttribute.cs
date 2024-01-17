namespace ProtocolTests.Protocol.Params.Param.ArrayOptions.CheckOptionsAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckOptionsAttribute;

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
        public void Param_CheckOptionsAttribute_ValidNamingOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidNamingOption",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_ValidViewTables()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidViewTables",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckOptionsAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "100"),  // Empty
                    Error.EmptyAttribute(null, null, null, "200"),  // Spaces
                    Error.EmptyAttribute(null, null, null, "300"),  // ColumnOptions & attribute
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_NamingEmpty()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NamingEmpty",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NamingEmpty(null, null, null, "1000"),
                    Error.NamingEmpty(null, null, null, "1100"),
                    Error.NamingEmpty(null, null, null, "10000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_NamingRefersToNonExistingParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NamingRefersToNonExistingParam",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NamingRefersToNonExistingParam(null, null, null, "1009", "1000"),
                    Error.NamingRefersToNonExistingParam(null, null, null, "1109", "1100"),

                    Error.NamingRefersToNonExistingParam(null, null, null, "10008", "10000"),
                    Error.NamingRefersToNonExistingParam(null, null, null, "10009", "10000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_PreserveStateShouldBeAvoided()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "PreserveStateShouldBeAvoided",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.PreserveStateShouldBeAvoided(null, null, null, "1"),
                    Error.PreserveStateShouldBeAvoided(null, null, null, "2"),
                    Error.PreserveStateShouldBeAvoided(null, null, null, "3"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "1000", " ;naming=/1002"),
                    Error.UntrimmedAttribute(null, null, null, "1100", ";naming=/1101,1102 "),

                    Error.UntrimmedAttribute(null, null, null, "10000", " ;volatile;view=1000;naming=/10001,10002 "),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_ViewTableDirectViewInvalidColumn()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ViewTableDirectViewInvalidColumn",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ViewTableDirectViewInvalidColumn(null, null, null, "12345", "100"),
                    Error.ViewTableDirectViewInvalidColumn(null, null, null, "201", "200"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_ViewTableFilterChangeInvalidColumns()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ViewTableFilterChangeInvalidColumns",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ViewTableFilterChangeInvalidColumns(null, null, null, "1001, 1002", "20").WithSubResults(
                        Error.ViewTableFilterChangeInvalidColumns(null, null, null, "1001", "20"),
                        Error.ViewTableFilterChangeInvalidColumns(null, null, null, "1002", "20"))
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_ViewTableInvalidReference()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ViewTableInvalidReference",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ViewTableInvalidReference(null, null, null, Severity.Critical, "10", "10"),
                    Error.ViewTableInvalidReference(null, null, null, Severity.Major, "12345", "11"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckOptionsAttribute();

        [TestMethod]
        public void Param_CheckNamingFormatTag_EmptyAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "EmptyAttribute",
            };

            Generic.Fix(check, data);
        }

        [TestMethod]
        public void Param_CheckNamingFormatTag_UntrimmedAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedAttribute",
            };

            Generic.Fix(check, data);
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

        [TestMethod]
        public void Param_CheckOptionsAttribute_LoggerTable()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "LoggerTable",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckOptionsAttribute_RemovedLoggerTableDatabaseLink()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "RemovedLoggerTableDatabaseLink",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.RemovedLoggerTableDatabaseLink(null, null, "100"),
                    ErrorCompare.RemovedLoggerTableDatabaseLink(null, null, "200"),
                    ErrorCompare.RemovedLoggerTableDatabaseLink(null, null, "300"),
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