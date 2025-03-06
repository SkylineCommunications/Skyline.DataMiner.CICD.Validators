namespace Skyline.DataMiner.CICD.Tools.ValidatorErrorsToMarkdown
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    using Grynwald.MarkdownGenerator;

    /// <summary>
    /// Converts the XML file to a tree structure with markDown files
    /// </summary>
    public class ParseXmlToMarkDown
    {
        private const string baseUrl = "https://skylinecommunications.github.io/Skyline.DataMiner.CICD.Validators";

        private readonly XDocument xml;
        private readonly string outputDirectoryPath;
        private readonly DescriptionTemplates descriptionTemplates;

        /// <summary>
        /// Creates an instance of class <see cref="ParseXmlToMarkDown"/>
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="outputDirectoryPath"></param>
        public ParseXmlToMarkDown(XDocument xml, string outputDirectoryPath)
        {
            this.xml = xml;
            this.outputDirectoryPath = outputDirectoryPath;
            descriptionTemplates = new DescriptionTemplates(xml.Descendants("DescriptionTemplates").FirstOrDefault());
        }

        /// <summary>
        /// Creates the layout of the markdown files
        /// </summary>
        public void ConvertToMarkDown()
        {
            var categories = xml?.Element("Validator")?.Element("ValidationChecks")?.Element("Categories")?.Elements("Category");

            if (categories == null)
            {
                return;
            }

            foreach (var category in categories)
            {
                string categoryId = category.Attribute("id")?.Value;
                string categoryName = category.Element("Name")?.Value;

                var checks = category.Element("Checks")?.Elements("Check");

                if (checks == null)
                {
                    continue;
                }

                foreach (var check in checks)
                {
                    XDocCheckHelper helper = new(check, descriptionTemplates);
                    string nameSpace = helper.GetCheckNamespace();
                    string namespacePath = String.Join('/', nameSpace.Split('.'));
                    string checkName = helper.GetCheckName();
                    string checkId = helper.GetCheckId();

                    var errorMessages = check.Descendants("ErrorMessage");
                    foreach (var errorMessage in errorMessages)
                    {
                        string fullId = $"{categoryId}.{checkId}.{XDocCheckHelper.GetCheckErrorMessageId(errorMessage)}";
                        string uid = $"{XDocCheckHelper.GetCheckSource(errorMessage)}_{categoryId}_{checkId}_{XDocCheckHelper.GetCheckErrorMessageId(errorMessage)}";
                        string checkUid = $"{categoryId}_{checkId}_{XDocCheckHelper.GetCheckErrorMessageId(errorMessage)}";

                        MdDocument doc = new();
                        // uid
                        doc.Root.Add(new MdParagraph(new MdRawMarkdownSpan($"---\r\nuid: {uid}\r\n---")));
                        // check name
                        doc.Root.Add(new MdHeading(checkName, 1));
                        // error message name
                        doc.Root.Add(new MdHeading(XDocCheckHelper.GetCheckErrorMessageName(errorMessage), 2));
                        // description
                        doc.Root.Add(new MdHeading("Description", 3));
                        doc.Root.Add(new MdParagraph(helper.GetCheckDescription(errorMessage)));
                        // properties table
                        doc.Root.Add(new MdHeading("Properties", 3));
                        doc.Root.Add(CreateTable(errorMessage, categoryName, fullId));

                        string howToFix = XDocCheckHelper.GetCheckHowToFix(errorMessage);
                        if (howToFix is not "")
                        {
                            doc.Root.Add(new MdHeading("How to fix", 3));
                            doc.Root.Add(new MdParagraph(howToFix));
                        }

                        string exampleCode = XDocCheckHelper.GetCheckExampleCode(errorMessage);
                        if (exampleCode is not "")
                        {
                            doc.Root.Add(new MdHeading("Example code", 3));
                            doc.Root.Add(new MdCodeBlock(exampleCode, "xml"));
                        }

                        string details = XDocCheckHelper.GetCheckDetails(errorMessage);
                        if (details is not "")
                        {
                            doc.Root.Add(new MdHeading("Details", 3));
                            doc.Root.Add(new MdParagraph(details));
                        }

                        List<string> autofixWarnings = XDocCheckHelper.GetCheckAutoFixWarnings(errorMessage);
                        if (autofixWarnings is not null)
                        {
                            MdParagraph warnings = new();
                            warnings.Add(new MdRawMarkdownSpan("[!WARNING]\r\n"));
                            foreach (string autofixWarning in autofixWarnings)
                            {
                                warnings.Add(new MdRawMarkdownSpan($"{autofixWarning}\r\n"));
                            }
                        }

                        string source = XDocCheckHelper.GetCheckSource(errorMessage);
                        Directory.CreateDirectory($"{outputDirectoryPath}/{source}/{namespacePath}/{checkName}");

                        string url = $"{baseUrl}/checks/{source}/{namespacePath}/{checkName}/{uid}.html";
                        WriteRedirectFile(checkUid, url);

                        if (!File.Exists($"{outputDirectoryPath}/{source}/{namespacePath}/{checkName}/{uid}.md"))
                        {
                            // Note: When a file already exists, it will not be overwritten.
                            // This allows users to provide overrides for the automatically generated pages (in case they want to provide e.g. additional info).
                            doc.Save($"{outputDirectoryPath}/{source}/{namespacePath}/{checkName}/{uid}.md");
                        }
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            CreateToc("Validator", sb);
            CreateToc("MajorChangeChecker", sb);

            File.WriteAllText($"{outputDirectoryPath}/toc.yml", sb.ToString());
        }

        private void WriteRedirectFile(string checkId, string redirectUrl)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("---");
            stringBuilder.Append("redirect_url: ");
            stringBuilder.Append(redirectUrl);
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.AppendLine("---");

            File.WriteAllText($"{outputDirectoryPath}/Check_{checkId}.md", stringBuilder.ToString());
        }

        private void CreateToc(string folder, StringBuilder sb)
        {
            string startFolder = $"{outputDirectoryPath}/{folder}";

            TocFolder root = new TocFolder(startFolder);
            root.Build(sb);
        }

        /// <summary>
        /// Creates a table with all the properties 
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="categoryName"></param>
        /// <param name="fullId"></param>
        /// <returns>A <see cref="MdTable"/> table</returns>
        private static MdTable CreateTable(XElement errorMessage, string categoryName, string fullId)
        {
            MdSpan[] headingCells = ["Name", "Value"];
            MdTableRow tableRowHeading = new(headingCells);

            MdSpan[] categoryCells = ["Category", categoryName];
            MdTableRow tableRowCategory = new(categoryCells);

            MdSpan[] fullIdCells = ["Full Id", fullId];
            MdTableRow tableRowfullId = new(fullIdCells);

            MdSpan[] severityCells = ["Severity", XDocCheckHelper.GetCheckSeverity(errorMessage) ?? "Variable"];
            MdTableRow tableRowSeverity = new(severityCells);

            MdSpan[] certaintyCells = ["Certainty", XDocCheckHelper.GetCheckCertainty(errorMessage) ?? "Variable"];
            MdTableRow tableRowCertainty = new(certaintyCells);

            MdSpan[] sourceCells = ["Source", XDocCheckHelper.GetCheckSource(errorMessage)];
            MdTableRow tableRowSource = new(sourceCells);

            MdSpan[] fixImpactCells = ["Fix Impact", XDocCheckHelper.GetCheckFixImpact(errorMessage)];
            MdTableRow tableRowFixImpact = new(fixImpactCells);

            MdSpan[] hasCodeFixCells = ["Has Code Fix", XDocCheckHelper.HasCheckErrorMessageCodeFix(errorMessage).ToString()];
            MdTableRow tableRowHasCodeFix = new(hasCodeFixCells);

            MdTableRow[] tableRows =
                [tableRowCategory, tableRowfullId, tableRowSeverity, tableRowCertainty, tableRowSource, tableRowFixImpact, tableRowHasCodeFix];
            MdTable table = new(tableRowHeading, tableRows);

            string groupDescription = XDocCheckHelper.GetCheckGroupDescription(errorMessage);
            if (groupDescription != "")
            {
                MdSpan[] groupDescriptionCells = ["Group Description", groupDescription];
                MdTableRow tableRowDescriptionGroup = new(groupDescriptionCells);
                table.Add(tableRowDescriptionGroup);
            }

            return table;
        }
    }
}