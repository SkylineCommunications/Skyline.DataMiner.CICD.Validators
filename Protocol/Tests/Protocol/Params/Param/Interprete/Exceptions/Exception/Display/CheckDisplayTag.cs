namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.Exceptions.Exception.Display.CheckDisplayTag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckDisplayTag, Category.Param)]
    internal class CheckDisplayTag : IValidate, ICodeFix/*, ICompare*/
    {
        private const string CorrectNotApplicableDisplayValue = "N/A";

        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();
            List<IValidationResult> casingResults = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.Interprete?.Exceptions == null)
                {
                    continue;
                }

                foreach (var exception in param.Interprete.Exceptions)
                {
                    var display = exception.Display;
                    (GenericStatus status, string rawDisplayValue, string displayValue) = GenericTests.CheckBasics(display, isRequired: true);

                    // Missing
                    if (status.HasFlag(GenericStatus.Missing))
                    {
                        results.Add(Error.MissingTag(this, exception, exception, param.Id?.RawValue));
                        continue;
                    }

                    // Empty
                    if (status.HasFlag(GenericStatus.Empty))
                    {
                        results.Add(Error.EmptyTag(this, exception, display, param.Id?.RawValue));
                        continue;
                    }

                    // Using N/A for Not Applicable
                    if (displayValue != CorrectNotApplicableDisplayValue &&
                        (String.Equals(displayValue, "Not Applicable", StringComparison.OrdinalIgnoreCase) ||
                        Regex.IsMatch(displayValue, @"^N([^A-Z0-9]*)A$", RegexOptions.IgnoreCase)))
                    {
                        IValidationResult unrecommendedNaDisplayValue = Error.UnrecommendedNADisplayValue(this, param, display, displayValue, param.Id?.RawValue, CorrectNotApplicableDisplayValue);
                        unrecommendedNaDisplayValue.WithExtraData(ExtraData.ExceptionToFix, exception);
                        results.Add(unrecommendedNaDisplayValue);
                        continue;
                    }

                    // Title Casing
                    if (!context.Helpers.TitleCasing.IsTitleCase(displayValue, out string expectedDisplayValue))
                    {
                        IValidationResult wrongCasingSub = Error.WrongCasing_Sub(this, param, display, rawDisplayValue, expectedDisplayValue, param.Id?.RawValue);
                        wrongCasingSub.WithExtraData(ExtraData.ExceptionToFix, exception)
                                      .WithExtraData(ExtraData.ExpectedValue, expectedDisplayValue);
                        casingResults.Add(wrongCasingSub);
                    }

                    // Untrimmed
                    if (status.HasFlag(GenericStatus.Untrimmed))
                    {
                        IValidationResult untrimmedTag = Error.UntrimmedTag(this, param, display, param.Id?.RawValue, rawDisplayValue);
                        untrimmedTag.WithExtraData(ExtraData.ExceptionToFix, exception);
                        results.Add(untrimmedTag);
                        continue;
                    }
                }

                var resultsForDuplicates = GenericTests.CheckDuplicates(
                    items: param.Interprete.Exceptions.Where(e => !String.IsNullOrEmpty(e.Display?.RawValue)).Select(e => e.Display),
                    getDuplicationIdentifier: d => d.Value,
                    generateSubResult: x => Error.DuplicatedValue(this, x.item, x.item, x.duplicateValue, param.Id?.RawValue),
                    generateSummaryResult: x => Error.DuplicatedValue(this, param.Interprete.Exceptions, param.Interprete.Exceptions, x.duplicateValue, param.Id?.RawValue).WithSubResults(x.subResults)
                    );

                results.AddRange(resultsForDuplicates);
            }

            if (casingResults.Count > 0)
            {
                IValidationResult wrongCasing = Error.WrongCasing(this, null, null).WithSubResults(casingResults.ToArray());
                results.Add(wrongCasing);
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (!(context.Result.ReferenceNode is IParamsParam readParam))
            {
                result.Message = "context.Result.Node is not of type IParamsParam";
                return result;
            }

            var editParam = context.Protocol?.Params?.Get(readParam);
            if (editParam == null)
            {
                result.Message = "editParam is null.";
                return result;
            }

            if (!context.Result.ExtraData.TryGetValue(ExtraData.ExceptionToFix, out object o)
                || !(o is IParamsParamInterpreteExceptionsException readException))
            {
                result.Message = "ExtraData[exceptionToFix] is not of type IParamsParamInterpreteExceptionsException";
                return result;
            }

            var editException = editParam.Interprete?.Exceptions?.Get(readException);
            if (editException == null)
            {
                result.Message = "editException is null.";
                return result;
            }

            if (editException.Display == null)
            {
                result.Message = "editException.Display is null.";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UnrecommendedNADisplayValue:
                    {
                        editException.Display.Value = CorrectNotApplicableDisplayValue;
                        result.Success = true;

                        break;
                    }
                case ErrorIds.UntrimmedTag:
                    {
                        editException.Display.Value = readException.Display.Value.Trim();
                        result.Success = true;

                        break;
                    }
                case ErrorIds.WrongCasing_Sub:
                    {
                        editException.Display.Value = Convert.ToString(context.Result.ExtraData[ExtraData.ExpectedValue]);
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
        ExceptionToFix,
        ExpectedValue
    }
}