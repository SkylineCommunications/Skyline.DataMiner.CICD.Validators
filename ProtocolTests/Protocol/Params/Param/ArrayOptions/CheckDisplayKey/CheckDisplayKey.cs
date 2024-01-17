namespace ProtocolTests.Protocol.Params.Param.ArrayOptions.CheckDisplayKey
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckDisplayKey;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckDisplayKey();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckDisplayKey_Valid()
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
        public void Param_CheckDisplayKey_ValidIssuesToIgnore()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidIssuesToIgnore",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckDisplayKey_DisplayColumnSameAsPK()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DisplayColumnSameAsPK",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DisplayColumnSameAsPK(null, null, null, "1000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDisplayKey_DisplayColumnUnrecommended()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DisplayColumnUnrecommended",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DisplayColumnUnrecommended(null, null, null, "1000", false),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDisplayKey_DisplayKeyColumnInvalidInterpreteType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DisplayKeyColumnInvalidInterpreteType",
                ExpectedResults = new List<IValidationResult>
                {
                    // NamingFormat (only old displayColumn attribute requires the Interprete/type to be string)
                    ////Error.DisplayKeyColumnInvalidInterpreteType(null, null, null, "double", "1002"),
                    ////Error.DisplayKeyColumnInvalidInterpreteType(null, null, null, "high nibble", "1102"),
                    ////Error.DisplayKeyColumnInvalidInterpreteType(null, null, null, "double", "1209"),
                    ////Error.DisplayKeyColumnInvalidInterpreteType(null, null, null, "high nibble", "1309"),
                    ////Error.DisplayKeyColumnInvalidInterpreteType(null, null, null, "double", "1509"),

                    // Naming option (only old displayColumn attribute requires the Interprete/type to be string)
                    ////Error.DisplayKeyColumnInvalidInterpreteType(null, null, null, "high nibble", "2002"),
                    ////Error.DisplayKeyColumnInvalidInterpreteType(null, null, null, "double", "2109"),
                    ////Error.DisplayKeyColumnInvalidInterpreteType(null, null, null, "high nibble", "2209"),

                    // DisplayColumn attribute
                    Error.DisplayColumnUnrecommended(null, null, null, "3000", false),
                    Error.DisplayKeyColumnInvalidInterpreteType(null, null, null, "double", "3002"),

                    // No option -> default to PK -> already covered by check on PK.
                    ////Error.DisplayKeyColumnInvalidInterpreteType(null, null, null, "high nibble", "10001"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDisplayKey_DisplayKeyColumnInvalidMeasurementType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DisplayKeyColumnInvalidMeasurementType",
                ExpectedResults = new List<IValidationResult>
                {
                    // NamingFormat (only old displayColumn attribute requires the Interprete/type to be string/doube)
                    ////Error.DisplayKeyColumnInvalidMeasurementType(null, null, null, "analog", "1002"),
                    ////Error.DisplayKeyColumnInvalidMeasurementType(null, null, null, "button", "1102"),
                    ////Error.DisplayKeyColumnInvalidMeasurementType(null, null, null, "chart", "1209"),
                    ////Error.DisplayKeyColumnInvalidMeasurementType(null, null, null, "digital threshold", "1309"),
                    ////Error.DisplayKeyColumnInvalidMeasurementType(null, null, null, "discreet", "1509"),

                    // Naming option (only old displayColumn attribute requires the Interprete/type to be string)
                    ////Error.DisplayKeyColumnInvalidMeasurementType(null, null, null, "matrix", "2002"),
                    ////Error.DisplayKeyColumnInvalidMeasurementType(null, null, null, "pagebutton", "2109"),
                    ////Error.DisplayKeyColumnInvalidMeasurementType(null, null, null, "progress", "2209"),

                    // DisplayColumn attribute
                    Error.DisplayColumnUnrecommended(null, null, null, "3000", false),
                    Error.DisplayKeyColumnInvalidMeasurementType(null, null, null, "title", "3002"),

                    // No option -> default to PK -> Already covered by check on PK
                    ////Error.DisplayKeyColumnInvalidMeasurementType(null, null, null, "togglebutton", "10001"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDisplayKey_DisplayKeyColumnInvalidType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DisplayKeyColumnInvalidType",
                ExpectedResults = new List<IValidationResult>
                {
                    // NamingFormat
                    Error.DisplayKeyColumnInvalidType(null, null, null, "write", "1002"),
                    Error.DisplayKeyColumnInvalidType(null, null, null, "group", "1102"),
                    Error.DisplayKeyColumnInvalidType(null, null, null, "read bit", "1209"),
                    Error.DisplayKeyColumnInvalidType(null, null, null, "write bit", "1309"),
                    //Error.DisplayKeyColumnInvalidType(null, null, null, "dummy", "1509"), // Covered by Param.GetColumns

                    // Naming option
                    Error.DisplayKeyColumnInvalidType(null, null, null, "write", "2002"),
                    Error.DisplayKeyColumnInvalidType(null, null, null, "write", "2109"),
                    Error.DisplayKeyColumnInvalidType(null, null, null, "write", "2209"),

                    // DisplayColumn attribute
                    Error.DisplayColumnUnrecommended(null, null, null, "3000", false),
                    Error.DisplayKeyColumnInvalidType(null, null, null, "write", "3002"),

                    // No option -> default to PK -> Already covered by check on PK.
                    ////Error.DisplayKeyColumnInvalidType(null, null, null, "header", "10001"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDisplayKey_DisplayKeyColumnMissing()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DisplayKeyColumnMissing",
                ExpectedResults = new List<IValidationResult>
                {
                    // NamingFormat
                    Error.DisplayKeyColumnMissing(null, null, null,  "1200"),
                    Error.DisplayKeyColumnMissing(null, null, null,  "1300"),
                    Error.DisplayKeyColumnMissing(null, null, null,  "1500"),

                    // Naming option
                    Error.DisplayKeyColumnMissing(null, null, null,  "2100"),
                    Error.DisplayKeyColumnMissing(null, null, null,  "2200"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDisplayKey_DuplicateDisplayKeyColumn()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateDisplayKeyColumn",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicateDisplayKeyColumn(null, null, null, "1200"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDisplayKey_DuplicateDisplayKeyDefinition()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateDisplayKeyDefinition",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicateDisplayKeyDefinition(null, null, null, "1000"),
                    Error.DuplicateDisplayKeyDefinition(null, null, null, "2000"),
                    Error.DuplicateDisplayKeyDefinition(null, null, null, "3000"),
                    Error.DuplicateDisplayKeyDefinition(null, null, null, "4000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDisplayKey_UnexpectedIdxSuffix()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnexpectedIdxSuffix",
                ExpectedResults = new List<IValidationResult>
                {
                    // NamingFormat
                    Error.UnexpectedIdxSuffix(null, null, null, "1001"),

                    Error.UnexpectedIdxSuffix(null, null, null, "1101"),

                    Error.UnexpectedIdxSuffix(null, null, null, "1201"),
                    Error.UnexpectedIdxSuffix(null, null, null, "1202"),

                    Error.UnexpectedIdxSuffix(null, null, null, "1301"),
                    Error.UnexpectedIdxSuffix(null, null, null, "1302"),
                    Error.UnexpectedIdxSuffix(null, null, null, "1303"),

                    Error.UnexpectedIdxSuffix(null, null, null, "1501"),
                    Error.UnexpectedIdxSuffix(null, null, null, "1502"),
                    
                    // Naming option
                    Error.UnexpectedIdxSuffix(null, null, null, "2001"),

                    Error.UnexpectedIdxSuffix(null, null, null, "2101"),
                    Error.UnexpectedIdxSuffix(null, null, null, "2102"),

                    Error.UnexpectedIdxSuffix(null, null, null, "2201"),
                    Error.UnexpectedIdxSuffix(null, null, null, "2202"),

                    // displayColumn
                    Error.DisplayColumnUnrecommended(null, null, null, "3000", false),
                    Error.UnexpectedIdxSuffix(null, null, null, "3001"),

                    // NoDisplayKey -> defaults to PK
                    Error.UnexpectedIdxSuffix(null, null, null, "10002"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckDisplayKey();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckDisplayKey_Valid()
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
        public void Param_CheckDisplayKey_FormatChanged()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "FormatChanged",
                ExpectedResults = new List<IValidationResult>
                {
                    // Naming to Naming
                    ErrorCompare.FormatChanged(null, null, "ArrayOptions@options:naming", "/102,103", "ArrayOptions@options:naming", "*102,103", "100"),
                    ErrorCompare.FormatChanged(null, null, "ArrayOptions@options:naming", "\\102,103,104", "ArrayOptions@options:naming", "*102,103,104", "110"),
                    ErrorCompare.FormatChanged(null, null, "ArrayOptions@options:naming", "/102,103", "ArrayOptions@options:naming", "*102,103", "150"),
                    ErrorCompare.FormatChanged(null, null, "ArrayOptions@options:naming", "\\102,103,104", "ArrayOptions@options:naming", "*102,103,104", "160"),
                    ErrorCompare.FormatChanged(null, null, "ArrayOptions@options:naming", "/102", "ArrayOptions@options:naming", "/202", "200"),
                    ErrorCompare.FormatChanged(null, null, "ArrayOptions@options:naming", "\\102,103", "ArrayOptions@options:naming", "\\212,213", "210"),

                    // Naming to NamingFormat
                    ErrorCompare.FormatChanged(null, null, "ArrayOptions@options:naming", "/102,103", "ArrayOptions/NamingFormat", ",102,*,103,", "1000"),
                    ErrorCompare.FormatChanged(null, null, "ArrayOptions@options:naming", "\\102,103,104", "ArrayOptions/NamingFormat", ",102,*,103,\\,104,", "1010"),
                    ErrorCompare.FormatChanged(null, null, "ArrayOptions@options:naming", "/102", "ArrayOptions/NamingFormat", ",1052,", "1050"),
                    ErrorCompare.FormatChanged(null, null, "ArrayOptions@options:naming", "\\102,103", "ArrayOptions/NamingFormat", ",1062,\\,1063,", "1060"),

                    // NamingFormat to Naming
                    ErrorCompare.FormatChanged(null, null, "ArrayOptions/NamingFormat", ",102,/,103,", "ArrayOptions@options:naming", "*102,103", "2000"),
                    ErrorCompare.FormatChanged(null, null, "ArrayOptions/NamingFormat", ",102,\\,103,\\,104,", "ArrayOptions@options:naming", "*102,103,104", "2010"),
                    ErrorCompare.FormatChanged(null, null, "ArrayOptions/NamingFormat", ",102,", "ArrayOptions@options:naming", "/2052", "2050"),
                    ErrorCompare.FormatChanged(null, null, "ArrayOptions/NamingFormat", ",102,\\,103,", "ArrayOptions@options:naming", "\\2062,2063", "2060"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckDisplayKey_FormatRemoved()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "FormatRemoved",
                ExpectedResults = new List<IValidationResult>
                {
                    // Naming
                    ErrorCompare.FormatRemoved(null, null, "ArrayOptions@options:naming", "1000"),
                    ErrorCompare.FormatRemoved(null, null, "ArrayOptions@options:naming", "1010"),
                    ErrorCompare.FormatRemoved(null, null, "ArrayOptions@options:naming", "1020"),

                    // NamingFormat
                    ErrorCompare.FormatRemoved(null, null, "ArrayOptions/NamingFormat", "2000"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckDisplayKey();

        [TestMethod]
        public void Param_CheckDisplayKey_DuplicateDisplayKeyDefinition()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "DuplicateDisplayKeyDefinition",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckDisplayKey_DisplayColumnSameAsPK()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "DisplayColumnSameAsPK",
            };

            Generic.Fix(codeFix, data);
        }

    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckDisplayKey();

        [TestMethod]
        public void Param_CheckDisplayKey_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckDisplayKey_CheckId() => Generic.CheckId(root, CheckId.CheckDisplayKey);
    }
}