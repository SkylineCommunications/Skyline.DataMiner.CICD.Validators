namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ExportRules.ExportRule.CheckWhereTagAttribute
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckWhereTagAttribute, Category.ExportRule)]
    internal class CheckWhereTagAttribute : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var exportRules = context?.ProtocolModel?.Protocol?.ExportRules;
            if (exportRules == null || !exportRules.Any())
            {
                return results;
            }

            foreach (IExportRulesExportRule exportRule in exportRules)
            {
                // Both of these attributes don't mean anything without the whereTag.
                bool isRequired = exportRule.WhereValue != null || exportRule.WhereAttribute != null;

                (GenericStatus status, string rawValue, _) = GenericTests.CheckBasics(exportRule.WhereTag, isRequired);

                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingAttribute(this, exportRule, exportRule));
                    continue;
                }

                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, exportRule, exportRule.WhereTag));
                    continue;
                }

                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedAttribute(this, exportRule, exportRule.WhereTag, rawValue));
                }
            }

            return results;
        }

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {

        ////        default:
        ////            result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
        ////            break;
        ////    }

        ////    return result;
        ////}

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }
}