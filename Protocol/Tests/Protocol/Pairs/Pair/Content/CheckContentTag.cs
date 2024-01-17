namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Pairs.Pair.Content.CheckContentTag
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

    [Test(CheckId.CheckContentTag, Category.Pair)]
    internal class CheckContentTag : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            ValidateHelper helper = new ValidateHelper(this);

            ValidateHelper.GetTriggersAfterResponses(context, out List<ITriggersTrigger> triggersAfterResponseEach, out Dictionary<uint, List<ITriggersTrigger>> triggersAfterResponses);

            foreach (IPairsPair pair in context.EachPairWithValidId())
            {
                if (!HasCommand(pair))
                {
                    // No need to check pairs that don't have any command.
                    continue;
                }

                var pairResponses = ValidateHelper.GetResponses(pair);
                if (pairResponses.Count > 1)
                {
                    results.AddIfNotNull(helper.ValidateClearRoutine(context, triggersAfterResponseEach, triggersAfterResponses, pair.Id.RawValue, pair, pairResponses));
                }
            }

            return results;

            bool HasCommand(IPairsPair pair)
            {
                return pair.Content?.Any(item => item is IPairsPairContentCommand) ?? false;
            }
        }
    }

    internal class ValidateHelper
    {
        private readonly IValidate test;

        public ValidateHelper(IValidate test)
        {
            this.test = test;
        }

        public static List<IPairsPairContentItem> GetResponses(IPairsPair pair)
        {
            List<IPairsPairContentItem> responses = new List<IPairsPairContentItem>();

            foreach (var contentItem in pair.Content)
            {
                if (contentItem is IPairsPairContentResponse || contentItem is IPairsPairContentResponseOnBadCommand)
                {
                    responses.Add(contentItem);
                }
            }

            return responses;
        }

        public IValidationResult ValidateClearRoutine(ValidatorContext context, List<ITriggersTrigger> triggersAfterResponseEach, Dictionary<uint, List<ITriggersTrigger>> triggersAfterResponses, string pairId, IPairsPair pair, List<IPairsPairContentItem> pairResponses)
        {
            List<IValidationResult> missingClearItselfSubResults = new List<IValidationResult>();
            List<IValidationResult> missingClearOthersSubResults = new List<IValidationResult>();

            foreach (IPairsPairContentItem pairResponse in pairResponses)
            {
                if (!pairResponse.Value.HasValue)
                {
                    // Wrongly defined response tag
                    continue;
                }

                uint responseId = pairResponse.Value.Value;

                if (!triggersAfterResponses.TryGetValue(responseId, out List<ITriggersTrigger> triggersAfterResponse))
                {
                    // If no specific trigger is found for that response, we fallback to the triggers on 'each'
                    triggersAfterResponse = triggersAfterResponseEach;
                }

                bool clearsItself = false;
                List<uint> unclearedOtherResponsesInPair = pairResponses.Where(x => x != pairResponse && x.Value != null).Select(x => x.Value.Value).ToList();
                foreach (var triggerAfterResponse in triggersAfterResponse)
                {
                    foreach (var action in triggerAfterResponse.GetActions(context.ProtocolModel.RelationManager))
                    {
                        if (!IsActionClearOnResponse(action))
                        {
                            continue;
                        }

                        var actionOnIds = action.On.GetId();
                        if (actionOnIds.Count == 0)
                        {
                            // Generic Clear Action => Takes id from trigger
                            clearsItself = true;
                            continue;
                        }

                        // Specific Clear Action => Look at Id(s) in attribute
                        foreach (var clearedResponseId in actionOnIds)
                        {
                            if (responseId == clearedResponseId)
                            {
                                // Clear itself
                                clearsItself = true;
                            }
                            else if (unclearedOtherResponsesInPair.Contains(clearedResponseId))
                            {
                                unclearedOtherResponsesInPair.Remove(clearedResponseId);
                            }
                        }
                    }
                }

                if (!clearsItself)
                {
                    missingClearItselfSubResults.Add(Error.MissingClearResponseRoutine_Sub(test, pairResponse, pairResponse, Convert.ToString(responseId), Convert.ToString(responseId)));
                }

                bool clearsAllOthers = unclearedOtherResponsesInPair.Count == 0;
                if (!clearsAllOthers)
                {
                    missingClearOthersSubResults.Add(Error.MissingClearResponseRoutine_Sub(test, pairResponse, pairResponse, String.Join(",", unclearedOtherResponsesInPair), Convert.ToString(responseId)));
                }
            }

            if (missingClearItselfSubResults.Count > 0 && missingClearOthersSubResults.Count > 0)
            {
                if (missingClearOthersSubResults.Count <= missingClearItselfSubResults.Count)
                {
                    return Error.MissingClearResponseRoutine(test, pair, pair, pairId).WithSubResults(missingClearOthersSubResults.ToArray());
                }
                else
                {
                    return Error.MissingClearResponseRoutine(test, pair, pair, pairId).WithSubResults(missingClearItselfSubResults.ToArray());
                }
            }
            //TODO: For performance reasons, after a specific response is received, it is, in most cases, recommended to clear other responses of the same pair instead of clearing the response itself.
            //else if (missingClearOthersSubResults.Count > 0)
            //{
            //    return Error.UnrecommendedClearResponseRoutine()
            //}

            return null;
        }

        public static void GetTriggersAfterResponses(ValidatorContext context, out List<ITriggersTrigger> triggersAfterResponseEach, out Dictionary<uint, List<ITriggersTrigger>> triggersAfterResponses)
        {
            triggersAfterResponseEach = new List<ITriggersTrigger>();
            triggersAfterResponses = new Dictionary<uint, List<ITriggersTrigger>>();

            foreach (ITriggersTrigger trigger in context.EachTriggerWithValidId())
            {
                if (trigger.On?.Value == null || trigger.Time?.Value == null || trigger.Type?.Value == null)
                {
                    continue;
                }

                EnumTriggerTime? triggerTime = EnumTriggerTimeConverter.Convert(trigger.Time?.Value);
                if (!triggerTime.HasValue)
                {
                    continue;
                }

                if (trigger.On.Value != EnumTriggerOn.Response || triggerTime.Value != EnumTriggerTime.After || trigger.Type.Value != EnumTriggerType.Action)
                {
                    continue;
                }

                var onId = trigger.On.GetId();
                if (onId == null)
                {
                    continue;
                }

                if (onId.Each)
                {
                    triggersAfterResponseEach.Add(trigger);
                }
                else if (onId.Id.HasValue)
                {
                    if (!triggersAfterResponses.TryGetValue(onId.Id.Value, out List<ITriggersTrigger> triggersAfterResponse))
                    {
                        triggersAfterResponse = new List<ITriggersTrigger>();
                        triggersAfterResponses.Add(onId.Id.Value, triggersAfterResponse);
                    }

                    triggersAfterResponse.Add(trigger);
                }
            }
        }

        private static bool IsActionClearOnResponse(IActionsAction action)
        {
            return action?.On?.Value == EnumActionOn.Response && action.Type?.Value == EnumActionType.Clear;
        }
    }
}