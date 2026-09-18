namespace ProtocolTests.Protocol.Actions.Action.Type.CheckTypeRunActions
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Actions.Action.Type.CheckTypeRunActions;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckTypeRunActions();

        #region Valid Checks

        [TestMethod]
        public void Action_CheckTypeRunActions_Valid()
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
        public void Action_CheckTypeRunActions_ActionParameterNotTriggeringQAction()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ActionParameterNotTriggeringQAction",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ActionParameterNotTriggeringQAction(null, null, null, "1", "1"),
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
        [Ignore]
        public void Action_CheckTypeRunActions_ActionParameterNotTriggeringQAction()
        {
            // Create ErrorMessage
            var message = Error.ActionParameterNotTriggeringQAction(null, null, null, "actionId", "paramId");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Action 'actionId' references parameter 'paramId' which does not trigger any QAction.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    [Ignore]
    public class Attribute
    {
        private readonly IRoot check = new CheckTypeRunActions();

        [TestMethod]
        public void Action_CheckTypeRunActions_CheckCategory() => Generic.CheckCategory(check, Category.Action);

        [TestMethod]
        public void Action_CheckTypeRunActions_CheckId() => Generic.CheckId(check, CheckId.CheckTypeRunActions);
    }
}