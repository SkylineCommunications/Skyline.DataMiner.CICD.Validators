namespace ProtocolTests.Protocol.Params.Param.SNMP.Type.CheckInconsistentSnmpReadWriteTypes
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.SNMP.Type.CheckInconsistentSnmpReadWriteTypes;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckInconsistentSnmpReadWriteTypes();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckInconsistentSnmpReadWriteTypes_Valid()
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
        public void Param_CheckInconsistentSnmpReadWriteTypes_InconsistentSnmpReadWriteTypes()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InconsistentSnmpReadWriteTypes",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InconsistentSnmpReadWriteTypes(null, null, null, "snmpParam", "integer", "octetstring"),
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
        public void Param_CheckInconsistentSnmpReadWriteTypes_InconsistentSnmpReadWriteTypes()
        {
            // Create ErrorMessage
            var message = Error.InconsistentSnmpReadWriteTypes(null, null, null, "paramName", "readType", "writeType");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Uncertain,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = "Read and Write of SNMP Parameter 'paramName' have different SNMP Types. As SNMP Types are usually the same, this might cause the read or write to not work. Read type: 'readType' ; Write type: 'writeType'",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckInconsistentSnmpReadWriteTypes();

        [TestMethod]
        public void Param_CheckInconsistentSnmpReadWriteTypes_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckInconsistentSnmpReadWriteTypes_CheckId() => Generic.CheckId(check, CheckId.CheckInconsistentSnmpReadWriteTypes);
    }
}