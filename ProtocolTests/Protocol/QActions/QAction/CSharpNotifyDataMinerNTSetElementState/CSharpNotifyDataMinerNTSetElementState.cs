namespace ProtocolTests.Protocol.QActions.QAction.CSharpNotifyDataMinerNTSetElementState
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpNotifyDataMinerNTSetElementState;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpNotifyDataMinerNTSetElementState();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpNotifyDataMinerNTSetElementState_Valid()
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
        public void QAction_CSharpNotifyDataMinerNTSetElementState_DeltIncompatible()
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
                    //Error.DeltIncompatible(null, null, null, "101"),
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
        public void QAction_CSharpNotifyDataMinerNTSetElementState_DeltIncompatible()
        {
            // Create ErrorMessage
            var message = Error.DeltIncompatible(null, null, null, "1");

            var expected = new ValidationResult()
            {
                ErrorId = 1,
                FullId = "3.16.1",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Invocation of method 'SLProtocol.NotifyDataMiner(Queued)(115/*NT_SET_ELEMENT_STATE*/, ...)' is not compatible with 'DELT'. QAction ID '1'.",
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
        private readonly IRoot check = new CSharpNotifyDataMinerNTSetElementState();

        [TestMethod]
        public void QAction_CSharpNotifyDataMinerNTSetElementState_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpNotifyDataMinerNTSetElementState_CheckId() => Generic.CheckId(check, CheckId.CSharpNotifyDataMinerNTSetElementState);
    }
}