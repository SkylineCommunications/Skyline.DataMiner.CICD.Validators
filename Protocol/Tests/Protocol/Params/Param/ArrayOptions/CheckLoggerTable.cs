namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckLoggerTable
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckLoggerTable, Category.Param)]
    internal class CheckLoggerTable : /*IValidate, ICodeFix, */ICompare
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
                string newPid = newParam?.Id?.RawValue;

                var oldColumns = oldParam?.ArrayOptions;
                var newColumns = newParam?.ArrayOptions;

                if (oldColumns == null || newColumns == null)
                {
                    continue;
                }

                string oldArrayOptionOptions = oldColumns.Options?.Value;
                if (String.IsNullOrEmpty(oldArrayOptionOptions) || !oldArrayOptionOptions.Contains("database"))
                {
                    continue;
                }

                foreach (var oldColumn in oldColumns)
                {
                    var newColumn = newColumns.FirstOrDefault(p => p?.Pid?.Value == oldColumn?.Pid?.Value);
                    if (newColumn == null)
                    {
                        results.Add(ErrorCompare.RemovedLoggerColumn(newParam, newParam, oldColumn?.Pid?.RawValue, newPid));
                    }
                }
            }

            return results;
        }
    }
}