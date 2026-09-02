namespace ProtocolTests.Protocol.Params.Param.SNMP.Type.CheckMismatchingSNMPReadWriteType
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.SNMP.Type.CheckMismatchingSNMPReadWriteType;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckMismatchingSNMPReadWriteType();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckMismatchingSNMPReadWriteType_Valid()
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
        public void Param_CheckMismatchingSNMPReadWriteType_MismatchedSNMPReadWriteType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MismatchedSNMPReadWriteType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MismatchedSNMPReadWriteType(null, null, null, "snmpParam", "integer", "octetstring"),
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
        public void Param_CheckMismatchingSNMPReadWriteType_MismatchedSNMPReadWriteType()
        {
            // Create ErrorMessage
            var message = Error.MismatchedSNMPReadWriteType(null, null, null, "paramName", "readType", "writeType");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Uncertain,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = "Read and Write of SNMP Parameter 'paramName' have different SNMP Types. Read type: 'readType' ; Write type: 'writeType'",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckMismatchingSNMPReadWriteType();

        [TestMethod]
        public void Param_CheckMismatchingSNMPReadWriteType_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckMismatchingSNMPReadWriteType_CheckId() => Generic.CheckId(check, CheckId.CheckMismatchingSNMPReadWriteType);
    }
}