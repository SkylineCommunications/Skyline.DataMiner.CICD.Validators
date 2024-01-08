namespace ProtocolTests.Protocol.Params.Param.Type.CheckOptionsAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Type.CheckOptionsAttribute;

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
                FileName = "Valid.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_HeaderTrailerLinkSerial()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidHeaderTrailerLinkSerial.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_HeaderTrailerLinkSmartSerial()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidHeaderTrailerLinkSmartSerial.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_HeaderTrailerLink_SingleConnection()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidHeaderTrailerLinkSingleConnection.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_HeaderTrailerLink_MultipleConnections()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidHeaderTrailerLinkMultipleConnections.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_Matrix()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidMatrix.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckOptionsAttribute_HeaderTrailerLink_Duplicate()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateHeaderTrailerLinkOptions",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicateHeaderTrailerLinkOptions(null, null, null, "1", "header", "1, 2").WithSubResults(
                        Error.DuplicateHeaderTrailerLinkOptions(null, null, null, "1", "header", "1"),
                        Error.DuplicateHeaderTrailerLinkOptions(null, null, null, "1", "header", "2")),
                    Error.DuplicateHeaderTrailerLinkOptions(null, null, null, "2", "trailer", "3, 4").WithSubResults(
                        Error.DuplicateHeaderTrailerLinkOptions(null, null, null, "2", "trailer", "3"),
                        Error.DuplicateHeaderTrailerLinkOptions(null, null, null, "2", "trailer", "4"))
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_HeaderTrailerLink_Excessive()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ExcessiveHeaderTrailerLinkOptions.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ExcessiveHeaderTrailerLinkOptions(null, null, null, "1")
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_HeaderTrailerLink_Invalid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidHeaderTrailerLinkOptions.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidHeaderTrailerLinkOptions(null, null, null, "trailer", "2")
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_HeaderTrailerLink_Missing()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingHeaderTrailerLinkOptions.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingHeaderTrailerLinkOptions(null, null, null, "header", "1"),
                    Error.MissingHeaderTrailerLinkOptions(null, null, null, "trailer", "2")
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_HeaderTrailerLink_Missing_MultipleConnections()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "HeaderTrailerLinkMissingMultipleConnections.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingHeaderTrailerLinkOptions(null, null, null, "header", "1"),
                    Error.MissingHeaderTrailerLinkOptions(null, null, null, "trailer", "2")
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_HeaderTrailerLink_Missing_SmartSerialSingle()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "HeaderTrailerLinkMissingSmartSerialSingle.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingHeaderTrailerLinkOptions(null, null, null, "header", "1"),
                    Error.MissingHeaderTrailerLinkOptions(null, null, null, "trailer", "2")
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_HeaderTrailerLink_Connection_Missing()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "HeaderTrailerLinkShouldHaveConnection.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.HeaderTrailerLinkShouldHaveConnection(null, null, null, "header", "1"),
                    Error.HeaderTrailerLinkShouldHaveConnection(null, null, null, "trailer", "2"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_HeaderTrailerLink_Connection_Wrong()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "HeaderTrailerConnectionShouldBeValid.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.HeaderTrailerConnectionShouldBeValid(null, null, null, "0", "header", "1"),
                    Error.HeaderTrailerConnectionShouldBeValid(null, null, null, "0", "trailer", "2"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_InconsistentColumnTypeDimensions()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InconsistentColumnTypeDimensions.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InconsistentColumnTypeDimensions(null, null, null, "columntypes=500:0-14", "dimensions=32,16", "1"),
                    Error.InconsistentColumnTypeDimensions(null, null, null, "columntypes=500:0-16", "dimensions=32,16", "3")
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_MissingMatrixOptions()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingMatrixOptions.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingMatrixOptions(null, null, null, "dimensions", "1"),
                    Error.MissingMatrixOptions(null, null, null, "columntypes", "2")
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_MissingAttributeForMatrix()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttributeForMatrix",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttributeForMatrix(null, null, null, "1")
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_InvalidMatrixParamType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidMatrixParamType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidMatrixParamType(null, null, null, "bus", "1"),
                    Error.InvalidMatrixParamType(null, null, null, "read", "2"),
                    Error.InvalidMatrixParamType(null, null, null, "write bit", "3"),
                    Error.InvalidMatrixParamType(null, null, null, "", "4")
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
                    Error.InvalidMatrixOption(null, null, null, "columntypes=:100:0-", "1"),
                    Error.InvalidMatrixOption(null, null, null, "columntypes=100:-31", "2"),
                    Error.InvalidMatrixOption(null, null, null, "columntypes=100:0-15,16|17-31", "3"),
                    Error.InvalidMatrixOption(null, null, null, "columntypes=0-31", "4"),
                    Error.InvalidMatrixOption(null, null, null, "columntypes=100:0,31", "5"),
                    Error.InvalidMatrixOption(null, null, null, "dimensions=16-32", "50"),
                    Error.InvalidMatrixOption(null, null, null, "dimensions=16", "51"),
                    Error.InvalidMatrixOption(null, null, null, "dimensions=,32", "52"),
                    Error.InvalidMatrixOption(null, null, null, "dimensions=16,", "53"),
                    Error.InvalidMatrixOption(null, null, null, "dimensions=,", "54"),
                    Error.InvalidMatrixOption(null, null, null, "dimensions=-1,32", "55"),
                    Error.InvalidMatrixOption(null, null, null, "dimensions=16,-32", "56")
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_MissingColumnTypeParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingColumnTypeParam",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingColumnTypeParam(null, null, null, "100", "1"),
                    Error.MissingColumnTypeParam(null, null, null, "101", "2")
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_MissingColumnTypeParamInterprete()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingColumnTypeParamInterprete",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingColumnTypeParamInterprete(null, null, null, "100", "1"),
                    Error.MissingColumnTypeParamInterprete(null, null, null, "101", "2")
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_InvalidColumnTypeParamInterprete()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidColumnTypeParamInterprete",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidColumnTypeParamInterprete(null, null, null, "101", "1").WithSubResults(
                        Error.InvalidColumnTypeParamRawType(null, null, null, "other", "101", "1"),
                        Error.InvalidColumnTypeParamType(null, null, null, "string", "101", "1")),
                    Error.InvalidColumnTypeParamInterprete(null, null, null, "102", "2").WithSubResults(
                        Error.InvalidColumnTypeParamRawType(null, null, null, "other", "102", "2"),
                        Error.InvalidColumnTypeParamType(null, null, null, "string", "102", "2"))
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_InvalidColumnTypeParamRawType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidColumnTypeParamRawType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidColumnTypeParamRawType(null, null, null, "other", "100", "1"),
                    Error.InvalidColumnTypeParamRawType(null, null, null, "other", "101", "2")
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_InvalidColumnTypeParamType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidColumnTypeParamType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidColumnTypeParamType(null, null, null, "high nibble", "101", "1"),
                    Error.InvalidColumnTypeParamType(null, null, null, "high nibble", "101", "2"),
                    Error.InvalidColumnTypeParamType(null, null, null, "string", "102", "3"),
                    Error.InvalidColumnTypeParamType(null, null, null, "string", "102", "4")
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_InvalidColumnTypeParamLengthType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidColumnTypeParamLengthType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidColumnTypeParamLengthType(null, null, null, "last next param", "101", "1"),
                    Error.InvalidColumnTypeParamLengthType(null, null, null, "last next param", "101", "2"),
                    Error.InvalidColumnTypeParamLengthType(null, null, null, "other param", "102", "3"),
                    Error.InvalidColumnTypeParamLengthType(null, null, null, "other param", "102", "4")
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_UnrecommendedSshOptions()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedSshOptions",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedSshOptions(null, null, null, "SSH Username", "101"),
                    Error.UnrecommendedSshOptions(null, null, null, "SSH PWD", "102"),
                    Error.UnrecommendedSshOptions(null, null, null, "SSH Options", "103"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_InvalidMixOfSshOptionsAndPortSettings()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidMixOfSshOptionsAndPortSettings",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedSshOptions(null, null, null, "SSH PWD", "12"),
                    Error.InvalidMixOfSshOptionsAndPortSettings(null, null, null, "SSH PWD", "12"),
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
        public void Param_CheckOptionsAttribute_MissingAttributeForMatrix()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingAttributeForMatrix",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_MissingColumnTypeParam()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingColumnTypeParam",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_MissingColumnTypeParamInterprete()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingColumnTypeParamInterprete",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_ExcessiveHeaderTrailerLinkOptions()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "ExcessiveHeaderTrailerLinkOptions",
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
        public void Param_CheckOptionsAttribute_MatrixDimensionsChanged()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "MatrixDimensionsChanged",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.MatrixDimensionsChanged(null, null, "1", "64,64", "128,128"),
                    ErrorCompare.MatrixDimensionsChanged(null, null, "2", "64,64", "64,128"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_MatrixDimensionsRemoved()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "MatrixDimensionsRemoved",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.MatrixDimensionsRemoved(null, null, "128,128", "1"),
                    ErrorCompare.MatrixDimensionsRemoved(null, null, "64,128", "2"),
                    ErrorCompare.MatrixDimensionsRemoved(null, null, "64,128", "3"),
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