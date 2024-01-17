namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Database.ColumnDefinition.CheckColumnDefinitionTag
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckColumnDefinitionTag, Category.Param)]
    internal class CheckColumnDefinitionTag : /*IValidate, ICodeFix, */ICompare
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

            foreach (var (oldParam, newParam) in context.EachMatchingParam())
            {
                string newPid = newParam?.Id?.RawValue;

                string oldDefinition = oldParam?.Database?.ColumnDefinition?.Value;
                string newDefinition = newParam?.Database?.ColumnDefinition?.Value;

                if (oldDefinition == null || oldDefinition == newDefinition)
                {
                    continue;
                }

                if (newDefinition == null)
                {
                    // Removed? TODO: Add error message?
                    continue;
                }

                if (oldDefinition.StartsWith("VARCHAR") && newDefinition.StartsWith("VARCHAR"))
                {
                    int oldCharSize = ExtractVarCharSize(oldDefinition);
                    int newCharSize = ExtractVarCharSize(newDefinition);
                    if (oldCharSize != -1 && newCharSize != -1 && newCharSize > oldCharSize)
                    {
                        continue;
                    }
                }

                results.Add(ErrorCompare.ChangedLoggerDataType(newParam, newParam, oldDefinition, newPid, newDefinition));
            }

            return results;

            int ExtractVarCharSize(string columnDefinition)
            {
                var definitionSplit = columnDefinition.Split('(', ')');
                if (definitionSplit.Length > 1 && Int32.TryParse(definitionSplit[1], out int charSize))
                {
                    return charSize;
                }

                return -1;
            }
        }
    }
}