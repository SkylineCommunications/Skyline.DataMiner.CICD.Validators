// <auto-generated>This is auto-generated code by Validator Management Tool. Do not modify.</auto-generated>
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpCoreInterAppBrokerSupport
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    internal static class Error
    {
        public static IValidationResult InvalidInterAppReplyLogic(IValidate test, IReadable referenceNode, IReadable positionNode, string methodClassType, string incompatibleMethod, string qactionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CSharpCoreInterAppBrokerSupport,
                ErrorId = ErrorIds.InvalidInterAppReplyLogic,
                FullId = "3.39.1",
                Category = Category.QAction,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invocation of method '{0}.{1}' is not compatible with '{2}'. QAction ID '{3}'.", methodClassType, incompatibleMethod, "DataMiner.Core.InterApp >= v1.0.1.1", qactionId),
                HowToFix = "To ensure correct handling of messages, always respond to incoming interapp messages by invoking the `.Reply()` method on the original message object received from an external source. " + Environment.NewLine + "" + Environment.NewLine + "It is incorrect to use the `.Reply()` method on newly created message objects. " + Environment.NewLine + "Additionally, avoid performing `.SetParameter(9000001, data);` or invoking `.Send` to the `ReturnAddress`, as these actions are also incorrect.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint InvalidInterAppReplyLogic = 1;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CSharpCoreInterAppBrokerSupport = 39;
    }
}