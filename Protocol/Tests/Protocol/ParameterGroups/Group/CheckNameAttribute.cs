namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ParameterGroups.Group.CheckNameAttribute
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

    [Test(CheckId.CheckNameAttribute, Category.ParameterGroup)]
    internal class CheckNameAttribute : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            // ToList so it won't recalculate (See yield return)
            var parameterGroups = context.EachParameterGroupWithValidId().ToList();

            foreach (IParameterGroupsGroup paramGroup in parameterGroups)
            {
                var name = paramGroup.Name;
                (GenericStatus status, string rawValue, string value) = GenericTests.CheckBasics(name, isRequired: true);

                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingAttribute(this, paramGroup, paramGroup, paramGroup.Id.RawValue));
                    continue;
                }

                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, paramGroup, name, paramGroup.Id.RawValue));
                    continue;
                }

                // Checking if name is too long
                if (value.Length > 25)
                {
                    results.Add(Error.LengthyValue(this, paramGroup, name, rawValue));
                    continue;
                }

                // Checking if name contains invalid characters.
                IList<char> invalidChars = Helper.CheckInvalidChars(value, DcfHelper.RestrictedParameterGroupNameChars).ToList();
                if (invalidChars.Any())
                {
                    results.Add(Error.InvalidChars(this, paramGroup, name, rawValue, String.Join(" ", invalidChars)));
                    continue;
                }

                // Untrimmed check should always be last
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedAttribute(this, paramGroup, name, paramGroup.Id.RawValue, rawValue));
                }
            }

            var parameterGroupsTag = context.ProtocolModel?.Protocol?.ParameterGroups;

            // Check duplicated names.
            results.AddRange(GenericTests.CheckDuplicates(
                items: parameterGroups,
                getDuplicationIdentifier: x => x.Name?.Value,
                getId: x => x.Id?.RawValue,
                generateSubResult: x => Error.DuplicatedValue(this, x.item, x.item.Name, x.duplicateValue, x.id),
                generateSummaryResult: x => Error.DuplicatedValue(this, parameterGroupsTag, parameterGroupsTag, x.duplicateValue, String.Join(", ", x.ids)).WithSubResults(x.subResults)
                ));

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    {
                        if (context.Protocol?.ParameterGroups != null)
                        {
                            var readNode = (IParameterGroupsGroup)context.Result.ReferenceNode;
                            var editNode = context.Protocol.ParameterGroups.Get(readNode);

                            editNode.Name.Value = readNode.Name.Value.Trim();
                            result.Success = true;
                        }

                        break;
                    }

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IParameterGroupsGroup oldGroup, IParameterGroupsGroup newGroup) in context.EachMatchingParameterGroup())
            {
                uint? groupId = newGroup.Id?.Value;
                if (groupId == null)
                {
                    continue;
                }

                string oldName = oldGroup.Name?.Value;
                string newName = newGroup.Name?.Value;

                if (oldName != newName)
                {
                    results.Add(ErrorCompare.DcfParameterGroupNameChanged(newGroup, newGroup, Convert.ToString(groupId), oldName, newName));
                }
            }

            return results;
        }
    }
}