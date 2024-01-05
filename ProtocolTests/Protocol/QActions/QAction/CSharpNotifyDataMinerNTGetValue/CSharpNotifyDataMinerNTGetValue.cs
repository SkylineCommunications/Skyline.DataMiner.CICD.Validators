namespace ProtocolTests.Protocol.QActions.QAction.CSharpNotifyDataMinerNTGetValue
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpNotifyDataMinerNTGetValue;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpNotifyDataMinerNTGetValue();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpNotifyDataMinerNTGetValue_Valid()
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
        public void QAction_CSharpNotifyDataMinerNTGetValue_DeltIncompatible()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DeltIncompatible",
                ExpectedResults = new List<IValidationResult>
                {
		            // Different ways to define NT
                    Error.DeltIncompatible(null, null, null, "100"),
                    Error.DeltIncompatible(null, null, null, "100"),

                    Error.DeltIncompatible(null, null, null, "100"),
                    Error.DeltIncompatible(null, null, null, "100"),
                    //Error.DeltIncompatible(null, null, null, "100"),
                    Error.DeltIncompatible(null, null, null, "100"),

                    Error.DeltIncompatible(null, null, null, "100"),
                    Error.DeltIncompatible(null, null, null, "100"),
                    //Error.DeltIncompatible(null, null, null, "100"),
                    Error.DeltIncompatible(null, null, null, "100"),
                    Error.DeltIncompatible(null, null, null, "100"),

                    Error.DeltIncompatible(null, null, null, "100"),
                    
		            // Different ways to define element
                    Error.DeltIncompatible(null, null, null, "101"),
                    Error.DeltIncompatible(null, null, null, "101"),
                    Error.DeltIncompatible(null, null, null, "101"),
                    Error.DeltIncompatible(null, null, null, "101"),

                    Error.DeltIncompatible(null, null, null, "101"),
                    Error.DeltIncompatible(null, null, null, "101"),
                    Error.DeltIncompatible(null, null, null, "101"),
                    Error.DeltIncompatible(null, null, null, "101"),

                    Error.DeltIncompatible(null, null, null, "101"),
                    Error.DeltIncompatible(null, null, null, "101"),
                    //Error.DeltIncompatible(null, null, null, "101"),

		            // Process result
                    Error.DeltIncompatible(null, null, null, "102"),
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
        public void QAction_CSharpNotifyDataMinerNTGetValue_DeltIncompatible()
        {
            // Create ErrorMessage
            var message = Error.DeltIncompatible(null, null, null, "1");

            var expected = new ValidationResult()
            {
                ErrorId = 1,
                FullId = "3.25.1",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Invocation of method 'SLProtocol.NotifyDataMiner(69/*NT_GET_VALUE*/, ...)' is not compatible with 'DELT'. QAction ID '1'.",
                HowToFix = "",
                ExampleCode = "uint[] elementDetails = new uint[] { agentId, elementId };" + Environment.NewLine + "int parameterId = 120;" + Environment.NewLine + "" + Environment.NewLine + "object[] result = (object[]) protocol.NotifyDataMiner(69 /*NT_GET_VALUE*/, elementDetails, parameterId);",
                Details = "To make this call DELT compatible, the DMA ID needs to be provided as argument." + Environment.NewLine + "See Example code." + Environment.NewLine + "" + Environment.NewLine + "More information about the syntax can be found in the DataMiner Development Library.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CSharpNotifyDataMinerNTGetValue();

        [TestMethod]
        public void QAction_CSharpNotifyDataMinerNTGetValue_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpNotifyDataMinerNTGetValue_CheckId() => Generic.CheckId(check, CheckId.CSharpNotifyDataMinerNTGetValue);
    }
}