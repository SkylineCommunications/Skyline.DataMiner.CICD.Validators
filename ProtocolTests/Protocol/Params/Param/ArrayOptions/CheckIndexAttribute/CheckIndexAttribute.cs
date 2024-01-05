namespace ProtocolTests.Protocol.Params.Param.ArrayOptions.CheckIndexAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckIndexAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckIndexAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckIndexAttribute_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckIndexAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "1000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIndexAttribute_InvalidAttributeValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAttributeValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttributeValue(null, null, null, "1100", " 01 "),
                    Error.InvalidAttributeValue(null, null, null, "1200", "01"),
                    Error.InvalidAttributeValue(null, null, null, "1300", "+1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIndexAttribute_InvalidColumnInterpreteType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidColumnInterpreteType",
                ExpectedResults = new List<IValidationResult>
                {
                    // PK: Invalid Interprete/Type
                    Error.InvalidColumnInterpreteType(null, null, null, "double", "1001"),      // TableSyntax1
                    Error.InvalidColumnInterpreteType(null, null, null, "high nibble", "1101"), // TableSyntax2
                    
                    // PK: No Interprete
                    Error.InvalidColumnInterpreteType(null, null, null, "UNDEFINED", "2001"),
                    
                    // PK: No Interprete/Type
                    Error.InvalidColumnInterpreteType(null, null, null, "UNDEFINED", "2101"),

                    // View Tables
                    Error.InvalidColumnInterpreteType(null, null, null, "double", "1001"),  // Currently referring to the base PK and not the duplicated one
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIndexAttribute_InvalidColumnMeasurementType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidColumnMeasurementType",
                ExpectedResults = new List<IValidationResult>
                {
                    // PK: Invalid Measurement/Type
                    Error.InvalidColumnMeasurementType(null, null, null, "analog", "1001"),
                    Error.InvalidColumnMeasurementType(null, null, null, "button", "1101"),
                    
                    // PK: No Measurement
                    Error.InvalidColumnMeasurementType(null, null, null, "UNDEFINED", "2001"),
                    
                    // PK: No Measurement/Type
                    Error.InvalidColumnMeasurementType(null, null, null, "UNDEFINED", "2101"),

                    // View Tables
                    Error.InvalidColumnMeasurementType(null, null, null, "analog", "1001"),  // Currently referring to the base PK and not the duplicated one
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIndexAttribute_InvalidColumnType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidColumnType",
                ExpectedResults = new List<IValidationResult>
                {
                    // PK: Invalid Param/Type
                    Error.InvalidColumnType(null, null, null, "write", "1001"),
                    
                    // View Tables
                    Error.InvalidColumnType(null, null, null, "write", "1001"),  // Currently referring to the base PK and not the duplicated one

                    // Note that missing type and other forbidden types such as bus, dummy, etc are covered by Param.CheckColumns()
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIndexAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null, "1000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIndexAttribute_NonExistingColumn()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingColumn",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingColumn(null, null, null, " 0 ", "1000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIndexAttribute_UnrecommendedValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedValue(null, null, null, " 1 ", "1000", "0"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIndexAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "1000", " 0 "),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckIndexAttribute();

        [TestMethod]
        public void Param_CheckIndexAttribute_MissingAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingAttribute",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckIndexAttribute_EmptyAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "EmptyAttribute",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckIndexAttribute_UntrimmedAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedAttribute",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckIndexAttribute();

        [TestMethod]
        public void Param_CheckIndexAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckIndexAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckIndexAttribute);
    }
}