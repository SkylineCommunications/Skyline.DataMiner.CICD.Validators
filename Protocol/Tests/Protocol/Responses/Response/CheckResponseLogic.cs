namespace SLDisValidator2.Tests.Protocol.Responses.Response.CheckResponseLogic
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckResponseLogic, Category.Response)]
    public class CheckResponseLogic : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var model = context.ProtocolModel;

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
                if (trigger.On?.Value == EnumTriggerOn.Response && trigger.On.GetId().Each&& triggerTime == EnumTriggerTime.Before)
                {
                    beforeEachResponseTriggers.Add(trigger);
                }
            }

            return beforeEachResponseTriggers.ToArray();
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
    }
}