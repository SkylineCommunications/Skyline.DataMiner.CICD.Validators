namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.HTTP.Session.Connection.Request.Headers.Header.CheckKeyAttribute
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckKeyAttribute, Category.HTTP)]
    internal class CheckKeyAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            ValidateHelper helper = new ValidateHelper(this, context, results);
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
                        helper.ValidateHeader(session, connection, request.Verb?.Value, header);
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
                case ErrorIds.UntrimmedHeaderKey:
                    {
                        var extraData = context.Result.ExtraData;

                        var session = (IHTTPSession)extraData[ExtraData.Session];
                        var connection = (IHTTPSessionConnection)extraData[ExtraData.Connection];
                        var header = (IHttpRequestHeadersHeader)context.Result.ReferenceNode;

                        Skyline.DataMiner.CICD.Models.Protocol.Edit.HTTPSession editSession = context.Protocol.HTTP.Get(session);
                        var editConnection = editSession.Get(connection);
                        var editHeader = editConnection.Request.Headers.Get(header);

                        editHeader.Key.Value = header.Key.Value.Trim();

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
        Connection
    }

    internal class ValidateHelper : ValidateHelperBase
    {
        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results) : base(test, context, results)
        {
        }

        public void ValidateHeader(IHTTPSession session, IHTTPSessionConnection connection, EnumHttpRequestVerb? verb, IHttpRequestHeadersHeader header)
        {
            var headerKey = header.Key;
            (GenericStatus status, string _, string _) = GenericTests.CheckBasics(headerKey, isRequired: true);

            // Missing
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingKeyAttribute(test, header, header, session.Id.RawValue, connection.Id.RawValue));
                return;
            }

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyKeyAttribute(test, header, header, session.Id.RawValue, connection.Id.RawValue));
                return;
            }

            if (EnumHttpRequestHeaderConverter.Convert(headerKey.Value) == null &&
                EnumHttpHeaderConverter.Convert(headerKey.Value) == null)
            {
                // Invalid header key.
                results.Add(Error.UnknownHeaderKey(test, header, header, headerKey.RawValue, session.Id.RawValue, connection.Id.RawValue));
            }
            else
            {
                if (HttpHelper.IsRedundantHttpRequestHeader(headerKey.Value))
                {
                    // Redundant header key.
                    results.Add(Error.RedundantHeaderKey(test, header, header, headerKey.RawValue, session.Id.RawValue, connection.Id.RawValue));
                }
                else
                {
                    if (verb != null)
                    {
                        var requestHeader = EnumHttpRequestHeaderConverter.Convert(headerKey.Value);

                        switch (verb.Value)
                        {
                            case EnumHttpRequestVerb.GET:
                            case EnumHttpRequestVerb.HEAD:
                                {
                                    if (requestHeader != null &&
                                        (requestHeader.Value == EnumHttpRequestHeader.ContentLength
                                        || requestHeader.Value == EnumHttpRequestHeader.ContentType
                                        || requestHeader.Value == EnumHttpRequestHeader.TransferEncoding))
                                    {
                                        // Invalid header key for verb.
                                        results.Add(Error.InvalidHeaderKeyForVerb(test, header, header, headerKey.RawValue, verb.ToString(), session.Id.RawValue, connection.Id.RawValue));
                                    }
                                }
                                break;
                            case EnumHttpRequestVerb.POST:
                            case EnumHttpRequestVerb.PUT:
                                {
                                    if (requestHeader == EnumHttpRequestHeader.ContentLength)
                                    {
                                        results.Add(Error.UnsupportedHeaderKey(test, header, header, Certainty.Certain, headerKey.RawValue, session.Id.RawValue, connection.Id.RawValue));
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            if (headerKey.Value.Equals("Connection", StringComparison.OrdinalIgnoreCase) && header.Value.Equals("Keep-Alive", StringComparison.OrdinalIgnoreCase))
            {
                results.Add(Error.RedundantHeaderKey(test, header, header, "Connection", session.Id.RawValue, connection.Id.RawValue));
            }
            else if (headerKey.Value.Equals("Accept-Encoding", StringComparison.OrdinalIgnoreCase) && header.Value != null)
            {
                string[] acceptedEncodings = header.Value.Split(',');
                bool unsupportedEncodingFound = false;

                foreach (string acceptedEncoding in acceptedEncodings)
                {
                    if (!acceptedEncoding.Equals("identity") && !acceptedEncoding.Equals("gzip") && !acceptedEncoding.Equals("deflate"))
                    {
                        unsupportedEncodingFound = true;
                        break;
                    }
                }

                if (unsupportedEncodingFound)
                {
                    IValidationResult unsupportedHeaderKey = Error.UnsupportedHeaderKey(test, header, header, Certainty.Uncertain, "Accept-Encoding", session.Id.RawValue, connection.Id.RawValue);
                    unsupportedHeaderKey.WithExtraData(ExtraData.Session, session)
                                        .WithExtraData(ExtraData.Connection, connection);
                    results.Add(unsupportedHeaderKey);
                }
            }

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                IValidationResult untrimmedHeaderKey = Error.UntrimmedHeaderKey(test, header, header, headerKey.RawValue, session.Id.RawValue, connection.Id.RawValue);
                untrimmedHeaderKey.WithExtraData(ExtraData.Session, session)
                                  .WithExtraData(ExtraData.Connection, connection);
                results.Add(untrimmedHeaderKey);
            }
        }
    }
}