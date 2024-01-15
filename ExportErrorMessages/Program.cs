namespace ExportErrorMessages
{
    using System.Collections.Generic;

    using ExportErrorMessages.Serialization;

    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using NPOI.XSSF.UserModel;

    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Category = Skyline.DataMiner.CICD.Validators.Common.Model.Category;

    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length > 2)
            {
                ExportToExcelPipeline(args[0], args[1], args[2]);
            }
        }

        private static void ExportToExcelPipeline(string xmlFilePath, string version, string outputDirectory)
        {
            try
            {
                // Retrieve errorMessages
                var validator = Serializer.ReadXml(xmlFilePath);

                var allErrorMessages = validator.ValidationChecks.Categories.Category.SelectMany(category =>
                    category.Checks.Check.SelectMany(check =>
                        check.ErrorMessages.ErrorMessage.Select(errorMessage =>
                            new FullErrorMessage { Category = category, Check = check, ErrorMessage = errorMessage }))).ToList();

                string fileName = $"Validator Error Messages - {version}";
                Directory.CreateDirectory(outputDirectory);
                string filePath = Path.Combine(outputDirectory, fileName);
                CreateWorksheet(allErrorMessages, filePath, validator);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                Console.WriteLine("Finished");
            }
        }

        /// <summary>
        /// Creates a Excel Worksheet in the specified location with all the error messages from the list.
        /// </summary>
        /// <param name="errorMessages">List of errorMessages that has to be written to the file.</param>
        /// <param name="filePath">Path of the file that has to be created.</param>
        /// <param name="validator"></param>
        private static void CreateWorksheet(ICollection<FullErrorMessage> errorMessages, string filePath, Validator validator)
        {
            if (!Path.HasExtension(filePath))
            {
                filePath += ".xlsx";
            }

            IWorkbook workbook = new XSSFWorkbook();

            CreateWorksheetSplit(workbook, errorMessages, "All Error Messages", validator);
            CreateWorksheetSplit(workbook, errorMessages.Where(x => x.ErrorMessage.Source == Source.Validator).ToList(), "Validator", validator);
            CreateWorksheetSplit(workbook, errorMessages.Where(x => x.ErrorMessage.Source == Source.MajorChangeChecker).ToList(), "Major Change Checker", validator);

            using (var fs = File.Create(filePath))
            {
                workbook.Write(fs, false);
            }

            workbook.Close();
        }

        private static void CreateWorksheetSplit(IWorkbook workbook, ICollection<FullErrorMessage> errorMessages, string sheetName,
            Validator validator)
        {
            try
            {
                ISheet excelWorkSheet = workbook.CreateSheet(sheetName);

                IRow headerRow = excelWorkSheet.CreateRow(0);

                // Add table headers going cell by cell.
                headerRow.CreateCell(0).SetCellValue("Full ID");
                headerRow.CreateCell(1).SetCellValue("Category");
                headerRow.CreateCell(2).SetCellValue("Namespace");
                headerRow.CreateCell(3).SetCellValue("Check Name");
                headerRow.CreateCell(4).SetCellValue("Error Message Name");
                headerRow.CreateCell(5).SetCellValue("Description");
                headerRow.CreateCell(6).SetCellValue("Severity");
                headerRow.CreateCell(7).SetCellValue("Certainty");
                headerRow.CreateCell(8).SetCellValue("Fix Impact");
                headerRow.CreateCell(9).SetCellValue("Has Code Fix");
                headerRow.CreateCell(10).SetCellValue("Source");
                headerRow.CreateCell(11).SetCellValue("Details");
                headerRow.CreateCell(12).SetCellValue("Example Code");
                headerRow.CreateCell(13).SetCellValue("How To Fix");

                int rowCount = 2;

                var sortedErrorMessages = errorMessages.OrderBy(c => c.Category.Id).ThenBy(c => c.Check.Id).ThenBy(c => c.ErrorMessage.Id);
                foreach (var errorMessage in sortedErrorMessages)
                {
                    IRow row = excelWorkSheet.CreateRow(rowCount);

                    row.CreateCell(0).SetCellValue(errorMessage.FullId);
                    row.CreateCell(1).SetCellValue(((Category)errorMessage.Category.Id).ToString());
                    row.CreateCell(2).SetCellValue(SimplifyNamespace2(errorMessage));
                    row.CreateCell(3).SetCellValue(errorMessage.Check.Name.Text);
                    row.CreateCell(4).SetCellValue(errorMessage.ErrorMessage.Name.Text);

                    (string format, List<InputParameter>? parameters) = GetDescriptionAndInputParameters(errorMessage.ErrorMessage, validator);
                    string description = format;
                    try
                    {
                        if (parameters is not null)
                        {
                            for (int i = 0; i < parameters.Count; i++)
                            {
                                var param = parameters[i];
                                string oldValue = String.Format("{{{0}}}", i);

                                string newValue;
                                if (String.IsNullOrWhiteSpace(param.Value))
                                {
                                    newValue = String.Format("{{{0}}}", param.Text);
                                }
                                else
                                {
                                    // No need to add the braces as it's a hard-coded value anyway.
                                    newValue = String.Format("{0}", param.Value);
                                }

                                description = format.Replace(oldValue, newValue);
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        description = format;
                    }

                    row.CreateCell(5).SetCellValue(description);
                    row.CreateCell(6).SetCellValue(errorMessage.ErrorMessage.Severity.ToString());
                    row.CreateCell(7).SetCellValue(errorMessage.ErrorMessage.Certainty.ToString());
                    row.CreateCell(8).SetCellValue(errorMessage.ErrorMessage.FixImpact.ToString());
                    row.CreateCell(9).SetCellValue(errorMessage.ErrorMessage.HasCodeFix.GetValueOrDefault());
                    row.CreateCell(10).SetCellValue(errorMessage.ErrorMessage.Source.ToString());
                    row.CreateCell(11).SetCellValue(errorMessage.ErrorMessage.Details?.Value);
                    row.CreateCell(12).SetCellValue(errorMessage.ErrorMessage.ExampleCode?.Value);
                    row.CreateCell(13).SetCellValue(errorMessage.ErrorMessage.HowToFix?.Value);

                    rowCount++;
                }

                // Disabled autosizing as it somehow breaks on GitHub and just stops the program fully.
                //excelWorkSheet.AutoSizeColumn(0);
                //excelWorkSheet.AutoSizeColumn(1);
                //excelWorkSheet.AutoSizeColumn(2);
                //excelWorkSheet.AutoSizeColumn(3);
                //excelWorkSheet.AutoSizeColumn(4);
                //excelWorkSheet.AutoSizeColumn(5);
                //excelWorkSheet.AutoSizeColumn(6);
                //excelWorkSheet.AutoSizeColumn(7);
                //excelWorkSheet.AutoSizeColumn(8);
                //excelWorkSheet.AutoSizeColumn(9);
                //excelWorkSheet.AutoSizeColumn(10);
                //excelWorkSheet.AutoSizeColumn(11);
                //excelWorkSheet.AutoSizeColumn(12);
                //excelWorkSheet.AutoSizeColumn(13);

                excelWorkSheet.SetAutoFilter(new CellRangeAddress(0, rowCount, 0, 13));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        private static (string format, List<InputParameter>? parameters) GetDescriptionAndInputParameters(ErrorMessage errorMessage, Validator validator)
        {
            if (errorMessage.Description.Format != null)
            {
                return (errorMessage.Description.Format, errorMessage.Description.InputParameters?.InputParameter);
            }

            DescriptionTemplate? template = validator.ValidationChecks.DescriptionTemplates.DescriptionTemplate.FirstOrDefault(x => x.Id == errorMessage.Description.TemplateId);

            if (template is null)
            {
                return ("FAILED TO RETRIEVE DESCRIPTION", null);
            }

            List<InputParameter> parameters = errorMessage.Description.InputParameters?.InputParameter ?? new List<InputParameter>();

            if (template.TemplateInputParameters.InputParameter.Count != errorMessage.Description.InputParameters?.InputParameter.Count)
            {
                foreach (var item in template.TemplateInputParameters.InputParameter)
                {
                    // Only for the ones that aren't covered
                    if (parameters.Any(x => x.Id == item.Id))
                    {
                        continue;
                    }

                    // Add remaining parameters
                    parameters.Add(item);
                }
            }

            return (template.Format, parameters);
        }

        private static string SimplifyNamespace2(FullErrorMessage fullErrorMessage)
        {
            List<string> nsParts = fullErrorMessage.Check.Name.Namespace.Split('.').ToList();

            List<string> simpleNs = new List<string>();
            bool found = false;
            Category cat = (Category)fullErrorMessage.Category.Id;
            string catString = cat.ToString();
            foreach (string item in nsParts)
            {
                string partToCheck = item;
                if (found)
                {
                    simpleNs.Add(partToCheck);
                    continue;
                }

                if (cat == Category.ParameterGroup && String.Equals(partToCheck, "ParameterGroups"))
                {
                    partToCheck = "ParameterGroup";
                }

                if (String.Equals(catString, partToCheck, StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                }
            }

            string ns;
            if (simpleNs.Count == 0)
            {
                ns = nsParts.Last();
            }
            else
            {
                ns = String.Join(".", simpleNs);
            }

            return ns;
        }

        private class FullErrorMessage
        {
            public Serialization.Category Category { get; set; }

            public Check Check { get; set; }

            public ErrorMessage ErrorMessage { get; set; }

            public string FullId => $"{Category.Id}.{Check.Id}.{ErrorMessage.Id}";
        }
    }
}