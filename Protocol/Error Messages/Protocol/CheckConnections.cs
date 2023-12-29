namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.CheckConnections
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
        public static IValidationResult MismatchingNames(IValidate test, IReadable referenceNode, IReadable positionNode, string connectionId, string names, bool hasCodeFix)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckConnections,
                ErrorId = ErrorIds.MismatchingNames,
                FullId = "1.23.1",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Connection {0} has mismatching names: {1}.", connectionId, names),
                HowToFix = "",
                ExampleCode = "",
                Details = "- When having only one connection for a specific type, the name needs to be of following format, where the second option can (optionally) be used to add more info about the goal of the connection (ex: XXX = Traps, XXX = Events, XXX = Alarms, etc):" + Environment.NewLine + "     - \"IP Connection\" or \"IP Connection - XXX\": for drivers that support TCP and/or UDP (See PortTypeIP, PortTypeUDP and PortTypeSerial tags under PortSettings tag)" + Environment.NewLine + "     - \"HTTP Connection\" or \"HTTP Connection - XXX\"" + Environment.NewLine + "     - \"SSH Connection\" or \"SSH Connection - XXX\"" + Environment.NewLine + "     - \"SNMP Connection\" or \"SNMP Connection - XXX\"" + Environment.NewLine + "     - \"Serial Connection\" or \"Serial Connection - XXX\": for drivers that only support the physical serial port. In other words, driver connections of type serial which don't support TCP nor UDP (See PortTypeIP, PortTypeUDP and PortTypeSerial tags under PortSettings tag)." + Environment.NewLine + "- When having more than one connection for a specific type, the name needs to be of following format where XXX is used to distinguish them (ex: XXX = Redundant, XXX = Redundant 2, XXX = Backup, XXX = Traps, XXX = Events, etc):" + Environment.NewLine + "     - \"IP Connection - XXX\"" + Environment.NewLine + "     - \"HTTP Connection - XXX\"" + Environment.NewLine + "     - etc",
                HasCodeFix = hasCodeFix,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidConnectionName(IValidate test, IReadable referenceNode, IReadable positionNode, string connectionName, string connectionType, string connectionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckConnections,
                ErrorId = ErrorIds.InvalidConnectionName,
                FullId = "1.23.2",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid connection name '{0}' for a '{1}' connection. Connection ID '{2}'.", connectionName, connectionType, connectionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "- When having only one connection for a specific type, the name needs to be of following format, where the second option can (optionally) be used to add more info about the goal of the connection (ex: XXX = Traps, XXX = Events, XXX = Alarms, etc):" + Environment.NewLine + "     - \"IP Connection\" or \"IP Connection - XXX\": for drivers that support TCP and/or UDP (See PortTypeIP, PortTypeUDP and PortTypeSerial tags under PortSettings tag)" + Environment.NewLine + "     - \"HTTP Connection\" or \"HTTP Connection - XXX\"" + Environment.NewLine + "     - \"SSH Connection\" or \"SSH Connection - XXX\"" + Environment.NewLine + "     - \"SNMP Connection\" or \"SNMP Connection - XXX\"" + Environment.NewLine + "     - \"Serial Connection\" or \"Serial Connection - XXX\": for drivers that only support the physical serial port. In other words, driver connections of type serial which don't support TCP nor UDP (See PortTypeIP, PortTypeUDP and PortTypeSerial tags under PortSettings tag)." + Environment.NewLine + "- When having more than one connection for a specific type, the name needs to be of following format where XXX is used to distinguish them (ex: XXX = Redundant, XXX = Redundant 2, XXX = Backup, XXX = Traps, XXX = Events, etc):" + Environment.NewLine + "     - \"IP Connection - XXX\"" + Environment.NewLine + "     - \"HTTP Connection - XXX\"" + Environment.NewLine + "     - etc",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult DuplicateConnectionName(IValidate test, IReadable referenceNode, IReadable positionNode, string duplicateName)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckConnections,
                ErrorId = ErrorIds.DuplicateConnectionName,
                FullId = "1.23.3",
                Category = Category.Protocol,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Duplicated {0} {1} '{2}'.", "Connection", "name", duplicateName),
                HowToFix = "",
                ExampleCode = "",
                Details = "- When having only one connection for a specific type, the name needs to be of following format, where the second option can (optionally) be used to add more info about the goal of the connection (ex: XXX = Traps, XXX = Events, XXX = Alarms, etc):" + Environment.NewLine + "     - \"IP Connection\" or \"IP Connection - XXX\": for drivers that support TCP and/or UDP (See PortTypeIP, PortTypeUDP and PortTypeSerial tags under PortSettings tag)" + Environment.NewLine + "     - \"HTTP Connection\" or \"HTTP Connection - XXX\"" + Environment.NewLine + "     - \"SSH Connection\" or \"SSH Connection - XXX\"" + Environment.NewLine + "     - \"SNMP Connection\" or \"SNMP Connection - XXX\"" + Environment.NewLine + "     - \"Serial Connection\" or \"Serial Connection - XXX\": for drivers that only support the physical serial port. In other words, driver connections of type serial which don't support TCP nor UDP (See PortTypeIP, PortTypeUDP and PortTypeSerial tags under PortSettings tag)." + Environment.NewLine + "- When having more than one connection for a specific type, the name needs to be of following format where XXX is used to distinguish them (ex: XXX = Redundant, XXX = Redundant 2, XXX = Backup, XXX = Traps, XXX = Events, etc):" + Environment.NewLine + "     - \"IP Connection - XXX\"" + Environment.NewLine + "     - \"HTTP Connection - XXX\"" + Environment.NewLine + "     - etc",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult DuplicateConnectionName_Sub(IValidate test, IReadable referenceNode, IReadable positionNode, string duplicateName, string connectionIds)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckConnections,
                ErrorId = ErrorIds.DuplicateConnectionName_Sub,
                FullId = "1.23.4",
                Category = Category.Protocol,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Duplicated {0} {1} '{2}'. {0} IDs '{3}'.", "Connection", "name", duplicateName, connectionIds),
                HowToFix = "",
                ExampleCode = "",
                Details = "- When having only one connection for a specific type, the name needs to be of following format, where the second option can (optionally) be used to add more info about the goal of the connection (ex: XXX = Traps, XXX = Events, XXX = Alarms, etc):" + Environment.NewLine + "     - \"IP Connection\" or \"IP Connection - XXX\": for drivers that support TCP and/or UDP (See PortTypeIP, PortTypeUDP and PortTypeSerial tags under PortSettings tag)" + Environment.NewLine + "     - \"HTTP Connection\" or \"HTTP Connection - XXX\"" + Environment.NewLine + "     - \"SSH Connection\" or \"SSH Connection - XXX\"" + Environment.NewLine + "     - \"SNMP Connection\" or \"SNMP Connection - XXX\"" + Environment.NewLine + "     - \"Serial Connection\" or \"Serial Connection - XXX\": for drivers that only support the physical serial port. In other words, driver connections of type serial which don't support TCP nor UDP (See PortTypeIP, PortTypeUDP and PortTypeSerial tags under PortSettings tag)." + Environment.NewLine + "- When having more than one connection for a specific type, the name needs to be of following format where XXX is used to distinguish them (ex: XXX = Redundant, XXX = Redundant 2, XXX = Backup, XXX = Traps, XXX = Events, etc):" + Environment.NewLine + "     - \"IP Connection - XXX\"" + Environment.NewLine + "     - \"HTTP Connection - XXX\"" + Environment.NewLine + "     - etc",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidConnectionCount(IValidate test, IReadable referenceNode, IReadable positionNode, string connectionCount, string portSettingCount)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckConnections,
                ErrorId = ErrorIds.InvalidConnectionCount,
                FullId = "1.23.5",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Connection count in 'Protocol/Type' tag '{0}' does not match with PortSettings count '{1}'.", connectionCount, portSettingCount),
                HowToFix = "",
                ExampleCode = "",
                Details = "For each port that is defined, a PortSettings element should be defined. In addition, the order of these PortSettings elements must correspond with the order of the con­nections defined in the Protocol/Type@advanced attribute." + Environment.NewLine + "- Connection count = number of connections defined in 'Protocol/Type@advanced' + 1 for the main connection define in 'Protocol/Type' tag." + Environment.NewLine + "- PortSettings count = number of PortSettings in 'Protocol/Ports' tag + 1 for main PortSettings define in 'Protocol/PortSettings'.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidCombinationOfSyntax1And2(IValidate test, IReadable referenceNode, IReadable positionNode)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckConnections,
                ErrorId = ErrorIds.InvalidCombinationOfSyntax1And2,
                FullId = "1.23.6",
                Category = Category.Protocol,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Connections can not be defined simultaneously via 'Protocol/Type' and 'Protocol/Connections'.",
                HowToFix = "",
                ExampleCode = "",
                Details = "Connections can be defined either via 'Protocol/Type' or via 'Protocol/Connections' but both syntaxes can not be used in the same protocol." + Environment.NewLine + "- 'Protocol/Type' is the recommended syntax." + Environment.NewLine + "- 'Protocol/Connections' should only be used in case one of the rare features only available in this syntax is needed.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UnrecommendedSyntax2(IValidate test, IReadable referenceNode, IReadable positionNode)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckConnections,
                ErrorId = ErrorIds.UnrecommendedSyntax2,
                FullId = "1.23.7",
                Category = Category.Protocol,
                Severity = Severity.Minor,
                Certainty = Certainty.Uncertain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Unrecommended use of the 'Protocol/Connections' syntax.",
                HowToFix = "",
                ExampleCode = "",
                Details = "Connections can be defined either via 'Protocol/Type' or via 'Protocol/Connections' but both syntaxes can not be used in the same protocol." + Environment.NewLine + "- 'Protocol/Type' is the recommended syntax." + Environment.NewLine + "- 'Protocol/Connections' should only be used in case one of the rare features only available in this syntax is needed.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorCompare
    {
        public static IValidationResult ConnectionsOrderChanged(IReadable referenceNode, IReadable positionNode, string oldOrder, string newOrder)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckConnections,
                ErrorId = ErrorIds.ConnectionsOrderChanged,
                FullId = "1.23.8",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Order of connections changed from '{0}' to '{1}'.", oldOrder, newOrder),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult ConnectionTypeChanged(IReadable referenceNode, IReadable positionNode, string connectionType, string connectionId, string connectionName, string newConnectionType)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckConnections,
                ErrorId = ErrorIds.ConnectionTypeChanged,
                FullId = "1.23.9",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("{0} Connection '{1}' with name '{2}' was changed into '{3}'.", connectionType, connectionId, connectionName, newConnectionType),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult ConnectionAdded(IReadable referenceNode, IReadable positionNode, string connectionType, string connectionId, string connectionName)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckConnections,
                ErrorId = ErrorIds.ConnectionAdded,
                FullId = "1.23.10",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("{0} Connection '{1}' with name '{2}' was added.", connectionType, connectionId, connectionName),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint MismatchingNames = 1;
        public const uint InvalidConnectionName = 2;
        public const uint DuplicateConnectionName = 3;
        public const uint DuplicateConnectionName_Sub = 4;
        public const uint InvalidConnectionCount = 5;
        public const uint InvalidCombinationOfSyntax1And2 = 6;
        public const uint UnrecommendedSyntax2 = 7;
        public const uint ConnectionsOrderChanged = 8;
        public const uint ConnectionTypeChanged = 9;
        public const uint ConnectionAdded = 10;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckConnections = 23;
    }
}