namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.HTTP.Session.Connection.Request.Headers.CheckHeaders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckHeaders, Category.HTTP)]
    internal class CheckHeaders : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            ValidateHelper helper = new ValidateHelper(this, results);
            foreach (IHTTPSession session in context.EachHttpSessionWithValidId())
            {
                foreach (IHTTPSessionConnection connection in context.EachHttpConnectionWithValidId(session))
                {
                    var request = connection.Request;

                    if (request?.Headers == null)
                    {
                        continue;
                    }

                    Dictionary<string, List<IHttpRequestHeadersHeader>> headers = new Dictionary<string, List<IHttpRequestHeadersHeader>>(StringComparer.OrdinalIgnoreCase);

                    foreach (var header in request.Headers)
                    {
                        string headerKey = header?.Key?.Value;

                        if (String.IsNullOrWhiteSpace(headerKey))
                        {
                            continue;
                        }

                        if (!headers.TryGetValue(headerKey, out List<IHttpRequestHeadersHeader> list))
                        {
                            list = new List<IHttpRequestHeadersHeader>();
                            headers.Add(headerKey, list);
                        }

                        list.Add(header);
                    }

                    var verb = request.Verb?.Value;
                    helper.ValidateHeaders(session, connection, verb, headers, request.Headers);
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();
            var extraData = context.Result.ExtraData;

            switch (context.Result.ErrorId)
            {
                case ErrorIds.DuplicateHeaderKeys:
                    {
                        var session = (IHTTPSession)extraData[ExtraData.Session];
                        var connection = (IHTTPSessionConnection)extraData[ExtraData.Connection];
                        var headerKey = (string)extraData[ExtraData.HeaderKey];

                        Skyline.DataMiner.CICD.Models.Protocol.Edit.HTTPSession editSession = context.Protocol.HTTP.Get(session);
                        var editConnection = editSession.Get(connection);
                        Skyline.DataMiner.CICD.Models.Protocol.Edit.HttpRequestHeaders editHeaders = editConnection.Request.Headers;

                        List<string> values = new List<string>();

                        var mergedHeader = editHeaders.FirstOrDefault(h => h?.Key?.Value?.Equals(headerKey) ?? false);

                        if (mergedHeader != null)
                        {
                            for (int i = editHeaders.Count - 1; i >= 0; i--)
                            {
                                var editHeader = editHeaders[i];
                                var editHeaderKey = editHeader?.Key?.Value;

                                if (editHeaderKey != null && editHeaderKey.Equals(headerKey))
                                {
                                    // Do not keep duplicate values.
                                    if (!values.Contains(editHeader.Value))
                                    {
                                        values.Add(editHeader.Value);
                                    }

                                    if (editHeader != mergedHeader)
                                    {
                                        editHeaders.Remove(editHeader);
                                    }
                                }
                            }

                            values.Reverse();
                            mergedHeader.Value = String.Join(", ", values);
                        }

                        result.Success = true;
                        break;
                    }

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
        Connection,
        HeaderKey
    }

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly List<IValidationResult> results;

        public ValidateHelper(IValidate test, List<IValidationResult> results)
        {
            this.test = test;
            this.results = results;
        }

        public void ValidateHeaders(IHTTPSession session, IHTTPSessionConnection connection, EnumHttpRequestVerb? verb, Dictionary<string, List<IHttpRequestHeadersHeader>> headers, IHttpRequestHeaders headersNode)
        {
            string connectionId = connection.Id.RawValue;
            string sessionId = session.Id.RawValue;

            if (verb != null && (verb.Value == EnumHttpRequestVerb.POST || verb.Value == EnumHttpRequestVerb.PUT) && !headers.ContainsKey("Content-Type"))
            {
                results.Add(Error.MissingHeaderForVerb(test, headersNode, headersNode, "Content-Type", verb.Value.ToString(), sessionId.ToString(), connectionId.ToString()));
            }

            foreach (string headerKey in headers.Keys)
            {
                if (headerKey.Equals("Set-Cookie") || headers[headerKey].Count <= 1)
                {
                    continue;
                }


                IValidationResult duplicateHeaderKeys = Error.DuplicateHeaderKeys(test, headersNode, headersNode, headerKey, sessionId.ToString(), connectionId.ToString(), hasCodeFix: true);

                foreach (var header in headers[headerKey])
                {
                    IValidationResult subResult = Error.DuplicateHeaderKeys(test, header, header, headerKey, sessionId.ToString(), connectionId.ToString(), hasCodeFix: false);
                    duplicateHeaderKeys.WithSubResults(subResult);
                }

                duplicateHeaderKeys.WithExtraData(ExtraData.Session, session)
                                   .WithExtraData(ExtraData.Connection, connection)
                                   .WithExtraData(ExtraData.HeaderKey, headerKey);

                results.Add(duplicateHeaderKeys);
            }
        }

    }
}