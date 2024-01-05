namespace ProtocolTests.Protocol.Params.Param.ArrayOptions.ColumnOption.CheckOptionsAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.ColumnOption.CheckOptionsAttribute;

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
        public void Param_CheckOptionsAttribute_ValidRelations_ChildrenToParent()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidRelations_ChildrenToParent",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_ValidRelations_NToMRelation()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidRelations_NToMRelation",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_ValidRelations_ParentToChildren()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidRelations_ParentToChildren",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_ValidRelations_RecursiveRelations()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidRelations_RecursiveRelations",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void Param_CheckOptionsAttribute_ColumnOptionExpectingRTDisplay()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ColumnOptionExpectingRTDisplay",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ColumnOptionExpectingRTDisplay(null, null, null, "1002", "xPox", "1000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_ForeignKeyColumnInvalidInterpreteType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ForeignKeyColumnInvalidInterpreteType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ForeignKeyColumnInvalidInterpreteType(null, null, null, "double", "2002"),
                    Error.ForeignKeyColumnInvalidInterpreteType(null, null, null, "high nibble", "3002"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_ForeignKeyColumnInvalidMeasurementType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ForeignKeyColumnInvalidMeasurementType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ForeignKeyColumnInvalidMeasurementType(null, null, null, "analog", "2002"),
                    Error.ForeignKeyColumnInvalidMeasurementType(null, null, null, "discreet", "3002"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_ForeignKeyColumnInvalidType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ForeignKeyColumnInvalidType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ForeignKeyColumnInvalidType(null, null, null, "write", "2002"),
                    Error.ForeignKeyColumnInvalidType(null, null, null, "read bit", "3002"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_ForeignKeyMissingRelation()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ForeignKeyMissingRelation",
                ExpectedResults = new List<IValidationResult>
                {
                    // Relation 1 : 2 levels
                    Error.ForeignKeyMissingRelation(null, null, null, "1000", "1100", "1102"),
                    
                    // Relation 2 : 3 levels
                    Error.ForeignKeyMissingRelation(null, null, null, "2000", "2100", "2102"),
                    Error.ForeignKeyMissingRelation(null, null, null, "2100", "2200", "2202"),
                    
                    // Relation 3 : 4 levels
                    Error.ForeignKeyMissingRelation(null, null, null, "3000", "3100", "3102"),
                    Error.ForeignKeyMissingRelation(null, null, null, "3100", "3200", "3202"),
                    Error.ForeignKeyMissingRelation(null, null, null, "3200", "3300", "3302"),
                    
                    // Relation 10 : N to M relation
                    Error.ForeignKeyMissingRelation(null, null, null, "10000", "10100", "10102"),
                    Error.ForeignKeyMissingRelation(null, null, null, "10200", "10100", "10103"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_ForeignKeyMissingRelation_NoRelations()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ForeignKeyMissingRelation_NoRelations",
                ExpectedResults = new List<IValidationResult>
                {
                    // Relation 1 : 2 levels
                    Error.ForeignKeyMissingRelation(null, null, null, "1000", "1100", "1102"),
                    
                    // Relation 2 : 3 levels
                    Error.ForeignKeyMissingRelation(null, null, null, "2000", "2100", "2102"),
                    Error.ForeignKeyMissingRelation(null, null, null, "2100", "2200", "2202"),
                    
                    // Relation 3 : 4 levels
                    Error.ForeignKeyMissingRelation(null, null, null, "3000", "3100", "3102"),
                    Error.ForeignKeyMissingRelation(null, null, null, "3100", "3200", "3202"),
                    Error.ForeignKeyMissingRelation(null, null, null, "3200", "3300", "3302"),
                    
                    // Relation 10 : N to M relation
                    Error.ForeignKeyMissingRelation(null, null, null, "10000", "10100", "10102"),
                    Error.ForeignKeyMissingRelation(null, null, null, "10200", "10100", "10103"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void Param_CheckOptionsAttribute_ForeignKeyTargetExpectingRTDisplayOnPK()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ForeignKeyTargetExpectingRTDisplayOnPK",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ForeignKeyTargetExpectingRTDisplayOnPK(null, null, null, "1001", "foreignKey=1000", "2000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_ViewInvalidColumnReference()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ViewInvalidColumnReference",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ViewInvalidColumnReference(null, null, null, Severity.Critical, "101", "100"),
                    Error.ViewInvalidColumnReference(null, null, null, Severity.Major, "52", "100"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_ViewInvalidCombinationFilterChange()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ViewInvalidCombinationFilterChange",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ViewInvalidCombinationFilterChange(null, null, null, "10"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_ViewInvalidSyntax()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ViewInvalidSyntax",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ViewInvalidSyntax(null, null, null, "0", "10"),
                    Error.ViewInvalidSyntax(null, null, null, "1", "10"),
                    Error.ViewInvalidSyntax(null, null, null, "2", "10"),
                    Error.ViewInvalidSyntax(null, null, null, "3", "10"),
                }
            };

            Generic.Validate(test, data);
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