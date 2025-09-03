namespace ProtocolTests.Protocol.QActions.QAction.CSharpCoreInterAppBrokerSupport
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpCoreInterAppBrokerSupport;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpCoreInterAppBrokerSupport();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpCoreInterAppBrokerSupport_Valid_XmlBased()
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
        public void QAction_CSharpCoreInterAppBrokerSupport_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                IsSolution = true,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        [Ignore("Isn't really relevant and causes other checks to fail")]
        public void QAction_CSharpCoreInterAppBrokerSupport_InvalidInterAppReplyLogic_XmlBased()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidInterAppReplyLogic",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
#if NETFRAMEWORK
        [Ignore("Roslyn part gives build errors, so no QActions can be properly checked in .NET Framework")]
#endif
        public void QAction_CSharpCoreInterAppBrokerSupport_InvalidInterAppReplyLogic()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidInterAppReplyLogic",
                IsSolution = true,
                ExpectedResults = new List<IValidationResult>
                {
                    // Invocation of method '{0}.{1}' is not compatible with 'DataMiner.Core.InterApp >= 1.0.1.1'. QAction ID '{3}'.
                    Error.InvalidInterAppReplyLogic(null, null, null, "Message", "Send(ReturnAddress", "1"),
                    Error.InvalidInterAppReplyLogic(null, null, null, "Protocol", "SetParameter(9000001", "2"), // best effort
                    Error.InvalidInterAppReplyLogic(null, null, null, "Protocol", "SetParameter(ReturnAddress", "3"),
                    Error.InvalidInterAppReplyLogic(null, null, null, "Message", "Send(ReturnAddress", "4"),
                    Error.InvalidInterAppReplyLogic(null, null, null, "Message", "Send(ReturnAddress", "5"),
                    // Due to time constraints SetParameters currently not validated.
                    // SetParameters(  is unlikely to be used, reply message had to be sent one by one.
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
        public void QAction_CSharpCoreInterAppBrokerSupport_InvalidInterAppReplyLogic()
        {
            // Create ErrorMessage
            var message = Error.InvalidInterAppReplyLogic(null, null, null, "methodClassType", "incompatibleMethod", "qactionId");

            var expected = new ValidationResult
            {
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Invocation of method 'methodClassType.incompatibleMethod' is not compatible with 'DataMiner.Core.InterApp >= v1.0.1.1'. QAction ID 'qactionId'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CSharpCoreInterAppBrokerSupport();

        [TestMethod]
        public void QAction_CSharpCoreInterAppBrokerSupport_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpCoreInterAppBrokerSupport_CheckId() => Generic.CheckId(check, CheckId.CSharpCoreInterAppBrokerSupport);
    }
}