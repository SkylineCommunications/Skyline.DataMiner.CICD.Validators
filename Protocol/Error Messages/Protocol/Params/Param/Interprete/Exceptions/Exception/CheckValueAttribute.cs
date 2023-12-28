namespace SLDisValidator2.Tests.Protocol.Params.Param.Interprete.Exceptions.Exception.CheckValueAttribute
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    public static class Error
    {
        public static IValidationResult MissingAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckValueAttribute,
                ErrorId = ErrorIds.MissingAttribute,
                FullId = "2.70.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing attribute '{0}' in {1} '{2}'.", "Exception@value", "Param", pid),
                HowToFix = "",
                ExampleCode = "",
                Details = "For each exception, the Exception@value attribute is required in order to define the incoming value that should be interpreted as exceptional." + Environment.NewLine + "The value should be compliant with the defined Param/Interprete/Type.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult ValueIncompatibleWithInterpreteType(IValidate test, IReadable referenceNode, IReadable positionNode, string exceptionValue, string interpreteType, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckValueAttribute,
                ErrorId = ErrorIds.ValueIncompatibleWithInterpreteType,
                FullId = "2.70.2",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Incompatible '{0}' value '{1}' with '{2}' value '{3}'. {4} {5} '{6}'.", "Exception@value", exceptionValue, "Interprete/Type", interpreteType, "Param", "ID", pid),
                HowToFix = "",
                ExampleCode = "",
                Details = "For each exception, the Exception@value attribute is required in order to define the incoming value that should be interpreted as exceptional." + Environment.NewLine + "The value should be compliant with the defined Param/Interprete/Type.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    public static class ErrorIds
    {
        public const uint MissingAttribute = 1;
        public const uint ValueIncompatibleWithInterpreteType = 2;
    }

    public static class CheckId
    {
        public const uint CheckValueAttribute = 70;
    }
}