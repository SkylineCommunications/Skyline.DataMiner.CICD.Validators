namespace SLDisValidator2.Interfaces
{
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    using SLDisValidator2.Common;

    internal interface ICodeFix : IRoot
    {
        ICodeFixResult Fix(CodeFixContext context);
    }
}