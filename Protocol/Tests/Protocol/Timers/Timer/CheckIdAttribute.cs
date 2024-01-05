namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Timers.Timer.CheckIdAttribute
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

    [Test(CheckId.CheckIdAttribute, Category.Timer)]
    internal class CheckIdAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (context?.ProtocolModel?.Protocol?.Timers == null)
            {
                return results;
            }

            foreach (ITimersTimer timer in context.ProtocolModel.Protocol.Timers)
            {
                (GenericStatus status, string rawId, uint? _) = GenericTests.CheckBasics(timer.Id, isRequired: true);

                // Missing
                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingAttribute(this, timer, timer));
                    continue;
                }

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, timer, timer));
                    continue;
                }

                // Invalid
                if (status.HasFlag(GenericStatus.Invalid) || !GenericTests.IsPlainNumbers(rawId))
                {
                    results.Add(Error.InvalidValue(this, timer, timer, rawId, timer.Name?.RawValue));
                    continue;
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedAttribute(this, timer, timer, rawId));
                    continue;
                }
            }

            // Duplicate
            var duplicateResults = GenericTests.CheckDuplicateIds(
                items: context.EachTimerWithValidId(),
                getDuplicationIdentifier: timer => timer.Id?.Value,
                getName: timer => timer.Name?.RawValue,
                generateSubResult: x => Error.DuplicatedId(this, x.item, x.item, x.id, x.name),
                generateSummaryResult: x => Error.DuplicatedId(this, null, null, x.id, String.Join(", ", x.names)).WithSubResults(x.subResults)
                );

            results.AddRange(duplicateResults);

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    if (!(context.Result.ReferenceNode is ITimersTimer readTimer))
                    {
                        break;
                    }

                    var editTimer = context.Protocol?.Timers?.Get(readTimer);

                    var idAttribute = editTimer?.EditNode?.Attribute["id"];
                    if (idAttribute == null)
                    {
                        break;
                    }

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