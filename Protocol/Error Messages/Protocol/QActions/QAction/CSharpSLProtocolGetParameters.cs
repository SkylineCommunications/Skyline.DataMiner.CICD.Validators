namespace SLDisValidator2.Tests.Protocol.QActions.QAction.CSharpSLProtocolGetParameters
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    internal static class Error
    {
        internal static IValidationResult UnexpectedImplementation(IValidate test, IReadable referenceNode, IReadable positionNode, string arguments, string qactionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CSharpSLProtocolGetParameters,
                ErrorId = ErrorIds.UnexpectedImplementation,
                FullId = "3.33.1",
                Category = Category.QAction,
                Severity = Severity.BubbleUp,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Method '{0}' with arguments '{1}' is not implemented as expected. QAction ID '{2}'.", "SLProtocol.GetParameters", arguments, qactionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "SLProtocol.GetParameters is used to get current values of standalone parameters." + Environment.NewLine + "Make sure to provide it with a uint array of existing standalone parameter IDs." + Environment.NewLine + "Using Parameter class is recommended.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult NonExistingParam(IValidate test, IReadable referenceNode, IReadable positionNode, string paramId, string qactionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CSharpSLProtocolGetParameters,
                ErrorId = ErrorIds.NonExistingParam,
                FullId = "3.33.2",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Method '{0}' references a non-existing '{1}' with {2} '{3}'. QAction ID '{4}'.", "SLProtocol.GetParameters", "Param", "ID", paramId, qactionId),
                HowToFix = "",
                ExampleCode = "protocol.GetParameters(new uint[] { Parameter.ParameterName, Parameter.ParameterName2 });",
                Details = "SLProtocol.GetParameters is used to get current values of standalone parameters." + Environment.NewLine + "Make sure to provide it with a uint array of existing standalone parameter IDs." + Environment.NewLine + "Using Parameter class is recommended.",
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
                CheckId = CheckId.CSharpSLProtocolGetParameters,
                ErrorId = ErrorIds.HardCodedPid,
                FullId = "3.33.3",
                Category = Category.QAction,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Unrecommended use of magic number '{0}', use '{1}' {2} instead. QAction ID '{3}'.", hardCodedPid, "Parameter", "class", qactionId),
                HowToFix = "",
                ExampleCode = "protocol.GetParameters(new uint[] { Parameter.ParameterName, Parameter.ParameterName2 });",
                Details = "SLProtocol.GetParameters is used to get current values of standalone parameters." + Environment.NewLine + "Make sure to provide it with a uint array of existing standalone parameter IDs." + Environment.NewLine + "Using Parameter class is recommended.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult UnsupportedArgumentTypeForIds(IValidate test, IReadable referenceNode, IReadable positionNode, string argumentType, string qactionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CSharpSLProtocolGetParameters,
                ErrorId = ErrorIds.UnsupportedArgumentTypeForIds,
                FullId = "3.33.4",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invocation of method '{0}' has an invalid type '{1}' for the argument '{2}'. QAction ID '{3}'.", "SLProtocol.GetParameters", argumentType, "ids", qactionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "SLProtocol.GetParameters is used to get current values of standalone parameters." + Environment.NewLine + "Make sure to provide it with a uint array of existing standalone parameter IDs." + Environment.NewLine + "Using Parameter class is recommended.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint UnexpectedImplementation = 1;
        public const uint NonExistingParam = 2;
        public const uint HardCodedPid = 3;
        public const uint UnsupportedArgumentTypeForIds = 4;
    }

    public static class CheckId
    {
        public const uint CSharpSLProtocolGetParameters = 33;
    }
}