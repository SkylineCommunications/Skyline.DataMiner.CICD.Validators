namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Positions.Position.Page.CheckPageTag
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckPageTag, Category.Param)]
    internal class CheckPageTag : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();
            List<IValidationResult> casingResults = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.Display?.Positions == null)
                {
                    continue;
                }

                foreach (var position in param.Display.Positions)
                {
                    (GenericStatus status, string rawPageName, string pageName) = GenericTests.CheckBasics(position.Page, isRequired: true);

                    // Missing
                    if (status.HasFlag(GenericStatus.Missing))
                    {
                        results.Add(Error.MissingTag(this, position, position, param.Id.RawValue));
                        continue;
                    }

                    // Empty
                    if (status.HasFlag(GenericStatus.Empty))
                    {
                        results.Add(Error.EmptyTag(this, position, position, param.Id.RawValue));
                        continue;
                    }

                    // Title Casing
                    string[] casingExceptions =
                    {
                        "CPEIntegration_",
                    };

                    if (!context.Helpers.TitleCasing.IsTitleCase(pageName, out string expectedPageName, casingExceptions))
                    {
                        IValidationResult wrongCasingSub = Error.WrongCasing_Sub(this, position, position, rawPageName, expectedPageName, param.Id.RawValue);
                        wrongCasingSub.WithExtraData(ExtraData.ExpectedPageName, expectedPageName);
                        casingResults.Add(wrongCasingSub);
                    }

                    // Untrimmed
                    if (status.HasFlag(GenericStatus.Untrimmed))
                    {
                        results.Add(Error.UntrimmedTag(this, position, position, param.Id.RawValue, rawPageName));
                        continue;
                    }
                }

                if (param.IsTitleEnd())
                {
                    // Title end parameters can be used multiple times on the same page
                    // since the same title end can be re-used to close different group boxes
                    continue;
                }

                var resultsForDuplicates = GenericTests.CheckDuplicates(
                    items: param.Display.Positions.Where(p => !String.IsNullOrEmpty(p.Page?.RawValue)).Select(p => p.Page),
                    getDuplicationIdentifier: p => p.Value,
                    generateSubResult: x => Error.DuplicatedValue(this, x.item, x.item, x.duplicateValue, param.Id?.RawValue),
                    generateSummaryResult: x => Error.DuplicatedValue(this, param.Display.Positions, param.Display.Positions, x.duplicateValue, param.Id?.RawValue).WithSubResults(x.subResults)
                    );

                results.AddRange(resultsForDuplicates);
            }

            if (casingResults.Count > 0)
            {
                results.Add(Error.WrongCasing(this, null, null).WithSubResults(casingResults.ToArray()));
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (!(context.Result.ReferenceNode is IParamsParamDisplayPositionsPosition readPosition))
            {
                result.Message = "context.Result.Node is not of type IParamsParamDisplayPositionsPosition";
                return result;
            }

            if (context.Protocol?.Params == null)
            {
                result.Message = "context.Protocol?.Params == null";
                return result;
            }

            foreach (var param in context.Protocol.Params)
            {
                var editPosition = param.Display?.Positions?.Get(readPosition);
                if (editPosition?.Page == null)
                {
                    continue;
                }

                switch (context.Result.ErrorId)
                {
                    case ErrorIds.UntrimmedTag:
                        {
                            editPosition.Page.Value = readPosition.Page.Value.Trim();
                            result.Success = true;

                            break;
                        }
                    case ErrorIds.WrongCasing_Sub:
                        {
                            editPosition.Page.Value = Convert.ToString(context.Result.ExtraData[ExtraData.ExpectedPageName]);
                            result.Success = true;

                            break;
                        }
                    default:
                        result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                        break;
                }
            }

            return result;
        }

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IParamsParam oldParam, IParamsParam newParam) in context.EachMatchingParam())
            {
                if (!oldParam.GetRTDisplay())
                {
                    continue;
                }

                var oldPages = oldParam.Display?.Positions?.Select(p => p.Page?.Value?.Trim());
                var newPages = newParam.Display?.Positions?.Select(p => p.Page?.Value?.Trim()).ToList();
                if (oldPages == null)
                {
                    continue;
                }

                string newPid = newParam.Id?.RawValue;

                foreach (string oldPage in oldPages)
                {
                    if (newPages?.FirstOrDefault(newPage => String.Equals(newPage, oldPage, StringComparison.OrdinalIgnoreCase)) == null)
                    {
                        results.Add(ErrorCompare.RemovedFromPage(newParam, newParam, newPid, oldPage));
                    }
                }
            }

            return results;
        }
    }

    internal enum ExtraData
    {
        ExpectedPageName
    }
}