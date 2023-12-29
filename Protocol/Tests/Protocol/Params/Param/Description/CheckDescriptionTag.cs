namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Description.CheckDescriptionTag
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

    [Test(CheckId.CheckDescriptionTag, Category.Param)]
    internal class CheckDescriptionTag : ICompare, IValidate, ICodeFix
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();
            List<IValidationResult> casingResults = new List<IValidationResult>();

            var parameters = context.EachParamWithValidId().ToList();

            foreach (IParamsParam param in parameters)
            {
                if (!param.IsInSLElement(context.ProtocolModel.RelationManager))
                {
                    // If param is not in SLElement, we don't mind the description
                    continue;
                }

                var description = param.Description;
                (GenericStatus status, string descriptionRaw, string _) = GenericTests.CheckBasics(description, isRequired: true);

                // Missing
                if (status.HasFlag(GenericStatus.Missing))
                {
                    if (IsDescriptionMandatory(param))
                    {
                        results.Add(Error.MissingTag(this, param, param, param.Id.RawValue));
                    }

                    continue;
                }

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    if (IsDescriptionMandatory(param))
                    {
                        results.Add(Error.EmptyTag(this, param, description, param.Id.RawValue));
                    }

                    continue;
                }

                // Title Casing
                if (!context.Helpers.TitleCasing.IsTitleCase(descriptionRaw, out string expectedDescription))
                {
                    //Dictionary<Enum, object> extraData = new Dictionary<Enum, object>
                    //{
                    //    { ExtraData.ExpectedDescription, expectedDescription },
                    //};

                    var error = Error.WrongCasing_Sub(this, param, description, descriptionRaw, expectedDescription, param.Id.RawValue)
                                     .WithExtraData(ExtraData.ExpectedDescription, expectedDescription);

                    casingResults.Add(error);
                    //casingResults.Add(Error.WrongCasing_Sub(this, param, description, descriptionRaw, expectedDescription, param.Id.RawValue, null, extraData));
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedTag(this, param, description, param.Id.RawValue, descriptionRaw));
                    continue;
                }
            }

            // Duplicate
            var duplicateResults = GenericTests.CheckDuplicates(
                items: parameters,
                getDuplicationIdentifier: p => p.Description?.Value,
                getId: p => p.Id?.RawValue,
                isValidDuplicate: ParamHelper.IsValidParamAssociation,
                generateSubResult: x => Error.DuplicatedValue(this, x.item, x.item, x.duplicateValue, x.id),
                generateSummaryResult: x => Error.DuplicatedValue(this, null, null, x.duplicateValue, String.Join(", ", x.ids)).WithSubResults(x.subResults)
                );

            if (casingResults.Count > 0)
            {
                results.Add(Error.WrongCasing(this, null, null).WithSubResults(casingResults.ToArray()));
            }

            results.AddRange(duplicateResults);

            return results;

            bool IsDescriptionMandatory(IParamsParam param)
            {
                return !param.IsButton() &&
                       !param.IsPageButton() &&
                       !param.IsTitleEnd();
            }
        }

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IParamsParam oldParam, IParamsParam newParam) in context.EachMatchingParam())
            {
                if (!oldParam.IsInSLElement(context.PreviousProtocolModel.RelationManager) &&
                        !oldParam.WillBeExported() &&
                        oldParam.Trending?.Value != true &&
                        oldParam.Alarm?.Monitored?.Value != true)
                {
                    continue;
                }

                string newPid = newParam.Id?.RawValue;

                string oldDescriptionValue = oldParam.Description?.Value;
                if (String.IsNullOrWhiteSpace(oldDescriptionValue))
                {
                    continue;
                }

                string newDescriptionValue = newParam.Description?.Value;
                if (newDescriptionValue == null)
                {
                    if (!newParam.IsTitleEnd() && !newParam.IsTreeControl(context.NewProtocolModel.RelationManager))
                    {
                        results.Add(ErrorCompare.RemovedItem(newParam, newParam, oldDescriptionValue, newPid));
                    }

                    continue;
                }

                // Adding/removing spaces is allowed
                newDescriptionValue = Regex.Replace(newDescriptionValue, @"\s*", String.Empty, RegexOptions.IgnoreCase).Trim();
                oldDescriptionValue = Regex.Replace(oldDescriptionValue, @"\s*", String.Empty, RegexOptions.IgnoreCase).Trim();

                // Adding/removing [IDX] indication is allowed
                newDescriptionValue = Regex.Replace(newDescriptionValue, @"(\[idx\])", String.Empty, RegexOptions.IgnoreCase).Trim();
                oldDescriptionValue = Regex.Replace(oldDescriptionValue, @"(\[idx\])", String.Empty, RegexOptions.IgnoreCase).Trim();

                if (!oldDescriptionValue.Equals(newDescriptionValue, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(ErrorCompare.UpdatedValue(newParam, newParam, newPid, oldParam.Description?.RawValue, newParam.Description?.RawValue));
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            var readParam = (IParamsParam)context.Result.ReferenceNode;
            if (readParam == null)
            {
                result.Message = "'readParam' is null.";
                return result;
            }

            var editParam = context.Protocol.Params.Get(readParam);
            if (editParam == null)
            {
                result.Message = "'editParam' is null.";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedTag:
                    {
                        editParam.Description.Value = readParam.Description.Value.Trim();
                        result.Success = true;

                        break;
                    }

                case ErrorIds.WrongCasing_Sub:
                    {
                        editParam.Description.Value = Convert.ToString(context.Result.ExtraData[ExtraData.ExpectedDescription]);
                        result.Success = true;

                        break;
                    }

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) is not implemented.";
                    break;
            }

            return result;
        }
    }

    internal enum ExtraData
    {
        ExpectedDescription
    }
}