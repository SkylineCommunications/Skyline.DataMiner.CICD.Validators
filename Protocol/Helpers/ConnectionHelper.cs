namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests
{
    using System;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    internal static class ConnectionHelper
    {
        internal static string CreateConnectionName(IProtocol protocol, IPortSettingsBase portSettings)
        {
            // Try to add redundant or numbers??

            var connections = protocol.GetConnections();

            var connection = connections.FirstOrDefault(c => c.PortSettings == portSettings);

            if (connection == null)
            {
                return String.Empty;
            }

            if (IsSshConnection(connection))
            {
                return "SSH Connection";
            }

            switch (connection.Type)
            {
                case EnumProtocolType.Snmp:
                case EnumProtocolType.Snmpv2:
                case EnumProtocolType.Snmpv3:
                    return "SNMP Connection";

                case EnumProtocolType.Http:
                    return "HTTP Connection";

                case EnumProtocolType.Serial:
                case EnumProtocolType.SerialSingle:
                case EnumProtocolType.SmartSerial:
                case EnumProtocolType.SmartSerialSingle:
                    if (portSettings.PortTypeSerial?.Disabled?.Value == true)
                    {
                        return "IP Connection";
                    }

                    if (portSettings.PortTypeIP?.Disabled?.Value == true && portSettings?.PortTypeUDP?.Disabled.Value == true)
                    {
                        return "Serial Connection";
                    }

                    //TODO: what do we expect if both IP (tcp and/or udp) AND serial are supported by the driver?
                    //return "IP/Serial Connection";

                    break;

                case EnumProtocolType.Virtual:
                    return "Virtual Connection";
            }

            return null;
        }

        private static bool IsSshConnection(Connection connection)
        {
            // SSH is not supported for smart-serial connections
            bool isSerialConnection = connection.Type == EnumProtocolType.Serial || connection.Type == EnumProtocolType.SerialSingle;
            if (!isSerialConnection)
            {
                return false;
            }

            // SLPort will create an SSH client when the configured port number is 22 or when PortSettings has the <SSH> element.
            switch (connection.PortSettings)
            {
                case IPortSettingsMain portSettingsMainSsh:
                    if (portSettingsMainSsh.IPport?.DefaultValue?.Value == 22)
                    {
                        return true;
                    }

                    return portSettingsMainSsh.SSH != null;

                case IPortSettings portSettings:
                    if (portSettings.IPport?.DefaultValue?.Value == 22)
                    {
                        return true;
                    }

                    return portSettings.SSH != null;

                default:
                    return false;
            }
        }
    }
}