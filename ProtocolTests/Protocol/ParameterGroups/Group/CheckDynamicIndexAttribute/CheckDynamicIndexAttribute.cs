namespace ProtocolTests.Protocol.ParameterGroups.Group.CheckDynamicIndexAttribute
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ParameterGroups.Group.CheckDynamicIndexAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckDynamicIndexAttribute();

        #region Valid Checks

        [TestMethod]
        public void ParameterGroup_CheckDynamicIndexAttribute_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void ParameterGroup_CheckDynamicIndexAttribute_MissingDynamicIdAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingDynamicIdAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingDynamicIdAttribute(null, null, null, "1002"),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void ParameterGroup_CheckDynamicIndexAttribute_MissingDynamicIdAttribute()
        {
            // Create ErrorMessage
            var message = Error.MissingDynamicIdAttribute(null, null, null, "2");

            var expected = new ValidationResult()
            {
                ErrorId = 1,
                FullId = "16.8.1",
                Category = Category.ParameterGroup,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Filtering via 'Group@dynamicIndex' attribute requires a 'Group@dynamicId' attribute. ParameterGroup ID '2'.",
                HowToFix = "",
                ExampleCode = "",
                Details = "'Group@dynamicIndex' attribute allows to filter on Display Keys before creating dynamic DCF interfaces." + Environment.NewLine + "Such filter is applied on the table referred to via the 'Group@dynamicId' attribute." + Environment.NewLine + "This means that the presence of a 'Group@dynamicIndex' attribute while there is no 'Group@dynamicIndex' doesn't make sense.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckDynamicIndexAttribute();

        [TestMethod]
        public void ParameterGroup_CheckDynamicIndexAttribute_CheckCategory() => Generic.CheckCategory(check, Category.ParameterGroup);

        [TestMethod]
        public void ParameterGroup_CheckDynamicIndexAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckDynamicIndexAttribute);
    }
}