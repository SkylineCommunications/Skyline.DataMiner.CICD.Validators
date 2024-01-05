namespace ProtocolTests.Protocol.Params.Param.Measurement.Type.CheckTypeTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Type.CheckTypeTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckTypeTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckTypeTag_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion Valid Checks

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckTypeTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_InvalidParamType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidParamType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidParamType(null, null, null, "dummy", "button", "1"),
                    Error.InvalidParamType(null, null, null, "read", "button", "2"),

                    Error.InvalidParamType(null, null, null, "dummy", "pagebutton", "51"),
                    Error.InvalidParamType(null, null, null, "read", "pagebutton", "52"),

                    Error.InvalidParamType(null, null, null, "dummy", "togglebutton", "101"),
                    Error.InvalidParamType(null, null, null, "read", "togglebutton", "102"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, " ABC ", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixAlarmingDisabled()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MatrixAlarmingDisabled",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MatrixAlarmingDisabled(null, null, null, "10000"),
                    Error.MatrixAlarmingDisabled(null, null, null, "10001"),
                    Error.MatrixAlarmingDisabled(null, null, null, "10002"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixInvalidInterprete()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MatrixInvalidInterprete",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MatrixInvalidInterprete(null, null, null, "100").WithSubResults(
                        Error.MatrixInvalidInterpreteRawType(null, null, null, "numeric text", "100", false),
                        Error.MatrixInvalidInterpreteType(null, null, null, "high nibble", "100", "double", false)),
                    Error.MatrixInvalidInterprete(null, null, null, "101").WithSubResults(
                        Error.MatrixInvalidInterpreteRawType(null, null, null, "", "101", false),
                        Error.MatrixInvalidInterpreteType(null, null, null, "", "101", "double", false),
                        Error.MatrixInvalidInterpreteLengthType(null, null, null, "", "101", false))
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixInvalidInterpreteLengthType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MatrixInvalidInterpreteLengthType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MatrixInvalidInterpreteLengthType(null, null, null, "", "9000", true),
                    Error.MatrixInvalidInterpreteLengthType(null, null, null, "", "9001", true),
                    Error.MatrixInvalidInterpreteLengthType(null, null, null, "fixed", "10000", true),
                    Error.MatrixInvalidInterpreteLengthType(null, null, null, "fixed", "10001", true),
                    Error.MatrixInvalidInterpreteLengthType(null, null, null, "other param", "10002", true),
                    Error.MatrixInvalidInterpreteLengthType(null, null, null, "other param", "10003", true),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixInvalidInterpreteRawType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MatrixInvalidInterpreteRawType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MatrixInvalidInterpreteRawType(null, null, null, "", "9000", true),
                    Error.MatrixInvalidInterpreteRawType(null, null, null, "", "9001", true),
                    Error.MatrixInvalidInterpreteRawType(null, null, null, "numeric text", "10000", true),
                    Error.MatrixInvalidInterpreteRawType(null, null, null, "numeric text", "10001", true),
                    Error.MatrixInvalidInterpreteRawType(null, null, null, "text", "10002", true),
                    Error.MatrixInvalidInterpreteRawType(null, null, null, "text", "10003", true),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixInvalidInterpreteType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MatrixInvalidInterpreteType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MatrixInvalidInterpreteType(null, null, null, "", "9000", "double", true),
                    Error.MatrixInvalidInterpreteType(null, null, null, "", "9001", "string", true),
                    Error.MatrixInvalidInterpreteType(null, null, null, "string", "10000", "double", true),
                    Error.MatrixInvalidInterpreteType(null, null, null, "high nibble", "10002", "double", true),
                    Error.MatrixInvalidInterpreteType(null, null, null, "high nibble", "10003", "string", true),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixSetterOnWrite()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MatrixSetterOnWrite",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MatrixSetterOnWrite(null, null, null, "10001"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixTrendingEnabled()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MatrixTrendingEnabled",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MatrixTrendingEnabled(null, null, null, "10000"),
                    Error.MatrixTrendingEnabled(null, null, null, "10001"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_TogglebuttonRecommended()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "TogglebuttonRecommended",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.TogglebuttonRecommended(null, null, null, "200"),     // Write param
                    Error.TogglebuttonRecommended(null, null, null, "1011"),    // WriteBit param
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, "1", " pagebutton "),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion  
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckTypeTag();

        [TestMethod]
        public void Param_CheckTypeTag_TogglebuttonRecommended()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "TogglebuttonRecommended",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixAlarmingDisabled()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MatrixAlarmingDisabled",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixInvalidInterprete()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MatrixInvalidInterprete",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixInvalidInterpreteLengthType()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MatrixInvalidInterpreteLengthType",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixInvalidInterpreteRawType()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MatrixInvalidInterpreteRawType",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixInvalidInterpreteType()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MatrixInvalidInterpreteType",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixSetterOnWrite()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MatrixSetterOnWrite",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixTrendingEnabled()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MatrixTrendingEnabled",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_UntrimmedTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedTag",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Param_CheckTypeTag_EmptyTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyTag(null, null, null, "0");

            string description = "Empty tag 'Measurement/Type' in Param '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckTypeTag_InvalidParamType()
        {
            // Create ErrorMessage
            var message = Error.InvalidParamType(null, null, null, "read", "button", "2");

            string description = "Invalid value 'read' in 'Param/Type' for 'button'. Param ID '2'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckTypeTag_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "0", "1");

            string description = "Invalid value '0' in tag 'Measurement/Type'. Param ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixAlarmingDisabled()
        {
            // Create ErrorMessage
            var message = Error.MatrixAlarmingDisabled(null, null, null, "0");

            string description = "Matrix Param '0' should be alarmed.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixInvalidInterprete()
        {
            // Create ErrorMessage
            var message = Error.MatrixInvalidInterprete(null, null, null, "0");

            string description = "Invalid Interprete for matrix Param '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixInvalidInterpreteLengthType()
        {
            // Create ErrorMessage
            var message = Error.MatrixInvalidInterpreteLengthType(null, null, null, "0", "1", false);

            string description = "Invalid LengthType '0' for matrix Param '1'. Expected LengthType 'next param'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixInvalidInterpreteRawType()
        {
            // Create ErrorMessage
            var message = Error.MatrixInvalidInterpreteRawType(null, null, null, "0", "1", false);

            string description = "Invalid RawType '0' for matrix Param '1'. Expected RawType 'other'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixInvalidInterpreteType()
        {
            // Create ErrorMessage
            var message = Error.MatrixInvalidInterpreteType(null, null, null, "0", "1", "2", false);

            string description = "Invalid Interprete/Type '0' for matrix Param '1'. Expected Type '2'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixSetterOnWrite()
        {
            // Create ErrorMessage
            var message = Error.MatrixSetterOnWrite(null, null, null, "0");

            string description = "Unsupported attribute 'setter' in Matrix '0'. Current value 'true'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckTypeTag_MatrixTrendingEnabled()
        {
            // Create ErrorMessage
            var message = Error.MatrixTrendingEnabled(null, null, null, "0");

            string description = "Matrix Param '0' should not be trended.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckTypeTag_TogglebuttonRecommended()
        {
            // Create ErrorMessage
            var message = Error.TogglebuttonRecommended(null, null, null, "1");

            string description = "Measurement/Type 'togglebutton' is recommended for Param with ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckTypeTag_UntrimmedTag()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedTag(null, null, null, "0", "1");

            string description = "Untrimmed tag 'Measurement/Type' in Param '0'. Current value '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckTypeTag();

        [TestMethod]
        public void Param_CheckTypeTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckTypeTag_CheckId() => Generic.CheckId(root, CheckId.CheckTypeTag);
    }
}