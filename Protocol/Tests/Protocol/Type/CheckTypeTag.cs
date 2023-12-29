namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Type.CheckTypeTag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckTypeTag, Category.Protocol)]
    internal class CheckTypeTag : IValidate/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (context.ProtocolModel?.Protocol == null)
            {
                return results;
            }

            var protocol = context.ProtocolModel.Protocol;
            var type = protocol.Type;

            (GenericStatus status, string rawValue, EnumProtocolType? _) = GenericTests.CheckBasics(type, true);

            // Check if tag is there
            if (status.HasFlag(GenericStatus.Missing))
            {
                // Tag can be left out if Connections are defined via another Syntax
                if (protocol.ReadNode.Element["Connections"] != null)
                {
                    return results;
                }

                // If no other syntax is used for Connections, Type tag is mandatory
                results.Add(Error.MissingTag(this, protocol, protocol));
                return results;
            }

            // Check if tag is filled in
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyTag(this, type, type));
                return results;
            }

            // Temp, replace with auto string join of enum
            string[] possibleValues =
            {
                "gpib",
                "http",
                "opc",
                "serial",
                "serial single",
                "service",
                "sla",
                "smart-serial",
                "smart-serial single",
                "snmp",
                "snmpv2",
                "snmpv3",
                "virtual"
            };

            // Check if correct value
            if (status.HasFlag(GenericStatus.Invalid) || !possibleValues.Contains(rawValue))
            {
                // Couldn't be parsed or invalid casing.
                results.Add(Error.InvalidValue(this, type, type, type.ReadNode.InnerText, String.Join(", ", possibleValues)));
                return results;
            }

            // Extra checks?

            return results;
        }

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }
}