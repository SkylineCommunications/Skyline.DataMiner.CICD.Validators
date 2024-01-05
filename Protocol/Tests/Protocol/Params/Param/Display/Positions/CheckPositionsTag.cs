namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Positions.CheckPositionsTag
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

    [Test(CheckId.CheckPositionsTag, Category.Param)]
    internal class CheckPositionsTag : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var param in context.EachParamWithValidId())
            {
                var positions = param.Display?.Positions;
                if (positions == null)
                {
                    continue;
                }

                ValidateHelper helper = new ValidateHelper(this, context, results, param);
                helper.Validate();
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
                case ErrorIds.EmptyTag:
                    {
                        var paramReadNode = (IParamsParam)context.Result.ReferenceNode;
                        var paramEditNode = context.Protocol.Params.Get(paramReadNode);

                        var displayEditNode = paramEditNode.Display;

                        displayEditNode.Cleanup(cleanupSelf: true);

                        // For now, the above cleanup will do.
                        // However, on the long run, we might want to change it to something like the below
                        // Cause the above cleanup might cleanup much more than what we expect it to
                        // It will indeed cleanup any other empty tag and attributes.
                        // The below would only cleanup what we want but is currently not quite working yet.
                        ////displayEditNode.Positions = null;
                        ////bool hasAttributes = displayEditNode.ReadNode.GetAttributes().Any();
                        ////var elements = displayEditNode.ReadNode.GetElements();
                        ////bool hasElements = displayEditNode.ReadNode.GetElements().Any();
                        ////if (!hasAttributes && !hasElements)
                        ////{
                        ////    paramEditNode.Display = null;
                        ////}

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

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly ValidatorContext context;
        private readonly List<IValidationResult> results;

        private readonly IParamsParam param;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam param)
        {
            this.test = test;
            this.context = context;
            this.results = results;

            this.param = param;
        }

        public void Validate()
        {
            var positions = param.Display.Positions;
            if (!positions.Any())
            {
                results.Add(Error.EmptyTag(test, param, positions, param.Id.RawValue));
                return;
            }

            // Param Requiring RTDisplay
            if (HasValidPosition(positions))
            {
                // If no valid position, we want to first have the developer fix or remove invalid positions to be sure no one adds excessive RTDisplay
                IValidationResult rtDisplayError = Error.RTDisplayExpected(test, param, positions, param.Id.RawValue);
                context.CrossData.RtDisplay.AddParam(param, rtDisplayError);
            }
        }

        private static bool HasValidPosition(IParamsParamDisplayPositions positions)
        {
            return positions.Any(position => !String.IsNullOrWhiteSpace(position.Page?.Value) && position.Column?.Value != null && position.Row?.Value != null);
        }
    }
}