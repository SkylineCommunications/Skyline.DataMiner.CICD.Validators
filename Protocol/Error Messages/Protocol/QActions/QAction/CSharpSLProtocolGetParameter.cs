namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpSLProtocolGetParameter
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    internal static class Error
    {
        internal static IValidationResult NonExistingParam(IValidate test, IReadable referenceNode, IReadable positionNode, string paramId, string qactionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CSharpSLProtocolGetParameter,
                ErrorId = ErrorIds.NonExistingParam,
                FullId = "3.6.1",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Method '{0}' references a non-existing '{1}' with {2} '{3}'. QAction ID '{4}'.", "SLProtocol.GetParameter", "Param", "ID", paramId, qactionId),
                HowToFix = "",
                ExampleCode = "protocol.GetParameter(Parameter.ParameterName);",
                Details = "SLProtocol.GetParameter is used to get the current value of a standalone parameter." + Environment.NewLine + "Make sure to provide it with an ID of a standalone parameter that exists." + Environment.NewLine + "Using Parameter class is recommended.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult HardCodedPid(IValidate test, IReadable referenceNode, IReadable positionNode, string hardCodedPid, string qactionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CSharpSLProtocolGetParameter,
                ErrorId = ErrorIds.HardCodedPid,
                FullId = "3.6.2",
                Category = Category.QAction,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Unrecommended use of magic number '{0}', use '{1}' {2} instead. QAction ID '{3}'.", hardCodedPid, "Parameter", "class", qactionId),
                HowToFix = "",
                ExampleCode = "protocol.GetParameter(Parameter.ParameterName);",
                Details = "SLProtocol.GetParameter is used to get the current value of a standalone parameter." + Environment.NewLine + "Make sure to provide it with an ID of a standalone parameter that exists." + Environment.NewLine + "Using Parameter class is recommended.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint NonExistingParam = 1;
        public const uint HardCodedPid = 2;
    }

    public static class CheckId
    {
        public const uint CSharpSLProtocolGetParameter = 6;
    }
}