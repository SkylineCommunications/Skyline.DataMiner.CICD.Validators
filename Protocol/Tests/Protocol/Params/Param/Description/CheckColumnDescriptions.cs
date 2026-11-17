namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Description.CheckColumnDescriptions
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

    [Test(CheckId.CheckColumnDescriptions, Category.Param)]
    internal class CheckColumnDescriptions : IValidate, ICodeFix
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (!param.IsTable())
                {
                    continue;
                }

                if (String.IsNullOrWhiteSpace(param.Description?.Value))
                {
                    continue;
                }

                string tableDescription = param.Description.Value.Trim();
                List<IValidationResult> subResults = new List<IValidationResult>();

                foreach ((uint? _, string _, IParamsParam columnParam) in param.GetColumns(context.ProtocolModel.RelationManager, returnBaseColumnsIfDuplicateAs: false))
                {
                    // For columns made via duplicateAs, columnParam will be null since column description should not be checked here.
                    if (columnParam == null)
                    {
                        continue;
                    }

                    if (!String.IsNullOrWhiteSpace(columnParam.Description?.Value) && !IsCorrectColumnDescription(columnParam, tableDescription))
                    {
                        IValidationResult missingTableDescriptionAsSuffix = Error.MissingTableDescriptionAsSuffix(this, columnParam, columnParam.Description, tableDescription, columnParam.Description.RawValue, columnParam.Id?.RawValue);
                        missingTableDescriptionAsSuffix.WithExtraData(ExtraData.TableDescription, tableDescription);
                        subResults.Add(missingTableDescriptionAsSuffix);
                    }

                    if (columnParam.TryGetWrite(context.ProtocolModel.RelationManager, out var writeColumn) &&
                        !String.IsNullOrWhiteSpace(writeColumn.Description?.Value) &&
                        !IsCorrectColumnDescription(writeColumn, tableDescription))
                    {
                        IValidationResult missingTableDescriptionAsSuffixWrite = Error.MissingTableDescriptionAsSuffix(this, writeColumn, writeColumn.Description, tableDescription, writeColumn.Description.RawValue, writeColumn.Id?.RawValue);
                        missingTableDescriptionAsSuffixWrite.WithExtraData(ExtraData.TableDescription, tableDescription);
                        subResults.Add(missingTableDescriptionAsSuffixWrite);
                    }
                }

                if (subResults.Count > 0)
                {
                    if (subResults.Count > 1)
                    {
                        IValidationResult missingTableDescriptionAsSuffixes = Error.MissingTableDescriptionAsSuffixes(this, param, param, tableDescription, param.Id.RawValue);
                        missingTableDescriptionAsSuffixes.WithSubResults(subResults.ToArray());
                        results.Add(missingTableDescriptionAsSuffixes);
                    }
                    else
                    {
                        results.Add(subResults.First());
                    }
                }
            }

            return results;

            bool IsCorrectColumnDescription(IParamsParam column, string tableDescription)
            {
                return column.Description?.Value.EndsWith($" ({tableDescription})") ?? false;
            }
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.MissingTableDescriptionAsSuffix:
                    var readNode = context.Result.ReferenceNode as IParamsParam;
                    var editNode = context.Protocol.Params.Get(readNode);
                    editNode.Description.Value = $"{editNode.Description.Value.Trim()} ({context.Result.ExtraData[ExtraData.TableDescription]})";
                    result.Success = true;
                    break;
                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }
    }

    internal enum ExtraData
    {
        TableDescription
    }
}
