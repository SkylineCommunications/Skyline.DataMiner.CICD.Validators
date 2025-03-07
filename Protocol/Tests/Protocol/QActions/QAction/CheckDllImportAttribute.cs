namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CheckDllImportAttribute
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckDllImportAttribute, Category.QAction)]
    internal class CheckDllImportAttribute : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IQActionsQAction qAction in context.EachQActionWithValidId())
            {
                if (qAction.DllImport?.Value == null)
                {
                    continue;
                }

                var dllImports = qAction.DllImport.Value;

                var dlls = dllImports.Split(';');

                foreach (string dll in dlls)
                {
                    if (QActionHelper.DeprecatedDlls.Contains(dll, StringComparer.InvariantCultureIgnoreCase))
                    {
                        results.Add(Error.DeprecatedDll(this, qAction, qAction, dll, qAction.Id.RawValue));
                    }
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