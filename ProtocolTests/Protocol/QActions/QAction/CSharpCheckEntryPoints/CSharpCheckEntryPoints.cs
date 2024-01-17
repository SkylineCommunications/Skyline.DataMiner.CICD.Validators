namespace ProtocolTests.Protocol.QActions.QAction.CSharpCheckEntryPoints
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpCheckEntryPoints;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpCheckEntryPoints();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpCheckEntryPoints_Valid()
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
        public void QAction_CSharpCheckEntryPoints_MissingEntryPoint()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingEntryPoint",
                ExpectedResults = new List<IValidationResult>
                {
                    // DefaultClass_DefaultMethod
                    Error.MissingEntryPoint(null, null, null, "QAction", "Run", "10"),
                    Error.MissingEntryPoint(null, null, null, "QAction", "Run", "11"),
                    Error.MissingEntryPoint(null, null, null, "QAction", "Run", "12"),

                    // DefaultClass_MyMethod
                    Error.MissingEntryPoint(null, null, null, "QAction", "MyMethod", "20"),
                    Error.MissingEntryPoint(null, null, null, "QAction", "MyMethod", "21"),
                    Error.MissingEntryPoint(null, null, null, "QAction", "MyMethod", "22"),

                    // MyClass_MyMethod
                    Error.MissingEntryPoint(null, null, null, "MyClass", "MyMethod", "30"),
                    Error.MissingEntryPoint(null, null, null, "MyClass", "MyMethod", "31"),
                    Error.MissingEntryPoint(null, null, null, "MyClass", "MyMethod", "32"),

                    // Combination
                    Error.MissingEntryPoint(null, null, null, "QAction", "MyMethodTypo", "100"),
                    Error.MissingEntryPoint(null, null, null, "MyClass1", "MyMethodTypo", "100"),
                    Error.MissingEntryPoint(null, null, null, "MyClass2Typo", "MyMethod", "100"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckEntryPoints_UnexpectedAccessModifierForEntryPointClass()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnexpectedAccessModifierForEntryPointClass",
                ExpectedResults = new List<IValidationResult>
                {
                    // DefaultClass_DefaultMethod
                    Error.UnexpectedAccessModifierForEntryPointClass(null, null, null, "QAction", "internal", "10"),

                    // DefaultClass_MyMethod
                    Error.UnexpectedAccessModifierForEntryPointClass(null, null, null, "QAction", "internal", "20"),

                    // MyClass_MyMethod
                    Error.UnexpectedAccessModifierForEntryPointClass(null, null, null, "MyClass", "internal", "30"),

                    // MultipleEntryPoints
                    Error.UnexpectedAccessModifierForEntryPointClass(null, null, null, "QAction", "internal", "100"),
                    Error.UnexpectedAccessModifierForEntryPointClass(null, null, null, "MyClass", "internal", "100"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckEntryPoints_UnexpectedAccessModifierForEntryPointMethod()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnexpectedAccessModifierForEntryPointMethod",
                ExpectedResults = new List<IValidationResult>
                {
                    // DefaultClass_DefaultMethod
                    Error.UnexpectedAccessModifierForEntryPointMethod(null, null, null, "QAction", "Run", "internal", "10"),

                    // DefaultClass_MyMethod
                    Error.UnexpectedAccessModifierForEntryPointMethod(null, null, null, "QAction", "MyMethod", "internal", "20"),

                    // MyClass_MyMethod
                    Error.UnexpectedAccessModifierForEntryPointMethod(null, null, null, "MyClass", "MyMethod", "private", "30"),

                    // MultipleEntryPoints
                    Error.UnexpectedAccessModifierForEntryPointMethod(null, null, null, "QAction", "MyMethod", "internal", "100"),
                    Error.UnexpectedAccessModifierForEntryPointMethod(null, null, null, "MyClass", "MyMethod", "private", "100"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckEntryPoints_UnexpectedArg0TypeForEntryPointMethod()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnexpectedArg0TypeForEntryPointMethod",
                ExpectedResults = new List<IValidationResult>
                {
                    // DefaultClass_DefaultMethod
                    Error.UnexpectedArg0TypeForEntryPointMethod(null, null, null, "QAction", "Run", "int", "10"),
                    Error.UnexpectedArg0TypeForEntryPointMethod(null, null, null, "QAction", "Run", "", "11"),

                    // DefaultClass_MyMethod
                    Error.UnexpectedArg0TypeForEntryPointMethod(null, null, null, "QAction", "MyMethod", "string", "20"),
                    Error.UnexpectedArg0TypeForEntryPointMethod(null, null, null, "QAction", "MyMethod", "", "21"),

                    // MyClass_MyMethod
                    Error.UnexpectedArg0TypeForEntryPointMethod(null, null, null, "MyClass", "MyMethod", "object", "30"),
                    Error.UnexpectedArg0TypeForEntryPointMethod(null, null, null, "MyClass", "MyMethod", "", "31"),

                    // Combinations
                    Error.UnexpectedArg0TypeForEntryPointMethod(null, null, null, "QAction", "MyMethod", "SLProtocol[]", "100"),
                    Error.UnexpectedArg0TypeForEntryPointMethod(null, null, null, "MyClass", "MyMethod", "", "100"),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CSharpCheckEntryPoints();

        [TestMethod]
        [Ignore("Waiting on feedback from TomW")]
        public void QAction_CSharpCheckEntryPoints_UnexpectedAccessModifierForEntryPointClass()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnexpectedAccessModifierForEntryPointClass",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("Waiting on feedback from TomW")]
        public void QAction_CSharpCheckEntryPoints_UnexpectedAccessModifierForEntryPointMethod()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnexpectedAccessModifierForEntryPointMethod",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void QAction_CSharpCheckEntryPoints_MissingEntryPoint()
        {
            // Create ErrorMessage
            var message = Error.MissingEntryPoint(null, null, null, "1", "2", "3");

            var expected = new ValidationResult()
            {
                ErrorId = 1,
                FullId = "3.12.1",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Entry point '1.2' not found in QAction. QAction ID 3.",
                HowToFix = "",
                ExampleCode = "",
                Details = "The QAction@entryPoint attribute can optionally be used to overwrite the default entry point of a QAction." + Environment.NewLine + "" + Environment.NewLine + "By default, the entry point will be the method 'Run' that can be found under the class 'QAction'." + Environment.NewLine + "Defining multiple entry points can be done by providing a semicolon list of entry points." + Environment.NewLine + "In that case, each entry point corresponds to a QAction trigger." + Environment.NewLine + "" + Environment.NewLine + "One of the two following format can be used in order to overwrite default entry points (curly braces are here used as place holder indicators in the below explanation so are not to be included when defining your entry points):" + Environment.NewLine + "- {entryPointMethod}: the given method name in the 'QAction' class will be the new entry point." + Environment.NewLine + "- {entryPointClass}.{entryPointMethod}: the given method name in the given class name will be the new entry point." + Environment.NewLine + "" + Environment.NewLine + "Note that both the entry point class and methods are expected to be public and the first argument of the entry point is expected to be of type SLProtocol or SLProtocolExt.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CSharpCheckEntryPoints();

        [TestMethod]
        public void QAction_CSharpCheckEntryPoints_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpCheckEntryPoints_CheckId() => Generic.CheckId(check, CheckId.CSharpCheckEntryPoints);
    }
}