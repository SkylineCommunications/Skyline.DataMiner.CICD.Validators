namespace ProtocolTests.Protocol.Triggers.Trigger.CheckAfterStartupFlow
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Triggers.Trigger.CheckAfterStartupFlow;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckAfterStartupFlow();

        #region Valid Checks

        [TestMethod]
        public void Trigger_CheckAfterStartupFlow_Valid()
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
        public void Trigger_CheckAfterStartupFlow_ValidExecute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidExecute",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckAfterStartupFlow_ValidExecuteOneNow()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidExecuteOneNow",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckAfterStartupFlow_ValidExecuteOneTop()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidExecuteOneTop",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Trigger_CheckAfterStartupFlow_InvalidAfterStartupTriggerCondition()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAfterStartupTriggerCondition",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAfterStartupTriggerCondition(null, null, null, "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckAfterStartupFlow_InvalidAfterStartupActionCondition()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAfterStartupActionCondition",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAfterStartupActionCondition(null, null, null, "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckAfterStartupFlow_InvalidAfterStartupTriggerType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAfterStartupTriggerType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAfterStartupTriggerType(null, null, null, "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckAfterStartupFlow_InvalidAfterStartupActionOn()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAfterStartupActionOn",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAfterStartupActionOn(null, null, null, "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckAfterStartupFlow_InvalidAfterStartupActionType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAfterStartupActionType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAfterStartupActionType(null, null, null, "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckAfterStartupFlow_InvalidAfterStartupGroupType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAfterStartupGroupType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAfterStartupGroupType(null, null, null, "1"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckAfterStartupFlow();

        [TestMethod]
        public void Trigger_CheckAfterStartupFlow_CheckCategory() => Generic.CheckCategory(root, Category.Trigger);

        [TestMethod]
        public void Trigger_CheckAfterStartupFlow_CheckId() => Generic.CheckId(root, CheckId.CheckAfterStartupFlow);
    }
}