namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Name.CheckColumnNames
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

    [Test(CheckId.CheckColumnNames, Category.Param)]
    internal class CheckColumnNames : IValidate, ICodeFix
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

                if (String.IsNullOrWhiteSpace(param.Name?.Value))
                {
                    continue;
                }

                string tableName = GetTableName(param.Name.Value);
                List<IValidationResult> subResults = new List<IValidationResult>();

                foreach ((uint? _, string _, IParamsParam columnParam) in param.GetColumns(context.ProtocolModel.RelationManager, returnBaseColumnsIfDuplicateAs: false))
                {
                    // For columns made via duplicateAs, columnParam will be null since column name should not be checked here.
                    if (columnParam == null)
                    {
                        continue;
                    }

                    if (IsCorrectColumnName(columnParam, tableName))
                    {
                        continue;
                    }

                    IValidationResult missingTableNameAsPrefix = Error.MissingTableNameAsPrefix(this, columnParam, columnParam.Name, tableName, columnParam.Name.RawValue, columnParam.Id?.RawValue);
                    missingTableNameAsPrefix.WithExtraData(ExtraData.TableName, tableName);
                    subResults.Add(missingTableNameAsPrefix);

                    if (columnParam.TryGetWrite(context.ProtocolModel.RelationManager, out var writeColumn))
                    {
                        IValidationResult missingTableNameAsPrefixWrite = Error.MissingTableNameAsPrefix(this, writeColumn, writeColumn.Name, tableName, writeColumn.Name.RawValue, writeColumn.Id?.RawValue);
                        missingTableNameAsPrefixWrite.WithExtraData(ExtraData.TableName, tableName);
                        subResults.Add(missingTableNameAsPrefixWrite);
                    }
                }

                if (subResults.Count > 0)
                {
                    if (subResults.Count > 1)
                    {
                        IValidationResult missingTableNameAsPrefixes = Error.MissingTableNameAsPrefixes(this, param, param, tableName, param.Id.RawValue);
                        missingTableNameAsPrefixes.WithSubResults(subResults.ToArray());
                        results.Add(missingTableNameAsPrefixes);
                    }
                    else
                    {
                        results.Add(subResults.First());
                    }
                }
            }

            return results;

            bool IsCorrectColumnName(IParamsParam column, string tableName)
            {
                return column.Name?.Value.StartsWith(tableName) ?? false;
            }

            string GetTableName(string tableName)
            {
                const string IgnoreEnding = "table";

                if (tableName.EndsWith(IgnoreEnding, StringComparison.InvariantCultureIgnoreCase))
                {
                    tableName = tableName.Remove(tableName.Length - IgnoreEnding.Length);
                }

                return tableName.TrimEnd(' ');
            }
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.MissingTableNameAsPrefix:
                    var readNode = context.Result.ReferenceNode as IParamsParam;
                    var editNode = context.Protocol.Params.Get(readNode);
                    editNode.Name.Value = context.Result.ExtraData[ExtraData.TableName] + editNode.Name.Value.Trim();
                    // If we decide to introduce the underscore as below, we also need to adapt snippets for consistency (and maybe also generation from XML, SNMP...)
                    //editNode.Name.Value = context.Result.ExtraData[ExtraData.TableName] + "_" + editNode.Name.Value.Trim();
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
        TableName
    }
}