namespace ProtocolTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Schema;

    using FluentAssertions;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.FindSymbols;
    using Microsoft.CodeAnalysis.MSBuild;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.CSharpAnalysis;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Classes;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Enums;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [TestClass]
    public class CheckTests
    {
        /// <summary>
        /// Checks if there is a namespace for each test.
        /// </summary>
        [TestMethod]
        public void CheckTestsForUnitTest_Validate()
        {
            // Get all tests
            var allTests = TestCollector.GetAllValidateTests().Tests;

            // Get all namespaces
            var namespaces = Assembly
                .GetAssembly(typeof(CheckTests))
                .GetTypes()
                .Select(type => type.Namespace)
                .ToList();

            List<string> testNamespaces = new List<string>();
            foreach ((IValidate test, TestAttribute _) in allTests)
            {
                // Get test name
                Type temp = test.GetType();

                string newNamespace = temp.Namespace?.Replace("Skyline.DataMiner.CICD.Validators.Protocol.Tests", "ProtocolTests");

                if (!namespaces.Contains(newNamespace))
                {
                    testNamespaces.Add(newNamespace?.Remove(0, "ProtocolTests.".Length));
                }
            }

            // Check if UnitTest already exists
            testNamespaces.Should().BeEmpty();
        }

        /// <summary>
        /// Checks if there is a namespace for each test.
        /// </summary>
        [TestMethod]
        public void CheckTestsForUnitTest_Compare()
        {
            // Get all tests
            var allTests = TestCollector.GetAllCompareTests().Tests;

            // Get all namespaces
            var namespaces = Assembly
                             .GetAssembly(typeof(CheckTests))
                             .GetTypes()
                             .Select(type => type.Namespace)
                             .ToList();

            List<string> testNamespaces = new List<string>();
            foreach ((ICompare test, TestAttribute _) in allTests)
            {
                // Get test name
                Type temp = test.GetType();

                string newNamespace = temp.Namespace?.Replace("Skyline.DataMiner.CICD.Validators.Protocol.Tests", "ProtocolTests");

                if (!namespaces.Contains(newNamespace))
                {
                    testNamespaces.Add(newNamespace?.Remove(0, "ProtocolTests.".Length));
                }
            }

            // Check if UnitTest already exists
            testNamespaces.Should().BeEmpty();
        }

        [TestMethod]
        public void CheckNamespacesOfUnitTests()
        {
            // Get all namespaces
            var namespaces = Assembly
                .GetAssembly(typeof(CheckTests))
                .GetTypes()
                .Where(x => x.Namespace != null)
                .Select(x => x.Namespace.Replace("ProtocolTests.", String.Empty));

            // Get all Files
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var files = Directory
                .GetParent(currentDirectory)
                .Parent
                .Parent
                .GetFiles("*.cs", SearchOption.AllDirectories);

            files.Should().NotBeEmpty();

            List<string> lsWrongFiles = new List<string>();

            string[] asExcludeDirectories =
            {
                "bin", "obj", "Generic Tests", "Software Parameters",
            };

            foreach (var item in files)
            {
                if (String.Equals(item.Directory.Name, "ProtocolTests"))
                {
                    // No need to check generic stuff.
                    continue;
                }

                List<string> lsParents = new List<string>();
                bool bStillGoing = true;
                var directory = item.Directory;
                while (bStillGoing)
                {
                    if (String.Equals(directory.Name, "ProtocolTests")
                        || directory.Parent == null)
                    {
                        bStillGoing = false;
                    }
                    else if (asExcludeDirectories.Contains(directory.Name))
                    {
                        lsParents.Clear();
                        bStillGoing = false;
                    }
                    else
                    {
                        lsParents.Add(directory.Name);
                        directory = directory.Parent;
                    }
                }

                if (lsParents.Count == 0)
                {
                    continue;
                }

                lsParents.Reverse();

                string newNamespace = String.Join(".", lsParents);

                if (!namespaces.Contains(newNamespace))
                {
                    lsWrongFiles.Add(String.Join("/", lsParents));
                }
            }

            lsWrongFiles.Should().BeEmpty();
        }

        [TestMethod]
        public void CheckNamespacesOfTests()
        {
            // Get all namespaces
            var namespaces = Assembly
                .GetAssembly(typeof(Validator))
                .GetTypes()
                .Where(x => !String.IsNullOrWhiteSpace(x.Namespace) && x.Namespace.StartsWith("Skyline.DataMiner.CICD.Validators.Protocol.Tests"))
                .Select(x => x.Namespace.Replace("Skyline.DataMiner.CICD.Validators.Protocol.Tests.", String.Empty));

            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string solutiondir = Directory.GetParent(currentDirectory).Parent.Parent.Parent.FullName;

            string newPath = Path.Combine(solutiondir, "Protocol", "Tests");

            // Get all Files
            var files = Directory
                .GetFiles(newPath, "*.cs", SearchOption.AllDirectories);

            List<string> lsWrongFiles = new List<string>();

            string[] asExcludeDirectories = new string[]
            {
                "bin", "obj"
            };

            foreach (var temp in files)
            {
                var item = new FileInfo(temp);

                List<string> lsParents = new List<string>
                {
                    item.Name.Replace(".cs", String.Empty)
                };
                bool bStillGoing = true;
                var directory = item.Directory;
                while (bStillGoing)
                {
                    if (String.Equals(directory.Name, "Tests"))
                    {
                        bStillGoing = false;
                    }
                    else if (asExcludeDirectories.Contains(directory.Name))
                    {
                        lsParents.Clear();
                        bStillGoing = false;
                    }
                    else
                    {
                        lsParents.Add(directory.Name);
                        directory = directory.Parent;
                    }
                }

                if (lsParents.Count == 1)
                {
                    continue;
                }

                lsParents.Reverse();
                string newNamespace = String.Join(".", lsParents);

                if (!namespaces.Contains(newNamespace))
                {
                    lsWrongFiles.Add(String.Join("/", lsParents));
                }
            }

            lsWrongFiles.Should().BeEmpty();
        }

        /// <summary>
        /// Checks if the unit tests don't have failing QActions.
        /// Will also fail if any have MinDmaVersion tag in the compliancies (currently it isn't possible to validate newer C#)
        /// </summary>
        [TestMethod]
        public void CheckQActionCompilation()
        {
            var test = new Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpQActionCompilation.CSharpQActionCompilation();

            // Search for Protocol directory which is the root folder of all unit tests
            if (!Files.TryGetProtocolDirectory(out DirectoryInfo protocolDirectory))
            {
                Assert.Fail("Protocol folder not found.");
            }

            // Get all XML files
            List<(string fileName, string fileLocation, Generic.TestType testType)> allFiles = protocolDirectory
                .GetFiles("*.xml", SearchOption.AllDirectories)
                .Select(GetFileData)
                .ToList();

            allFiles.Should().NotBeEmpty();

            List<string> failedTests = new List<string>();
            foreach ((string fileName, string fileLocation, Generic.TestType testType) in allFiles)
            {
                if (fileName == null || fileLocation.Contains("CSharpQActionCompilation"))
                {
                    continue;
                }

                try
                {
                    Generic.ValidateData data = new Generic.ValidateData
                    {
                        ExpectedResults = new List<IValidationResult>(),
                        FileName = fileName,
                        TestType = testType,
                    };

                    Generic.Validate(test, data, fileLocation);
                }
                catch (AssertFailedException /*afEx*/)
                {
                    Regex r = new Regex(@".*\\[ProtocolTests]+\\(.*)");

                    var location = r.Match(fileLocation).Groups[1].Value;

                    //failedTests.Add($"{location}{testType}\\{fileName}|\n{afex.Message}");
                    failedTests.Add($"{location}{testType}\\{fileName}");
                }
                catch (Exception ex)
                {
                    failedTests.Add($"EXCEPTION: {ex.Message}");
                }
            }

            if (failedTests.Any())
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                foreach (var item in failedTests)
                {
                    sb.AppendLine($"=> {item}");
                }

                Assert.Fail(sb.ToString());
            }

            (string, string, Generic.TestType) GetFileData(FileInfo fileInfo)
            {
                Generic.TestType testType = Generic.TestType.Invalid;
                DirectoryInfo directory = fileInfo.Directory;
                while (!String.Equals(directory.Name, "Samples"))
                {
                    if (String.Equals(directory.Name, "Compare") || String.Equals(directory.Name, "Codefix", StringComparison.OrdinalIgnoreCase))
                    {
                        return (null, null, testType);
                    }

                    if (String.Equals(directory.Name, "Valid"))
                    {
                        testType = Generic.TestType.Valid;
                    }

                    directory = directory.Parent;
                }

                return (fileInfo.Name, $"{directory.Parent.FullName}\\", testType);
            }
        }
    }

    [TestClass]
    public class BasicChecks
    {
        /// <summary>
        /// Will fail when a check throws an error/exception when the protocol tag or Params, Groups, Triggers, ... is missing
        /// </summary>
        [TestMethod]
        public void CheckBasics()
        {
            // Get all tests
            var allTests = TestCollector.GetAllValidateTests().Tests;

            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string solutiondir = Directory.GetParent(currentDirectory).Parent.Parent.FullName;

            // Need to add the backslashes as it doesn't recognize it as a directory.
            string fileLocation = Path.Combine(solutiondir, "Generic Tests\\");

            List<string> failedTests = new List<string>();
            foreach ((IValidate test, TestAttribute attribute) in allTests)
            {
                if (attribute.CheckId == 1 /* Protocol Tag itself */ && attribute.Category == Category.Protocol)
                {
                    continue;
                }

                try
                {
                    // No Protocol Tag
                    Generic.ValidateData data = new Generic.ValidateData
                    {
                        ExpectedResults = new List<IValidationResult>(),
                        FileName = "NoProtocol",
                        TestType = Generic.TestType.Valid,
                    };

                    Generic.Validate(test, data, fileLocation);

                    if (attribute.Category > Category.Protocol)
                    {
                        // No Params, Groups, Triggers, Actions, Timers, ...
                        Generic.ValidateData data2 = new Generic.ValidateData
                        {
                            ExpectedResults = new List<IValidationResult>(),
                            FileName = "NoSubChildren",
                            TestType = Generic.TestType.Valid,
                        };

                        Generic.Validate(test, data2, fileLocation);
                    }
                }
                catch (AssertFailedException)
                {
                    failedTests.Add($"{Convert.ToString(attribute.Category)}.{test.GetType().Name}");
                }
                catch (NullReferenceException)
                {
                    failedTests.Add($"NULL REF: {Convert.ToString(attribute.Category)}.{test.GetType().Name}");
                }
                catch (Exception e)
                {
                    failedTests.Add($"EXCEPTION: {attribute.Category}.{test.GetType().Name} ({e.Message})");
                }
            }

            failedTests.Should().BeEmpty();
        }

        [TestMethod]
        public void ValidatorChecksShouldNotStoreData()
        {
            var tests = TestCollector.GetAllValidateTests().Tests;

            foreach ((IValidate test, TestAttribute _) in tests)
            {
                var type = test.GetType();

                var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                foreach (var fi in fields)
                {
                    // Exception for Regex
                    if (fi.FieldType == typeof(Regex))
                    {
                        continue;
                    }

                    bool isConstant = fi.IsLiteral && !fi.IsInitOnly;
                    if (!isConstant)
                    {
                        Assert.Fail("Check contains fields: " + type.FullName);
                    }
                }

                var props = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                if (props.Any())
                {
                    Assert.Fail("Check contains properties: " + type.FullName);
                }
            }
        }
    }

    [TestClass]
    public class RoslynUnitTests
    {
        private static Solution solution;
        private static Project valProject;
        private static Project testProject;
        private static Compilation compilationVal2;
        private static Compilation compilationUnitTest;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            solution = Roslyn.GetSolution();

            valProject = solution.Projects.Single(x => x.Name == "Protocol");

#if NETFRAMEWORK
            testProject = solution.Projects.Single(x => x.Name == "ProtocolTests");
#elif NET
            testProject = solution.Projects.Single(x => x.Name == "ProtocolTests(net6.0)");
#endif

            // SDK style projects don't have each file mentioned in the csproj anymore
            valProject = valProject.WithAllSourceFiles();
            testProject = testProject.WithAllSourceFiles();

            compilationVal2 = valProject.GetCompilationAsync().Result;
            compilationUnitTest = testProject.GetCompilationAsync().Result;

            compilationVal2.SyntaxTrees.Should().NotBeEmpty();
            compilationUnitTest.SyntaxTrees.Should().NotBeEmpty();
        }

        [ClassCleanup]
        public static void ClassClean()
        {
            solution = null;

            valProject = null;
            testProject = null;

            compilationVal2 = null;
            compilationUnitTest = null;
        }

        [TestMethod]
        [Ignore("TODO")]
        public void Testing()
        {
            List<string> errors = new List<string>();

            foreach (var tree in compilationVal2.SyntaxTrees)
            {
                if (!tree.FilePath.Contains(Path.Combine("Skyline.DataMiner.CICD.Validators.Protocol", "Tests")))
                {
                    continue;
                }

                // Get Root
                var rootSyntaxNode = tree.GetRootAsync().Result;
                var analyzer = new TestAnalyzer(errors);

                RoslynVisitor visitor = new RoslynVisitor(analyzer);
                visitor.Visit(rootSyntaxNode);
            }

            if (errors.Count > 0)
            {
                Assert.Fail(String.Join(Environment.NewLine, errors));
            }
        }

        private class TestAnalyzer : CSharpAnalyzerBase
        {
            private readonly List<string> errors;

            public TestAnalyzer(List<string> errors)
            {
                this.errors = errors;
            }

            public override void CheckClass(ClassClass classClass)
            {
                if (CoverCheckClass(classClass))
                {
                    return;
                }

                if (classClass.Access == AccessModifier.Public)
                {
                    errors.Add($"'{classClass.Name}' has public access but shouldn't have.");
                }
            }

            private bool CoverCheckClass(ClassClass classClass)
            {
                if (!classClass.Name.StartsWith("Check") && !classClass.Name.StartsWith("CSharp"))
                {
                    return false;
                }

                if (classClass.Access != AccessModifier.Public)
                {
                    errors.Add($"'{classClass.Name}' has no public access.");
                }

                if (classClass.Attributes.All(attribute => attribute.Name != "Test"))
                {
                    errors.Add($"'{classClass.Name}' has no {nameof(TestAttribute)}.");
                }

                string[] interfaces =
                {
                    nameof(IValidate),
                    nameof(ICodeFix),
                    nameof(ICompare)
                };

                if (!classClass.InheritanceItems.Any(x => interfaces.Contains(x)))
                {
                    errors.Add($"'{classClass.Name}' has no valid interfaces.");
                }

                string[] methods = new[]
                {
                    nameof(IValidate.Validate),
                    nameof(ICodeFix.Fix),
                    nameof(ICompare.Compare)
                };

                var remainingMethods = classClass.Methods.Where(methodClass => !methods.Contains(methodClass.Name))
                                                 .Select(methodClass => methodClass.Name)
                                                 .ToList();
                if (remainingMethods.Any())
                {
                    errors.Add($"'{classClass.Name}' has extra methods: {String.Join(", ", remainingMethods)}.");
                }

                return true;
            }
        }

        /// <summary>
        /// Check if all the unit tests are correct.
        /// Will fail when an unit test is encountered that doesn't have any ExpectedResults.
        /// Will fail when an unit test is encountered that has results, but is ignored.
        /// Will fail when an unit test is encountered without the TestMethod attribute.
        /// </summary>
        [TestMethod]
        public void CheckUnitTests()
        {
            HashSet<string> ignoredTests = new HashSet<string>();
            HashSet<string> testsWithoutAttribute = new HashSet<string>();
            HashSet<string> emptyTests = new HashSet<string>();

            foreach (var tree in compilationUnitTest.SyntaxTrees)
            {
                if (!tree.FilePath.Contains(Path.Combine("ProtocolTests", "Protocol")))
                {
                    continue;
                }

                // Get Root
                var rootSyntaxNode = tree.GetRootAsync().Result;

                // Get Namespace
                string ns = Roslyn.GetNamespace(rootSyntaxNode);

                // Remove the first and second part of the namespace
                ns = ns.Replace("ProtocolTests.", String.Empty);

                // Find Validate/Compare/CodeFix Class
                foreach (ClassDeclarationSyntax @class in rootSyntaxNode.GetClasses("Validate", "Compare", "CodeFix"))
                {
                    // Find all the UnitTest Methods
                    foreach (MethodDeclarationSyntax method in @class.GetMethods())
                    {
                        bool isIgnored = false;
                        bool? isIgnoredWithReason = null;
                        bool isTestMethod = false;
                        bool hasErrorMessages = false;
                        bool isInvalidType = false;

                        // Check Attributes
                        foreach (AttributeSyntax attr in method.GetAttributes())
                        {
                            switch (attr.Name.ToString())
                            {
                                case "TestMethod":
                                case "DataTestMethod":
                                    isTestMethod = true;
                                    break;

                                case "Ignore":
                                    isIgnored = attr.ArgumentList?.Arguments.Count != 1;
                                    isIgnoredWithReason = !isIgnored;
                                    break;
                            }
                        }

                        // Check if using ErrorMessages
                        foreach (var item in method.DescendantNodes().OfType<AssignmentExpressionSyntax>())
                        {
                            var left = item.Left.ToString();
                            var right = item.Right.ToString();

                            if (String.Equals(left, "ExpectedResults") && item.Right is ObjectCreationExpressionSyntax list && list.Initializer?.Expressions.Any() == true)
                            {
                                hasErrorMessages = true;
                            }
                            else if (String.Equals(left, "TestType") && String.Equals(right, "Generic.TestType.Invalid"))
                            {
                                isInvalidType = true;
                            }
                        }

                        if (isTestMethod)
                        {
                            if (isIgnoredWithReason.GetValueOrDefault())
                            {
                                // Test is ignored for a reason. This needs to be checked manual.
                                continue;
                            }

                            if (hasErrorMessages && isIgnored)
                            {
                                ignoredTests.Add($"{ns}.{method.Identifier.Text}");
                            }

                            if (!hasErrorMessages && !isIgnored && isInvalidType)
                            {
                                emptyTests.Add($"{ns}.{method.Identifier.Text}");
                            }

                            // Doesn't have error messages and is ignored => New tests
                        }
                        else
                        {
                            testsWithoutAttribute.Add($"{ns}.{method.Identifier.Text}");
                        }
                    }
                }
            }

            StringBuilder sb = new StringBuilder();

            if (ignoredTests.Count > 0)
            {
                sb
                    .AppendLine($"{ignoredTests.Count} ignored UnitTests with ErrorMessages:")
                    .AppendLine(String.Join(Environment.NewLine, ignoredTests))
                    .AppendLine();
            }

            if (testsWithoutAttribute.Count > 0)
            {
                sb
                    .AppendLine($"{testsWithoutAttribute.Count} UnitTests without TestMethod attribute:")
                    .AppendLine(String.Join(Environment.NewLine, testsWithoutAttribute))
                    .AppendLine();
            }

            if (emptyTests.Count > 0)
            {
                sb
                    .AppendLine($"{emptyTests.Count} UnitTests without ErrorMessages:")
                    .AppendLine(String.Join(Environment.NewLine, emptyTests));
            }

            string message = sb.ToString().Trim();
            if (!String.IsNullOrWhiteSpace(message))
            {
                Assert.Fail($"{Environment.NewLine}{message}");
            }
        }

        /// <summary>
        /// Checks if all the error messages are being used in an unit test.
        /// Will fail when an error message is encountered that has no reference in an unit test.
        /// Will fail when an error message is encountered that has no references at all. (Needs to be improved on)
        /// TODO: Expand so it checks the ErrorMessages class.
        /// </summary>
        [TestMethod]
        public void CheckErrorMessages()
        {
            HashSet<string> unusedErrorMessages = new HashSet<string>();
            HashSet<string> untestedErrorMessages = new HashSet<string>();

            foreach (var tree in compilationVal2.SyntaxTrees)
            {
                if (!tree.FilePath.Contains(Path.Combine("Skyline.DataMiner.CICD.Validators.Protocol", "Error Messages")))
                {
                    continue;
                }

                // Get Semantic Model
                var semantic = compilationVal2.GetSemanticModel(tree);

                // Get Root
                var rootSyntaxNode = tree.GetRootAsync().Result;

                // Get Namespace
                string ns = Roslyn.GetNamespace(rootSyntaxNode);

                // Remove the first and second part of the namespace (So it starts with Protocol.)
                ns = ns.Replace("Skyline.DataMiner.CICD.Validators.Protocol.Tests.", String.Empty);

                string expectedClassName = ns.Substring(ns.LastIndexOf('.') + 1);

                // Find Error Class
                foreach (ClassDeclarationSyntax @class in rootSyntaxNode.GetClasses("Error"))
                {
                    // Find all the ErrorMessage Methods
                    foreach (MethodDeclarationSyntax method in @class.GetMethods())
                    {
                        ISymbol symbol = Roslyn.GetSymbol(semantic, method);
                        var callers = SymbolFinder.FindCallersAsync(symbol, solution).Result;

                        foreach (var reference in callers)
                        {
                            string errorMessageFullPath = $"{ns}.{method.Identifier.Text}";

                            if (!reference.Locations.Any())
                            {
                                // Check if corresponding class has TestAttribute
                                // Find class in the same namespace with the correct name and see if it has correct attribute(s)
                                ClassDeclarationSyntax checkClass = Roslyn.FindClass(compilationVal2, ns, expectedClassName);

                                if (checkClass == null)
                                {
                                    unusedErrorMessages.Add(errorMessageFullPath);
                                }
                                else
                                {
                                    // Find TestAttribute
                                    bool hasTestAttribute = false;
                                    foreach (AttributeSyntax attr in checkClass.GetAttributes())
                                    {
                                        switch (attr.Name.ToString())
                                        {
                                            case "Test":
                                                hasTestAttribute = true;
                                                break;
                                        }
                                    }

                                    if (hasTestAttribute)
                                    {
                                        unusedErrorMessages.Add(errorMessageFullPath);
                                    }
                                }
                            }

                            foreach (var location in reference.Locations)
                            {
                                // Check if reference is in UnitTest project
                                if (!reference.CallingSymbol.ContainingAssembly.Name.StartsWith(testProject.DefaultNamespace))
                                {
                                    continue;
                                }

                                var testMethods = location.SourceTree.GetRoot().DescendantNodes(location.SourceSpan).OfType<MethodDeclarationSyntax>().ToList();

                                foreach (MethodDeclarationSyntax testMethod in testMethods)
                                {
                                    // Check if it has TestMethod attribute and not the Ignore attribute?
                                    bool isTestMethod = false;
                                    bool isIgnored = false;

                                    foreach (AttributeSyntax attr in testMethod.GetAttributes())
                                    {
                                        switch (attr.Name.ToString())
                                        {
                                            case "TestMethod":
                                            case "DataTestMethod":
                                                isTestMethod = true;
                                                break;

                                            case "Ignore":
                                                // Ignore with argument means that there is a reason why it's ignored.
                                                // These need to be looked at manually.
                                                isIgnored = attr.ArgumentList?.Arguments.Count != 1;
                                                break;
                                        }
                                    }

                                    if (isIgnored || !isTestMethod)
                                    {
                                        untestedErrorMessages.Add(errorMessageFullPath);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            StringBuilder sb = new StringBuilder();

            if (untestedErrorMessages.Count > 0)
            {
                sb
                    .AppendLine($"{untestedErrorMessages.Count} ErrorMessages that aren't tested:")
                    .AppendLine(String.Join(Environment.NewLine, untestedErrorMessages))
                    .AppendLine();
            }

            if (unusedErrorMessages.Count > 0)
            {
                sb
                    .AppendLine($"{unusedErrorMessages.Count} ErrorMessages that aren't used anywhere:")
                    .AppendLine(String.Join(Environment.NewLine, unusedErrorMessages));
            }

            string message = sb.ToString().Trim();
            if (!String.IsNullOrWhiteSpace(message))
            {
                Assert.Fail($"{Environment.NewLine}{message}");
            }
        }

        /// <summary>
        /// Checks the checks themselves.
        /// Will fail when a check is encountered that has no Test attribute but has one or more error messages.
        /// Will fail when a check is encountered that doesn't match the name from the namespace.
        /// [Disabled] Will fail when a check is encountered that has a Test attribute but has no error messages.
        /// </summary>
        [TestMethod]
        public void CheckChecks()
        {
            HashSet<string> inactiveChecks = new HashSet<string>();
            HashSet<string> wrongNamedClasses = new HashSet<string>();
            HashSet<string> emptyChecks = new HashSet<string>();

            foreach (var tree in compilationVal2.SyntaxTrees)
            {
                if (!tree.FilePath.Contains(Path.Combine("Protocol", "Tests")))
                {
                    continue;
                }

                // Get Root
                var rootSyntaxNode = tree.GetRootAsync().Result;

                // Get Namespace
                string ns = Roslyn.GetNamespace(rootSyntaxNode);

                // Remove the first and second part of the namespace (So it starts with Protocol.)
                ns = ns.Replace("Skyline.DataMiner.CICD.Validators.Protocol.Tests.", String.Empty);

                string expectedClassName = ns.Substring(ns.LastIndexOf('.') + 1);

                // Find Check Class
                bool foundClass = false;
                foreach (ClassDeclarationSyntax @class in rootSyntaxNode.GetClasses(expectedClassName))
                {
                    foundClass = true;

                    // See if any have the TestAttribute
                    bool hasTestAttribute = false;
                    foreach (AttributeSyntax attr in @class.GetAttributes())
                    {
                        switch (attr.Name.ToString())
                        {
                            case "Test":
                                hasTestAttribute = true;
                                break;
                        }
                    }

                    bool hasMessages = false;
                    foreach (var node in @class.DescendantNodes().OfType<InvocationExpressionSyntax>())
                    {
                        if (node.Expression is MemberAccessExpressionSyntax maes && maes.Expression is IdentifierNameSyntax ins &&
                            (ins.Identifier.Text == "Error" || ins.Identifier.Text == "ErrorCompare"))
                        {
                            hasMessages = true;
                            break;
                        }
                    }

                    if (hasMessages && !hasTestAttribute)
                    {
                        inactiveChecks.Add(ns);
                    }

                    //if (!hasMessages && hasTestAttribute)
                    //{
                    //    emptyChecks.Add(ns);
                    //}
                }

                if (!foundClass)
                {
                    wrongNamedClasses.Add(ns);
                }
            }

            StringBuilder sb = new StringBuilder();

            if (inactiveChecks.Count > 0)
            {
                sb
                    .AppendLine($"{inactiveChecks.Count} Inactive Checks:")
                    .AppendLine(String.Join(Environment.NewLine, inactiveChecks))
                    .AppendLine();
            }

            if (emptyChecks.Count > 0)
            {
                sb
                    .AppendLine($"{emptyChecks.Count} Empty Checks:")
                    .AppendLine(String.Join(Environment.NewLine, emptyChecks))
                    .AppendLine();
            }

            if (wrongNamedClasses.Count > 0)
            {
                sb
                    .AppendLine($"{wrongNamedClasses.Count} WrongNamed Checks:")
                    .AppendLine(String.Join(Environment.NewLine, wrongNamedClasses));
            }

            string message = sb.ToString().Trim();
            if (!String.IsNullOrWhiteSpace(message))
            {
                Assert.Fail($"{Environment.NewLine}{message}");
            }
        }

        /// <summary>
        /// Checks if there should be a code fix and checks if there is an unit test for it.
        /// </summary>
        [TestMethod]
        [Ignore("Currently this one is giving errors (fix is ignored) on CodeFixes that aren't implemented yet.")]
        public void CheckUnitTestsForCodeFix()
        {
            HashSet<string> ignoredUnitTest = new HashSet<string>();
            HashSet<string> notUnitTest = new HashSet<string>();
            HashSet<string> messagesNotTested = new HashSet<string>();

            var docsUnitTest = testProject.Documents.ToImmutableHashSet();
            foreach (var tree in compilationVal2.SyntaxTrees)
            {
                if (!tree.FilePath.Contains(Path.Combine("Protocol", "Error Messages")))
                {
                    continue;
                }

                // Get Semantic Model
                var semantic = compilationVal2.GetSemanticModel(tree);

                // Get Root
                var rootSyntaxNode = tree.GetRootAsync().Result;

                // Get Namespace
                string ns = Roslyn.GetNamespace(rootSyntaxNode);

                // Remove the first and second part of the namespace
                ns = ns.Replace("Skyline.DataMiner.CICD.Validators.Protocol.Tests.", String.Empty);

                // Find Error Class
                foreach (ClassDeclarationSyntax @class in rootSyntaxNode.GetClasses("Error"))
                {
                    // Find all the UnitTest Methods
                    foreach (MethodDeclarationSyntax method in @class.GetMethods())
                    {
                        if (!CodeFixRoslyn.HasCodeFix(method))
                        {
                            continue;
                        }

                        ISymbol symbol = Roslyn.GetSymbol(semantic, method);

                        // Only search in UnitTests
                        var references = SymbolFinder.FindReferencesAsync(symbol, solution, docsUnitTest).Result;

                        foreach (var reference in references)
                        {
                            if (!reference.Locations.Any())
                            {
                                continue;
                            }

                            foreach (var location in reference.Locations)
                            {
                                var unitTestMethods = location.Location.SourceTree.GetRoot().DescendantNodesAndSelf(location.Location.SourceSpan).OfType<MethodDeclarationSyntax>();

                                var semanticUnitTest = compilationUnitTest.GetSemanticModel(location.Location.SourceTree);

                                // Prepare list with current implemented code fixes.
                                List<string> existingCodeFixes = new List<string>();
                                foreach (ClassDeclarationSyntax unitTestClass in location.Location.SourceTree.GetRoot().GetClasses())
                                {
                                    if (!String.Equals(unitTestClass.Identifier.Text, "CodeFix"))
                                    {
                                        continue;
                                    }

                                    foreach (MethodDeclarationSyntax codeFixMethod in unitTestClass.GetMethods())
                                    {
                                        bool isNewWay = codeFixMethod.ExpressionBody == null;

                                        string fileName = null;

                                        if (isNewWay)
                                        {
                                            foreach (var item in codeFixMethod.DescendantNodes().OfType<AssignmentExpressionSyntax>())
                                            {
                                                var left = item.Left.ToString();

                                                if (String.Equals(left, "FileNameBase"))
                                                {
                                                    fileName = item.Right.ToString().Replace("\"", String.Empty).Replace(".xml", String.Empty);
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // FileName is based on name
                                            fileName = codeFixMethod.Identifier.Text.Split('_').Last();
                                        }

                                        bool isTestMethod = false;
                                        bool isIgnored = false;

                                        foreach (AttributeSyntax attr in codeFixMethod.GetAttributes())
                                        {
                                            var name = semanticUnitTest.GetTypeInfo(attr).Type.Name;

                                            switch (name)
                                            {
                                                case "TestMethod":
                                                case "DataTestMethod":
                                                    isTestMethod = true;
                                                    break;

                                                case "Ignore":
                                                    // Ignore with argument means that there is a reason why it's ignored.
                                                    // These need to be looked at manually.
                                                    isIgnored = attr.ArgumentList?.Arguments.Count != 1;
                                                    break;
                                            }

                                            if (isIgnored && isTestMethod)
                                            {
                                                ignoredUnitTest.Add(codeFixMethod.Identifier.Text);
                                            }

                                            if (isTestMethod)
                                            {
                                                existingCodeFixes.Add(fileName);
                                            }
                                            else
                                            {
                                                notUnitTest.Add(codeFixMethod.Identifier.Text);
                                            }
                                        }
                                    }
                                }

                                foreach (MethodDeclarationSyntax testMethod in unitTestMethods)
                                {
                                    bool isNewWay = testMethod.ExpressionBody == null;

                                    string fileName = null;

                                    if (isNewWay)
                                    {
                                        foreach (var item in testMethod.DescendantNodes().OfType<AssignmentExpressionSyntax>())
                                        {
                                            var left = item.Left.ToString();

                                            if (String.Equals(left, "FileName"))
                                            {
                                                fileName = item.Right.ToString().Replace("\"", String.Empty).Replace(".xml", String.Empty);
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // FileName is based on name
                                        fileName = testMethod.Identifier.Text.Split('_').Last();
                                    }

                                    // Check if CodeFix unittest exists with that FileName as name or inside the method
                                    if (!existingCodeFixes.Contains(fileName))
                                    {
                                        string text = $"{ns}.{method.Identifier.Text}";
                                        messagesNotTested.Add(text);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            StringBuilder sb = new StringBuilder();

            if (notUnitTest.Count > 0)
            {
                // Error messages that are being used in the Unittest project, but not in a test method.
                sb.AppendLine("CodeFix Methods without [TestMethod] attribute:")
                    .AppendLine(String.Join(Environment.NewLine, notUnitTest))
                    .AppendLine();
            }

            if (ignoredUnitTest.Count > 0)
            {
                // Testmethods with Error messages that are ignored.
                sb.AppendLine("Ignored CodeFix Test methods that have a CodeFix:")
                    .AppendLine(String.Join(Environment.NewLine, ignoredUnitTest))
                    .AppendLine();
            }

            if (messagesNotTested.Count > 0)
            {
                sb.AppendLine("CodeFix error messages that aren't unittested yet:")
                    .AppendLine(String.Join(Environment.NewLine, messagesNotTested));
            }

            string result = sb.ToString().Trim();
            if (!String.IsNullOrWhiteSpace(result))
            {
                Assert.Fail($"{Environment.NewLine}{result}");
            }
        }

        /// <summary>
        /// Checks if the .xml files in the project are used in UnitTests or not.
        /// Will fail when an unit test is encountered that doesn't have a file.
        /// Will fail when a file is encountered that doesn't have an unit test.
        /// </summary>
        [TestMethod]
        public void CheckFiles()
        {
            List<string> missingFiles = new List<string>();
            List<string> foundFiles = new List<string>();

            // Search for Protocol directory which is the root folder of all unit tests
            if (!Files.TryGetProtocolDirectory(out DirectoryInfo protocolDirectory))
            {
                Assert.Fail("Protocol folder not found.");
            }

            // Get all XML files
            List<string> allFiles = protocolDirectory
                .GetFiles("*.xml", SearchOption.AllDirectories)
                .Where(x => !x.Name.EndsWith("_Results.xml")) // Maybe add .xml? Need to check
                .Select(x => x.FullName.Replace($"{protocolDirectory.Parent.FullName}\\", String.Empty)) // Remove C://... part. Only keep the part starting with Protocol
                .ToList();

            allFiles.Should().NotBeEmpty();

            foreach (var tree in compilationUnitTest.SyntaxTrees)
            {
                if (!tree.FilePath.Contains(Path.Combine("ProtocolTests", "Protocol")))
                {
                    continue;
                }

                // Get Root
                var rootSyntaxNode = tree.GetRootAsync().Result;

                // Get Namespace
                string ns = Roslyn.GetNamespace(rootSyntaxNode);

                // Remove the first part of the namespace
                ns = ns.Replace("ProtocolTests.", String.Empty);
                string nsFolder = ns.Replace('.', '\\');

                // Find Validate/Compare/CodeFix Class
                foreach (ClassDeclarationSyntax @class in rootSyntaxNode.GetClasses("Validate", "Compare", "CodeFix"))
                {
                    bool isValidate = String.Equals(@class.Identifier.Text, "Validate");
                    bool isCompare = String.Equals(@class.Identifier.Text, "Compare");
                    bool isCodeFix = String.Equals(@class.Identifier.Text, "CodeFix");

                    // Find all the UnitTest Methods
                    IEnumerable<MethodDeclarationSyntax> methodsInClass = @class.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
                    methodsInClass.Should().NotBeEmpty();
                    foreach (MethodDeclarationSyntax method in methodsInClass)
                    {
                        string testCategory = String.Empty;
                        bool isNewWay = method.ExpressionBody == null;

                        // Check Attributes
                        foreach (AttributeSyntax attr in method.GetAttributes())
                        {
                            switch (attr.Name.ToString())
                            {
                                case "TestCategory":
                                    testCategory = attr.ArgumentList.Arguments[0].ToString().Replace("\"", String.Empty);
                                    break;
                            }
                        }

                        string fileName = String.Empty;
                        if (isNewWay)
                        {
                            foreach (var item in method.DescendantNodes().OfType<AssignmentExpressionSyntax>())
                            {
                                var left = item.Left.ToString();

                                if (String.Equals(left, "FileName") || String.Equals(left, "FileNameBase"))
                                {
                                    fileName = item.Right.ToString().Replace("\"", String.Empty);
                                    break;
                                }
                                else if (String.Equals(left, "TestType"))
                                {
                                    testCategory = item.Right.ToString().Replace("Generic.TestType.", String.Empty);
                                }
                            }
                        }
                        else
                        {
                            // FileName is based on name
                            fileName = method.Identifier.Text.Split('_').Last();
                        }

                        // Check extension
                        if (isValidate)
                        {
                            fileName = fileName.EndsWith(".xml") ? fileName : $"{fileName}.xml";

                            // Create full path
                            string fullFilePath = Path.Combine(nsFolder, "Samples", @class.Identifier.Text, testCategory, fileName);

                            if (allFiles.Contains(fullFilePath))
                            {
                                foundFiles.Add(fullFilePath);
                            }
                            else
                            {
                                // File doesn't exists
                                missingFiles.Add(fullFilePath);
                            }
                        }
                        else if (isCompare)
                        {
                            string oldFile = $"{fileName}_Old.xml";
                            string newFile = $"{fileName}_New.xml";

                            // Create full path
                            string fullFilePathOld = Path.Combine(nsFolder, "Samples", @class.Identifier.Text, testCategory, oldFile);
                            string fullFilePathNew = Path.Combine(nsFolder, "Samples", @class.Identifier.Text, testCategory, newFile);

                            if (allFiles.Contains(fullFilePathOld))
                            {
                                foundFiles.Add(fullFilePathOld);
                            }
                            else
                            {
                                // File doesn't exists
                                missingFiles.Add(fullFilePathOld);
                            }

                            if (allFiles.Contains(fullFilePathNew))
                            {
                                foundFiles.Add(fullFilePathNew);
                            }
                            else
                            {
                                // File doesn't exists
                                missingFiles.Add(fullFilePathNew);
                            }
                        }
                        else if (isCodeFix)
                        {
                            fileName = fileName.EndsWith(".xml") ? fileName : $"{fileName}.xml";

                            // Create full path
                            string fullFilePath = Path.Combine(nsFolder, "Samples", @class.Identifier.Text, fileName);

                            if (String.Equals(@class.Identifier.Text, "CodeFix"))
                            {
                                fullFilePath = fullFilePath.Replace("CodeFix", "Codefix");
                            }

                            if (allFiles.Contains(fullFilePath))
                            {
                                foundFiles.Add(fullFilePath);
                            }
                            else
                            {
                                // File doesn't exists
                                missingFiles.Add(fullFilePath);
                            }
                        }
                    }
                }
            }

            StringBuilder sb = new StringBuilder();

            if (missingFiles.Count > 0)
            {
                sb.AppendLine($"{missingFiles.Count} Files that are used in unit tests but don't exist:")
                    .AppendLine(String.Join(Environment.NewLine, missingFiles))
                    .AppendLine();
            }

            List<string> excessiveFiles = allFiles.Except(foundFiles).ToList();

            if (excessiveFiles.Count > 0)
            {
                sb.AppendLine($"{excessiveFiles.Count} Files that aren't used in any unit test:")
                    .AppendLine(String.Join(Environment.NewLine, excessiveFiles));
            }

            string result = sb.ToString().Trim();
            if (!String.IsNullOrWhiteSpace(result))
            {
                Assert.Fail($"{Environment.NewLine}{result}");
            }
        }

        /// <summary>
        /// Checks if the .xml files in the project are using the correct XSD.
        /// Will fail when a file doesn't have a XSD linked to it.
        /// Will fail when a file doesn't have the correct XSD linked to it.
        /// Will fail when a file has unknown tags or attributes
        /// </summary>
        [TestMethod]
        public void CheckFiles_Xsd()
        {
            /* 
             * Different unit tests on which we don't want to validate via xsd:
             * - Some use the 'Protocol/Connections' tag which we don't want to add to xsd because it should remain unknown to most developer but still needs to be used in some rare cases.
             * - Some where we deliberately add xsd mistakes cause those also need to be covered by the Validator.
             */
            string[] filesToSkip =
            {
                @"Protocol\CheckConnections\Samples\Compare\Valid\Valid_Syntax1To2_New.xml",
                @"Protocol\CheckConnections\Samples\Compare\Valid\Valid_Syntax1To3_New.xml",
                @"Protocol\CheckConnections\Samples\Compare\Valid\Valid_Syntax2To1_Old.xml",
                @"Protocol\CheckConnections\Samples\Compare\Valid\Valid_Syntax3To1_Old.xml",
                @"Protocol\CheckConnections\Samples\Validate\Invalid\InvalidCombinationOfSyntax1And2.xml",
                @"Protocol\CheckConnections\Samples\Validate\Invalid\UnrecommendedSyntax2.xml",
                @"Protocol\Groups\Group\Content\CheckContentTag\Samples\Validate\Invalid\MixedTypes.xml",
                @"Protocol\Triggers\Trigger\Content\Id\CheckIdTag\Samples\Validate\Invalid\MissingTag.xml",
                @"Protocol\Type\CheckTypeTag\Samples\Validate\Valid\Valid_OtherSyntax.xml"
            };

            const string NAMESPACE = "http://www.skyline.be/validatorProtocolUnitTest";

            List<string> filesWithMissingXsd = new List<string>();
            List<string> filesWithInvalidXsd = new List<string>();
            List<string> filesWithXsdErrors = new List<string>();

            // Search for Protocol directory which is the root folder of all unit tests
            if (!Files.TryGetProtocolDirectory(out DirectoryInfo protocolDirectory))
            {
                Assert.Fail("Protocol folder not found.");
            }

            // Get all XML files
            List<string> allFiles = protocolDirectory
                .GetFiles("*.xml", SearchOption.AllDirectories)
                .Select(x => x.FullName)
                .ToList();

            allFiles.Should().NotBeEmpty();

            var xsds = protocolDirectory.Parent.GetFiles("*.xsd").ToList();

            if (xsds.Count != 1)
            {
                Assert.Fail("Multiple XSD files found.");
            }

            var settings = new XmlReaderSettings();

            XmlSchemaSet schemaSet = new XmlSchemaSet
            {
                XmlResolver = new XmlUrlResolver()
            };
            schemaSet.Add(NAMESPACE, xsds.First().FullName);
            settings.Schemas.Add(schemaSet);

            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += Settings_ValidationEventHandler;


            foreach (var filePath in allFiles)
            {
                string readablePath = filePath.Replace($"{protocolDirectory.Parent.FullName}\\", String.Empty);
                if (readablePath == @"Protocol\CheckProtocolTag\Samples\Validate\Invalid\MissingTag.xml")
                {
                    // Hasn't a Protocol tag, so no point in checking.
                    continue;
                }

                if (filesToSkip.Contains(readablePath))
                {
                    continue;
                }

                (bool success, Stream stream) = Files.ReadTextFromFile(filePath);
                if (!success)
                {
                    Assert.Fail("Failed to retrieve the file: " + filePath);
                }

                try
                {
                    using (var reader = XmlReader.Create(stream, settings))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name != "Protocol" || reader.NodeType == XmlNodeType.EndElement)
                            {
                                // Let it continue the reading => Checking the XSD.
                                continue;
                            }

                            string ns = reader.NamespaceURI;

                            if (String.IsNullOrWhiteSpace(ns))
                            {
                                filesWithMissingXsd.Add(readablePath);
                                continue;
                            }

                            if (!String.Equals(ns, NAMESPACE))
                            {
                                filesWithInvalidXsd.Add(readablePath);
                            }
                        }
                    }
                }
                catch (InvalidDataException)
                {
                    filesWithXsdErrors.Add(readablePath);
                }
                catch (Exception e)
                {
                    filesWithMissingXsd.Add("BROKEN|" + e.Message + "|" + readablePath);
                }
            }

            StringBuilder sb = new StringBuilder();

            if (filesWithMissingXsd.Count > 0)
            {
                sb.AppendLine($"{filesWithMissingXsd.Count} Files that don't have an XSD:")
                    .AppendLine(" - " + String.Join(Environment.NewLine + " - ", filesWithMissingXsd))
                    .AppendLine();
            }

            if (filesWithInvalidXsd.Count > 0)
            {
                sb.AppendLine($"{filesWithInvalidXsd.Count} Files that have a wrong XSD:")
                    .AppendLine(" - " + String.Join(Environment.NewLine + " - ", filesWithInvalidXsd))
                    .AppendLine();
            }

            if (filesWithXsdErrors.Count > 0)
            {
                sb.AppendLine($"{filesWithXsdErrors.Count} Files that have XSD errors (unknown tags/attributes):")
                    .AppendLine(" - " + String.Join(Environment.NewLine + " - ", filesWithXsdErrors));
            }

            string result = sb.ToString().Trim();
            if (!String.IsNullOrWhiteSpace(result))
            {
                Assert.Fail($"{Environment.NewLine}{result}");
            }
        }

        private void Settings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            string[] errorsToCatch = new[]
            {
                "has invalid child element", // Unknown tag
                "attribute is not declared", // Unknown attribute
            };

            if (!errorsToCatch.Any(x => e.Message.Contains(x)))
            {
                return;
            }

            throw new InvalidDataException();
        }
    }

    internal static class CodeFixRoslyn
    {
        internal static bool HasCodeFix(MethodDeclarationSyntax method)
        {
            foreach (var item in method.DescendantNodes().OfType<AssignmentExpressionSyntax>())
            {
                var left = item.Left.ToString();

                if (String.Equals(left, "HasCodeFix"))
                {
                    Boolean.TryParse(item.Right.ToString(), out bool hasCodeFix);
                    return hasCodeFix;
                }
            }

            return false;
        }
    }

    internal static class Roslyn
    {
        internal static IEnumerable<ClassDeclarationSyntax> GetClasses(this SyntaxNode node)
        {
            return node.DescendantNodes().OfType<ClassDeclarationSyntax>();
        }

        internal static IEnumerable<ClassDeclarationSyntax> GetClasses(this SyntaxNode node, params string[] classNames)
        {
            return node.DescendantNodes().OfType<ClassDeclarationSyntax>().Where(x => classNames.Contains(x.Identifier.Text));
        }

        internal static IEnumerable<MethodDeclarationSyntax> GetMethods(this ClassDeclarationSyntax @class)
        {
            return @class.DescendantNodes().OfType<MethodDeclarationSyntax>();
        }

        internal static IEnumerable<AttributeSyntax> GetAttributes(this MethodDeclarationSyntax method)
        {
            return method.DescendantNodes().OfType<AttributeSyntax>();
        }

        internal static IEnumerable<AttributeSyntax> GetAttributes(this ClassDeclarationSyntax method)
        {
            return method.DescendantNodes().OfType<AttributeSyntax>();
        }

        internal static Solution GetSolution()
        {
            try
            {
                string solutionPath = GetSolutionPath();

                // Creating a build workspace.
                var workspace = MSBuildWorkspace.Create();

                // Opening the solution.
                Solution solution = workspace.OpenSolutionAsync(solutionPath).Result;

                return solution;
            }
            catch (ReflectionTypeLoadException tle)
            {
                string text = String.Join(";", tle.LoaderExceptions.Select(x => x.Message));
                throw new Exception($"ReflectionTypeLoadException with these LoaderExceptions:{Environment.NewLine}{text}");
            }
        }

        private static string GetSolutionPath()
        {
            string solutionPath = String.Empty;
            DirectoryInfo a = Directory.GetParent(Assembly.GetExecutingAssembly().Location);
            while (solutionPath == String.Empty)
            {
                var temp = a.GetDirectories("Skyline.DataMiner.CICD.Validators", SearchOption.TopDirectoryOnly);
                if (temp.Length == 1)
                {
                    var files = temp[0].GetFiles("*.sln");
                    if (files.Length > 0)
                    {
                        solutionPath = files[0].FullName;
                    }
                    else
                    {
                        a = a.Parent;
                    }
                }
                else
                {
                    a = a.Parent;
                }
            }

            return solutionPath;
        }

        internal static string GetNamespace(SyntaxNode rootSyntaxNode)
        {
            var temp = rootSyntaxNode.DescendantNodes().OfType<NamespaceDeclarationSyntax>().ToList();

            if (temp == null || !temp.Any())
            {
                return String.Empty;
            }

            return temp[0].Name?.ToString() ?? String.Empty;
        }

        internal static ISymbol GetSymbol(SemanticModel semantic, MethodDeclarationSyntax method)
        {
            return semantic.GetSymbolInfo(method).Symbol ?? semantic.GetDeclaredSymbol(method);
        }

        internal static ClassDeclarationSyntax FindClass(Compilation compilationVal2, string ns, string expectedClassName)
        {
            foreach (var tree in compilationVal2.SyntaxTrees)
            {
                var rootSyntaxNode = tree.GetRootAsync().Result;

                if (!GetNamespace(rootSyntaxNode).Contains(ns))
                {
                    continue;
                }

                var classes = rootSyntaxNode.GetClasses(expectedClassName).ToList();

                if (classes.Count == 1)
                {
                    return classes[0];
                }
            }

            return null;
        }
    }

    internal static class Files
    {
        public static bool TryGetProtocolDirectory(out DirectoryInfo protocolDirectory)
        {
            bool found = false;

            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            protocolDirectory = Directory.GetParent(currentDirectory);
            while (!found)
            {
                var temp = protocolDirectory.GetDirectories("Protocol", SearchOption.TopDirectoryOnly);
                if (temp.Length == 1)
                {
                    // Found protocol folder
                    protocolDirectory = temp[0];
                    found = true;
                }
                else
                {
                    protocolDirectory = protocolDirectory.Parent;
                }
            }

            return found;
        }

        public static (bool success, Stream stream) ReadTextFromFile(string pathToFile)
        {
            try
            {
                pathToFile = @"\\?\" + pathToFile;

                var fileStream = new FileStream(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                return (true, fileStream);
            }
            catch (FileNotFoundException)
            {
                throw;
            }
        }
    }

    internal static class ProjectExtensions
    {
        private static Project AddDocuments(this Project project, IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                project = project.AddDocument(file, File.ReadAllText(file)).Project;
            }
            return project;
        }

        private static IEnumerable<string> GetAllSourceFiles(string directoryPath)
        {
            var res = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);

            return res;
        }

        public static Project WithAllSourceFiles(this Project project)
        {
            string projectDirectory = Directory.GetParent(project.FilePath).FullName;
            var files = GetAllSourceFiles(projectDirectory);
            var newProject = project.AddDocuments(files);
            return newProject;
        }
    }
}