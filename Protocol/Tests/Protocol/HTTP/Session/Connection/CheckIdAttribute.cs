namespace SLDisValidator2.Tests.Protocol.HTTP.Session.Connection.CheckIdAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Generic;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckIdAttribute, Category.HTTP)]
    internal class CheckIdAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IHTTPSession session in context.EachHttpSessionWithValidId())
            {
                foreach (var connection in session)
                {
                    (GenericStatus status, string rawId, uint? _) = GenericTests.CheckBasics(connection.Id, isRequired: true);

                    // Missing
                    if (status.HasFlag(GenericStatus.Missing))
                    {
                        results.Add(Error.MissingAttribute(this, connection, connection, session.Id.RawValue));
                        continue;
                    }

                    // Empty
                    if (status.HasFlag(GenericStatus.Empty))
                    {
                        results.Add(Error.EmptyAttribute(this, connection, connection, session.Id.RawValue));
                        continue;
                    }

                    // Invalid
                    if (status.HasFlag(GenericStatus.Invalid) || !GenericTests.IsPlainNumbers(rawId))
                    {
                        results.Add(Error.InvalidValue(this, connection, connection, rawId, session.Id.RawValue));
                        continue;
                    }

                    // Untrimmed
                    if (status.HasFlag(GenericStatus.Untrimmed))
                    {
                        results.Add(Error.UntrimmedAttribute(this, connection, connection, session.Id.RawValue, rawId));
                        continue;
                    }
                }

                // Duplicate
                var duplicateResults = GenericTests.CheckDuplicateIds(
                    items: context.EachHttpConnectionWithValidId(session),
                    getDuplicationIdentifier: x => (x.Id?.Value),
                    getName: x => (x.Name?.RawValue),
                    generateSubResult: x => Error.DuplicatedId(this, x.item, x.item, x.id, x.name, session.Id.RawValue),
                    generateSummaryResult: x => Error.DuplicatedId(this, null, null, x.id, String.Join(", ", x.names), session.Id.RawValue).WithSubResults(x.subResults)
                    );

                results.AddRange(duplicateResults);
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    if (!(context.Result.ReferenceNode is IHTTPSessionConnection readConnection))
                    {
                        break;
                    }

                    if (context.Protocol?.HTTP == null)
                    {
                        break;
                    }

                    foreach (var session in context.Protocol.HTTP)
                    {
                        var editConnection = session.Get(readConnection);
                        if (editConnection == null)
                        {
                            continue;
                        }

                        var idAttribute = editConnection.EditNode.Attribute["id"];
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

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }
}