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
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
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
                var alarm = param.Alarm;
                if (alarm == null)
                {
                    continue;
                }

                var info = param.Alarm.Info;
                (GenericStatus status, string infoRawValue, string infoValue) = GenericTests.CheckBasics<string>(info, isRequired: true);

                // Missing is OK
                if (status.HasFlag(GenericStatus.Missing))
                {
                    continue;
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedTag(this, param, info, param.Id.RawValue, infoRawValue));
                }

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyTag(this, param, info, param.Id.RawValue));
                }

                // Tag exists but is unrecommended
                results.Add(Error.UnrecommendedInfoTag(this, param, info, param.Id.RawValue));
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
                    {
                        var paramReadNode = (IParamsParam)context.Result.ReferenceNode;
                        var paramEditNode = context.Protocol.Params.Get(paramReadNode);

                        paramEditNode.Alarm.Info = null;
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

            return results;
        }
    }
}