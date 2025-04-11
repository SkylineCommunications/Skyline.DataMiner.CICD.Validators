namespace ProtocolTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    using Common.Testing;

    using FluentAssertions;
    using FluentAssertions.Equivalency;

    using Microsoft.Build.Locator;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.MSBuild;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Parsers.Common.Xml;
    using Skyline.DataMiner.CICD.Parsers.Protocol.VisualStudio;
    using Skyline.DataMiner.CICD.Validators.Common.Data;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    internal static class Generic
    {
        #region Actual Check Methods

        public static void CheckCategory(IRoot test, Category category)
        {
            // Get Category from test
            TestAttribute testAttribute = (TestAttribute)Attribute.GetCustomAttribute(test.GetType(), typeof(TestAttribute));

            if (testAttribute == null)
            {
                Assert.Fail("No TestAttribute defined on " + test.GetType().Name);
            }
            else
            {
                Assert.AreEqual(category, testAttribute.Category);
            }
        }

        public static void CheckId(IRoot test, uint checkId)
        {
            // Get CheckId from test
            TestAttribute testAttribute = (TestAttribute)Attribute.GetCustomAttribute(test.GetType(), typeof(TestAttribute));

            if (testAttribute == null)
            {
                Assert.Fail("No TestAttribute defined on " + test.GetType().Name);
            }
            else
            {
                Assert.AreEqual(checkId, testAttribute.CheckId);
            }
        }

        /// <summary>
        /// Will run all the checks and filter out all the error messages that aren't part of the specified test.
        /// </summary>
        /// <param name="test">Test that will be used to filter out all the other results.</param>
        /// <param name="validate">Unit test data.</param>
        /// <param name="pathToClassFile">Path to where this class is located. Don't fill in manually unless you know what you are doing.</param>
        internal static void FullValidate(IValidate test, ValidateData validate, [CallerFilePath] string pathToClassFile = "")
        {
            // Arrange
            Validator validator = new Validator();
            string code = GetValidate(validate, pathToClassFile);
            var qactionCompilationModel = ProtocolTestsHelper.GetQActionCompilationModel(code);
            TestAttribute testAttribute = TestAttribute.GetAttribute(test);
            string idFilter = $"{(int)testAttribute.Category}.{testAttribute.CheckId}.";

            IProtocolInputData input = new ProtocolInputData(code, qactionCompilationModel);

            // Act
            ValidatorSettings settings = GetValidatorSettingsFromEnvironmentData(validate.IsSkylineUser);
            IList<IValidationResult> results = validator.RunValidate(input, settings, CancellationToken.None);

            // Modify results
            results = results.Where(result => result.FullId.StartsWith(idFilter)).ToList();

            foreach (var result in results)
            {
                if (result.Severity == Severity.BubbleUp && result.SubResults.Count == 0)
                {
                    Assert.Fail($"Result ({result.FullId}) as BubbleUp as severity but has no sub results.");
                }
            }

            string testName = test.GetType().Name;
            if (testName.StartsWith("CSharp"))
            {
                foreach (var result in results)
                {
                    if (result.PositionNode is IQActionsQAction && !(result is ICSharpValidationResult))
                    {
                        // TODO: Check if there is a way with the ReferenceNode & PositionNode & probably a list with errormessages that need to be excluded?
                        // Cause PositionNode can also be the attribute instead of the QAction itself? => Something to review.
                        //Assert.Fail($"Result in test '{testName}' doesn't contain CSharp information");
                    }
                    else if (!(result.PositionNode is IQActionsQAction) && result is ICSharpValidationResult)
                    {
                        Assert.Fail($"CSharp information found in result of test '{testName}' but the position node is not a QAction");
                    }
                }
            }

            AssertResults(results, validate.ExpectedResults);
        }

        internal static void Validate(IValidate test, ValidateData validate, [CallerFilePath] string pathToClassFile = "")
        {
            // Arrange.
            ValidatorContext context = GetValidatorContext(validate, pathToClassFile);

            // Act.
            List<IValidationResult> results = test.Validate(context);
            foreach (var result in results)
            {
                if (result is ISeverityBubbleUpResult bubbleUpResult)
                {
                    bubbleUpResult.DoSeverityBubbleUp();
                }

                if (result.Severity == Severity.BubbleUp && result.SubResults.Count == 0)
                {
                    Assert.Fail($"Result ({result.FullId}) as BubbleUp as severity but has no sub results.");
                }
            }

            foreach (var result in validate.ExpectedResults)
            {
                if (result is ISeverityBubbleUpResult bubbleUpResult)
                {
                    bubbleUpResult.DoSeverityBubbleUp();
                }

                if (result.Severity == Severity.BubbleUp && result.SubResults.Count == 0)
                {
                    Assert.Fail($"Expected result ({result.FullId}) as BubbleUp as severity but has no sub results.");
                }
            }

            // Assert.
            string testName = test.GetType().Name;

            if (testName.StartsWith("CSharp"))
            {
                foreach (var result in results)
                {
                    if (result.PositionNode is IQActionsQAction && !(result is ICSharpValidationResult))
                    {
                        // TODO: Check if there is a way with the ReferenceNode & PositionNode & probably a list with errormessages that need to be excluded?
                        // Cause PositionNode can also be the attribute instead of the QAction itself? => Something to review.
                        // Assert.Fail($"Result in test '{testName}' doesn't contain CSharp information");
                    }
                    else if (!(result.PositionNode is IQActionsQAction) && result is ICSharpValidationResult)
                    {
                        Assert.Fail($"CSharp information found in result of test '{testName}' but the position node is not a QAction");
                    }
                }
            }

            AssertResults(results, validate.ExpectedResults);
        }

        internal static void Compare(ICompare test, CompareData compare, [CallerFilePath] string pathToClassFile = "")
        {
            // Arrange.
            MajorChangeCheckContext context = GetMajorChangeCheckContext(compare, pathToClassFile);

            // Act.
            List<IValidationResult> results = test.Compare(context);
            foreach (var result in results.OfType<ISeverityBubbleUpResult>())
            {
                result.DoSeverityBubbleUp();
            }

            foreach (var result in compare.ExpectedResults.OfType<ISeverityBubbleUpResult>())
            {
                result.DoSeverityBubbleUp();
            }

            // Assert.
            AssertResults(results, compare.ExpectedResults);
        }

        internal static void Fix(IRoot test, FixData data, [CallerFilePath] string pathToClassFile = "")
        {
            // Execute Validate first to receive the results.

            var (model, document, code) = GetValidatorContextData(data, pathToClassFile);

            var qactionCompilationModel = ProtocolTestsHelper.GetQActionCompilationModel(model, code);

            ProtocolInputData input = new ProtocolInputData(model, document, qactionCompilationModel);
            ValidatorSettings settings = GetValidatorSettingsFromEnvironmentData(data.IsSkylineUser);
            ValidatorContext validatorContext = new ValidatorContext(input, settings);

            if (test is IValidate valTest && test is ICodeFix fixTest)
            {
                var validateResults = valTest.Validate(validatorContext);

                StringBuilder sb = new StringBuilder();

                if (validateResults.Count == 0)
                {
                    sb.AppendLine("No results from the test!");
                }

                // Recreate each time the CodeFixContent, but with the same editProtocol (so all the changes are done on the same protocol)

                var (editDocument, editProtocol) = GetCodeFixContextData(document, model);

                bool hasCodeFix = false;
                FixRecursive(settings, fixTest, validateResults, sb, editDocument, editProtocol, ref hasCodeFix);

                if (!hasCodeFix)
                {
                    Assert.Fail($"No CodeFix for this result");
                }

                string message = sb.ToString();

                if (!String.IsNullOrWhiteSpace(message))
                {
                    Assert.Fail(message);
                }

                // Compare the fixed protocol
                string fixedCode = GetFix(data, pathToClassFile);

                var fixedData = new ProtocolInputData(fixedCode);
                var wrongData = new ProtocolInputData(editProtocol.EditNode.GetXml());

                // WARNING! Currently there is an issue in FluentAssertions regarding the scenario for ArrayOptions, HTTP, ...
                // When you have a class that inherits from IEnumerable (down the line) and has properties itself, then those properties won't be checked by FluentAssertions.
                // https://github.com/fluentassertions/fluentassertions/issues/860
                wrongData.Model.Protocol.Should().BeEquivalentTo(fixedData.Model.Protocol, ExcludePropertiesForFix);

                // Backup check for the situation where a tag inherits IReadableList and has properties.
                ////editProtocol.EditNode.GetXml().Should().BeEquivalentTo(fixedCode);
                // TODO: Can't use the above as that formats the xml differently. (Also doesn't add the xml declaration)
            }
            else
            {
                Assert.Fail("Check isn't IValidate or ICodeFix!");
            }

            Assert.IsTrue(true);
        }

        private static void FixRecursive(ValidatorSettings settings, ICodeFix fixTest, List<IValidationResult> validateResults, StringBuilder sb, Skyline.DataMiner.CICD.Parsers.Common.XmlEdit.XmlDocument editDocument, Skyline.DataMiner.CICD.Models.Protocol.Edit.Protocol editProtocol, ref bool hasCodeFix)
        {
            foreach (var validateResult in validateResults)
            {
                if (validateResult.SubResults != null && validateResult.SubResults.Count > 0)
                {
                    FixRecursive(settings, fixTest, validateResult.SubResults, sb, editDocument, editProtocol, ref hasCodeFix);
                }

                if (!validateResult.HasCodeFix)
                {
                    continue;
                }

                hasCodeFix = true;

                // Get CodeFixContext
                CodeFixContext codeFixContext = new CodeFixContext(editDocument, editProtocol, (ValidationResult)validateResult, settings);

                var fixResult = fixTest.Fix(codeFixContext);

                if (!fixResult.Success)
                {
                    sb.AppendLine($"CodeFix on '{validateResult.Description}' failed with message: {fixResult.Message}");
                }
            }
        }

        private static void AssertResults(IList<IValidationResult> results, IList<IValidationResult> expectedResults)
        {
            List<string> errors = new List<string>();

            AssertResults(errors, results, expectedResults);

            if (errors.Count > 0)
            {
                string s = Environment.NewLine + String.Join(Environment.NewLine, errors);
                Assert.Fail(s);
            }
        }

        private static void AssertResults(ICollection<string> errors, IList<IValidationResult> results, IList<IValidationResult> expectedResults, string parent = null)
        {
            // Sort so it's easier to compare later on
            results = results.OrderBy(result => result.FullId).ThenBy(result => result.Description).ToList();
            expectedResults = expectedResults.OrderBy(result => result.FullId).ThenBy(result => result.Description).ToList();

            if (results.Count != expectedResults.Count)
            {
                errors.Add($"[{parent}] {MismatchAmountResults(results, expectedResults)}");
                return;
            }

            // Go over every single result and check if they match
            for (int i = 0; i < results.Count; i++)
            {
                IValidationResult result = results[i];
                IValidationResult expectedResult = expectedResults[i];

                string first = String.Empty;
                if (parent != null)
                {
                    first = $"{parent} - ";
                }

                string key = $"{first}{result.FullId}";

                try
                {
                    result.Should().BeEquivalentTo(expectedResult, ExcludePropertiesForGeneric);
                }
                catch (Exception e)
                {
                    string filteredMessage = e.Message;
                    int index = e.Message.IndexOf("With configuration:", StringComparison.OrdinalIgnoreCase);
                    if (index != -1)
                    {
                        filteredMessage = e.Message.Remove(index);
                    }

                    errors.Add($"[{key}] {filteredMessage}");
                }

                // Rerun for the subresults
                AssertResults(errors, result.SubResults, expectedResult.SubResults, key);
            }
        }

        #endregion Actual Check Methods

        #region Misc

        public static EquivalencyAssertionOptions<IValidationResult> ExcludePropertiesForGeneric(EquivalencyAssertionOptions<IValidationResult> options)
        {
            options.ComparingByMembers<IValidationResult>();
            options.ComparingEnumsByName();
            options.IgnoringCyclicReferences();
            options.AllowingInfiniteRecursion();
            options.Excluding(x => x.Position)
                   .Excluding(x => x.SubResults)
                   .Excluding(x => x.Line)
                   .Excluding(x => x.PositionNode)
                   .Excluding(x => x.ReferenceNode)
                   .Excluding(x => x.Type == typeof(IValidate)) // Test
                   .Excluding(x => x.Path.EndsWith("ExtraData"));

            return options;
        }

        public static EquivalencyAssertionOptions<ValidationResult> ExcludePropertiesForErrorMessages(EquivalencyAssertionOptions<ValidationResult> options)
        {
            options.ComparingByMembers<IValidationResult>();
            options.ComparingEnumsByName();
            options.IgnoringCyclicReferences();
            options.AllowingInfiniteRecursion();
            options.Excluding(x => x.Position)
                   .Excluding(x => x.Line)
                   .Excluding(x => x.PositionNode)
                   .Excluding(x => x.ReferenceNode)
                   .Excluding(x => x.CheckId)
                   .Excluding(x => x.Category)
                   .Excluding(x => x.ErrorId)
                   .Excluding(x => x.FullId)
                   .Excluding(x => x.Source)
                   .Excluding(x => x.HowToFix)
                   .Excluding(x => x.Type == typeof(IValidate)) // Test
                   .Excluding(x => x.Path.EndsWith("ExtraData"));

            return options;
        }

        private static EquivalencyAssertionOptions<IProtocol> ExcludePropertiesForFix(EquivalencyAssertionOptions<IProtocol> options)
        {
            options.Excluding((IMemberInfo x) => x.Type == typeof(IProtocolModel)) // Model
                   .Excluding((IMemberInfo x) => x.Type == typeof(XmlElement)) // ReadNode
                   .Excluding((IMemberInfo x) => x.Type == typeof(XmlAttribute)) // ReadAttribute
                   .Excluding((IMemberInfo x) => x.Type == typeof(XmlCDATA)) // CodeCDATA (QActions)
                   .Excluding(x => x.Path.Contains("Parent"));

            options.IgnoringCyclicReferences();
            options.ComparingEnumsByName();
            options.AllowingInfiniteRecursion();
            return options;
        }

        internal static (string code, bool success) ReadTextFromFile(string pathToFile)
        {
            try
            {
                pathToFile = @"\\?\" + pathToFile;

                string code;
                var fileStream = new FileStream(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                using (var textReader = new StreamReader(fileStream))
                {
                    code = textReader.ReadToEnd();
                }

                return (code, true);
            }
            catch (FileNotFoundException)
            {
                return (String.Format("Missing file:{0}{1}", Environment.NewLine, pathToFile), false);
            }
        }

        #endregion Misc

        #region Dynamic Classes

        public class ValidateData
        {
            public TestType TestType { get; set; }

            /// <summary>
            /// Gets or sets the file name base. Make sure to add it without the extension!
            /// </summary>
            public string FileName { get; set; }

            public List<IValidationResult> ExpectedResults { get; set; }

            public bool IsSkylineUser { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating whether the provided file name is a solution.
            /// </summary>
            public bool IsSolution { get; set; }
        }

        public class CompareData
        {
            public TestType TestType { get; set; }

            /// <summary>
            /// Gets or sets the file name base. Make sure to add it without the extension!
            /// </summary>
            public string FileNameBase { get; set; }

            public List<IValidationResult> ExpectedResults { get; set; }

            public bool IsSkylineUser { get; set; } = true;
        }

        public class FixData
        {
            /// <summary>
            /// Gets or sets the file name base. Make sure to add it without the extension!
            /// </summary>
            public string FileNameBase { get; set; }

            public bool IsSkylineUser { get; set; } = true;
        }

        #endregion

        #region Get From Samples

        private static string GetValidate(ValidateData data, string pathToClassFile)
        {
            string validatePath = Path.Combine(Path.GetDirectoryName(pathToClassFile), "Samples", "Validate");

            string type = data.TestType == TestType.Valid ? "Valid" : "Invalid";

            string fileName = data.FileName.EndsWith(".xml") ? data.FileName : $"{data.FileName}.xml";
            string filePath = Path.Combine(validatePath, type, fileName);
            (string code, bool success) = ReadTextFromFile(filePath);

            return success ? code : throw new FileNotFoundException(code);
        }

        private static string GetValidatePath(ValidateData data, string pathToClassFile)
        {
            string validatePath = Path.Combine(Path.GetDirectoryName(pathToClassFile), "Samples", "Validate");

            string type = data.TestType == TestType.Valid ? "Valid" : "Invalid";

            string fileName = data.FileName.EndsWith(".sln") ? data.FileName : $"{data.FileName}.sln";

            // ..\\Samples\\Validate\\Valid\\solutionFolder\\solution.sln
            string filePath = Path.Combine(validatePath, type, data.FileName, fileName);
            return filePath;
        }

        private static string GetFix(FixData data, string pathToClassFile)
        {
            string path = Path.Combine(Path.GetDirectoryName(pathToClassFile), "Samples", "Codefix");

            string fileName = data.FileNameBase.EndsWith(".xml") ? data.FileNameBase : $"{data.FileNameBase}.xml";
            string filePath = Path.Combine(path, fileName);
            (string code, bool success) = ReadTextFromFile(filePath);

            return success ? code : throw new FileNotFoundException(code);
        }

        private static (string oldP, string newP) GetCompare(CompareData data, string pathToClassFile)
        {
            string path = Path.Combine(Path.GetDirectoryName(pathToClassFile), "Samples", "Compare");

            string type = data.TestType == TestType.Valid ? "Valid" : "Invalid";

            string oldProtocol = Path.Combine(path, type, $"{data.FileNameBase}_Old.xml");
            string newProtocol = Path.Combine(path, type, $"{data.FileNameBase}_New.xml");
            (string oldP, bool successOld) = ReadTextFromFile(oldProtocol);
            (string newP, bool successNew) = ReadTextFromFile(newProtocol);

            if (!successOld)
            {
                throw new FileNotFoundException(oldP);
            }

            if (!successNew)
            {
                throw new FileNotFoundException(newP);
            }

            return (oldP, newP);
        }

        #endregion

        #region Get Context

        private static ValidatorContext GetValidatorContext(ValidateData data, string pathToClassFile)
        {
            try
            {
                ProtocolInputData input;
                if (data.IsSolution)
                {
                    string solutionPath = GetValidatePath(data, pathToClassFile);

                    input = ProtocolTestsHelper.GetProtocolInputDataFromSolution(solutionPath);
                }
                else
                {
                    string code = GetValidate(data, pathToClassFile);

                    input = ProtocolTestsHelper.GetProtocolInputDataFromXml(code);
                }

                return new ValidatorContext(input, GetValidatorSettingsFromEnvironmentData(data.IsSkylineUser));
            }
            catch (Exception e)
            {
                throw new FormatException($"Sample Code could not be parsed by the {nameof(ValidatorContext)} object. FileName: {data.FileName}{Environment.NewLine}{e}");
            }
        }

        private static MajorChangeCheckContext GetMajorChangeCheckContext(CompareData data, string pathToClassFile = "")
        {
            MajorChangeCheckContext context;

            (string oldP, string newP) = GetCompare(data, pathToClassFile);

            try
            {
                var inputOld = new ProtocolInputData(oldP);
                var inputNew = new ProtocolInputData(newP);

                context = new MajorChangeCheckContext(inputNew, inputOld, GetValidatorSettingsFromEnvironmentData(data.IsSkylineUser));
            }
            catch (Exception e)
            {
                throw new FormatException($"Sample Code could not be parsed by the {nameof(context)} object. FileName: {data.FileNameBase}{Environment.NewLine}{e}");
            }

            return context;
        }

        private static (Skyline.DataMiner.CICD.Parsers.Common.XmlEdit.XmlDocument editDocument, Skyline.DataMiner.CICD.Models.Protocol.Edit.Protocol editProtocol) GetCodeFixContextData(XmlDocument document, IProtocolModel model)
        {
            var editDocument = new Skyline.DataMiner.CICD.Parsers.Common.XmlEdit.XmlDocument(document);
            var editProtocol = ProtocolTestsHelper.GetEditProtocol(model, editDocument);

            return (editDocument, editProtocol);
        }

        private static (IProtocolModel model, XmlDocument document, string code) GetValidatorContextData(FixData data, string pathToClassFile)
        {
            ValidateData validateData = new ValidateData
            {
                TestType = TestType.Invalid,
                FileName = data.FileNameBase,
            };

            string code = GetValidate(validateData, pathToClassFile);

            try
            {
                var a = new ProtocolInputData(code);

                return (a.Model, a.Document, code);
            }
            catch (Exception e)
            {
                throw new FormatException($"Sample Code could not be parsed. FileName: {data.FileNameBase}{Environment.NewLine}{e}");
            }
        }

        private static ValidatorSettings GetValidatorSettingsFromEnvironmentData(bool isSkylineUser)
        {
            ValidatorSettings validatorSettings = new ValidatorSettings
            {
                ExpectedProvider = isSkylineUser ? "Skyline Communications" : String.Empty
            };

            return validatorSettings;
        }

        #endregion

        #region Enums

        public enum TestType
        {
            Valid,
            Invalid,
        }

        #endregion

        #region Custom Error Logging

        private static string MismatchAmountResults(ICollection<IValidationResult> results, ICollection<IValidationResult> expectedResults)
        {
            var a = expectedResults.Count > results.Count ? "only " : " ";
            StringBuilder sb = new StringBuilder()
                .AppendLine($"Expected {expectedResults.Count} results, but found {a}{results.Count}.")
                .AppendLine("Expected results:");

            foreach (var item in expectedResults)
            {
                sb.AppendLine($"\t- [{item.FullId}] {item.Description}");
            }

            sb.AppendLine()
              .AppendLine("Actual results:");

            foreach (var item in results)
            {
                sb.AppendLine($"\t- [{item.FullId}] {item.Description}");
            }

            return sb.ToString();
        }

        #endregion
    }
}