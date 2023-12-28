namespace SLDisValidator2.Tests.Protocol.Params.Param.Measurement.Discreets.Discreet.CheckOptionsAttribute
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    internal static class Error
    {
        internal static IValidationResult MisconfiguredConfirmOptions(IValidate test, IReadable referenceNode, IReadable positionNode, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.MisconfiguredConfirmOptions,
                FullId = "2.50.1",
                Category = Category.Param,
                Severity = Severity.BubbleUp,
                Certainty = Certainty.Uncertain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Misconfigured 'confirm' option(s) in 'Discreet@options' for ContextMenu. Param ID '{0}'.", pid),
                HowToFix = "",
                ExampleCode = "<Discreet options=\"confirm:The selected item(s) will be deleted permanently.\">" + Environment.NewLine + "    <Display>Delete selected row(s)</Display>" + Environment.NewLine + "    <Value>delete</Value>" + Environment.NewLine + "</Discreet>",
                Details = "A context menu action executing a critical action should have a confirmation message." + Environment.NewLine + "This can be done by adding the confirm option via the 'Discreet@options' attribute.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult MissingConfirmOption(IValidate test, IReadable referenceNode, IReadable positionNode, string contextMenuItem, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.MissingConfirmOption,
                FullId = "2.50.2",
                Category = Category.Param,
                Severity = Severity.Minor,
                Certainty = Certainty.Uncertain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing value '{0}' in attribute '{1}' for {2} '{3}'. {4} {5} '{6}'.", "confirm", "Discreet@options", "context-menu item", contextMenuItem, "Param", "ID", pid),
                HowToFix = "",
                ExampleCode = "<Discreet options=\"confirm:The selected item(s) will be deleted permanently.\">" + Environment.NewLine + "    <Display>Delete selected row(s)</Display>" + Environment.NewLine + "    <Value>delete</Value>" + Environment.NewLine + "</Discreet>",
                Details = "A context menu action executing a critical action should have a confirmation message." + Environment.NewLine + "This can be done by adding the confirm option via the 'Discreet@options' attribute.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult EmptyConfirmOption(IValidate test, IReadable referenceNode, IReadable positionNode, string contextMenuItem, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.EmptyConfirmOption,
                FullId = "2.50.3",
                Category = Category.Param,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty option '{0}' in attribute '{1}' for {2} '{3}'. {4} {5} {6}'.", "confirm", "Discreet@options", "context-menu item", contextMenuItem, "Param", "ID", pid),
                HowToFix = "",
                ExampleCode = "<Discreet options=\"confirm:The selected item(s) will be deleted permanently.\">" + Environment.NewLine + "    <Display>Delete selected row(s)</Display>" + Environment.NewLine + "    <Value>delete</Value>" + Environment.NewLine + "</Discreet>",
                Details = "A context menu action executing a critical action should have a confirmation message." + Environment.NewLine + "This can be done by adding the confirm option via the 'Discreet@options' attribute.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult UntrimmedConfirmOption(IValidate test, IReadable referenceNode, IReadable positionNode, string contextMenuItem, string pid, string untrimmedValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.UntrimmedConfirmOption,
                FullId = "2.50.4",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Untrimmed option '{0}' in attribute '{1}' for {2} '{3}' in {4} with {5} '{6}'. Current value '{7}'.", "confirm", "Discreet@options", "context-menu item", contextMenuItem, "Param", "ID", pid, untrimmedValue),
                HowToFix = "",
                ExampleCode = "<Discreet options=\"confirm:The selected item(s) will be deleted permanently.\">" + Environment.NewLine + "    <Display>Delete selected row(s)</Display>" + Environment.NewLine + "    <Value>delete</Value>" + Environment.NewLine + "</Discreet>",
                Details = "A context menu action executing a critical action should have a confirmation message." + Environment.NewLine + "This can be done by adding the confirm option via the 'Discreet@options' attribute.",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint MisconfiguredConfirmOptions = 1;
        public const uint MissingConfirmOption = 2;
        public const uint EmptyConfirmOption = 3;
        public const uint UntrimmedConfirmOption = 4;
    }

    public static class CheckId
    {
        public const uint CheckOptionsAttribute = 50;
    }
}