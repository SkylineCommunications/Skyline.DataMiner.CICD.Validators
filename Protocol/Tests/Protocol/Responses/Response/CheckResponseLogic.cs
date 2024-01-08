namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Responses.Response.CheckResponseLogic
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckResponseLogic, Category.Response)]
    internal class CheckResponseLogic : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var model = context.ProtocolModel;
            HashSet<uint> smartserialConnectionIds = ValidateHelper.GetSmartserialConnectionIds(context);
            Dictionary<uint, HashSet<uint>> connectionIdToHeaderTrailerLinkIds = new Dictionary<uint, HashSet<uint>>();
            Dictionary<uint, HashSet<uint>> headerTrailerLinkIdToParamIds = new Dictionary<uint, HashSet<uint>>();
            ValidateHelper.LoadHeaderTrailerLinkOptions(context, smartserialConnectionIds, connectionIdToHeaderTrailerLinkIds, headerTrailerLinkIdToParamIds);

            bool onEachTriggerHasCrcAction = false;
            ITriggersTrigger[] onEachTriggers = ValidateHelper.GetOnEachTrigger(context);
            for (int i = 0; i < onEachTriggers.Length; i++)
            {
                if (ValidateHelper.TriggersCrcAction(context, onEachTriggers[i]))
                {
                    onEachTriggerHasCrcAction = true;
                    break;
                }
            }

            foreach (IResponsesResponse response in context.EachResponseWithValidId())
            {
                uint connectionId = ValidateHelper.GetResponseConnectionId(response);
                if (!connectionIdToHeaderTrailerLinkIds.TryGetValue(connectionId, out var headerTrailerLinkIds))
                {
                    headerTrailerLinkIds = new HashSet<uint>();
                }

                ValidateHelper.ValidateIfResponseContainsHeaderTrailerParams(response, connectionId, headerTrailerLinkIds,
                    headerTrailerLinkIdToParamIds, results, this);

                foreach (IParamsParam param in response.GetParameters(model.RelationManager))
                {
                    if (param.Type?.Value == EnumParamType.Crc)
                    {
                        var trigger = ValidateHelper.GetDedicatedTrigger(context, response.Id.RawValue);

                        if (trigger != null)
                        {
                            if (!ValidateHelper.TriggersCrcAction(context, trigger))
                            {
                                results.Add(Error.MissingCrcResponseAction(this, response, response, response.Id.RawValue, param.Id?.RawValue));
                            }
                        }
                        else if (!onEachTriggerHasCrcAction)
                        {
                            results.Add(Error.MissingCrcResponseAction(this, response, response, response.Id.RawValue, param.Id?.RawValue));
                        }
                    }
                }
            }

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

    internal static class ValidateHelper
    {
        public static void LoadHeaderTrailerLinkOptions(ValidatorContext context, HashSet<uint> smartserialConnectionIds, Dictionary<uint, HashSet<uint>> connectionIdToHeaderTrailerLinkIds, Dictionary<uint, HashSet<uint>> headerTrailerLinkIdToParamIds)
        {
            if (smartserialConnectionIds.Count == 0)
            {
                return;
            }

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.Type == null || (param.Type.Value != EnumParamType.Header && param.Type.Value != EnumParamType.Trailer))
                {
                    continue;
                }

                var paramTypeOptions = param.Type.GetOptions();
                if (paramTypeOptions.HeaderTrailerLink == null || !paramTypeOptions.HeaderTrailerLink.Id.HasValue)
                {
                    continue;
                }

                if (!paramTypeOptions.Connection.HasValue)
                {
                    // No connection is specified, meaning that it will be applicable for all smart-serial connections.
                    foreach (var connectionId in smartserialConnectionIds)
                    {
                        AddHeaderTrailerLinkIdToSmartSerialConnection(connectionId, paramTypeOptions.HeaderTrailerLink.Id.Value, param.Id.Value.Value, connectionIdToHeaderTrailerLinkIds, headerTrailerLinkIdToParamIds);
                    }

                    continue;
                }

                if (smartserialConnectionIds.Contains(paramTypeOptions.Connection.Value))
                {
                    AddHeaderTrailerLinkIdToSmartSerialConnection(paramTypeOptions.Connection.Value, paramTypeOptions.HeaderTrailerLink.Id.Value, param.Id.Value.Value, connectionIdToHeaderTrailerLinkIds, headerTrailerLinkIdToParamIds);
                }
            }
        }

        /// <summary>
        /// Adds the HeaderTrailerLink parameter to the connection it belongs to.
        /// </summary>
        /// <param name="connectionId">ID of the connection.</param>
        /// <param name="headerTrailerLinkId">ID of the HeaderTrailerLink.</param>
        /// <param name="parameterId">ID of the parameter.</param>
        /// <param name="connectionIdToHeaderTrailerLinkIds">Links the ID of the connection with the IDs of the HeaderTrailerLink.</param>
        /// <param name="headerTrailerLinkIdToParamIds">Links the ID of the HeaderTrailerLink to the IDs of the parameters.</param>
        private static void AddHeaderTrailerLinkIdToSmartSerialConnection(uint connectionId, uint headerTrailerLinkId, uint parameterId, Dictionary<uint, HashSet<uint>> connectionIdToHeaderTrailerLinkIds, Dictionary<uint, HashSet<uint>> headerTrailerLinkIdToParamIds)
        {
            if (!connectionIdToHeaderTrailerLinkIds.TryGetValue(connectionId, out var headerTrailerLinkIds))
            {
                headerTrailerLinkIds = new HashSet<uint>();
                connectionIdToHeaderTrailerLinkIds[connectionId] = headerTrailerLinkIds;
            }

            headerTrailerLinkIds.Add(headerTrailerLinkId);
            if (!headerTrailerLinkIdToParamIds.TryGetValue(headerTrailerLinkId, out var paramIds))
            {
                paramIds = new HashSet<uint>();
                headerTrailerLinkIdToParamIds[headerTrailerLinkId] = paramIds;
            }

            paramIds.Add(parameterId);
        }

        public static ITriggersTrigger GetDedicatedTrigger(ValidatorContext context, string triggerId)
        {
            var model = context.ProtocolModel;

            if (model.Protocol?.Triggers == null)
            {
                return null;
            }

            foreach (ITriggersTrigger trigger in model.Protocol.Triggers)
            {
                EnumTriggerTime? triggerTime = EnumTriggerTimeConverter.Convert(trigger.Time?.Value);

                if (trigger.On?.Value == EnumTriggerOn.Response
                    && String.Equals(trigger.On.Id?.Value, triggerId, StringComparison.OrdinalIgnoreCase) && triggerTime == EnumTriggerTime.Before)
                {
                    return trigger;
                }
            }

            return null;
        }

        public static ITriggersTrigger[] GetOnEachTrigger(ValidatorContext context)
        {
            List<ITriggersTrigger> beforeEachResponseTriggers = new List<ITriggersTrigger>();

            var model = context.ProtocolModel;
            if (model.Protocol?.Triggers == null)
            {
                return beforeEachResponseTriggers.ToArray();
            }

            foreach (ITriggersTrigger trigger in model.Protocol.Triggers)
            {
                EnumTriggerTime? triggerTime = EnumTriggerTimeConverter.Convert(trigger.Time?.Value);
                if (trigger.On?.Value == EnumTriggerOn.Response && trigger.On.GetId().Each && triggerTime == EnumTriggerTime.Before)
                {
                    beforeEachResponseTriggers.Add(trigger);
                }
            }

            return beforeEachResponseTriggers.ToArray();
        }

        public static uint GetResponseConnectionId(IResponsesResponse response)
        {
            uint connectionId = 0;
            if (response.Options == null || String.IsNullOrEmpty(response.Options.Value))
            {
                return connectionId;
            }

            string[] options = response.Options.Value.Split(';');
            foreach (string option in options)
            {
                if (!option.StartsWith("connection:", StringComparison.InvariantCultureIgnoreCase) || option.Length < 12)
                {
                    continue;
                }

                string sConnection = option.Substring(11).Trim();
                if (UInt32.TryParse(sConnection, out uint iConnection))
                {
                    connectionId = iConnection;
                    break;
                }
            }

            return connectionId;
        }

        public static HashSet<uint> GetSmartserialConnectionIds(ValidatorContext context)
        {
            HashSet<uint> smartserialConnectionIds = new HashSet<uint>();
            var model = context.ProtocolModel;
            if (model.Protocol == null)
            {
                return smartserialConnectionIds;
            }

            foreach (var connection in model.Protocol.GetConnections())
            {
                if (connection.Type == EnumProtocolType.SmartSerial || connection.Type == EnumProtocolType.SmartSerialSingle)
                {
                    smartserialConnectionIds.Add(connection.Number);
                }
            }

            return smartserialConnectionIds;
        }

        public static bool TriggersCrcAction(ValidatorContext context, ITriggersTrigger trigger)
        {
            if (trigger == null)
            {
                return false;
            }

            foreach (IActionsAction action in trigger.GetActions(context.ProtocolModel.RelationManager))
            {
                if (action.On?.Value == EnumActionOn.Response && action.Type?.Value == EnumActionType.Crc)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Validates if a smart-serial response has the specified header and trailer for a connection.
        /// </summary>
        /// <param name="response">Response to be verified.</param>
        /// <param name="connectionId">ID of the connection.</param>
        /// <param name="headerTrailerLinkIds">Collection with the IDs of the HeaderTrailerLinks.</param>
        /// <param name="headerTrailerLinkIdToParamIds">Links the ID of the HeaderTrailerLink with the IDs of the parameters.</param>
        /// <param name="results">Results where the error code should be added to.</param>
        /// <param name="test">Test validation.</param>
        public static void ValidateIfResponseContainsHeaderTrailerParams(IResponsesResponse response, uint connectionId, HashSet<uint> headerTrailerLinkIds, Dictionary<uint, HashSet<uint>> headerTrailerLinkIdToParamIds, List<IValidationResult> results, CheckResponseLogic test)
        {
            if (headerTrailerLinkIds.Count == 0)
            {
                return;
            }

            foreach (var headerTrailerLinkId in headerTrailerLinkIds)
            {
                if (headerTrailerLinkIdToParamIds.TryGetValue(headerTrailerLinkId, out var headerAndTrailerParamIds) && HasResponseMatchingHeaderAndTrailer(response, headerAndTrailerParamIds))
                {
                    return;
                }
            }

            results.Add(Error.SmartSerialResponseShouldContainHeaderTrailer(test, response, response, Convert.ToString(connectionId), Convert.ToString(response.Id.Value.Value)));
        }

        /// <summary>
        /// Checks if the response contains the header and trailer parameters.
        /// </summary>
        /// <param name="response">Response to be verified.</param>
        /// <param name="headerAndTrailerParamIds">Header and trailer parameter IDs that need to be present in the response.</param>
        /// <returns>Boolean indicating if the header and trailer parameters are present in the response.</returns>
        private static bool HasResponseMatchingHeaderAndTrailer(IResponsesResponse response, HashSet<uint> headerAndTrailerParamIds)
        {
            foreach (var headerOrTrailerParamId in headerAndTrailerParamIds)
            {
                bool hasResponseParam = false;
                foreach (var responseParam in response.Content)
                {
                    if (responseParam.Value.Value == headerOrTrailerParamId)
                    {
                        hasResponseParam = true;
                        break;
                    }
                }

                if (!hasResponseParam)
                {
                    return false;
                }
            }

            return true;
        }
    }
}