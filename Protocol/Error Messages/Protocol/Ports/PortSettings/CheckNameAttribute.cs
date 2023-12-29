namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Ports.PortSettings.CheckNameAttribute
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
        public static IValidationResult MissingAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string connectionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckNameAttribute,
                ErrorId = ErrorIds.MissingAttribute,
                FullId = "12.1.2",
                Category = Category.Ports,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing attribute '{0}' in {1} '{2}'.", "Ports/PortSettings@name", "Connection", connectionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "- When having only one connection for a specific type, the name needs to be of following format, where the second option can (optionally) be used to add more info about the goal of the connection (ex: XXX = Traps, XXX = Events, XXX = Alarms, etc):" + Environment.NewLine + "     - \"IP Connection\" or \"IP Connection - XXX\": for drivers that support TCP and/or UDP" + Environment.NewLine + "     - \"HTTP Connection\" or \"HTTP Connection - XXX\"" + Environment.NewLine + "     - \"SSH Connection\" or \"SSH Connection - XXX\"" + Environment.NewLine + "     - \"SNMP Connection\" or \"SNMP Connection - XXX\"" + Environment.NewLine + "     - \"Serial Connection\" or \"Serial Connection - XXX\": for drivers that only support the physical serial port (driver connection of type serial but not support TCP nor UDP)" + Environment.NewLine + "- When having more than one connection for a specific type, the name need to be of following format where XXX is used to distinguish them (ex: XXX = Redundant, XXX = Redundant 2, XXX = Backup, XXX = Traps, XXX = Events, etc):" + Environment.NewLine + "     - \"IP Connection - XXX\"" + Environment.NewLine + "     - \"HTTP Connection - XXX\"" + Environment.NewLine + "     - etc",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult EmptyAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string connectionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckNameAttribute,
                ErrorId = ErrorIds.EmptyAttribute,
                FullId = "12.1.3",
                Category = Category.Ports,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty attribute '{0}' in {1} '{2}'.", "Ports/PortSettings@name", "Connection", connectionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "- When having only one connection for a specific type, the name needs to be of following format, where the second option can (optionally) be used to add more info about the goal of the connection (ex: XXX = Traps, XXX = Events, XXX = Alarms, etc):" + Environment.NewLine + "     - \"IP Connection\" or \"IP Connection - XXX\": for drivers that support TCP and/or UDP" + Environment.NewLine + "     - \"HTTP Connection\" or \"HTTP Connection - XXX\"" + Environment.NewLine + "     - \"SSH Connection\" or \"SSH Connection - XXX\"" + Environment.NewLine + "     - \"SNMP Connection\" or \"SNMP Connection - XXX\"" + Environment.NewLine + "     - \"Serial Connection\" or \"Serial Connection - XXX\": for drivers that only support the physical serial port (driver connection of type serial but not support TCP nor UDP)" + Environment.NewLine + "- When having more than one connection for a specific type, the name need to be of following format where XXX is used to distinguish them (ex: XXX = Redundant, XXX = Redundant 2, XXX = Backup, XXX = Traps, XXX = Events, etc):" + Environment.NewLine + "     - \"IP Connection - XXX\"" + Environment.NewLine + "     - \"HTTP Connection - XXX\"" + Environment.NewLine + "     - etc",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UntrimmedAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string connectionId, string attributeValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckNameAttribute,
                ErrorId = ErrorIds.UntrimmedAttribute,
                FullId = "12.1.4",
                Category = Category.Ports,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Untrimmed attribute '{0}' in {1} '{2}'. Current value '{3}'.", "Ports/PortSettings@name", "Connection", connectionId, attributeValue),
                HowToFix = "",
                ExampleCode = "",
                Details = "- When having only one connection for a specific type, the name needs to be of following format, where the second option can (optionally) be used to add more info about the goal of the connection (ex: XXX = Traps, XXX = Events, XXX = Alarms, etc):" + Environment.NewLine + "     - \"IP Connection\" or \"IP Connection - XXX\": for drivers that support TCP and/or UDP" + Environment.NewLine + "     - \"HTTP Connection\" or \"HTTP Connection - XXX\"" + Environment.NewLine + "     - \"SSH Connection\" or \"SSH Connection - XXX\"" + Environment.NewLine + "     - \"SNMP Connection\" or \"SNMP Connection - XXX\"" + Environment.NewLine + "     - \"Serial Connection\" or \"Serial Connection - XXX\": for drivers that only support the physical serial port (driver connection of type serial but not support TCP nor UDP)" + Environment.NewLine + "- When having more than one connection for a specific type, the name need to be of following format where XXX is used to distinguish them (ex: XXX = Redundant, XXX = Redundant 2, XXX = Backup, XXX = Traps, XXX = Events, etc):" + Environment.NewLine + "     - \"IP Connection - XXX\"" + Environment.NewLine + "     - \"HTTP Connection - XXX\"" + Environment.NewLine + "     - etc",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint MissingAttribute = 2;
        public const uint EmptyAttribute = 3;
        public const uint UntrimmedAttribute = 4;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckNameAttribute = 1;
    }
}