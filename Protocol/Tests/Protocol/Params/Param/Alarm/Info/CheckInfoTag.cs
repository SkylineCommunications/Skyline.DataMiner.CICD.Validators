namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Alarm.Info.CheckInfoTag
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckInfoTag, Category.Param)]
    internal class CheckInfoTag : IValidate, ICodeFix, ICompare
    {
        // Please comment out the interfaces that aren't used together with the respective methods.

        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var param in context.EachParamWithValidId())
            {
                var infoTag = param.Alarm?.Info;

                // No info tag
                if (infoTag == null)
                {
                    continue;
                }

                // Info tag should have a value
                if (infoTag.Value != null)
                {
                    var positionNode = infoTag ?? (IReadable)alarmTag;
                    results.Add(Error.UnrecommendedInfoTag(this, param, positionNode, param.Id.RawValue));
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();
            if (context.Protocol?.Params == null)
            {
                result.Message = "No Param found";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UnrecommendedInfoTag:
                    var paramReadNode = (IParamsParam)context.Result.ReferenceNode;
                    var paramEditNode = context.Protocol.Params.Get(paramReadNode);

                    paramEditNode.Alarm.Info = null;
                    result.Success = true;
                    break;

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }
        
        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            return results;
        }
    }
}
