namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CheckDllImportAttribute
{
    //[Test(CheckId.CheckDllImportAttribute, Category.QAction)]
    internal class CheckDllImportAttribute /*: IValidate, ICodeFix, ICompare*/
    {
        /*
         * !! WARNING !!
         * Don't implement errors in here as this check is not applicable for Protocol solutions, so within DIS these errors are not visible.
         * However, on Jenkins these errors are visible due to validating the 'compiled' XML and will cause the build to fail.
         */

        ////public List<IValidationResult> Validate(ValidatorContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}

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