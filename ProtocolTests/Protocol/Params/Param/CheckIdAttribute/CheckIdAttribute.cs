namespace ProtocolTests.Protocol.Params.Param.CheckIdAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.CheckIdAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckIdAttribute();

        #region Valid

        [TestMethod]
        public void Param_CheckIdAttribute_Mediation()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Mediation",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_Normal()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Normal",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_Service()
        {
            // Another unit test covers the full list of parameters.
            // See ProtocolTests\Helpers\Software Parameters\Skyline Service Definition Basic

            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Service",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_Sla()
        {
            // Another unit test covers the full list of parameters.
            // See ProtocolTests\Helpers\Software Parameters\Skyline SLA Definition Basic

            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Sla",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_Spectrum()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Spectrum",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_Valid()
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

        #region Invalid

        [TestMethod]
        public void Param_CheckIdAttribute_DuplicatedId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicatedId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicatedId(null, null, null, "101", "Duplicate_101_1, Duplicate_101_2").WithSubResults(
                        Error.DuplicatedId(null, null, null, "101", "Duplicate_101_1"),
                        Error.DuplicatedId(null, null, null, "101", "Duplicate_101_2"))
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null),
                    Error.EmptyAttribute(null, null, null),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "aaa", "String"),

                    Error.InvalidValue(null, null, null, "-2", "Number_Negative"),
                    Error.InvalidValue(null, null, null, "1.5", "Number_Double_1"),
                    Error.InvalidValue(null, null, null, "2,6", "Number_Double_2"),
                    Error.InvalidValue(null, null, null, "03", "Number_LeadingZero"),
                    Error.InvalidValue(null, null, null, "+4", "Number_LeadingPlusSign"),
                    Error.InvalidValue(null, null, null, "5x10^1", "Number_ScientificNotation"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        [Ignore("Currently ignored as it shouldn't give any results back...")]
        public void Param_CheckIdAttribute_InvalidUseOfDataMinerModulesIdRange()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidUseOfDataMinerModulesIdRange",
                ExpectedResults = new List<IValidationResult>
                {
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_InvalidUseOfEnhancedServiceIdRange()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidUseOfEnhancedServiceIdRange",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidUseOfEnhancedServiceIdRange(null, null, null, "1"),
                    Error.InvalidUseOfEnhancedServiceIdRange(null, null, null, "999"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_InvalidUseOfMediationIdRange()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidUseOfMediationIdRange",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidUseOfMediationIdRange(null, null, null, "70000"),
                    Error.InvalidUseOfMediationIdRange(null, null, null, "79999"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_InvalidUseOfSlaIdRange()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidUseOfSlaIdRange",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidUseOfSlaIdRange(null, null, null, "1"),
                    Error.InvalidUseOfSlaIdRange(null, null, null, "2999"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_InvalidUseOfSpectrumIdRange()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidUseOfSpectrumIdRange",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidUseOfSpectrumIdRange(null, null, null, "50000"),
                    Error.InvalidUseOfSpectrumIdRange(null, null, null, "59999"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_InvalidUseOfSpectrumIdRange2()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidUseOfSpectrumIdRange2",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidUseOfSpectrumIdRange(null, null, null, "64000"),
                    Error.InvalidUseOfSpectrumIdRange(null, null, null, "64299"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_OutOfRangeId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "OutOfRangeId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.OutOfRangeId(null, null, null, "0"),
                    Error.OutOfRangeId(null, null, null, "64300"),
                    Error.OutOfRangeId(null, null, null, "69999"),
                    Error.OutOfRangeId(null, null, null, "100000"),
                    Error.OutOfRangeId(null, null, null, "999999"),
                    Error.OutOfRangeId(null, null, null, "10000000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void Param_CheckIdAttribute_RTDisplayExpectedOnSpectrumParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpectedOnSpectrumParam",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpectedOnSpectrumParam(null, null, null, "64000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, " 101"),
                    Error.UntrimmedAttribute(null, null, null, "102 "),
                    Error.UntrimmedAttribute(null, null, null, " 103 "),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckIdAttribute();

        [TestMethod]
        public void Command_CheckIdAttribute_UntrimmedAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedAttribute",
            };

            Generic.Fix(check, data);
        }
    }

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckIdAttribute();

        [TestMethod]
        public void Param_CheckIdAttribute_Valid()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_MissingParam()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "MissingParam",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.MissingParam(null, null, "Standalone_MissingReadOnly", "read", "1"),
                    ErrorCompare.MissingParam(null, null, "Standalone_MissingRead", "read", "100"),
                    ErrorCompare.MissingParam(null, null, "Standalone_MissingWriteOnly", "write", "200"),
                    ErrorCompare.MissingParam(null, null, "Standalone_MissingWrite", "write", "350"),

                    ErrorCompare.MissingParam(null, null, "Table_MissingWrite", "write", "1052"),
                }
            };

            Generic.Compare(compare, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Param_CheckIdAttribute_MissingAttribute()
        {
            // Create ErrorMessage
            var message = Error.MissingAttribute(null, null, null);

            string description = "Missing attribute 'Param@id'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null);

            string description = "Empty attribute 'Param@id'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_InvalidId()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "0", "MyName");

            string description = "Invalid value '0' in attribute 'Param@id'. Param name 'MyName'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_OutOfRangeId()
        {
            // Create ErrorMessage
            var message = Error.OutOfRangeId(null, null, null, "0");

            string description = "Out of range Param ID '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_InvalidUseOfSpectrumIdRange()
        {
            // Create ErrorMessage
            var message = Error.InvalidUseOfSpectrumIdRange(null, null, null, "0");

            string description = "Invalid use of Spectrum ID range for Param with ID '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_InvalidUseOfMediationIdRange()
        {
            // Create ErrorMessage
            var message = Error.InvalidUseOfMediationIdRange(null, null, null, "0");

            string description = "Invalid use of Mediation ID range for Param with ID '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_InvalidUseOfDataMinerModulesIdRange()
        {
            // Create ErrorMessage
            var message = Error.InvalidUseOfDataMinerModulesIdRange(null, null, null, "0");

            string description = "Invalid use of DataMiner Modules ID range for Param with ID '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_InvalidUseOfEnhancedServiceIdRange()
        {
            // Create ErrorMessage
            var message = Error.InvalidUseOfEnhancedServiceIdRange(null, null, null, "0");

            string description = "Invalid use of Enhanced Service ID range for Param with ID '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_InvalidUseOfSlaIdRange()
        {
            // Create ErrorMessage
            var message = Error.InvalidUseOfSlaIdRange(null, null, null, "0");

            string description = "Invalid use of SLA ID range for Param with ID '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_DuplicatedId()
        {
            // Create ErrorMessage
            var message = Error.DuplicatedId(null, null, null, "0", "MyParam1, MyParam2");

            string description = "More than one Param with same ID '0'. Param Names 'MyParam1, MyParam2'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_MissingParam()
        {
            // Create ErrorMessage
            var message = ErrorCompare.MissingParam(null, null, "0", "1", "2");

            string description = "Missing displayed Param. Param Name '0'. Param Type '1'. Param ID '2'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckIdAttribute();

        [TestMethod]
        public void Param_CheckIdAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckIdAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckIdAttribute);
    }
}