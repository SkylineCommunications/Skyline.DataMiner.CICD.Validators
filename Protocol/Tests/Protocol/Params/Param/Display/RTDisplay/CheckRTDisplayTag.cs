namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.RTDisplay.CheckRTDisplayTag
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Enums;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckRTDisplayTag, Category.Param, TestOrder.Post1)]
    internal class CheckRTDisplayTag : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            ValidateHelper helper = new ValidateHelper(this, context, results);

            helper.RunRegularCheck();
            helper.RunPostCheck();

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol?.Params == null)
            {
                result.Message = "No Params found";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedTag:
                    {
                        var readNode = (IParamsParam)context.Result.ReferenceNode;
                        if (readNode == null)
                        {
                            result.Message = "readNode not parse-able as IParamsParam";
                            return result;
                        }

                        var editNode = context.Protocol.Params.Get(readNode);
                        if (editNode == null)
                        {
                            result.Message = "editNode not found";
                            return result;
                        }

                        editNode.Display.RTDisplay.Value = readNode.Display.RTDisplay.Value;
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
        private readonly RelationManager relationManager;
        private readonly List<IValidationResult> results;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results)
        {
            this.test = test;
            this.context = context;
            this.relationManager = context.ProtocolModel.RelationManager;
            this.results = results;
        }

        public void RunRegularCheck()
        {
            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.Display?.RTDisplay == null)
                {
                    // No RTDisplay detected
                    continue;
                }

                var tag = param.Display.RTDisplay;
                (GenericStatus status, string rawValue, bool? _) = GenericTests.CheckBasics(tag, false);

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyTag(test, param, tag, param.Id.RawValue));
                    continue;
                }

                // Invalid
                if (status.HasFlag(GenericStatus.Invalid))
                {
                    results.Add(Error.InvalidValue(test, param, tag, rawValue, param.Id.RawValue));
                    continue;
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedTag(test, param, tag, param.Id.RawValue, rawValue));
                }
            }
        }

        public void RunPostCheck()
        {
            if (context.ProtocolModel?.Protocol?.Params == null)
            {
                return;
            }

            // RTDisplay Expected
            var paramsAllowingRtDisplay = context.CrossData.RtDisplay.GetParamsAllowingRtDisplay();
            foreach ((IParamsParam paramExpectingRtDisplay, List<IValidationResult> subResults) in paramsAllowingRtDisplay)
            {
                // RTDisplay already there
                if (paramExpectingRtDisplay.GetRTDisplay())
                {
                    continue;
                }

                if (!context.ProtocolModel.IsExportedProtocolModel && paramExpectingRtDisplay.WillBeExported())
                {
                    // Will be checked when validating the exported protocol
                    continue;
                }

                if (!subResults.Any())
                {
                    // If no subResult, we consider RTDisplay is allowed but not required.
                    continue;
                }

                // NOTE: These params could technically have no or an invalid id. Shouldn't happen normally as the other checks should use the Each methods.
                IValidationResult missingTagWithExpectedValue = Error.RTDisplayExpected(test, paramExpectingRtDisplay, paramExpectingRtDisplay, paramExpectingRtDisplay.Id?.RawValue);
                missingTagWithExpectedValue.WithSubResults(subResults.ToArray());
                results.Add(missingTagWithExpectedValue);
            }

            // RTDisplay Unexpected
            IEnumerable<IParamsParam> paramsUnexpectingRtDisplay = context.ProtocolModel.Protocol.Params
                                                                    .Except(paramsAllowingRtDisplay.Select(tuple => tuple.param))
                                                                    .Where(param => param.Display?.RTDisplay != null);

            foreach (IParamsParam paramUnexpectingRtDisplay in paramsUnexpectingRtDisplay)
            {
                // No RTDisplay
                if (!paramUnexpectingRtDisplay.GetRTDisplay())
                {
                    continue;
                }

                var rtDisplay = paramUnexpectingRtDisplay.Display.RTDisplay;

                // OnAppLevel
                if (rtDisplay.OnAppLevel?.Value == true)
                {
                    continue;
                }

                // Columns that belong to table expecting RTDisplay(true)
                if (paramUnexpectingRtDisplay.TryGetTableFromReadAndWrite(relationManager, out var tableParam) &&
                    paramsAllowingRtDisplay.Any(i => i.param == tableParam) && !tableParam.GetRTDisplay())
                {
                    continue;
                }

                // Exported
                if (!context.ProtocolModel.IsExportedProtocolModel && paramUnexpectingRtDisplay.WillBeExported())
                {
                    // Will be checked when validating the exported protocol
                    continue;
                }

                results.Add(Error.RTDisplayUnexpected(test, paramUnexpectingRtDisplay, rtDisplay, paramUnexpectingRtDisplay.Id?.RawValue));
            }
        }
    }
}