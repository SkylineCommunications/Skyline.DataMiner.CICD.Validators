namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Commands.Command.CheckCommandLogic
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


    [Test(CheckId.CheckCommandLogic, Category.Command)]
    internal class CheckCommandLogic : IValidate/*, ICodeFix, ICompare*/
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

            foreach (ICommandsCommand command in context.EachCommandWithValidId())
            {
                foreach (var param in command.GetParameters(model.RelationManager))
                {
                    if (param.Type?.Value != EnumParamType.Crc)
                    {
                        continue;
                    }

                    var trigger = ValidateHelper.GetDedicatedTrigger(context, command.Id.RawValue);

                    if (trigger != null)
                    {
                        if (!ValidateHelper.TriggersCrcAction(context, trigger))
                        {
                            results.Add(Error.MissingCrcCommandAction(this, command, command, command.Id.RawValue, param.Id?.RawValue));
                        }
                    }
                    else if (!onEachTriggerHasCrcAction)
                    {
                        results.Add(Error.MissingCrcCommandAction(this, command, command, command.Id.RawValue, param.Id?.RawValue));
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

            var triggers = model.Protocol.Triggers;

            ITriggersTrigger result = null;
            foreach (var trigger in triggers)
            {
                var triggerTime = EnumTriggerTimeConverter.Convert(trigger.Time?.Value);

                if (trigger.On?.Value == EnumTriggerOn.Command
                    && String.Equals(trigger.On.Id?.Value, triggerId, StringComparison.OrdinalIgnoreCase) && triggerTime == EnumTriggerTime.Before)
                {
                    result = trigger;
                    break;
                }
            }

            return result;
        }

        public static ITriggersTrigger[] GetOnEachTrigger(ValidatorContext context)
        {
            var model = context.ProtocolModel;
            if (model.Protocol?.Triggers == null)
            {
                return Array.Empty<ITriggersTrigger>();
            }

            var triggers = model.Protocol.Triggers;

            List<ITriggersTrigger> beforeEachCommandTriggers = new List<ITriggersTrigger>();
            foreach (var trigger in triggers)
            {
                EnumTriggerTime? triggerTime = EnumTriggerTimeConverter.Convert(trigger.Time?.Value);

                if (trigger.On?.Value == EnumTriggerOn.Command
                    && String.Equals(trigger.On.Id?.Value, "each", StringComparison.OrdinalIgnoreCase) && triggerTime == EnumTriggerTime.Before)
                {
                    beforeEachCommandTriggers.Add(trigger);
                }
            }

            return beforeEachCommandTriggers.ToArray();
        }

        public static bool TriggersCrcAction(ValidatorContext context, ITriggersTrigger trigger)
        {
            if (trigger == null)
            {
                return false;
            }

            foreach (var action in trigger.GetActions(context.ProtocolModel.RelationManager))
            {
                if (action.On?.Value == EnumActionOn.Command && action.Type?.Value == EnumActionType.Crc)
                {
                    return true;
                }
            }

            return false;
        }
    }
}