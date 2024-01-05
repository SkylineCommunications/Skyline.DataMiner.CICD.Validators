namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Display.Pages.Page.Visibility.CheckOverridePidAttribute
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckOverridePidAttribute, Category.Protocol)]
    internal class CheckOverridePidAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (context.ProtocolModel.Protocol?.Display?.Pages == null)
            {
                return results;
            }

            foreach (var page in context.ProtocolModel.Protocol.Display.Pages)
            {
                if (page.Visibility == null)
                {
                    continue;
                }

                ValidateHelper helper = new ValidateHelper(this, context, results, page);
                helper.Validate();
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (!(context.Result.ReferenceNode is IDisplayPagesPage readPage))
            {
                result.Message = $"{nameof(readPage)} is null.";
                return result;
            }

            var editPage = context.Protocol?.Display?.Pages?.Get(readPage);
            if (editPage == null)
            {
                result.Message = $"{nameof(editPage)} is null.";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    editPage.Visibility.OverridePID = readPage.Visibility.OverridePID.Value;
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

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly ValidatorContext context;
        private readonly List<IValidationResult> results;

        private readonly IDisplayPagesPage page;
        private readonly IDisplayPagesPageVisibility visibility;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IDisplayPagesPage page)
        {
            this.test = test;
            this.context = context;
            this.results = results;

            this.page = page;
            this.visibility = page.Visibility;
        }

        public void Validate()
        {
            (GenericStatus status, _, _) = GenericTests.CheckBasics(visibility.OverridePID, isRequired: true);

            // Missing
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingAttribute(test, page, visibility, page.Name?.RawValue));
                return;
            }

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(test, page, visibility.OverridePID, page.Name?.RawValue));
                return;
            }

            // Referenced Items
            if (!ValidateReferencedParam())
            {
                return;
            }

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(test, page, page.Visibility.OverridePID, page.Name?.RawValue, visibility.OverridePID.RawValue));
            }
        }

        private bool ValidateReferencedParam()
        {
            // Non Existing
            string pid = visibility.OverridePID.RawValue.Trim();
            if (!context.ProtocolModel.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, pid, out IParamsParam referencedParam))
            {
                results.Add(Error.NonExistingParam(test, page, visibility.OverridePID, visibility.OverridePID.RawValue, page.Name?.RawValue));
                return false;
            }

            // RTDisplay
            context.CrossData.RtDisplay.AddParam(referencedParam, Error.ReferencedParamExpectingRTDisplay(test, page, visibility.OverridePID, referencedParam.Id.RawValue, page.Name?.RawValue));

            return true;
        }
    }
}