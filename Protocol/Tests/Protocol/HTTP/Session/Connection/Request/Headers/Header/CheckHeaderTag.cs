namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.HTTP.Session.Connection.Request.Headers.Header.CheckHeaderTag
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckHeaderTag, Category.HTTP)]
    internal class CheckHeaderTag : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IHTTPSession session in context.EachHttpSessionWithValidId())
            {
                foreach (IHTTPSessionConnection connection in context.EachHttpConnectionWithValidId(session))
                {
                    var request = connection.Request;

                    if (request?.Headers == null)
                    {
                        continue;
                    }

                    foreach (var header in request.Headers)
                    {
                        (GenericStatus status, string _, string _) = GenericTests.CheckBasics(header, isRequired: false);

                        // Untrimmed
                        if (status.HasFlag(GenericStatus.Untrimmed))
                        {
                            IValidationResult untrimmedTag = Error.UntrimmedTag(this, header, header, header.RawValue);
                            untrimmedTag.WithExtraData(ExtraData.Session, session)
                                        .WithExtraData(ExtraData.Connection, connection);
                            results.Add(untrimmedTag);
                        }
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
                    {
                        var extraData = context.Result.ExtraData;

                        var session = (IHTTPSession)extraData[ExtraData.Session];
                        var connection = (IHTTPSessionConnection)extraData[ExtraData.Connection];
                        var header = (IHttpRequestHeadersHeader)context.Result.ReferenceNode;

                        Skyline.DataMiner.CICD.Models.Protocol.Edit.HTTPSession editSession = context.Protocol.HTTP.Get(session);
                        var editConnection = editSession.Get(connection);
                        var editHeader = editConnection.Request.Headers.Get(header);

                        editHeader.Value = header.Value.Trim();

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

    internal enum ExtraData
    {
        Session,
        Connection
    }
}