namespace SLDisValidator2.Tests.Protocol.CheckXMLDeclaration
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckXMLDeclaration, Category.Protocol)]
    internal class CheckXMLDeclaration : IValidate, ICodeFix
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            string encoding = context.Document.Declaration?.Encoding;
            if (encoding == null)
            {
                // No encoding == OK (default is UTF-8)
                return results;
            }

            if (!String.Equals(encoding, "UTF-8", StringComparison.OrdinalIgnoreCase))
            {
                var protocol = context.ProtocolModel.Protocol;
                results.Add(Error.InvalidDeclaration(this, protocol, protocol, encoding, "UTF-8"));
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.InvalidDeclaration:
                    {
                        // Change to UTF-8
                        var declaration = context.Document.Declaration;
                        if (declaration != null)
                        {
                            declaration.Encoding = "UTF-8";
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
    }
}