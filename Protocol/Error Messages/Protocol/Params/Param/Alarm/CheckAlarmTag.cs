namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Alarm.CheckAlarmTag
{
    using System;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    internal static class Error
    {
        internal static IValidationResult MissingDefaultThreshold(IValidate test, IReadable referenceNode, IReadable positionNode, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckAlarmTag,
                ErrorId = ErrorIds.MissingDefaultThreshold,
                FullId = "2.5.1",
                Category = Category.Param,
                Severity = Severity.Minor,
                Certainty = Certainty.Uncertain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "Missing default thresholds on some monitored parameters.",
                Description = String.Format("Missing default thresholds on monitored parameter. Param ID '{0}'.", pid),
                HowToFix = "",
                ExampleCode = "",
                Details = "When possible, default thresholds should be provided on monitored parameter as starting point to make things easier for a user when configuring alarm templates.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint MissingDefaultThreshold = 1;
    }

    public static class CheckId
    {
        public const uint CheckAlarmTag = 5;
    }
}