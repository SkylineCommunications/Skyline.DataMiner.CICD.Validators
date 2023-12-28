namespace SLDisValidator2.Common
{
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    public class CodeFixResult : ICodeFixResult
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }
}