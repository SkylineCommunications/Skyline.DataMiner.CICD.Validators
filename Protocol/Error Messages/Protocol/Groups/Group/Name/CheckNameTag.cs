// <auto-generated>This is auto-generated code by Validator Management Tool. Do not modify.</auto-generated>
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Groups.Group.Name.CheckNameTag
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    internal static class Error
    {
        public static IValidationResult DuplicatedValue(IValidate test, IReadable referenceNode, IReadable positionNode, string duplicateName, string groupIds)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckNameTag,
                ErrorId = ErrorIds.DuplicatedValue,
                FullId = "4.1.1",
                Category = Category.Group,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Duplicated {0} {1} '{2}'. {0} IDs '{3}'.", "Group", "Name", duplicateName, groupIds),
                HowToFix = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint DuplicatedValue = 1;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckNameTag = 1;
    }
}