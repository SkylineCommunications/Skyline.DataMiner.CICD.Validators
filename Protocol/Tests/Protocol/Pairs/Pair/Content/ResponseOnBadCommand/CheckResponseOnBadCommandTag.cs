namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Pairs.Pair.Content.ResponseOnBadCommand.CheckResponseOnBadCommandTag
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckResponseOnBadCommandTag, Category.Pair)]
    internal class CheckResponseOnBadCommandTag : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IPairsPair pair in context.EachPairWithValidId())
            {
                if (pair.Content == null)
                {
                    continue;
                }

                foreach (var item in pair.Content)
                {
                    if (!(item is IPairsPairContentResponseOnBadCommand response))
                    {
                        continue;
                    }

                    (GenericStatus status, _, uint? id) = GenericTests.CheckBasics(response, isRequired: false);

                    // Empty
                    if (status.HasFlag(GenericStatus.Empty))
                    {
                        results.Add(Error.EmptyTag(this, response, response, pair.Id.RawValue));
                        continue;
                    }

                    // Invalid
                    if (status.HasFlag(GenericStatus.Invalid) || !GenericTests.IsPlainNumbers(response.RawValue))
                    {
                        results.Add(Error.InvalidValue(this, response, response, response.RawValue, pair.Id.RawValue));
                        continue;
                    }

                    // Non-Existing Response
                    if (!context.ProtocolModel.TryGetObjectByKey<IResponsesResponse>(Mappings.ResponsesById, Convert.ToString(id), out _))
                    {
                        results.Add(Error.NonExistingId(this, response, response, response.RawValue, pair.Id.RawValue));
                        continue;
                    }

                    // Untrimmed
                    if (status.HasFlag(GenericStatus.Untrimmed))
                    {
                        results.Add(Error.UntrimmedTag(this, response, response, pair.Id.RawValue, response.RawValue));
                        continue;
                    }
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedTag:
                    if (!(context.Result.ReferenceNode is IPairsPairContentResponseOnBadCommand readResponse))
                    {
                        result.Message = "context.Result.Node is not of type IPairsPairContentResponseOnBadCommand";
                        break;
                    }

                    if (context.Protocol?.Pairs == null)
                    {
                        result.Message = "context?.Protocol.Pairs == null";
                        break;
                    }

                    foreach (var pair in context.Protocol.Pairs)
                    {
                        var editResponse = pair.Content.Get(readResponse) as Skyline.DataMiner.CICD.Models.Protocol.Edit.PairsPairContentResponseOnBadCommand;
                        if (editResponse == null)
                        {
                            continue;
                        }

                        editResponse.RawValue = editResponse.RawValue.Trim();
                        result.Success = true;
                    }

                    if (!result.Success)
                    {
                        result.Message = "Matching 'Content/IPairsPairContentResponseOnBadCommand' tag not found.";
                    }

                    break;
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
}