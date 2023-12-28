namespace SLDisValidator2.Tests.Protocol.Display.CheckDefaultPageAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Generic;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckDefaultPageAttribute, Category.Protocol)]
    public class CheckDefaultPageAttribute : IValidate, ICodeFix
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var protocolElement = context?.ProtocolModel?.Protocol;
            if (protocolElement == null)
            {
                return results;
            }

            var displayElement = protocolElement.Display;
            if (displayElement == null)
            {
                return results;
            }

            var defaultPage = displayElement.DefaultPage;
            (GenericStatus status, string _, string _) = GenericTests.CheckBasics(defaultPage, isRequired: true);

            // Missing
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingAttribute(this, displayElement, displayElement));
                return results;
            }

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                // Empty attribute.
                results.Add(Error.EmptyAttribute(this, displayElement, displayElement));
                return results;
            }

            var pages = protocolElement.GetPages(context.ProtocolModel.RelationManager);

            bool isExistingPage = false;
            bool isPopupPage = false;

            // Non-existing page.
            foreach (var page in pages)
            {
                if (!String.Equals(page.Name, defaultPage.Value))
                {
                    continue;
                }

                isExistingPage = true;

                if (page.IsPopup)
                {
                    isPopupPage = true;
                }

                break;
            }

            if (!isExistingPage)
            {
                // Non-existing page.
                results.Add(Error.UnexistingPage(this, displayElement, displayElement, defaultPage.RawValue));
            }
            else
            {
                if (isPopupPage)
                {
                    results.Add(Error.UnsupportedPage(this, displayElement, displayElement, defaultPage.RawValue));
                }
                else
                {
                    if (defaultPage.Value != "General")
                    {
                        results.Add(Error.InvalidDefaultPage(this, displayElement, displayElement));
                    }
                }
            }

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(this, displayElement, displayElement, defaultPage.RawValue));
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();
            
            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    {
                        context.Protocol.Display.DefaultPage.Value = context.Protocol.Read.Display.DefaultPage.Value.Trim();
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