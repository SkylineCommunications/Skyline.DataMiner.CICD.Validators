namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.ColumnOption.CheckColumnOptionTag
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

    [Test(CheckId.CheckColumnOptionTag, Category.Param)]
    internal class CheckColumnOptionTag : /*IValidate, ICodeFix, */ICompare
    {
        ////public List<IValidationResult> Validate(ValidatorContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}

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

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IParamsParam oldParam, IParamsParam newParam) in context.EachMatchingParam())
            {
                var oldArrayOptions = oldParam?.ArrayOptions;
                var newArrayOptions = newParam?.ArrayOptions;

                if (oldArrayOptions == null || newArrayOptions == null)
                {
                    continue;
                }

                string tablePid = newParam.Id?.RawValue;

                bool oldRtDisplay = oldParam.GetRTDisplay();
                bool newRtDisplay = newParam.GetRTDisplay();

                if (!oldRtDisplay || !newRtDisplay)
                {
                    continue;
                }

                ISet<uint> oldColumnPids = CompareHelper.GetColumnPids(oldArrayOptions);
                ISet<uint> newColumnPids = CompareHelper.GetColumnPids(newArrayOptions);

                oldColumnPids.ExceptWith(newColumnPids);

                foreach (uint oldColumnPid in oldColumnPids)
                {
                    results.Add(ErrorCompare.RemovedColumnOptionTag(newArrayOptions, newArrayOptions, Convert.ToString(oldColumnPid), tablePid));
                }
            }

            return results;
        }
    }

    internal static class CompareHelper
    {
        public static ISet<uint> GetColumnPids(IParamsParamArrayOptions arrayOptions)
        {
            HashSet<uint> result = new HashSet<uint>();

            foreach (ITypeColumnOption column in arrayOptions)
            {
                uint? pid = column?.Pid?.Value;

                if (pid != null)
                {
                    result.Add(pid.Value);
                }
            }

            return result;
        }
    }
}