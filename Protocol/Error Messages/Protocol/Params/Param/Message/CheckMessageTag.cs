namespace SLDisValidator2.Tests.Protocol.Params.Param.Message.CheckMessageTag
{
    using System;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    public static class Error
    {
        public static IValidationResult MissingTag(IValidate test, IReadable referenceNode, IReadable positionNode, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckMessageTag,
                ErrorId = ErrorIds.MissingTag,
                FullId = "2.49.1",
                Category = Category.Param,
                Severity = Severity.BubbleUp,
                Certainty = Certainty.Uncertain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing tag '{0}' in {1} '{2}'.", "Param/Message", "Param", pid),
                HowToFix = "",
                ExampleCode = "<Message>Are you sure you want to restart the device?</Message>",
                Details = "A button executing a critical action should have a confirmation message." + Environment.NewLine + "This can be done by adding a 'Param/Message' tag.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult MissingTag_Sub(IValidate test, IReadable referenceNode, IReadable positionNode, string discreetDisplayValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckMessageTag,
                ErrorId = ErrorIds.MissingTag_Sub,
                FullId = "2.49.2",
                Category = Category.Param,
                Severity = Severity.Minor,
                Certainty = Certainty.Uncertain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing tag 'Param/Message' for button with caption '{0}'.", discreetDisplayValue),
                HowToFix = "",
                ExampleCode = "<Message>Are you sure you want to restart the device?</Message>",
                Details = "A button executing a critical action should have a confirmation message." + Environment.NewLine + "This can be done by adding a 'Param/Message' tag.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    public static class ErrorIds
    {
        public const uint MissingTag = 1;
        public const uint MissingTag_Sub = 2;
    }

    public static class CheckId
    {
        public const uint CheckMessageTag = 49;
    }
}