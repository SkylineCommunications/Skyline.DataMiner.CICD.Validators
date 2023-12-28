namespace SLDisValidator2.Tests.Protocol.Params.Param.Alarm.CheckAlarmTag
{
    using System;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    public static class Error
    {
        public static IValidationResult MissingDefaultThreshold(IValidate test, IReadable referenceNode, IReadable positionNode, string pid)
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

    public static class ErrorIds
    {
        public const uint MissingDefaultThreshold = 1;
    }

    public static class CheckId
    {
        public const uint CheckAlarmTag = 5;
    }
}