namespace SLDisValidator2.Common
{
    using System.Runtime.Serialization;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    public class CSharpValidationResult : ValidationResult, ICSharpValidationResult
    {
        [IgnoreDataMember]
        public Location CSharpLocation { get; set; }
    }
}