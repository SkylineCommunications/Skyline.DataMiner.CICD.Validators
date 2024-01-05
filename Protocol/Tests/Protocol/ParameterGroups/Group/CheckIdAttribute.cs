namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ParameterGroups.Group.CheckIdAttribute
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

    [Test(CheckId.CheckIdAttribute, Category.ParameterGroup)]
    internal class CheckIdAttribute : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (context?.ProtocolModel?.Protocol?.ParameterGroups == null)
            {
                return results;
            }

            foreach (var pg in context.ProtocolModel.Protocol.ParameterGroups)
            {
                (GenericStatus status, string rawId, uint? id) = GenericTests.CheckBasics(pg.Id, isRequired: true);

                // Missing
                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingAttribute(this, pg, pg));
                    continue;
                }

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, pg, pg));
                    continue;
                }

                // Invalid
                if (status.HasFlag(GenericStatus.Invalid) || !GenericTests.IsPlainNumbers(rawId))
                {
                    results.Add(Error.InvalidValue(this, pg, pg, rawId, pg.Name?.RawValue));
                    continue;
                }

                // A ParameterGroup should have a unique id > 0 and <= 10.000.
                if (id.Value <= 0 || id.Value > 10000)
                {
                    // From DM 9.0.4 onward, id > 0 and <=100.000 can be used.
                    var certainty = id.Value > 100000 ? Certainty.Certain : Certainty.Uncertain;
                    results.Add(Error.OutOfRangeId(this, pg, pg, certainty, rawId));
                    continue;
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedAttribute(this, pg, pg, rawId));
                    continue;
                }
            }

            // Duplicate
            var duplicateResults = GenericTests.CheckDuplicateIds(
                items: context.EachParameterGroupWithValidId(),
                getDuplicationIdentifier: x => x.Id?.Value,
                getName: x => x.Name?.RawValue,
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
                    {
                        if (!(context.Result.ReferenceNode is IParameterGroupsGroup readParameterGroup))
                        {
                            break;
                        }

                        var editParameterGroup = context.Protocol?.ParameterGroups?.Get(readParameterGroup);

                        var idAttribute = editParameterGroup?.EditNode.Attribute["id"];
                        if (idAttribute == null)
                        {
                            break;
                        }

                        idAttribute.Value = idAttribute.Value.Trim();
                        result.Success = true;
                    }
                    break;

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            return results;
        }
    }
}