namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.LengthType.CheckIdAttribute
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    internal static class Error
    {
        internal static IValidationResult MissingAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string paramId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckIdAttribute,
                ErrorId = ErrorIds.MissingAttribute,
                FullId = "2.69.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing attribute '{0}' due to '{1}' '{2}'. {3} {4} '{5}'.", "Interprete/LengthType@id", "LengthType", "other param", "Param", "ID", paramId),
                HowToFix = "",
                ExampleCode = "",
                Details = "When Interprete/LengthType tag is set to 'other param', the id attribute should be added to it and should refer to the id of an existing parameter of Interprete/Type double.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult EmptyAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string paramId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckIdAttribute,
                ErrorId = ErrorIds.EmptyAttribute,
                FullId = "2.69.2",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty attribute '{0}' in {1} '{2}'.", "Interprete/LengthType@id", "Param", paramId),
                HowToFix = "",
                ExampleCode = "",
                Details = "When Interprete/LengthType tag is set to 'other param', the id attribute should be added to it and should refer to the id of an existing parameter of Interprete/Type double.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult UntrimmedAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string paramId, string untrimmedValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckIdAttribute,
                ErrorId = ErrorIds.UntrimmedAttribute,
                FullId = "2.69.3",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Untrimmed attribute '{0}' in {1} '{2}'. Current value '{3}'.", "id", "Param", paramId, untrimmedValue),
                HowToFix = "",
                ExampleCode = "",
                Details = "When Interprete/LengthType tag is set to 'other param', the id attribute should be added to it and should refer to the id of an existing parameter of Interprete/Type double.",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult NonExistingId(IValidate test, IReadable referenceNode, IReadable positionNode, string nonExistingParamId, string paramId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckIdAttribute,
                ErrorId = ErrorIds.NonExistingId,
                FullId = "2.69.4",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Attribute '{0}' references a non-existing '{1}' with {2} '{3}'. {4} {5} '{6}'.", "Interprete/LengthType@id", "Param", "ID", nonExistingParamId, "Param", "ID", paramId),
                HowToFix = "",
                ExampleCode = "",
                Details = "When Interprete/LengthType tag is set to 'other param', the id attribute should be added to it and should refer to the id of an existing parameter of Interprete/Type double.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult ReferencedParamWrongInterpreteType(IValidate test, IReadable referenceNode, IReadable positionNode, string interpreteType, string paramId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckIdAttribute,
                ErrorId = ErrorIds.ReferencedParamWrongInterpreteType,
                FullId = "2.69.5",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid {0} '{1}' on {4} referenced by {2}. Expected value '{3}'. {4} {5} '{6}'.", "Interprete/Type", interpreteType, "Interprete/LengthType@id", "double", "Param", "ID", paramId),
                HowToFix = "",
                ExampleCode = "",
                Details = "When Interprete/LengthType tag is set to 'other param', the id attribute should be added to it and should refer to the id of an existing parameter of Interprete/Type double.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint MissingAttribute = 1;
        public const uint EmptyAttribute = 2;
        public const uint UntrimmedAttribute = 3;
        public const uint NonExistingId = 4;
        public const uint ReferencedParamWrongInterpreteType = 5;
    }

    public static class CheckId
    {
        public const uint CheckIdAttribute = 69;
    }
}