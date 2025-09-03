namespace ProtocolTests.Protocol.Groups.Group.Content.CheckContentTag
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Groups.Group.Content.CheckContentTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckContentTag();

        #region Valid Checks

        [TestMethod]
        public void Group_CheckContentTag_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckContentTag_Valid_Only1EmptyGroup()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_Only1EmptyGroup",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Group_CheckContentTag_IncompatibleContentWithGroupType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "IncompatibleContentWithGroupType",
                ExpectedResults = new List<IValidationResult>
                {
                    // Groups of type 'action'
                    ////Error.IncompatibleContentWithGroupType(null, null, null, "action", "Action", "1"),  // Valid
                    Error.IncompatibleContentWithGroupType(null, null, null, "action", "Pair", "2"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "action", "Pair", "2"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "action", "Param", "3"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "action", "Session", "4"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "action", "Trigger", "5"),
                    
                    // Groups of type 'poll' (explicit)
                    Error.IncompatibleContentWithGroupType(null, null, null, "poll", "Action", "101"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "poll", "Action", "101"),
                    ////Error.IncompatibleContentWithGroupType(null, null, null, "poll", "Pair", "102"),    // Valid
                    ////Error.IncompatibleContentWithGroupType(null, null, null, "poll", "Param", "103"),   // Valid
                    ////Error.IncompatibleContentWithGroupType(null, null, null, "poll", "Session", "104"), // Valid
                    Error.IncompatibleContentWithGroupType(null, null, null, "poll", "Trigger", "105"),
                    
                    // Groups of type 'poll' (implicit)
                    Error.IncompatibleContentWithGroupType(null, null, null, "poll", "Action", "151"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "poll", "Action", "151"),
                    ////Error.IncompatibleContentWithGroupType(null, null, null, "poll", "Pair", "152"),    // Valid
                    ////Error.IncompatibleContentWithGroupType(null, null, null, "poll", "Param", "153"),   // Valid
                    ////Error.IncompatibleContentWithGroupType(null, null, null, "poll", "Session", "154"), // Valid
                    Error.IncompatibleContentWithGroupType(null, null, null, "poll", "Trigger", "155"),

                    // Groups of type 'poll action'
                    ////Error.IncompatibleContentWithGroupType(null, null, null, "poll action", "Action", "201"),   // Valid
                    Error.IncompatibleContentWithGroupType(null, null, null, "poll action", "Pair", "202"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "poll action", "Pair", "202"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "poll action", "Param", "203"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "poll action", "Session", "204"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "poll action", "Trigger", "205"),

                    // Groups of type 'poll trigger'
                    Error.IncompatibleContentWithGroupType(null, null, null, "poll trigger", "Action", "301"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "poll trigger", "Action", "301"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "poll trigger", "Pair", "302"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "poll trigger", "Param", "303"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "poll trigger", "Session", "304"),
                    ////Error.IncompatibleContentWithGroupType(null, null, null, "poll trigger", "Trigger", "305"), // Valid

                    // Groups of type 'trigger'
                    Error.IncompatibleContentWithGroupType(null, null, null, "trigger", "Action", "401"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "trigger", "Action", "401"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "trigger", "Pair", "402"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "trigger", "Param", "403"),
                    Error.IncompatibleContentWithGroupType(null, null, null, "trigger", "Session", "404"),
                    ////Error.IncompatibleContentWithGroupType(null, null, null, "trigger", "Trigger", "405"),  // Valid
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckContentTag_MaxItems()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MaxItems",
                ExpectedResults = new List<IValidationResult>
                {
                    // Groups of type 'action'
                    ////Error.MaxItems(null, null, null, "1"),
                    
                    // Groups of type 'poll' (implicit)
                    Error.MaxItems(null, null, null, "102"),
                    
                    // Groups of type 'poll' (explicit)
                    Error.MaxItems(null, null, null, "152"),
                    
                    // Groups of type 'poll action'
                    ////Error.MaxItems(null, null, null, "201"),
                    
                    // Groups of type 'poll trigger'
                    ////Error.MaxItems(null, null, null, "305"),
                    
                    // Groups of type 'trigger'
                    ////Error.MaxItems(null, null, null, "405"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]

        public void Group_CheckContentTag_MaxItemsMultipleGet()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MaxItemsMultipleGet",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MaxItemsMultipleGet(null, null, null, "103"),
                    Error.MaxItemsMultipleGet(null, null, null, "153"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckContentTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null, "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckContentTag_MissingTag_MultiThreadedGroups()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag_MultiThreadedGroups",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null, "1"),
                    Error.MissingTag(null, null, null, "2"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckContentTag_MixedTypes()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MixedTypes",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MixedTypes(null, null, null, "Pair;Param", "101"),
                    Error.MixedTypes(null, null, null, "Pair;Session", "102"),
                    Error.MixedTypes(null, null, null, "Pair;Param;Session", "103"),

                    Error.MixedTypes(null, null, null, "Pair;Param", "111"),
                    Error.MixedTypes(null, null, null, "Param;Session", "112"),
                    Error.MixedTypes(null, null, null, "Pair;Param;Session", "113"),

                    Error.MixedTypes(null, null, null, "Pair;Session", "121"),
                    Error.MixedTypes(null, null, null, "Param;Session", "122"),
                    Error.MixedTypes(null, null, null, "Pair;Param;Session", "123"),
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
        public void Group_CheckContentTag_MaxItems()
        {
            // Create ErrorMessage
            var message = Error.MaxItems(null, null, null, "groupId");

            var expected = new ValidationResult
            {
                ErrorId = 4,
                FullId = "4.10.4",
                Category = Category.Group,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Group contains more than 10 content elements. Group ID 'groupId'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Group_CheckContentTag_MaxItemsMultipleGet()
        {
            // Create ErrorMessage
            var message = Error.MaxItemsMultipleGet(null, null, null, "groupId");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "4.10.3",
                Category = Category.Group,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Group with 'multipleGet' true contains more than 20 content elements. Group ID 'groupId'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Group_CheckContentTag_MissingTag()
        {
            // Create ErrorMessage
            var message = Error.MissingTag(null, null, null, "groupId");

            var expected = new ValidationResult
            {
                ErrorId = 5,
                FullId = "4.10.5",
                Category = Category.Group,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Missing tag 'Content' in Group 'groupId'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Group_CheckContentTag_MixedTypes()
        {
            // Create ErrorMessage
            var message = Error.MixedTypes(null, null, null, "contentTypes", "groupId");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "4.10.2",
                Category = Category.Group,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Unsupported mixed group content 'contentTypes'. Group ID 'groupId'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckContentTag();

        [TestMethod]
        public void Group_CheckContentTag_CheckCategory() => Generic.CheckCategory(check, Category.Group);

        [TestMethod]
        public void Group_CheckContentTag_CheckId() => Generic.CheckId(check, CheckId.CheckContentTag);
    }
}