namespace SLDisValidator2.Interfaces
{
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    using SLDisValidator2.Common;

    public interface ICodeFix : IRoot
    {
        ICodeFixResult Fix(CodeFixContext context);
    }
}