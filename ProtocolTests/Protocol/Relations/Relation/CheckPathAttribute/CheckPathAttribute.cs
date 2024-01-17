namespace ProtocolTests.Protocol.Relations.Relation.CheckPathAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Relations.Relation.CheckPathAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckPathAttribute();

        #region Valid Checks

        [TestMethod]
        public void Relation_CheckPathAttribute_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_Valid_ChildrenToParent()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_ChildrenToParent",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_Valid_NToMRelation()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_NToMRelation",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Relation_CheckPathAttribute_DuplicateValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicateValue(null, null, null, "1000;1100").WithSubResults(
                        Error.DuplicateValue(null, null, null, "1000;1100"),
                        Error.DuplicateValue(null, null, null, "1000;1100")),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null)
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "test1"),
                    Error.InvalidValue(null, null, null, "test2"),

                    Error.InvalidValue(null, null, null, "100;test3;200").WithSubResults(
                        Error.InvalidValue(null, null, null, "test3")),

                    Error.InvalidValue(null, null, null, "100;test4;200;test5").WithSubResults(
                        Error.InvalidValue(null, null, null, "test4"),
                        Error.InvalidValue(null, null, null, "test5"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null)
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_MissingForeignKeyForRelation()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingForeignKeyForRelation",
                ExpectedResults = new List<IValidationResult>
                {
                    // missing foreignKey in options' column - 2 levels
                    Error.MissingForeignKeyForRelation(null, null, null, "1000;2000").WithSubResults(
                        Error.MissingForeignKeyInTable_Sub(null, null, null, "1000", "2000")),

                    // missing foreignKey in options' column - 3 levels
                    Error.MissingForeignKeyForRelation(null, null, null, "1000;2000;3000").WithSubResults(
                        Error.MissingForeignKeyInTable_Sub(null, null, null, "1000", "2000")),

                     // missing column with fk in table - 3 levels
                    Error.MissingForeignKeyForRelation(null, null, null, "3000;4000;5000").WithSubResults(
                        Error.MissingForeignKeyInTable_Sub(null, null, null, "4000", "5000")),

                    // missing column with fk in table and missing foreignKey in options' column - 3 levels
                    Error.MissingForeignKeyForRelation(null, null, null, "4000;5000;6000").WithSubResults(
                        Error.MissingForeignKeyInTable_Sub(null, null, null, "4000", "5000"),
                        Error.MissingForeignKeyInTable_Sub(null, null, null, "5000", "6000"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_MissingForeignKeyForRelation_ChildrenToParent()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingForeignKeyForRelation_ChildrenToParent",
                ExpectedResults = new List<IValidationResult>
                {
                    // 2 Levels - Missing 1 FK
                    Error.MissingForeignKeyForRelation(null, null, null, "2000;1000").WithSubResults(
                        Error.MissingForeignKeyInTable_Sub(null, null, null, "2000", "1000")),

                    // 3 Levels - Missing 1 FK Option
                    Error.MissingForeignKeyForRelation(null, null, null, "3000;2000;1000").WithSubResults(
                        Error.MissingForeignKeyInTable_Sub(null, null, null, "2000", "1000")),

                     // 3 Levels - Missing 1 FK Column
                    Error.MissingForeignKeyForRelation(null, null, null, "5000;4000;3000").WithSubResults(
                        Error.MissingForeignKeyInTable_Sub(null, null, null, "5000", "4000")),

                    // 3 levels - Missing 2 FKs
                    Error.MissingForeignKeyForRelation(null, null, null, "6000;5000;4000").WithSubResults(
                        Error.MissingForeignKeyInTable_Sub(null, null, null, "6000", "5000"),
                        Error.MissingForeignKeyInTable_Sub(null, null, null, "5000", "4000"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_MissingForeignKeyForRelation_NToM()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingForeignKeyForRelation_NToM",
                ExpectedResults = new List<IValidationResult>
                {
                    // N to M Relation with missing foreignKey in options' column
                    Error.MissingForeignKeyForRelation(null, null, null, "10000;10100;10200").WithSubResults(
                        Error.MissingForeignKeyInTable_Sub(null, null, null, "10100", "10200")),

                    // N to M Relation with two missing foreignKey in options' column
                    Error.MissingForeignKeyForRelation(null, null, null, "10000;10150;10200").WithSubResults(
                        Error.MissingForeignKeyInTable_Sub(null, null, null, "10000", "10150"),
                        Error.MissingForeignKeyInTable_Sub(null, null, null, "10150", "10200"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_MissingForeignKeyForRelation_WithInvalidPathIds()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingForeignKeyForRelation_WithInvalidPathIds",
                ExpectedResults = new List<IValidationResult>
                {
                    // 1 missing foreignKey in options' column and 1 invalid path id - 4 levels
                    Error.MissingForeignKeyForRelation(null, null, null, "1000;test;3000;4000").WithSubResults(
                        Error.MissingForeignKeyInTable_Sub(null, null, null, "3000", "4000")),

                    Error.InvalidValue(null, null, null, "1000;test;3000;4000").WithSubResults(
                        Error.InvalidValue(null, null, null, "test")),

                    // 2 missing foreignKeys in options' column and 1 invalid path id - 4 levels
                    Error.MissingForeignKeyForRelation(null, null, null, "test2;3000;4000;5000").WithSubResults(
                        Error.MissingForeignKeyInTable_Sub(null, null, null, "3000", "4000"),
                        Error.MissingForeignKeyInTable_Sub(null, null, null, "4000", "5000")),

                    Error.InvalidValue(null, null, null, "test2;3000;4000;5000").WithSubResults(
                        Error.InvalidValue(null, null, null, "test2")),

                    // 1 missing foreignKeys in options' column and 2 invalid path id - 4 levels
                    Error.MissingForeignKeyForRelation(null, null, null, "test3;3000;4000;test4").WithSubResults(
                        Error.MissingForeignKeyInTable_Sub(null, null, null, "3000", "4000")),

                    Error.InvalidValue(null, null, null, "test3;3000;4000;test4").WithSubResults(
                        Error.InvalidValue(null, null, null, "test3"),
                        Error.InvalidValue(null, null, null, "test4"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "1001;1002").WithSubResults(
                        Error.NonExistingId(null, null, null, "1001"),
                        Error.NonExistingId(null, null, null, "1002")),

                    Error.InvalidValue(null, null, null, "1;1003;2;1004").WithSubResults(
                        Error.NonExistingId(null, null, null, "1003"),
                        Error.NonExistingId(null, null, null, "1004"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_NonExistingIdNoParamsTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdNoParamsTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "1001;1002").WithSubResults(
                        Error.NonExistingId(null, null, null, "1001"),
                        Error.NonExistingId(null, null, null, "1002"))
                    }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void Relation_CheckPathAttribute_ReferencedParamExpectingRTDisplay()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ReferencedParamExpectingRTDisplay",
                ExpectedResults = new List<IValidationResult>
                {
                    //Error.ReferencedParamExpectingRTDisplay(null, null, null, "")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_ReferencedParamWrongType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ReferencedParamWrongType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "1;2;3").WithSubResults(
                        Error.ReferencedParamWrongType(null, null, null, "bus", "1"),
                        Error.ReferencedParamWrongType(null, null, null, "read", "2"),
                        Error.ReferencedParamWrongType(null, null, null, "write", "3")),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Relation_CheckPathAttribute_DuplicateValue()
        {
            // Create ErrorMessage
            var message = Error.DuplicateValue(null, null, null, "1000;1100");

            string description = "Duplicated Relation path '1000;1100'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null);

            string description = "Empty attribute 'Relation@path'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "0");

            string description = "Invalid value '0' in attribute 'Relation@path'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_MissingAttribute()
        {
            // Create ErrorMessage
            var message = Error.MissingAttribute(null, null, null);

            string description = "Missing attribute 'Relation@path'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_MissingForeignKeyForRelation()
        {
            // Create ErrorMessage
            var message = Error.MissingForeignKeyForRelation(null, null, null, "myRelation");

            string description = "Missing foreignKey(s) detected for relation 'myRelation'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_MissingForeignKeyInTable()
        {
            // Create ErrorMessage
            var message = Error.MissingForeignKeyInTable_Sub(null, null, null, "1000", "2000");

            string description = "Missing foreignKey between table '1000' and table '2000'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Relation_CheckPathAttribute_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "1000");

            string description = "Attribute 'Relation@path' references a non-existing 'Table' with PID '1000'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckPathAttribute();

        [TestMethod]
        public void Relation_CheckPathAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Relation);

        [TestMethod]
        public void Relation_CheckPathAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckPathAttribute);
    }
}