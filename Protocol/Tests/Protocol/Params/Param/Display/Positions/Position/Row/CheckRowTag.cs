namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Positions.Position.Row.CheckRowTag
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckRowTag, Category.Param)]
    internal class CheckRowTag : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                var positions = param.Display?.Positions;
                if (positions == null)
                {
                    continue;
                }

                foreach (var position in positions)
                {
                    var row = position.Row;

                    (GenericStatus status, string rawValue, uint? _) = GenericTests.CheckBasics(row, isRequired: true);

                    // Missing
                    if (status.HasFlag(GenericStatus.Missing))
                    {
                        results.Add(Error.MissingTag(this, param, position, param.Id.RawValue));
                        continue;
                    }

                    // Empty
                    if (status.HasFlag(GenericStatus.Empty))
                    {
                        results.Add(Error.EmptyTag(this, param, row, param.Id.RawValue));
                        continue;
                    }

                    // Invalid
                    if (status.HasFlag(GenericStatus.Invalid) || !GenericTests.IsPlainNumbers(rawValue))
                    {
                        results.Add(Error.InvalidTag(this, param, row, rawValue, param.Id.RawValue));
                        continue;
                    }

                    // Untrimmed
                    if (status.HasFlag(GenericStatus.Untrimmed))
                    {
                        IValidationResult untrimmedTag = Error.UntrimmedTag(this, param, row, param.Id.RawValue, rawValue)
                            .WithExtraData(ExtraData.PositionNode, position);

                        results.Add(untrimmedTag);
                    }
                }
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

                        positionEditNode.Row.Value = positionReadNode.Row.Value;
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