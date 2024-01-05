namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ExportRules.ExportRule.CheckTableAttribute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckTableAttribute, Category.ExportRule)]
    internal class CheckTableAttribute : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var exportRules = context?.ProtocolModel?.Protocol?.ExportRules;
            if (exportRules == null || !exportRules.Any())
            {
                return results;
            }

            foreach (var exportRule in exportRules)
            {
                (GenericStatus status, string rawValue, string value) = GenericTests.CheckBasics(exportRule.Table, isRequired: true);

                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingAttribute(this, exportRule, exportRule));
                    continue;
                }

                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, exportRule, exportRule));
                    continue;
                }

                if (value == "*")
                {
                    continue;
                }

                if (!UInt32.TryParse(value, out uint _))
                {
                    results.Add(Error.InvalidAttribute(this, exportRule, exportRule, value));
                    continue;
                }

                if (!context.ProtocolModel.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, rawValue, out _))
                {
                    results.Add(Error.NonExistingId(this, exportRule, exportRule, value));
                    continue;
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
        ////            result.Message = String.Format("This error ({0}) isn't implemented.", context.Result.ErrorId.ToString());
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