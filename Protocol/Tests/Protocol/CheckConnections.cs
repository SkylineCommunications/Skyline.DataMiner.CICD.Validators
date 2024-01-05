namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.CheckConnections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckConnections, Category.Protocol)]
    internal class CheckConnections : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var protocol = context?.ProtocolModel?.Protocol;
            if (protocol == null)
            {
                return results;
            }

            IList<Connection> connections = protocol.GetConnections();

            bool isSyntax1Used = protocol.Type != null;
            bool isSyntax2Used = protocol.ReadNode.Element["Connections"] != null;

            if (isSyntax1Used && isSyntax2Used)
            {
                results.Add(Error.InvalidCombinationOfSyntax1And2(this, protocol, protocol));
                return results;
            }

            if (isSyntax2Used)
            {
                results.Add(Error.UnrecommendedSyntax2(this, protocol, protocol));
                return results;
            }

            if (connections == null || connections.Count == 0)
            {
                // When no connections, no point in checking anything.
                return results;
            }

            var resultsForDuplicateNames = GenericTests.CheckDuplicatesNonModel(
                items: connections,
                getDuplicationIdentifier: connection => connection.PortSettingsName,
                generateSubResult: x => Error.DuplicateConnectionName_Sub(this, x.item.PortSettings, x.item.PortSettings, x.duplicateValue, x.item.Number.ToString()),
                generateSummaryResult: x => Error.DuplicateConnectionName(this, null, null, x.duplicateValue).WithSubResults(x.subResults)
                );

            results.AddRange(resultsForDuplicateNames);

            ValidateHelper helper = new ValidateHelper(this, context, results, protocol);

            int connectionsCount = helper.CheckInvalidCount(connections);

            for (int i = 0; i < connectionsCount; i++)
            {
                Connection connection = connections[i];

                // Check PortSettings if valid
                if (results.AddIfNotNull(helper.CheckPortSettingsName(connection)))
                {
                    continue;
                }

                helper.CheckMismatching(connection);
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.MismatchingNames:
                    {
                        IPortSettings readNode = (IPortSettings)context.Result.ReferenceNode;
                        IList<Connection> temp = context.Protocol.Read.GetConnections();

                        Connection conn = temp.FirstOrDefault(connection => connection.PortSettings == readNode);

                        if (conn == null)
                        {
                            break;
                        }

                        // Replace wrong part in advanced attribute with the name of the port settings.

                        string oldName = context.Protocol.Type.Advanced.Value;

                        // Make an edit version of GetAdvanced (have the set public) and then 

                        string[] advancedParts = oldName.Split(';');

                        var editAdvancedConnections = new List<Skyline.DataMiner.CICD.Models.Protocol.Edit.AdvancedConnection>(advancedParts.Length);
                        for (int i = 0; i < advancedParts.Length; i++)
                        {
                            editAdvancedConnections.Add(new Skyline.DataMiner.CICD.Models.Protocol.Edit.AdvancedConnection((uint)i + 1, advancedParts[i]));
                        }

                        if (editAdvancedConnections.Count < (conn.Number - 1))
                        {
                            break;
                        }

                        var editConn = editAdvancedConnections[(int)conn.Number - 1];

                        editConn.Name = readNode.Name.Value;

                        // Update Advanced Attribute
                        context.Protocol.Type.Advanced.Value = String.Join(";", editAdvancedConnections);
                        result.Success = true;
                        break;
                    }

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var oldProtocol = context.PreviousProtocolModel?.Protocol;
            var newProtocol = context.NewProtocolModel?.Protocol;

            if (oldProtocol == null || newProtocol == null)
            {
                return results;
            }

            var newTypeTag = newProtocol.Type;

            var oldConnections = oldProtocol.GetConnections();
            var newConnections = newProtocol.GetConnections();

            results.AddRange(CompareHelper.CheckForAddedConnections(newTypeTag, oldConnections, newConnections));
            results.AddIfNotNull(CompareHelper.CheckForOrderChange(newTypeTag, oldConnections, newConnections));
            results.AddRange(CompareHelper.CheckForTypeChange(newTypeTag, oldConnections, newConnections));

            return results;
        }
    }

    internal class ValidateHelper : ValidateHelperBase
    {
        private readonly IProtocol protocol;

        internal ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IProtocol protocol)
            : base(test, context, results)
        {
            this.protocol = protocol;
        }

        internal int CheckInvalidCount(IList<Connection> connections)
        {
            if (connections.Count == 1 && connections[0].Type == EnumProtocolType.Virtual && connections[0].PortSettings == null)
            {
                // Virtual drivers don't need PortSettings
                return 1;
            }

            int typeCount = connections.Count(connection => connection.Type != null);
            int portCount = connections.Count(connection => connection.PortSettings != null);

            if (typeCount != portCount)
            {
                results.Add(Error.InvalidConnectionCount(test, protocol, protocol, typeCount.ToString(), portCount.ToString()));
            }

            return typeCount < portCount ? typeCount : portCount;
        }

        internal IValidationResult CheckPortSettingsName(Connection conn)
        {
            string name = conn.PortSettingsName;

            if (String.IsNullOrWhiteSpace(name))
            {
                // When missing, empty or untrimmed the specific check for the attribute will throw an error.
                return null;
            }

            string connId = conn.Number.ToString();
            var reference = conn.PortSettings;

            string expectedNamePrefix = ConnectionHelper.CreateConnectionName(protocol, conn.PortSettings);
            if (expectedNamePrefix == null)
            {
                // Couldn't determine the expected name
                return null;
            }

            bool hasOtherConnectionWithSameExpectedPrefix = false; // TODO
            if (!hasOtherConnectionWithSameExpectedPrefix && String.Equals(name, expectedNamePrefix))
            {
                // Names are the same
                return null;
            }

            if (!name.StartsWith(expectedNamePrefix + " - "))
            {
                return Error.InvalidConnectionName(test, reference, reference, name, conn.TypeRaw, connId);
            }

            string namePostfix = name.Replace(expectedNamePrefix + " - ", null);
            if (String.IsNullOrWhiteSpace(namePostfix))
            {
                return Error.InvalidConnectionName(test, reference, reference, name, conn.TypeRaw, connId);
            }

            return null;
        }

        internal void CheckMismatching(Connection conn)
        {
            if (conn.Number == 0)
            {
                // Only check extra connections.
                return;
            }

            if (String.Equals(conn.AdvancedName, conn.PortSettingsName))
            {
                return;
            }

            string connNumber = Convert.ToString(conn.Number);

            string names = $"'{String.Join("' vs '", conn.AdvancedName, conn.PortSettingsName)}'";
            IValidationResult mismatchingNames = Error.MismatchingNames(test, conn.PortSettings, null, connNumber, names, true);
            mismatchingNames.WithSubResults(
                // SubResult for Advanced
                Error.MismatchingNames(test, protocol.Type.Advanced, protocol.Type.Advanced, connNumber, $"'{conn.AdvancedName}'", false),

                // SubResult for PortSettings
                Error.MismatchingNames(test, conn.PortSettings, conn.PortSettings, connNumber, $"'{conn.PortSettingsName}'", false));

            results.Add(mismatchingNames);
        }
    }

    internal static class CompareHelper
    {
        private static bool IsConnectionChangeAllowed(EnumProtocolType? fromConnectionType, EnumProtocolType? toConnectionType)
        {
            switch (fromConnectionType)
            {
                case EnumProtocolType.Snmpv2:
                    return toConnectionType == EnumProtocolType.Snmp || toConnectionType == EnumProtocolType.Snmpv3;

                case EnumProtocolType.Snmpv3:
                    return toConnectionType == EnumProtocolType.Snmp || toConnectionType == EnumProtocolType.Snmpv2;

                default:
                    return false;
            }
        }

        internal static List<IValidationResult> CheckForAddedConnections(IProtocolType newTypeTag, IList<Connection> oldConnections, IList<Connection> newConnections)
        {
            if (newConnections.Count <= oldConnections.Count)
            {
                return new List<IValidationResult>(0);
            }

            List<IValidationResult> results = new List<IValidationResult>();
            for (int i = oldConnections.Count; i < newConnections.Count; i++)
            {
                results.Add(
                    ErrorCompare.ConnectionAdded(
                        newTypeTag,
                        newTypeTag,
                        newConnections[i].TypeRaw,
                        Convert.ToString(newConnections[i].Number),
                        newConnections[i].AdvancedName));
            }

            return results;
        }

        internal static IValidationResult CheckForOrderChange(IProtocolType newType, IList<Connection> oldConnections, IList<Connection> newConnections)
        {
            List<string> newOrderListForLogic = new List<string>();
            List<string> oldOrderListForLogic = new List<string>();

            List<string> newOrderListForDisplay = new List<string>();
            List<string> oldOrderListForDisplay = new List<string>();

            for (int i = 0; i < newConnections.Count; i++)
            {
                if (oldConnections.Count <= i)
                {
                    continue;
                }

                var newAdvancedConnection = newConnections[i];
                var oldAdvancedConnection = oldConnections[i];

                newOrderListForLogic.Add($"{newAdvancedConnection.Number}:{newAdvancedConnection.Type}");
                oldOrderListForLogic.Add($"{oldAdvancedConnection.Number}:{oldAdvancedConnection.Type}");

                newOrderListForDisplay.Add($"{newAdvancedConnection.Number}:{newAdvancedConnection.TypeRaw}");
                oldOrderListForDisplay.Add($"{oldAdvancedConnection.Number}:{oldAdvancedConnection.TypeRaw}");
            }

            if (newOrderListForDisplay.Count == 0)
            {
                return null;
            }

            string newOrderForLogic = String.Join(", ", newOrderListForLogic);
            string oldOrderForLogic = String.Join(", ", oldOrderListForLogic);

            if (newOrderForLogic == oldOrderForLogic || HasNewTypes(oldConnections, newConnections))
            {
                return null;
            }

            string newOrderForDisplay = String.Join(", ", newOrderListForDisplay);
            string oldOrderForDisplay = String.Join(", ", oldOrderListForDisplay);

            return ErrorCompare.ConnectionsOrderChanged(newType, newType, oldOrderForDisplay, newOrderForDisplay);
        }

        private static bool HasNewTypes(IList<Connection> oldConnections, IList<Connection> newConnections)
        {
            Dictionary<string, int> newTypeCount = new Dictionary<string, int>();
            Dictionary<string, int> oldTypeCount = new Dictionary<string, int>();

            for (int i = 0; i < newConnections.Count; i++)
            {
                var newAdvancedConnection = newConnections[i];
                if (oldConnections.Count <= i)
                {
                    continue;
                }

                var oldAdvancedConnection = oldConnections[i];
                string sNewType = Convert.ToString(newAdvancedConnection.Type);
                string sOldType = Convert.ToString(oldAdvancedConnection.Type);

                newTypeCount.TryGetValue(sNewType, out int newCount);
                newTypeCount[sNewType] = newCount + 1;
                oldTypeCount.TryGetValue(sOldType, out int oldCount);
                oldTypeCount[sOldType] = oldCount + 1;
            }

            bool differentAmountOfTypes = false;
            foreach (var newCountKvp in newTypeCount)
            {
                oldTypeCount.TryGetValue(newCountKvp.Key, out int oldCount);
                if (oldCount != newCountKvp.Value)
                {
                    differentAmountOfTypes = true;
                    break;
                }
            }

            return differentAmountOfTypes;
        }

        internal static List<IValidationResult> CheckForTypeChange(IProtocolType newType, IList<Connection> oldConnections, IList<Connection> newConnections)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            for (int i = 0; i < newConnections.Count; i++)
            {
                if (oldConnections.Count <= i)
                {
                    continue;
                }

                var newConnection = newConnections[i];
                var oldConnection = oldConnections[i];

                if (newConnection.Type != oldConnection.Type && !IsConnectionChangeAllowed(oldConnection.Type, newConnection.Type))
                {
                    string connectionName = oldConnection.AdvancedName ?? oldConnection.ConnectionXmlElement?.Attribute["name"]?.Value;
                    results.Add(ErrorCompare.ConnectionTypeChanged(newType, newType, oldConnection.TypeRaw, Convert.ToString(newConnection.Number), connectionName, newConnection.TypeRaw));
                }
            }

            if (results.Count > 0 && HasNewTypes(oldConnections, newConnections))
            {
                return results;
            }

            return new List<IValidationResult>(0);
        }
    }
}