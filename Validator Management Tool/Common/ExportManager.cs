namespace Validator_Management_Tool.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;

    using Microsoft.Win32;

    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using NPOI.XSSF.UserModel;

    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Validator_Management_Tool.Model;

    /// <summary>
    /// Static class that has methods to export the error messages.
    /// </summary>
    public static class ExportManager
    {
        /// <summary>
        /// Creates a chosen file with all the error messages in,
        ///  after it asked the location of the file with a save file dialog.
        /// </summary>
        /// <param name="checks">The list of checks for which a file has to be created.</param>
        public static void ExportToExcel(ICollection<Check> checks)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                DefaultExt = ".xlsx",
                Filter = "Excel Worksheet (*.xlsx)|*.xlsx",
                FileName = Settings.ExportPath + Settings.ExportFile,
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string extension = saveFileDialog.FileName.Split(new char[] { '.' })[1];
                string filename = saveFileDialog.FileName.Replace("." + extension, String.Empty).Split(new char[] { '\\' }).Last();
                Settings.ExportPath = saveFileDialog.FileName.Replace(filename + "." + extension, String.Empty);
                Settings.ExportFile = filename;

                // Export to Excel worksheet
                CreateWorksheet(checks, saveFileDialog.FileName);

                MessageBox.Show("Generating Done!", "Generation Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Creates a Excel Worksheet in the specified location with all the error messages from the list.
        /// </summary>
        /// <param name="checks">List of checks that has to be written to the file.</param>
        /// <param name="filePath">Path of the file that has to be created.</param>
        private static void CreateWorksheet(ICollection<Check> checks, string filePath)
        {
            if (!Path.HasExtension(filePath))
            {
                filePath += ".xlsx";
            }

            IWorkbook workbook = new XSSFWorkbook();

            CreateWorksheetSplit(workbook, checks, "All Error Messages");
            CreateWorksheetSplit(workbook, checks.Where(x => x.Source == Source.Validator).ToList(), "Validator");
            CreateWorksheetSplit(workbook, checks.Where(x => x.Source == Source.MajorChangeChecker).ToList(), "Major Change Checker");

            using (var fs = File.Create(filePath))
            {
                workbook.Write(fs, false);
            }

            workbook.Close();
        }

        private static void CreateWorksheetSplit(IWorkbook workbook, ICollection<Check> checks, string sheetName)
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
                headerRow.CreateCell(13).SetCellValue("How To Fix");

                int rowCount = 2;

                var sortedChecks = checks.OrderBy(c => c.CategoryId).ThenBy(c => c.CheckId).ThenBy(c => c.ErrorId);
                foreach (var check in sortedChecks)
                {
                    IRow row = excelWorkSheet.CreateRow(rowCount);

                    row.CreateCell(0).SetCellValue(check.FullId);
                    row.CreateCell(1).SetCellValue(check.Category.ToString());
                    row.CreateCell(2).SetCellValue(SimplifyNamespace(check));
                    row.CreateCell(3).SetCellValue(check.CheckName);
                    row.CreateCell(4).SetCellValue(check.Name);

                    string description;
                    try
                    {
                        description = check.Description;
                        for (int i = 0; i < check.Parameters.Count; i++)
                        {
                            var param = check.Parameters[i];
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

                            description = description.Replace(oldValue, newValue);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        description = check.Description;
                    }

                    row.CreateCell(5).SetCellValue(description);
                    row.CreateCell(6).SetCellValue(check.Severity.ToString());
                    row.CreateCell(7).SetCellValue(check.Certainty.ToString());
                    row.CreateCell(8).SetCellValue(check.FixImpact.ToString());
                    row.CreateCell(9).SetCellValue(check.HasCodeFix);
                    row.CreateCell(10).SetCellValue(check.Source.ToString());
                    row.CreateCell(13).SetCellValue(check.HowToFix);

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

        private static string SimplifyNamespace(Check check)
        {
            List<string> nsParts = check.Namespace.Split('.').ToList();

            List<string> simpleNs = new List<string>();
            bool found = false;
            string catString = check.Category.ToString();
            foreach (string item in nsParts)
            {
                string partToCheck = item;
                if (found)
                {
                    simpleNs.Add(partToCheck);
                    continue;
                }

                if (check.Category == Category.ParameterGroup && String.Equals(partToCheck, "ParameterGroups"))
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
    }
}