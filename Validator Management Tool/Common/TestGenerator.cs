namespace Validator_Management_Tool.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;

    using Validator_Management_Tool.Model;
    using Validator_Management_Tool.Templates.Error_Messages;
    using Validator_Management_Tool.Templates.Tests;
    using Validator_Management_Tool.Templates.Unit_Tests;

    /// <summary>
    /// Static class that contains all the methods to generate the classes and write them to files.
    /// </summary>
    public static class TestGenerator
    {
        /// <summary>
        /// String builder that holds all the code from the different classes, when the single file option for the error message classes is checked.
        /// </summary>
        private static StringBuilder singleFile = new StringBuilder();

        /// <summary>
        /// Generates test, error messages and unit test classes.
        /// </summary>
        /// <param name="checks">All the checks for which the classes needs to be generated.</param>
        public static void GenerateFiles(ObservableCollection<Check> checks)
        {
            DeletePreviousErrorMessageClasses();

            // Generate the Test classes
            List<string> namespaces = new List<string>();
            foreach (var check in checks)
            {
                if (!namespaces.Contains(check.FullNamespace))
                {
                    namespaces.Add(check.FullNamespace);
                    CreateTestClass(check);
                }
            }

            // Generate the error message classes and unit test classes
            foreach (var ns in namespaces)
            {
                List<Check> currentChecks = new List<Check>();
                foreach (var check in checks)
                {
                    if (check.FullNamespace == ns)
                    {
                        currentChecks.Add(check);
                    }
                }

                CreateErrorClass(currentChecks);
                CreateUnitTestClass(currentChecks);
                CreateExtraDetailsMarkdownFile(currentChecks);
            }

            if (Settings.ErrorClassesInOneFile)
            {
                CreateFile("ErrorMessages", singleFile.ToString(), Settings.ErrorMessagesPath, "Error Messages/", "Error Messages\\", true, false);
            }
        }

        /// <summary>
        /// Creates a markdown file that can be used to add additional information about the check such as extra details and example code.
        /// </summary>
        /// <param name="currentChecks">The current checks.</param>
        private static void CreateExtraDetailsMarkdownFile(List<Check> currentChecks)
        {
            foreach (var check in currentChecks)
            {
                // Do nothing if file already exists.
                string source = check.Source.ToString();
                string namespacePath = String.Join("/", check.Namespace.Split('.'));
                string checkName = check.CheckName;

                var categoryId = check.CategoryId;
                var checkId = check.CheckId;
                var errorMessageId = check.ErrorId;
                string uid = $"{source}_{categoryId}_{checkId}_{errorMessageId}";

                string directoryPath = $"{Settings.DocumentationMarkdownFilesPath}/checks/{source}/{namespacePath}/{checkName}";
                string filePath = $"{directoryPath}/{uid}.md";

                if (!File.Exists(filePath))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("---");
                    stringBuilder.AppendLine($"uid: {uid}");
                    stringBuilder.AppendLine("---");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine($"# {check.CheckName}");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine($"## {check.Name}");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("<!-- Description, Properties, ... sections are auto-generated. -->");
                    stringBuilder.AppendLine("<!-- REPLACE ME AUTO-GENERATION -->");
                    stringBuilder.AppendLine();

                    if (String.IsNullOrWhiteSpace(check.Details))
                    {
                        stringBuilder.AppendLine("<!-- Uncomment to add extra details -->");
                        stringBuilder.AppendLine("<!--### Details-->");
                    }
                    else
                    {
                        stringBuilder.AppendLine("### Details");
                        stringBuilder.AppendLine();
                        stringBuilder.AppendLine(check.Details);
                    }

                    stringBuilder.AppendLine();

                    if (String.IsNullOrWhiteSpace(check.ExampleCode))
                    {
                        stringBuilder.AppendLine("<!-- Uncomment to add example code -->");
                        stringBuilder.AppendLine("<!--### Example code-->");
                    }
                    else
                    {
                        stringBuilder.AppendLine("### Example code");
                        stringBuilder.AppendLine();
                        stringBuilder.AppendLine(check.ExampleCode);
                    }

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    File.WriteAllText(filePath, stringBuilder.ToString());
                }
            }
        }

        /// <summary>
        /// Deletes the already existing directory with the error messages.
        /// </summary>
        private static void DeletePreviousErrorMessageClasses()
        {
            if (!Directory.Exists(Settings.ErrorMessagesPath + "Error Messages/"))
            {
                return;
            }

            Directory.Delete(Settings.ErrorMessagesPath + "Error Messages/", true);
        }

        /// <summary>
        /// Creates a .cs file and makes the correct folder structure.
        /// </summary>
        /// <param name="namespace">The whole namespace of the file.</param>
        /// <param name="generatedCode">The code that has to be added to the file.</param>
        /// <param name="baseDirectoryPath">The base of the path where the file has to be added.</param>
        /// <param name="baseDirectorySuffix">The first folder within the directory path were the file has to be added.</param>
        /// <param name="baseConfigFileName">The base of the path to add the file to the .csproj file, split by backslashes.</param>
        /// <param name="overwrite">If true, the existing file will be overwritten.</param>
        /// <param name="unitTest">If true, the file is a unit test class and a different folder structure will be created.</param>
        private static void CreateFile(string @namespace, string generatedCode, string baseDirectoryPath, string baseDirectorySuffix, string baseConfigFileName, bool overwrite, bool unitTest, List<Check> checks = null)
        {
            var splitNamespace = @namespace.Split(new string[] { "." }, StringSplitOptions.None);

            StringBuilder directoryPath = new StringBuilder(baseDirectoryPath + baseDirectorySuffix);
            StringBuilder configFileName = new StringBuilder(baseConfigFileName);

            string className = splitNamespace[splitNamespace.Length - 1];

            for (int i = 0; i < splitNamespace.Length - 1; ++i)
            {
                directoryPath.Append(splitNamespace[i] + "/");
                configFileName.Append(splitNamespace[i] + "\\");
            }

            string filePath;
            if (unitTest)
            {
                directoryPath.Append(className + "/");
                configFileName.Append(className + "\\");

                Directory.CreateDirectory(directoryPath.ToString());
                filePath = directoryPath + className + ".cs";

                var codefixDir = Directory.CreateDirectory(directoryPath.ToString() + "Samples/Codefix/");

                var invalidCompareDir = Directory.CreateDirectory(directoryPath.ToString() + "Samples/Compare/Invalid/");
                var validCompareDir = Directory.CreateDirectory(directoryPath.ToString() + "Samples/Compare/Valid/");

                var invalidValidateDir = Directory.CreateDirectory(directoryPath.ToString() + "Samples/Validate/Invalid/");
                var validValidateDir = Directory.CreateDirectory(directoryPath.ToString() + "Samples/Validate/Valid/");

                if (checks != null)
                {
                    // Create default xml file
                    foreach (var check in checks)
                    {
                        if (check.Name.EndsWith("_Sub"))
                        {
                            // Ignore the ones that are specifically for SubErrors
                            continue;
                        }

                        string[] asNamespace = check.Namespace.Split('.');
                        StringBuilder sb = new StringBuilder();
                        int index = 0;
                        for (int i = 0; i < asNamespace.Length; i++, index++)
                        {
                            for (int j = 0; j < index; j++)
                            {
                                sb.Append("\t");
                            }

                            if (String.Equals(asNamespace[i], "Protocol"))
                            {
                                sb.AppendLine("<" + asNamespace[i] + " xmlns=\"http://www.skyline.be/validatorProtocolUnitTest\">");
                            }
                            else
                            {
                                sb.AppendLine("<" + asNamespace[i] + ">");
                            }
                        }

                        for (int i = asNamespace.Length; i > 0; i--, index--)
                        {
                            for (int j = 1; j < index; j++)
                            {
                                sb.Append("\t");
                            }

                            sb.AppendLine("</" + asNamespace[i - 1] + ">");
                        }

                        string tempXml = sb.ToString().Trim();

                        string tempPath;
                        if (check.Source == Skyline.DataMiner.CICD.Validators.Common.Model.Source.Validator)
                        {
                            // Create Valid Validate file
                            tempPath = Path.Combine(validValidateDir.FullName, "Valid.xml");
                            if (!File.Exists(tempPath))
                            {
                                File.WriteAllText(tempPath, tempXml, Encoding.UTF8);
                            }

                            // Create Invalid Validate files
                            tempPath = Path.Combine(invalidValidateDir.FullName, check.Name + ".xml");
                            if (!File.Exists(tempPath))
                            {
                                File.WriteAllText(tempPath, tempXml, Encoding.UTF8);
                            }

                            // Create CodeFix files
                            if (check.HasCodeFix)
                            {
                                tempPath = Path.Combine(codefixDir.FullName, check.Name + ".xml");
                                if (!File.Exists(tempPath))
                                {
                                    File.WriteAllText(tempPath, tempXml, Encoding.UTF8);
                                }
                            }
                        }
                        else if (check.Source == Skyline.DataMiner.CICD.Validators.Common.Model.Source.MajorChangeChecker)
                        {
                            // Create Valid Compare files
                            tempPath = Path.Combine(validCompareDir.FullName, "Valid_Old.xml");
                            if (!File.Exists(tempPath))
                            {
                                File.WriteAllText(tempPath, tempXml, Encoding.UTF8);
                            }

                            tempPath = Path.Combine(validCompareDir.FullName, "Valid_New.xml");
                            if (!File.Exists(tempPath))
                            {
                                File.WriteAllText(tempPath, tempXml, Encoding.UTF8);
                            }

                            // Create Invalid Compare files
                            tempPath = Path.Combine(invalidCompareDir.FullName, check.Name + "_Old.xml");
                            if (!File.Exists(tempPath))
                            {
                                File.WriteAllText(tempPath, tempXml, Encoding.UTF8);
                            }

                            tempPath = Path.Combine(invalidCompareDir.FullName, check.Name + "_New.xml");
                            if (!File.Exists(tempPath))
                            {
                                File.WriteAllText(tempPath, tempXml, Encoding.UTF8);
                            }
                        }
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(directoryPath.ToString());
                filePath = directoryPath + className + ".cs";
            }

            configFileName.Append(splitNamespace[splitNamespace.Length - 1] + ".cs");

            if (!File.Exists(filePath.ToString()) || overwrite)
            {
                // Create a file to write to.
                File.WriteAllText(filePath, generatedCode);

                Console.WriteLine("File: " + configFileName + " was generated!");
            }
        }

        /// <summary>
        /// Creates a unit test class for the corresponding checks.
        /// </summary>
        /// <param name="checks">A list with all the checks within a namespace.</param>
        private static void CreateUnitTestClass(List<Check> checks)
        {
            var splitNamespace = checks[0].FullNamespace.Split(new string[] { "." }, StringSplitOptions.None);
            var className = splitNamespace[splitNamespace.Length - 1];

            UnitTestClass unitTestClass = new UnitTestClass(checks, className);
            var generatedCode = unitTestClass.TransformText();

            CreateFile(checks[0].FullNamespace, generatedCode, Settings.UnitTestPath, String.Empty, String.Empty, false, true, checks);
        }

        /// <summary>
        /// Creates an error message class for the corresponding checks.
        /// </summary>
        /// <param name="checks">A list with all the checks within a namespace.</param>
        private static void CreateErrorClass(List<Check> checks)
        {
            ErrorMessagesClass errorMessagesClass = new ErrorMessagesClass(checks);
            var generatedCode = errorMessagesClass.TransformText();

            if (Settings.ErrorClassesInOneFile)
            {
                singleFile.Append(generatedCode);
                singleFile.Append(Environment.NewLine);
                singleFile.Append(Environment.NewLine);
            }
            else
            {
                CreateFile(checks[0].FullNamespace, generatedCode, Settings.ErrorMessagesPath, "Error Messages/", "Error Messages\\", true, false);
            }
        }

        /// <summary>
        /// Creates a test class for the corresponding check.
        /// </summary>
        /// <param name="check">A check that represents a group of checks within a category and namespace.</param>
        private static void CreateTestClass(Check check)
        {
            var splitNamespace = check.FullNamespace.Split(new string[] { "." }, StringSplitOptions.None);
            var className = splitNamespace[splitNamespace.Length - 1];

            TestsClass checkClass = new TestsClass(className, check.FullNamespace, check.Category);
            var generatedCode = checkClass.TransformText();

            CreateFile(check.FullNamespace, generatedCode, Settings.TestPath, "Tests/", "Tests\\", false, false);
        }
    }
}