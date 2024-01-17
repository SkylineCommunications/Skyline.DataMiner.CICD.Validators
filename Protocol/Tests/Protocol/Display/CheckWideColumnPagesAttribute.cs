namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Display.CheckWideColumnPagesAttribute
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckWideColumnPagesAttribute, Category.Protocol)]
    internal class CheckWideColumnPagesAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (context?.ProtocolModel?.Protocol?.Display?.WideColumnPages == null)
            {
                return results;
            }

            ValidateHelper helper = new ValidateHelper(this, context, results);
            helper.Validate();

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    {
                        List<string> names = new List<string>();
                        foreach (string pageName in context.Protocol.Display.WideColumnPages.RawValue.Split(';'))
                        {
                            names.Add(pageName.Trim());
                        }

                        context.Protocol.Display.WideColumnPages.Value = String.Join(";", names);
                        result.Success = true;

                        break;
                    }
                case ErrorIds.EmptyAttribute:
                    {


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

    internal class ValidateHelper : ValidateHelperBase
    {
        private readonly IProtocol protocol;
        private readonly IDisplay displayElement;
        private readonly IValueTag<string> wideColumnPages;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results)
            : base(test, context, results)
        {
            protocol = context.ProtocolModel.Protocol;
            displayElement = protocol.Display;
            wideColumnPages = displayElement.WideColumnPages;
        }

        public void Validate()
        {
            (GenericStatus status, string _, string _) = GenericTests.CheckBasics(wideColumnPages, isRequired: false);

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                // TODO: Disabled as long as the Display Editor automatically adds the empty attribute.
                ////results.Add(Error.EmptyAttribute(test, displayElement, displayElement));
                return;
            }

            var pages = protocol.GetPages(context.ProtocolModel.RelationManager);
            List<IValidationResult> untrimmedSubResults = new List<IValidationResult>();

            foreach (string pageName in wideColumnPages.RawValue.Split(';'))
            {
                (GenericStatus pageNameStatus, string trimmedPageName) = GenericTests.CheckBasics<string>(pageName);

                Page page = pages.FirstOrDefault(p => p.Name == trimmedPageName);
                if (page == null)
                {
                    // Non-existing page.
                    results.Add(Error.UnexistingPage(test, displayElement, wideColumnPages, pageName));
                    continue;
                }

                // Untrimmed
                if (pageNameStatus.HasFlag(GenericStatus.Untrimmed))
                {
                    untrimmedSubResults.Add(Error.UntrimmedAttribute(test, displayElement, wideColumnPages, pageName));
                }
            }

            if (untrimmedSubResults.Count > 0)
            {
                var error = Error.UntrimmedAttribute(test, displayElement, wideColumnPages, wideColumnPages.RawValue)
                                 .WithSubResults(untrimmedSubResults.ToArray());

                results.Add(error);
            }
        }
    }
}