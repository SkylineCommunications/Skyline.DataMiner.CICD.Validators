namespace ProtocolTests.Protocol.Params.Param.CheckColumns
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.CheckColumns;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckColumns();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckColumns_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckColumns_ColumnInvalidType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ColumnInvalidType",
                ExpectedResults = new List<IValidationResult>
                {
                    // Table Syntax 1
                    Error.ColumnInvalidType(null, null, null, "array", "1001"),
                    Error.ColumnInvalidType(null, null, null, "bus", "1002"),
                    
                    // Table Syntax 2
                    Error.ColumnInvalidType(null, null, null, "crc", "1101"),
                    Error.ColumnInvalidType(null, null, null, "dataminer info", "1102"),
                    
                    // Table Syntax 3
                    Error.ColumnInvalidType(null, null, null, "discreet info", "1201"),
                    Error.ColumnInvalidType(null, null, null, "dummy", "1202"),
                    
                    // View Table
                    Error.ColumnInvalidType(null, null, null, "array", "10001"),
                    Error.ColumnInvalidType(null, null, null, "bus", "10002"),
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
        public void Param_CheckColumns_ColumnInvalidType()
        {
            // Create ErrorMessage
            var message = Error.ColumnInvalidType(null, null, null, "columnType", "columnPid");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "2.64.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Invalid value 'columnType' in tag 'Param/Type' for column. Possible values 'read, write, group, read bit, write bit'. Column PID 'columnPid'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckColumns();

        [TestMethod]
        public void Param_CheckColumns_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckColumns_CheckId() => Generic.CheckId(check, CheckId.CheckColumns);
    }
}