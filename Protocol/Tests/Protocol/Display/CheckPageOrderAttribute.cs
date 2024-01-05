namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Display.CheckPageOrderAttribute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckPageOrderAttribute, Category.Protocol)]
    internal class CheckPageOrderAttribute : IValidate, ICodeFix
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            IDisplay displayElement = context?.ProtocolModel?.Protocol?.Display;
            if (displayElement == null)
            {
                return results;
            }

            var pageOrder = displayElement.PageOrder;
            (GenericStatus status, string _, string _) = GenericTests.CheckBasics(pageOrder, isRequired: true);

            // Missing
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingAttribute(this, displayElement, displayElement));
                return results;
            }

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(this, displayElement, displayElement));
                return results;
            }

            ValidateHelper helper = new ValidateHelper(this, context, results, displayElement);
            helper.CheckUnexistingOrUnsupportedPages();
            helper.CheckMissingPages();
            helper.CheckDuplicatePages();
            helper.CheckWebPageRequired();
            helper.CheckWebPagePositions();
            helper.CheckWebPageURL();

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(this, displayElement, pageOrder, pageOrder.RawValue));
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();
            switch (context.Result.ErrorId)
            {
                case ErrorIds.UnsupportedPage:
                    {
                        var extraData = context.Result.ExtraData;

                        if (extraData != null && extraData.TryGetValue(ExtraData.PageName, out object oPageName))
                        {
                            string pageName = Convert.ToString(oPageName);

                            string originalValue = context.Protocol.Read.Display.PageOrder.Value;
                            string[] originalValueParts = originalValue.Split(';');

                            List<string> fixedValueParts = new List<string>(originalValueParts.Length);

                            foreach (var part in originalValueParts)
                            {
                                if (!String.Equals(part, pageName, StringComparison.OrdinalIgnoreCase))
                                {
                                    fixedValueParts.Add(part);
                                }
                            }

                            string fixedValue = String.Join(";", fixedValueParts);
                            context.Protocol.Display.PageOrder.Value = fixedValue;
                        }

                        result.Success = true;
                        break;
                    }

                case ErrorIds.DuplicateEntries:
                    {
                        var extraData = context.Result.ExtraData;

                        if (extraData != null && extraData.TryGetValue(ExtraData.PageName, out object oPageName))
                        {
                            string pageName = Convert.ToString(oPageName);

                            string originalValue = context.Protocol.Read.Display.PageOrder.Value;
                            string[] originalValueParts = originalValue.Split(';');

                            List<string> fixedValueParts = new List<string>(originalValueParts.Length);

                            bool firstOccurrence = true;

                            foreach (var part in originalValueParts)
                            {
                                if (String.Equals(part, pageName, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (firstOccurrence)
                                    {
                                        fixedValueParts.Add(part);
                                        firstOccurrence = false;
                                    }
                                }
                                else
                                {
                                    fixedValueParts.Add(part);
                                }
                            }

                            string fixedValue = String.Join(";", fixedValueParts);

                            context.Protocol.Display.PageOrder.Value = fixedValue;
                        }

                        result.Success = true;
                        break;
                    }

                case ErrorIds.UntrimmedAttribute:
                    {
                        context.Protocol.Display.PageOrder.Value = context.Protocol.Read.Display.PageOrder.Value.Trim();
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

    internal enum ExtraData
    {
        PageName
    }

    /// <summary>
    /// Specifies the page type.
    /// </summary>
    internal enum PageType
    {
        Unknown,
        Regular,
        Separator,
        WebInterface
    }

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly ValidatorContext context;
        private readonly List<IValidationResult> results;
        private readonly IDisplay displayElement;

        private readonly IProtocolModel model;
        private readonly IProtocol protocol;
        private readonly IReadOnlyList<Page> protocolPages;
        private readonly HashSet<string> protocolPagesSet;

        private readonly IValueTag<string> pageOrderAttr;
        private readonly string[] pageOrderPages;

        internal ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IDisplay displayElement)
        {
            this.test = test;
            this.context = context;
            this.results = results;
            this.displayElement = displayElement;

            model = context.ProtocolModel;
            protocol = model.Protocol;
            protocolPages = protocol.GetPages(model.RelationManager);
            protocolPagesSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            BuildProtocolPageNamesSet();

            pageOrderAttr = displayElement.PageOrder;
            pageOrderPages = pageOrderAttr.Value.Split(';');
        }

        internal void CheckUnexistingOrUnsupportedPages()
        {
            foreach (string pageName in pageOrderPages)
            {
                PageType pageType = GetPageType(pageName, protocolPagesSet);

                if (pageType == PageType.Separator || pageType == PageType.WebInterface)
                {
                    continue;
                }

                Page page = protocolPages.FirstOrDefault(p => p.Name == pageName);

                if (page == null)
                {
                    // Non-existing page.
                    results.Add(Error.UnexistingPage(test, displayElement, displayElement, pageName));
                    continue;
                }

                if (page.IsPopup)
                {
                    // Unsupported page.
                    IValidationResult unsupportedPage = Error.UnsupportedPage(test, displayElement, displayElement, page.Name);
                    unsupportedPage.WithExtraData(ExtraData.PageName, page.Name);
                    results.Add(unsupportedPage);
                }
            }
        }

        internal void CheckMissingPages()
        {
            HashSet<string> mentionedPages = new HashSet<string>();

            foreach (string page in pageOrderPages)
            {
                PageType pageType = GetPageType(page, protocolPagesSet);

                if (pageType == PageType.Separator || pageType == PageType.WebInterface)
                {
                    continue;
                }

                mentionedPages.Add(page);
            }

            foreach (var page in protocolPages)
            {
                if (page.IsPopup || mentionedPages.Contains(page.Name))
                {
                    continue;
                }

                IValidationResult missingPage = Error.MissingPage(test, displayElement, displayElement, page.Name);
                foreach (var param in page.Params)
                {
                    missingPage.WithSubResults(Error.MissingPage_Sub(test, param, param, param.Id?.RawValue, page.Name));
                }

                results.Add(missingPage);
            }
        }

        internal void CheckDuplicatePages()
        {
            HashSet<string> pageOrderPagesSet = new HashSet<string>();
            HashSet<string> duplicates = new HashSet<string>();

            foreach (string mentionedPage in pageOrderPages)
            {
                PageType pageType = GetPageType(mentionedPage, protocolPagesSet);

                if (pageType == PageType.Separator || pageType == PageType.WebInterface)
                {
                    continue;
                }

                if (!pageOrderPagesSet.Contains(mentionedPage))
                {
                    pageOrderPagesSet.Add(mentionedPage);
                }
                else
                {
                    duplicates.Add(mentionedPage);
                }
            }

            foreach (string duplicate in duplicates)
            {
                IValidationResult duplicateEntries = Error.DuplicateEntries(test, displayElement, displayElement, duplicate);
                duplicateEntries.WithExtraData(ExtraData.PageName, duplicate);
                results.Add(duplicateEntries);
            }
        }

        internal void CheckWebPageRequired()
        {
            // Has webInterface?
            foreach (string page in pageOrderPages)
            {
                PageType pageType = GetPageType(page, protocolPagesSet);
                if (pageType == PageType.WebInterface)
                {
                    // Has web interface
                    return;
                }
            }

            // Requires webInterface?
            if (protocol.GetConnections().Any(x => x.Type == EnumProtocolType.Snmp || x.Type == EnumProtocolType.Snmpv2 || x.Type == EnumProtocolType.Snmpv3))
            {
                results.Add(Error.MissingWebPage(test, displayElement, displayElement));
            }
        }

        internal void CheckWebPageURL()
        {
            foreach (string page in pageOrderPages)
            {
                PageType pageType = GetPageType(page, protocolPagesSet);
                if (pageType != PageType.WebInterface)
                {
                    continue;
                }

                string pageLowerCase = page.ToLower();
                string regexPattern = @"\[id:[0-9]*]";
                foreach (Match match in Regex.Matches(pageLowerCase, regexPattern))
                {
                    string pid = match.Value.Trim("[id:]".ToCharArray());
                    // Not sure if we need such check since no matter if its parse-able to int or not, what matters is if the param exists or not.
                    // So if we leave out the TryParse, non-numerical PIDs will also be caught by the TryGetObjectByKey below and it should become obvious to the user what to do.
                    ////if (!Int32.TryParse(pid, out _))
                    ////{
                    ////    // Invalid ID Ref ?
                    ////    continue;
                    ////}

                    if (!model.TryGetObjectByKey(Mappings.ParamsById, pid, out IParamsParam referencedParam))
                    {
                        results.Add(Error.NonExistingId(test, displayElement, pageOrderAttr, pid));
                        continue;
                    }

                    // Reference Param Requiring RTDisplay
                    IValidationResult rtDisplayError = Error.ReferencedParamRTDisplayExpected(test, displayElement, pageOrderAttr, referencedParam.Id.RawValue);
                    context.CrossData.RtDisplay.AddParam(referencedParam, rtDisplayError);
                }
            }
        }

        internal void CheckWebPagePositions()
        {
            List<(int index, string pageName)> webInterfacePages = new List<(int index, string pageName)>();

            int lastSeparatorPageIndex = -1;
            int lastRegularPageIndex = -1;

            for (int i = 0; i < pageOrderPages.Length; i++)
            {
                string page = pageOrderPages[i];
                PageType pageType = GetPageType(page, protocolPagesSet);

                switch (pageType)
                {
                    case PageType.WebInterface:
                        webInterfacePages.Add((i, page));
                        break;
                    case PageType.Separator:
                        lastSeparatorPageIndex = i;
                        break;
                    default:
                        lastRegularPageIndex = i;
                        break;
                }
            }

            bool firstWebInterfacePage = true;
            foreach ((int index, string pageName) in webInterfacePages)
            {
                if (firstWebInterfacePage)
                {
                    firstWebInterfacePage = false;

                    // First web interface page should be preceded by separator.
                    if (index == 0 || (index > 0 && GetPageType(pageOrderPages[index - 1], protocolPagesSet) != PageType.Separator))
                    {
                        results.Add(Error.WrongWebPagePosition(test, displayElement, displayElement, pageName));
                        continue;
                    }
                }

                // Web interface page should be put at the end.
                if (index < lastRegularPageIndex || index < lastSeparatorPageIndex)
                {
                    results.Add(Error.WrongWebPagePosition(test, displayElement, displayElement, pageName));
                }
            }
        }

        private static PageType GetPageType(string pageName, ICollection<string> protocolPages)
        {
            if (protocolPages.Contains(pageName))
            {
                return PageType.Regular;
            }

            if (pageName.StartsWith("---"))
            {
                return PageType.Separator;
            }

            if (pageName.Contains("#"))
            {
                return PageType.WebInterface;
            }

            return PageType.Unknown;
        }

        private void BuildProtocolPageNamesSet()
        {
            if (protocolPages == null)
            {
                return;
            }

            foreach (var page in protocolPages)
            {
                protocolPagesSet.Add(page.Name);
            }
        }
    }
}