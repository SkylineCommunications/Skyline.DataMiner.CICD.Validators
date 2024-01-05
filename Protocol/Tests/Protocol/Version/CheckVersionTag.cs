namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Version.CheckVersionTag
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckVersionTag, Category.Protocol)]
    internal class CheckVersionTag : IValidate, ICodeFix
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var protocol = context?.ProtocolModel?.Protocol;
            if (protocol == null)
            {
                return results;
            }

            var version = protocol.Version;
            (GenericStatus status, string _, string _) = GenericTests.CheckBasics(version, true);

            // Missing
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingTag(this, protocol, protocol));
                return results;
            }

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyTag(this, version, version));
                return results;
            }

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedTag(this, version, version, version.RawValue));
                return results;
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.MissingTag:
                case ErrorIds.EmptyTag:
                    {
                        // If still not there or empty
                        if (String.IsNullOrWhiteSpace(context.Protocol.Version?.Value))
                        {
                            const string Version = "X.X.X.X";
                            if (context.Protocol.Version == null)
                            {
                                context.Protocol.Version = new Skyline.DataMiner.CICD.Models.Protocol.Edit.ElementValue<string>(Version);
                            }
                            else
                            {
                                context.Protocol.Version.Value = Version;
                            }

                            result.Success = true;
                        }

                        break;
                    }

                case ErrorIds.UntrimmedTag:
                    {
                        if (context.Protocol.Version != null)
                        {
                            context.Protocol.Version.Value = context.Protocol.Version.Value.Trim();
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
    }
}