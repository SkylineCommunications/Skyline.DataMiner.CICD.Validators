namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Groups.Group.CheckIdAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckIdAttribute, Category.Group)]
    internal class CheckIdAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (context?.ProtocolModel?.Protocol?.Groups == null)
            {
                return results;
            }

            foreach (var group in context.ProtocolModel.Protocol.Groups)
            {
                (GenericStatus status, string rawId, uint? _) = GenericTests.CheckBasics(group.Id, isRequired: true);

                // Missing
                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingAttribute(this, group, group));
                    continue;
                }

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, group, group));
                    continue;
                }

                // Ping Group
                bool isPingGroup = rawId.Trim() == "-1";
                if (isPingGroup)
                {
                    var connections = context.ProtocolModel.Protocol.GetConnections();
                    var mainConnection = connections != null && connections.Count > 0 ? connections[0] : null;

                    if (mainConnection == null || mainConnection.Type == EnumProtocolType.Virtual)
                    {
                        // No ping group should be implemented for virtual protocols
                        results.Add(Error.InvalidValue(this, group, group, rawId, group.Name?.RawValue));
                    }
                    //else if (mainConnection.Type != EnumProtocolType.Http)
                    //{
                    //    // There are other preferred syntaxes than using Group @id = "-1" in order to define the ping group.
                    //    // Such syntax doesn't exist (yet) for HTTP connections.
                    //    // TODO: double checked what's the best way to define a ping group for each connection type.
                    //    // Potentially, a better way could be to only keep the if as is in the below InvalidValue check,
                    //    //  get rid of this PingGroup code and make a dedicated check for ping groups.
                    //    results.Add(Error.ObsoletePingGroupSyntax(this, group, group, rawId, group.Name?.RawValue));
                    //}

                    continue;
                }

                // Invalid
                if (status.HasFlag(GenericStatus.Invalid) || !GenericTests.IsPlainNumbers(rawId))
                {
                    results.Add(Error.InvalidValue(this, group, group, rawId, group.Name?.RawValue));
                    continue;
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedAttribute(this, group, group, rawId));
                    continue;
                }
            }

            // Duplicate
            var duplicateResults = GenericTests.CheckDuplicateIds(
                items: context.EachGroupWithValidId(),
                getDuplicationIdentifier: x => (x.Id?.Value),
                getName: x => (x.Name?.RawValue),
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
                    if (!(context.Result.ReferenceNode is IGroupsGroup readGroup))
                    {
                        break;
                    }

                    var editGroup = context.Protocol?.Groups?.Get(readGroup);

                    var idAttribute = editGroup?.EditNode.Attribute["id"];
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