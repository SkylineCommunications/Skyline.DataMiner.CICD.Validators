namespace ProtocolTests.Protocol.Params.Param.Interprete.Exceptions.CheckTypeTag
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.Exceptions.CheckTypeTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckTypeTag();

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

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckTypeTag_ExceptionIncompatibleWithParamType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ExceptionIncompatibleWithParamType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ExceptionIncompatibleWithParamType(null, null, null, "1"),
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
        public void Param_CheckTypeTag_ExceptionIncompatibleWithParamType()
        {
            // Create ErrorMessage
            var message = Error.ExceptionIncompatibleWithParamType(null, null, null, "item1Value");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Incompatible 'Interprete/Exceptions' value 'item1Value' with 'Param/Type' value 'write'.",
                Details = "Do not use Exception tags to add exceptions to write parameters.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckTypeTag();

        [TestMethod]
        public void Param_CheckTypeTag_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckTypeTag_CheckId() => Generic.CheckId(check, CheckId.CheckTypeTag);
    }
}