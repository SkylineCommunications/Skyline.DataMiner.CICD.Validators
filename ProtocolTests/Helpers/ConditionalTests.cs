namespace ProtocolTests.Helpers
{
    using System;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers.Conditions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Groups.Group.Condition.CheckConditionTag;

    [TestClass]
    public class ConditionalTests
    {
        [TestMethod]
        [DoNotParallelize]
        [DataRow("id:10 > 1", "")]
        [DataRow("id:65008 > 1", "")]
        [DataRow("(id:10 + 10) > 20", "")]
        [DataRow("((id:10 * 10) * id:11) > 20", "")]
        [DataRow("(id:12 + \"efg\") == \"defefgabc\"", "")]
        [DataRow("((id:12 + \"efg\") + \"abc\") == \"defefgabc\"", "")]
        [DataRow("((id:10 * 20) > 100) AND (\"defefgabc\" > id:12)", "")]
        [DataRow("(\"defefgabc\" > id:12) AND ((id:10 * 20) > 100)", "")]
        [DataRow("(-10 > id:12) AND (id:10 + 20 + 20 > 100)", "")]
        [DataRow("(+10.5 > id:10) AND (id:10 + 20 + 20 > 100)", "")]
        [DataRow("(id:12 == \"\")", "")]
        [DataRow("(id:12 + \"A\") == \"Inhibit\"", "")]
        public void ValidConditionsSucceed(string inputValue, string expectedOutput)
        {
            // Arrange
            var parameter10InterPreteType = new Mock<IParamsParamInterpreteType>();
            parameter10InterPreteType.Setup(p => p.Value).Returns(Skyline.DataMiner.CICD.Models.Protocol.Enums.EnumParamInterpretType.Double);
            var parameter10Interprete = new Mock<IParamsParamInterprete>();
            parameter10Interprete.Setup(p => p.Type).Returns(parameter10InterPreteType.Object);
            var parameter10 = new Mock<IParamsParam>();
            parameter10.Setup(p => p.Interprete).Returns(parameter10Interprete.Object);

            var parameter11InterPreteType = new Mock<IParamsParamInterpreteType>();
            parameter11InterPreteType.Setup(p => p.Value).Returns(Skyline.DataMiner.CICD.Models.Protocol.Enums.EnumParamInterpretType.Double);
            var parameter11Interprete = new Mock<IParamsParamInterprete>();
            parameter11Interprete.Setup(p => p.Type).Returns(parameter11InterPreteType.Object);
            var parameter11 = new Mock<IParamsParam>();
            parameter11.Setup(p => p.Interprete).Returns(parameter11Interprete.Object);

            var parameter12InterPreteType = new Mock<IParamsParamInterpreteType>();
            parameter12InterPreteType.Setup(p => p.Value).Returns(Skyline.DataMiner.CICD.Models.Protocol.Enums.EnumParamInterpretType.String);
            var parameter12Interprete = new Mock<IParamsParamInterprete>();
            parameter12Interprete.Setup(p => p.Type).Returns(parameter12InterPreteType.Object);
            var parameter12 = new Mock<IParamsParam>();
            parameter12.Setup(p => p.Interprete).Returns(parameter12Interprete.Object);

            var parameter10Object = parameter10.Object;
            var parameter11Object = parameter11.Object;
            var parameter12Object = parameter12.Object;

            var protocolModel = new Mock<IProtocolModel>();
            protocolModel.Setup(p => p.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, "10", out parameter10Object)).Returns(true);
            protocolModel.Setup(p => p.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, "11", out parameter11Object)).Returns(true);
            protocolModel.Setup(p => p.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, "12", out parameter12Object)).Returns(true);

            string result = "";
            var addInvalidConditionError = new Action<string>(message => result = Error.InvalidCondition(null, null, null, inputValue, message, "1").Description);
            var addInvalidParamIdError = new Action<string>(message => result = Error.NonExistingId(null, null, null, message, "1").Description);
            var addConditionCanBeSimpliefiedWarning = new Action(() => result = Error.ConditionCanBeSimplified(null, null, null, inputValue, "1").Description);

            Conditional conditional = new Conditional(addInvalidConditionError, addInvalidParamIdError, addConditionCanBeSimpliefiedWarning);

            // Act
            conditional.ParseConditional(inputValue);
            conditional.CheckConditional(protocolModel.Object);

            // Assert
            result.Should().Be(expectedOutput);
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow("(id:10 + 10) > 20", "Tag 'Group/Condition' references a non-existing 'Param' with PID '10'. Group ID '1'.")]
        [DataRow("((id:10 * 10) * id:11) > 20", "Tag 'Group/Condition' references a non-existing 'Param' with PID '10'. Group ID '1'.")]
        [DataRow("(id:12 + \"efg\") == \"defefgabc\"", "Tag 'Group/Condition' references a non-existing 'Param' with PID '12'. Group ID '1'.")]
        public void ConditionsReferencingNonexistingParametersFail(string inputValue, string expectedOutput)
        {
            // Arrange
            var protocolModel = new Mock<IProtocolModel>();

            string result = "";
            var addInvalidConditionError = new Action<string>(message => result = Error.InvalidCondition(null, null, null, inputValue, message, "1").Description);
            var addInvalidParamIdError = new Action<string>(message => result = Error.NonExistingId(null, null, null, message, "1").Description);
            var addConditionCanBeSimpliefiedWarning = new Action(() => result = Error.ConditionCanBeSimplified(null, null, null, inputValue, "1").Description);

            Conditional conditional = new Conditional(addInvalidConditionError, addInvalidParamIdError, addConditionCanBeSimpliefiedWarning);

            // Act
            conditional.ParseConditional(inputValue);
            conditional.CheckConditional(protocolModel.Object);

            // Assert
            result.Should().Be(expectedOutput);
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow("id: 10 > 1", "Invalid condition 'id: 10 > 1'. Reason 'Invalid id: operand: 'id:'.'. Group ID '1'.")]         // Invalid id: placeholder use: no space allowed after 'id:'.
        [DataRow("id:10 == \"test", "Invalid condition 'id:10 == \"test'. Reason 'Unexpected condition member block: '\"test'.'. Group ID '1'.")]    // Missing quote for string literal.
        [DataRow("(id:10 + 10 > 20", "Invalid condition '(id:10 + 10 > 20'. Reason 'Number of opening parentheses '(' does not match number of closing parentheses ')'.'. Group ID '1'.")]   // Missing closing parenthesis.
        [DataRow("id:10 + 10) > 20", "Invalid condition 'id:10 + 10) > 20'. Reason 'Unexpected condition member block: '10)'.'. Group ID '1'.")]   // Missing opening parenthesis.
        [DataRow("((id:12 + \"efg\") + 10) == \"defefgabc\"", "Invalid condition '((id:12 + \"efg\") + 10) == \"defefgabc\"'. Reason 'The addition operator ('+') must be used with operands of the same type.'. Group ID '1'.")]  // Invalid operands for + operator: cannot mix double and string operand
        [DataRow("((id:12 + \"efg\") + id:10) == \"defefgabc\"", "Invalid condition '((id:12 + \"efg\") + id:10) == \"defefgabc\"'. Reason 'The addition operator ('+') must be used with operands of the same type.'. Group ID '1'.")]
        [DataRow("((id:10 + 20) + id:12) == \"defefgabc\"", "Invalid condition '((id:10 + 20) + id:12) == \"defefgabc\"'. Reason 'The addition operator ('+') must be used with operands of the same type.'. Group ID '1'.")]
        [DataRow("id:10 * 20", "Invalid condition 'id:10 * 20'. Reason 'Condition 'id:10 * 20' is not a boolean expression.'. Group ID '1'.")] // Not a boolean expression.
        [DataRow("id:10 AND (\"defefgabc\" > id:12)", "Invalid condition 'id:10 AND (\"defefgabc\" > id:12)'. Reason 'Not all operands of a conditional AND or OR expression are boolean expressions.'. Group ID '1'.")]
        [DataRow("(id:10 * 20) AND (\"defefgabc\" > id:12)", "Invalid condition '(id:10 * 20) AND (\"defefgabc\" > id:12)'. Reason 'Not all operands of a conditional AND or OR expression are boolean expressions.'. Group ID '1'.")]
        [DataRow("(\"defefgabc\" > id:12) AND (id:10 * 20)", "Invalid condition '(\"defefgabc\" > id:12) AND (id:10 * 20)'. Reason 'Not all operands of a conditional AND or OR expression are boolean expressions.'. Group ID '1'.")]
        [DataRow("(\"defefgabc\" > id:12) AND (id:10 * 20 > 20 < 10)", "Invalid condition '(\"defefgabc\" > id:12) AND (id:10 * 20 > 20 < 10)'. Reason 'Multiple relational or equality operators were detected in a single condition member.'. Group ID '1'.")] // Multiple relational or equation operators in a single condition member.
        [DataRow("(\"defefgabc\" > \"acb\") AND (id:10 * 20 > 20)", "Condition '(\"defefgabc\" > \"acb\") AND (id:10 * 20 > 20)' can be simplified. Group ID '1'.")]   // Constant condition member.
        [DataRow("(\"defefgabc\" > id:12) AND (20 > 40)", "Condition '(\"defefgabc\" > id:12) AND (20 > 40)' can be simplified. Group ID '1'.")]
        [DataRow("(\"defefgabc\" > id:12) AND (id:10 * 20 > 20 < id:10)", "Invalid condition '(\"defefgabc\" > id:12) AND (id:10 * 20 > 20 < id:10)'. Reason 'Multiple relational or equality operators were detected in a single condition member.'. Group ID '1'.")]   // Condition member has multiple equality or relational operators.
        [DataRow("(\"defefgabc\" > id:12) AND (id:10 + * 20 + 20 > 100)", "Invalid condition '(\"defefgabc\" > id:12) AND (id:10 + * 20 + 20 > 100)'. Reason 'Missing operator or operand detected.'. Group ID '1'.")]   // Condition member has operators without operands (or stated differently, multiple operators are used in sequence).
        [DataRow("(\"defefgabc\" id:12) AND (id:10 * 10 > 100)", "Invalid condition '(\"defefgabc\" id:12) AND (id:10 * 10 > 100)'. Reason 'Missing operator or operand detected.'. Group ID '1'.")]   // Condition member has operands missing operator.
        [DataRow("(> id:12) AND (id:10 * 10 > 100)", "Invalid condition '(> id:12) AND (id:10 * 10 > 100)'. Reason 'Missing operator or operand detected.'. Group ID '1'.")]
        [DataRow("(\"defefgabc\" > ) AND (id:10 * 10 > 100)", "Invalid condition '(\"defefgabc\" > ) AND (id:10 * 10 > 100)'. Reason 'Missing operator or operand detected.'. Group ID '1'.")]
        [DataRow("(\"defefgabc\" >) AND (id:10 * 10 > 100)", "Invalid condition '(\"defefgabc\" >) AND (id:10 * 10 > 100)'. Reason 'Missing operator or operand detected.'. Group ID '1'.")]   // Same as previous but without space in first condition member.
        [DataRow("(id:12 != \"[]\" OR id:12 != empty)AND id:10 == 0", "Invalid condition '(id:12 != \"[]\" OR id:12 != empty)AND id:10 == 0'. Reason 'Invalid formatted condition detected.'. Group ID '1'.")]    // No space between AND and preceeding closing parenthesis.
        [DataRow("(id:10 == \"\")", "Invalid condition '(id:10 == \"\")'. Reason 'Unexpected empty string used in combination with other operand that is a double.'. Group ID '1'.")]
        [DataRow("(id:12 + \"A\") == (\"Inhibit\")", "Condition '(id:12 + \"A\") == (\"Inhibit\")' can be simplified. Group ID '1'.")]
        [DataRow("(\"defefgabc\" > \"acb\") AND (id:123 * 20 > 20)", "Condition '(\"defefgabc\" > \"acb\") AND (id:123 * 20 > 20)' can be simplified. Group ID '1'.")]
        public void InvalidConditionReturnsErrorIndicatingError(string inputValue, string expectedOutput)
        {
            // Arrange
            var parameter10InterPreteType = new Mock<IParamsParamInterpreteType>();
            parameter10InterPreteType.Setup(p => p.Value).Returns(Skyline.DataMiner.CICD.Models.Protocol.Enums.EnumParamInterpretType.Double);
            var parameter10Interprete = new Mock<IParamsParamInterprete>();
            parameter10Interprete.Setup(p => p.Type).Returns(parameter10InterPreteType.Object);
            var parameter10 = new Mock<IParamsParam>();
            parameter10.Setup(p => p.Interprete).Returns(parameter10Interprete.Object);

            var parameter12InterPreteType = new Mock<IParamsParamInterpreteType>();
            parameter12InterPreteType.Setup(p => p.Value).Returns(Skyline.DataMiner.CICD.Models.Protocol.Enums.EnumParamInterpretType.String);
            var parameter12Interprete = new Mock<IParamsParamInterprete>();
            parameter12Interprete.Setup(p => p.Type).Returns(parameter12InterPreteType.Object);
            var parameter12 = new Mock<IParamsParam>();
            parameter12.Setup(p => p.Interprete).Returns(parameter12Interprete.Object);

            var parameter10Object = parameter10.Object;
            var parameter12Object = parameter12.Object;

            var protocolModel = new Mock<IProtocolModel>();
            protocolModel.Setup(p => p.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, "10", out parameter10Object)).Returns(true);
            protocolModel.Setup(p => p.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, "12", out parameter12Object)).Returns(true);

            string result = "";
            var addInvalidConditionError = new Action<string>(message => result = Error.InvalidCondition(null, null, null, inputValue, message, "1").Description);
            var addInvalidParamIdError = new Action<string>(message => result = Error.NonExistingId(null, null, null, message, "1").Description);
            var addConditionCanBeSimpliefiedWarning = new Action(() => result = Error.ConditionCanBeSimplified(null, null, null, inputValue, "1").Description);

            Conditional conditional = new Conditional(addInvalidConditionError, addInvalidParamIdError, addConditionCanBeSimpliefiedWarning);

            // Act
            conditional.ParseConditional(inputValue);

            if (result == String.Empty)
            {
                conditional.CheckConditional(protocolModel.Object);
            }

            // Assert
            result.Should().Be(expectedOutput);
        }
    }
}