namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.CheckConnectionPingGroups
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckConnectionPingGroups, Category.Protocol)]
    internal class CheckConnectionPingGroups : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            ValidateHelper helper = new ValidateHelper(this, context, results);
            helper.CheckMainConnectionPingGroup();
            helper.CheckPingPairs();

            return results;
        }

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {

        ////        default:
        ////            result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
        ////            break;
        ////    }

        ////    return result;
        ////}

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly ValidatorContext context;
        private readonly List<IValidationResult> results;

        private readonly Connection[] connections;
        private readonly IGroupsGroup[] groupsWithValidId;
        private readonly IGroupsGroup[] pollingGroups;
        private IGroupsGroup pingGroupGeneric;
        private IGroupsGroup pingGroupSnmp;

        internal ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results)
        {
            this.test = test;
            this.context = context;
            this.results = results;

            groupsWithValidId = context.EachGroupWithValidId().ToArray();
            pollingGroups = GetPollingGroups().ToArray();
            connections = context.ProtocolModel?.Protocol?.GetConnections().ToArray();

            LoadPingGroups();
        }

        private IEnumerable<IGroupsGroup> GetPollingGroups()
        {
            foreach (IGroupsGroup group in groupsWithValidId)
            {
                if (group.Type?.Value != null && group.Type.Value.Value != EnumGroupType.Poll)
                {
                    continue;
                }

                yield return group;
            }
        }

        private void LoadPingGroups()
        {
            // Generic Ping Group (ID '-1')
            if (context?.ProtocolModel?.Protocol?.Groups != null)
            {
                foreach (IGroupsGroup group in context.ProtocolModel.Protocol.Groups)
                {
                    if (group.Id?.RawValue?.Trim() == "-1")
                    {
                        pingGroupGeneric = group;
                        pingGroupSnmp = pingGroupGeneric;
                        return;
                    }
                }
            }

            // Specific Ping Groups
            if (connections == null || connections.Length <= 0 || !connections[0].Type.HasValue)
            {
                return;
            }

            if (groupsWithValidId.Length <= 0)
            {
                return;
            }

            switch (connections[0].Type.Value)
            {
                case EnumProtocolType.Snmp:
                case EnumProtocolType.Snmpv2:
                case EnumProtocolType.Snmpv3:
                    // First group defined in the xml
                    pingGroupSnmp = groupsWithValidId.OrderBy(group => group.ReadNode.FirstCharOffset).First();
                    break;

                case EnumProtocolType.Serial:
                case EnumProtocolType.SerialSingle:
                case EnumProtocolType.SmartSerial:
                case EnumProtocolType.SmartSerialSingle:
                    // No ping group but ping pair (see CheckPingPairs method)
                    break;

                case EnumProtocolType.Http:
                    // Currently no ping group functionality for HTTP connections
                    break;

                case EnumProtocolType.Virtual:
                case EnumProtocolType.Gpib:
                case EnumProtocolType.Opc:
                case EnumProtocolType.Service:
                case EnumProtocolType.Sla:
                default:
                    // No ping group
                    break;
            }
        }

        internal void CheckMainConnectionPingGroup()
        {
            if (pollingGroups.Length <= 0)
            {
                return;
            }

            if (connections == null || connections.Length <= 0 || !connections[0].Type.HasValue)
            {
                return;
            }

            switch (connections[0].Type.Value)
            {
                case EnumProtocolType.Snmp:
                case EnumProtocolType.Snmpv2:
                case EnumProtocolType.Snmpv3:
                    if (pingGroupSnmp == null)
                    {
                        // TODO: Missing polling group
                        break;
                    }

                    if (pingGroupSnmp.Content?.Any(contentItem => contentItem is IGroupsGroupContentParam) != true)
                    {
                        results.Add(Error.InvalidPingGroupType(test, pingGroupSnmp, pingGroupSnmp, EnumProtocolTypeConverter.ConvertBack(connections[0].Type.Value), pingGroupSnmp.Id?.RawValue));
                    }

                    break;

                case EnumProtocolType.Serial:
                case EnumProtocolType.SerialSingle:
                case EnumProtocolType.SmartSerial:
                case EnumProtocolType.SmartSerialSingle:
                    if (pingGroupGeneric == null)
                    {
                        // No ping group but ping pair (see CheckPingPairs method)
                        break;
                    }

                    if (pingGroupGeneric.Content?.Any(contentItem => contentItem is IGroupsGroupContentPair) != true)
                    {
                        results.Add(Error.InvalidPingGroupType(test, pingGroupGeneric, pingGroupGeneric, EnumProtocolTypeConverter.ConvertBack(connections[0].Type.Value), pingGroupGeneric.Id?.RawValue));
                    }

                    break;

                case EnumProtocolType.Http:
                    // Currently no ping group functionality for HTTP connections
                    break;

                case EnumProtocolType.Virtual:
                case EnumProtocolType.Gpib:
                case EnumProtocolType.Opc:
                case EnumProtocolType.Service:
                case EnumProtocolType.Sla:
                default:
                    // No ping group
                    break;
            }
        }

        internal void CheckPingPairs()
        {
            // Currently only running the check if main connection is (smart)serial since DM currently only support slow poll mode on the main connection anyway
            if (connections == null || connections.Length <= 0 || !connections[0].Type.HasValue)
            {
                return;
            }

            var mainConnectionType = connections[0].Type.Value;
            if (mainConnectionType != EnumProtocolType.Serial && mainConnectionType != EnumProtocolType.SerialSingle
                && mainConnectionType != EnumProtocolType.SmartSerial && mainConnectionType != EnumProtocolType.SmartSerialSingle)
            {
                return;
            }

            Dictionary<uint, List<IPairsPair>> pairsPerConnection = GetPingPairsPerConnection();

            foreach (KeyValuePair<uint, List<IPairsPair>> kvp in pairsPerConnection)
            {
                uint connectionId = kvp.Key;
                List<IPairsPair> pairs = kvp.Value;

                if (connections.Length <= connectionId)
                {
                    // connectionId referring to a non-existing connection would be covered by a check on the Pair@options attribute.
                    break;
                }

                var connection = connections[connectionId];

                List<IValidationResult> multiplePingPairsSubResults = new List<IValidationResult>();
                foreach (IPairsPair pair in pairs)
                {
                    multiplePingPairsSubResults.Add(Error.MultiplePingPairsForConnection_Sub(test, pair, pair, Convert.ToString(connectionId), pair.Id.RawValue));

                    if (pair.Content?.Any(contentItem => contentItem is IPairsPairContentResponse) != true)
                    {
                        results.Add(Error.PingSerialPairHasNoResponse(test, pair, pair, connection.TypeRaw, pair.Id.RawValue));
                    }
                }

                if (multiplePingPairsSubResults.Count > 1)
                {
                    IValidationResult multiplePingPairsForConnection = Error.MultiplePingPairsForConnection(test, context.ProtocolModel.Protocol, context.ProtocolModel.Protocol, connection.PortSettingsName, connection.TypeRaw, Convert.ToString(connectionId));
                    multiplePingPairsForConnection.WithSubResults(multiplePingPairsSubResults.ToArray());
                    results.Add(multiplePingPairsForConnection);
                }
            }
        }

        private Dictionary<uint, List<IPairsPair>> GetPingPairsPerConnection()
        {
            Dictionary<uint, List<IPairsPair>> pingPairsPerConnection = new Dictionary<uint, List<IPairsPair>>();

            // Pair from generic ping group
            if (pingGroupGeneric?.Content?.Where(contentItem => contentItem is IGroupsGroupContentPair)
                                .OrderBy(contentItem => contentItem.ReadNode.FirstCharOffset)
                                .FirstOrDefault() is IGroupsGroupContentPair groupContentFirstPair)
            {
                var pingPair = pingGroupGeneric.GetContentPairs(context.ProtocolModel.RelationManager)
                                               .First(p => p.Id.Value == groupContentFirstPair.Value);

                AddPairToDictionary(pingPairsPerConnection, pingPair);
            }

            int nbOfPairsWithResponse = 0;

            // Pair with ping="true" attribute
            IPairsPair[] pairsWithValidId = context.EachPairWithValidId().ToArray();
            foreach (IPairsPair pair in pairsWithValidId)
            {
                foreach (var item in pair.Content)
                {
                    if (item is IPairsPairContentResponse)
                    {
                        nbOfPairsWithResponse++;
                    }
                }

                if (pair.Ping?.Value != true)
                {
                    continue;
                }

                AddPairToDictionary(pingPairsPerConnection, pair);
            }

            // Pair with lowest id
            if (nbOfPairsWithResponse > 0 && pairsWithValidId.Length > 0 && pingPairsPerConnection.Count <= 0)
            {
                var defaultPingPair = pairsWithValidId.OrderBy(p => p.Id.Value.Value).First();

                AddPairToDictionary(pingPairsPerConnection, defaultPingPair);
            }

            return pingPairsPerConnection;
        }

        private static void AddPairToDictionary(IDictionary<uint, List<IPairsPair>> pingPairsPerConnection, IPairsPair pair)
        {
            uint connectionId = GetPairConnectionId();
            if (!pingPairsPerConnection.TryGetValue(connectionId, out List<IPairsPair> pairs))
            {
                pairs = new List<IPairsPair>();
                pingPairsPerConnection.Add(connectionId, pairs);
            }

            pairs.Add(pair);

            uint GetPairConnectionId()
            {
                PairOptions options = pair.GetOptions();
                return options?.Connection?.Id ?? 0;
            }
        }
    }
}