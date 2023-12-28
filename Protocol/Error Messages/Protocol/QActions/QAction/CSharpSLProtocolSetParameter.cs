namespace SLDisValidator2.Tests.Protocol.QActions.QAction.CSharpSLProtocolSetParameter
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    public static class Error
    {
        public static IValidationResult NonExistingParam(IValidate test, IReadable referenceNode, IReadable positionNode, string paramId, string qactionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CSharpSLProtocolSetParameter,
                ErrorId = ErrorIds.NonExistingParam,
                FullId = "3.7.1",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Method '{0}' references a non-existing '{1}' with {2} '{3}'. QAction ID '{4}'.", "SLProtocol.SetParameter", "Param", "ID", paramId, qactionId),
                HowToFix = "",
                ExampleCode = "protocol.SetParameter(Parameter.ParameterName, value);",
                Details = "SLProtocol.SetParameter is used to update the value of a standalone parameter." + Environment.NewLine + "Make sure to provide it with an ID of a standalone parameter that exists." + Environment.NewLine + "Using Parameter class is recommended.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult HardCodedPid(IValidate test, IReadable referenceNode, IReadable positionNode, string hardCodedPid, string qactionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CSharpSLProtocolSetParameter,
                ErrorId = ErrorIds.HardCodedPid,
                FullId = "3.7.2",
                Category = Category.QAction,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Unrecommended use of magic number '{0}', use '{1}' {2} instead. QAction ID '{3}'.", hardCodedPid, "Parameter", "class", qactionId),
                HowToFix = "",
                ExampleCode = "protocol.SetParameter(Parameter.ParameterName, value);",
                Details = "SLProtocol.SetParameter is used to update the value of a standalone parameter." + Environment.NewLine + "Make sure to provide it with an ID of a standalone parameter that exists." + Environment.NewLine + "Using Parameter class is recommended.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult ParamMissingHistorySet(IValidate test, IReadable referenceNode, IReadable positionNode, string paramId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CSharpSLProtocolSetParameter,
                ErrorId = ErrorIds.ParamMissingHistorySet,
                FullId = "3.7.3",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("{0} overload with '{1}' argument requires '{2}'. {3} {4} '{5}'.", "SLProtocol.SetParameter", "ValueType timeInfo", "Param@historySet=true", "Param", "ID", paramId),
                HowToFix = "",
                ExampleCode = "",
                Details = "Every overload of the 'SLProtocol.SetParameter' method having the 'ValueType timeInfo' argument is meant to execute a historySet on a standlone parameter." + Environment.NewLine + "Such method requires the standalone parameter to be set to have the 'Param@historySet' attribute set to 'true'.",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    public static class ErrorIds
    {
        public const uint NonExistingParam = 1;
        public const uint HardCodedPid = 2;
        public const uint ParamMissingHistorySet = 3;
    }

    public static class CheckId
    {
        public const uint CSharpSLProtocolSetParameter = 7;
    }
}