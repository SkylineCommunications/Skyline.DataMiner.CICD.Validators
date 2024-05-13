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
        public void QAction_CSharpCoreInterAppBrokerSupport_InvalidInterAppReplyLogic()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidInterAppReplyLogic",
                IsSolution = true,
                ExpectedResults = new List<IValidationResult>
                {
                  //("Invocation of method '{0}.{1}' is not compatible with 'DataMiner.Core.Interapp > 1.0.1.0'. QAction ID '{3}'
                  Error.InvalidInterAppReplyLogic(null, null, null, "Message", "Send(ReturnAddress", "1"),
                  Error.InvalidInterAppReplyLogic(null, null, null, "Protocol", "SetParameter(9000001", "2"), // best effort
                  Error.InvalidInterAppReplyLogic(null, null, null, "Protocol", "SetParameter(ReturnAddress", "3"),
                  Error.InvalidInterAppReplyLogic(null, null, null, "Message", "Send(ReturnAddress", "4"),
                  // Due to time contraints SetParameters currently not validated.
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
                Description = "Invocation of method 'methodClassType.incompatibleMethod' is not compatible with 'DataMiner.Core.InterApp > v1.0.1.0'. QAction ID 'qactionId'.",
                Details = "To maintain proper message handling, always use the .Reply() method on the original incoming message object received from an external source." + Environment.NewLine + "" + Environment.NewLine + "Do not use .Reply() on messages you create anew. " + Environment.NewLine + "Also, refrain from using .SetParameter(9000001, data); or .Send to the ReturnAddress, as these practices are incorrect." + Environment.NewLine + "" + Environment.NewLine + "Using the .Reply() method is crucial because it incorporates specific logic that optimizes communication between applications. It dynamically chooses the best delivery method, utilizing either SLNet Subscriptions or a more efficient message broker when available, ensuring that responses are handled efficiently and effectively based on the current configuration of the running agent. " + Environment.NewLine + "" + Environment.NewLine + "This optimization helps maintain system integrity and improves performance.",
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