namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Actions.Action.Type.CheckTypeRunActions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckTypeRunActions, Category.Action)]
    internal class CheckTypeRunActions : IValidate //, ICodeFix, ICompare
    {
        // Please comment out the interfaces that aren't used together with the respective methods.

        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            HashSet<uint> qActionTriggerParameterIds = new HashSet<uint>();

            foreach (var qAction in context.EachQActionWithValidId())
            {
                // Skip if "group" option is used (QAction then triggers on group instead of param).
                List<string> options = qAction.Options?.Value.ToLower().Split(';').ToList();

                if(options != null && options.Contains("group"))
                {
                    continue;
                }

                string triggers = qAction.Triggers?.Value;

                if (String.IsNullOrWhiteSpace(triggers))
                {
                    continue;
                }

                foreach (string trigger in triggers.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (UInt32.TryParse(trigger.Trim(), out uint parameterId))
                    {
                        qActionTriggerParameterIds.Add(parameterId);
                    }
                }
            }

            foreach (var action in context.EachActionWithValidId())
            {
                if (action.Type?.Value.Value != EnumActionType.RunActions ||
                    action.On?.Value.Value == null)
                {
                    continue;
                }

                foreach (uint parameterId in action.On.GetId())
                {
                    if (qActionTriggerParameterIds.Contains(parameterId))
                    {
                        continue;
                    }

                    results.Add(Error.ActionParameterNotTriggeringQAction(
                        this,
                        action,
                        action.On,
                        action.Id?.Value.Value.ToString(),
                        parameterId.ToString()));
                }
            }

            return results;
        }

        //public ICodeFixResult Fix(CodeFixContext context)
        //{
        //    CodeFixResult result = new CodeFixResult();

        //    switch (context.Result.ErrorId)
        //    {

        //        default:
        //            result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
        //            break;
        //    }

        //    return result;
        //}
        
        //public List<IValidationResult> Compare(MajorChangeCheckContext context)
        //{
        //    List<IValidationResult> results = new List<IValidationResult>();

        //    return results;
        //}
    }
}