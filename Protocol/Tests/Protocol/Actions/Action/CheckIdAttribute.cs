namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Actions.Action.CheckIdAttribute
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

    [Test(CheckId.CheckIdAttribute, Category.Action)]
    internal class CheckIdAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (context?.ProtocolModel?.Protocol?.Actions == null)
            {
                return results;
            }

            foreach (var action in context.ProtocolModel.Protocol.Actions)
            {
                (GenericStatus status, string rawId, uint? _) = GenericTests.CheckBasics(action.Id, isRequired: true);

                // Missing
                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingAttribute(this, action, action));
                    continue;
                }

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, action, action));
                    continue;
                }

                // Invalid
                if (status.HasFlag(GenericStatus.Invalid) || !GenericTests.IsPlainNumbers(rawId))
                {
                    results.Add(Error.InvalidValue(this, action, action, rawId, action.Name?.RawValue));
                    continue;
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedAttribute(this, action, action, rawId));
                    continue;
                }
            }

            // Duplicate
            var duplicateResults = GenericTests.CheckDuplicateIds(
                items: context.EachActionWithValidId(),
                getDuplicationIdentifier: action => action.Id?.Value,
                getName: action => action.Name?.RawValue,
                generateSubResult: x => Error.DuplicatedId(this, x.item, x.item, x.id, x.name),
                generateSummaryResult: x => Error.DuplicatedId(this, null, null, x.id, String.Join(", ", x.names)).WithSubResults(x.subResults)
                );

            results.AddRange(duplicateResults);

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (!(context.Result.ReferenceNode is IActionsAction readAction))
            {
                result.Message = "ReferenceNode not of type IActionsAction.";
                return result;
            }

            var editAction = context.Protocol?.Actions?.Get(readAction);
            if (editAction == null)
            {
                result.Message = "editAction is null.";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:

                    editAction.Id = readAction.Id?.Value;
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