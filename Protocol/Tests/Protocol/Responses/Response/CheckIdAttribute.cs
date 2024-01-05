namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Responses.Response.CheckIdAttribute
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

    [Test(CheckId.CheckIdAttribute, Category.Response)]
    internal class CheckIdAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (context?.ProtocolModel?.Protocol?.Responses == null)
            {
                return results;
            }

            foreach (var response in context.ProtocolModel.Protocol.Responses)
            {
                (GenericStatus status, string rawId, uint? _) = GenericTests.CheckBasics(response.Id, isRequired: true);

                // Missing
                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingAttribute(this, response, response));
                    continue;
                }

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, response, response));
                    continue;
                }

                // Invalid
                if (status.HasFlag(GenericStatus.Invalid) || !GenericTests.IsPlainNumbers(rawId))
                {
                    results.Add(Error.InvalidValue(this, response, response, rawId, response.Name?.RawValue));
                    continue;
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedAttribute(this, response, response, rawId));
                    continue;
                }
            }

            // Duplicate
            var duplicateResults = GenericTests.CheckDuplicateIds(
                items: context.EachResponseWithValidId(),
                getDuplicationIdentifier: response => response.Id?.Value,
                getName: response => response.Name?.RawValue,
                generateSubResult: responseNode => Error.DuplicatedId(this, responseNode.item, responseNode.item, responseNode.id, responseNode.name),
                generateSummaryResult: x => Error.DuplicatedId(this, null, null, x.id, String.Join(", ", x.names)).WithSubResults(x.subResults)
                );

            results.AddRange(duplicateResults);

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol.Responses == null)
            {
                result.Message = "No Responses found!";
                return result;
            }

            if (!(context.Result.ReferenceNode is IResponsesResponse readResponse))
            {
                result.Message = "Invalid node.";
                return result;
            }

            var editNode = context.Protocol.Responses.Get(readResponse);

            if (editNode == null)
            {
                result.Message = "No Edit Node found for response.";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    var idAttribute = editNode.EditNode.Attribute["id"];
                    idAttribute.Value = idAttribute.Value.Trim();
                    result.Success = true;
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