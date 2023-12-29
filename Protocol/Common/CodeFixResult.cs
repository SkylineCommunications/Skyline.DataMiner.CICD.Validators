namespace Skyline.DataMiner.CICD.Validators.Protocol.Common
{
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    internal class CodeFixResult : ICodeFixResult
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }
}