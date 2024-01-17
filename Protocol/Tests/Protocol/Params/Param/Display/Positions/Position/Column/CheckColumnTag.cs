namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Positions.Position.Column.CheckColumnTag
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

    [Test(CheckId.CheckColumnTag, Category.Param)]
    internal class CheckColumnTag : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var unrecommendedColumnsPerPage = new Dictionary<string, (List<IValidationResult> subResults, List<string> paramIds)>();
            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                var positions = param.Display?.Positions;
                if (positions == null)
                {
                    continue;
                }

                foreach (var position in positions)
                {
                    var column = position.Column;

                    (GenericStatus status, string rawValue, uint? columnValue) = GenericTests.CheckBasics(column, isRequired: true);

                    // Missing
                    if (status.HasFlag(GenericStatus.Missing))
                    {
                        results.Add(Error.MissingTag(this, param, position, param.Id.RawValue));
                        continue;
                    }

                    // Empty
                    if (status.HasFlag(GenericStatus.Empty))
                    {
                        results.Add(Error.EmptyTag(this, param, column, param.Id.RawValue));
                        continue;
                    }

                    // Invalid
                    if (status.HasFlag(GenericStatus.Invalid) || !GenericTests.IsPlainNumbers(rawValue))
                    {
                        results.Add(Error.InvalidTag(this, param, column, rawValue, param.Id.RawValue));
                        continue;
                    }

                    // Unrecommended (Skyline recommends max 2 columns on pages)
                    if (columnValue.Value > 1)
                    {
                        string pageName = position.Page?.Value ?? String.Empty;

                        if (!unrecommendedColumnsPerPage.TryGetValue(pageName, out var unrecommendedColumns))
                        {
                            unrecommendedColumns = (new List<IValidationResult>(), new List<string>());
                            unrecommendedColumnsPerPage.Add(pageName, unrecommendedColumns);
                        }

                        unrecommendedColumns.subResults.Add(Error.UnrecommendedValue(this, column, column, pageName, param.Id.RawValue));
                        unrecommendedColumns.paramIds.Add(param.Id.RawValue);

                        continue;
                    }

                    // Untrimmed
                    if (status.HasFlag(GenericStatus.Untrimmed))
                    {
                        IValidationResult untrimmedTag = Error.UntrimmedTag(this, param, column, param.Id.RawValue, rawValue)
                            .WithExtraData(ExtraData.PositionNode, position);

                        results.Add(untrimmedTag);
                    }
                }
            }

            foreach (KeyValuePair<string, (List<IValidationResult>, List<string>)> kvp in unrecommendedColumnsPerPage)
            {
                string pageName = kvp.Key;
                (List<IValidationResult> subResults, List<string> paramIds) = kvp.Value;

                IValidationResult unrecommendedValue = Error.UnrecommendedValue(this, null, null, pageName, String.Join(", ", paramIds))
                    .WithSubResults(subResults.ToArray());

                results.Add(unrecommendedValue);
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol.Params == null)
            {
                result.Message = "No Param found";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedTag:
                    {
                        var paramReadNode = (IParamsParam)context.Result.ReferenceNode;
                        var paramEditNode = context.Protocol.Params.Get(paramReadNode);

                        var positionReadNode = (IParamsParamDisplayPositionsPosition)context.Result.ExtraData[ExtraData.PositionNode];
                        var positionEditNode = paramEditNode.Display.Positions.Get(positionReadNode);

                        positionEditNode.Column.Value = positionReadNode.Column.Value;
                        result.Success = true;

                        break;
                    }

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }

    internal enum ExtraData
    {
        PositionNode
    }
}