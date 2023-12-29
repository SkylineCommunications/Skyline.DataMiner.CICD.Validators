namespace Skyline.DataMiner.CICD.Validators.Protocol.Interfaces
{
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    using Skyline.DataMiner.CICD.Validators.Protocol.Common;

    internal interface ICodeFix : IRoot
    {
        ICodeFixResult Fix(CodeFixContext context);
    }
}