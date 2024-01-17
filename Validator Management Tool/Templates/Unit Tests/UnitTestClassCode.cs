namespace Validator_Management_Tool.Templates.Unit_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Validator_Management_Tool.Common;
    using Validator_Management_Tool.Model;

    partial class UnitTestClass
    {
        // Properties to be excluded from the error messages unit tests.
        private static readonly List<string> ExcludedProperties = new List<string>
        {
            // These shouldn't change normally.
            "ErrorId",
            "FullId",
            "Category",
            "Source",

            // These are almost never used.
            "HowToFix",
            "ExampleCode"
        };

        private readonly List<Check> allChecks;
        private readonly List<Check> compares;
        private readonly List<Check> codefixes;
        private readonly List<Check> checks;
        private readonly string className;
        private readonly string @namespace;

        private readonly List<(bool hasArguments, string args, Dictionary<string, object> props, Check check)> validateChecks = new List<(bool hasArguments, string args, Dictionary<string, object> props, Check check)>();
        private readonly List<(bool hasArguments, string args, Dictionary<string, object> props, Check check)> compareChecks = new List<(bool hasArguments, string args, Dictionary<string, object> props, Check check)>();

        public UnitTestClass(List<Check> checks, string className)
        {
            this.allChecks = checks;
            this.checks = checks.Where(x => x.Source != Source.MajorChangeChecker).OrderBy(x => x.Name).ToList();
            this.compares = checks.Where(x => x.Source == Source.MajorChangeChecker).OrderBy(x => x.Name).ToList();
            this.codefixes = checks.Where(x => x.Source == Source.Validator && x.HasCodeFix && !x.Name.EndsWith("_Sub")).OrderBy(x => x.Name).ToList();
            this.className = className;
            this.@namespace = checks[0].FullNamespace;

            MakeLists();

            // Need to happen after MakeLists
            checks = checks.Where(x => !x.Name.EndsWith("_Sub")).ToList();
            compares = compares.Where(x => !x.Name.EndsWith("_Sub")).ToList();
        }

        private void MakeLists()
        {
            var interfaceProperties = typeof(IValidationResult).GetProperties();

            foreach (var check in this.checks)
            {
                List<string> args = new List<string>();
                Dictionary<string, object> properties = new Dictionary<string, object>();

                foreach (var interfaceProperty in interfaceProperties)
                {
                    // Add the description with the correct parameters
                    if (interfaceProperty.Name == "Description")
                    {
                        string desc = TemplateHelper.MakeCSharpString(check.Description);
                        if (check.Parameters?.Count > 0)
                        {
                            // Parameters exists
                            int argCount = 0;
                            foreach (var parameter in check.Parameters)
                            {
                                if (String.IsNullOrWhiteSpace(parameter.Value))
                                {
                                    string argName = parameter.Text;
                                    args.Add($"\"{argName}\"");
                                    desc = desc.Replace($"{{{argCount}}}", $"{argName}");
                                }
                                else
                                {
                                    desc = desc.Replace($"{{{argCount}}}", $"{parameter.Value}");
                                }

                                argCount++;
                            }
                        }

                        properties.Add("Description", "\"" + desc + "\"");
                    }
                    else if (check.SettedProperties.Contains(interfaceProperty.Name) && XMLParser.Types.ContainsKey(interfaceProperty.PropertyType) && interfaceProperty.GetValue(check) != null)
                    {
                        switch (XMLParser.Types[interfaceProperty.PropertyType])
                        {
                            // string
                            case 0:
                                properties.Add(interfaceProperty.Name, ("\"" + TemplateHelper.MakeCSharpString(interfaceProperty.GetValue(check).ToString()) + "\"").Replace(Environment.NewLine, "\" + Environment.NewLine + \""));
                                break;

                            // uint
                            case 1:
                            case 6:
                                properties.Add(interfaceProperty.Name, interfaceProperty.GetValue(check));
                                break;

                            // bool
                            case 2:
                                properties.Add(interfaceProperty.Name, interfaceProperty.GetValue(check).ToString().ToLower());
                                break;

                            // Enums
                            case 3:
                            case 4:
                            case 5:
                            case 7:
                            case 8:
                                properties.Add(interfaceProperty.Name, interfaceProperty.Name + "." + interfaceProperty.GetValue(check).ToString());
                                break;
                        }
                    }
                    else if (interfaceProperty.Name != "Line" && interfaceProperty.Name != "DescriptionFormat" &&
                        interfaceProperty.Name != "ErrorId" && interfaceProperty.Name != "CheckId" &&
                        interfaceProperty.Name != "DescriptionParameters" && interfaceProperty.Name != "ReferenceNode" &&
                        interfaceProperty.Name != "PositionNode" && interfaceProperty.Name != "Position" &&
                        interfaceProperty.Name != "SubResults" && interfaceProperty.Name != "AutoFixWarnings" &&
                        interfaceProperty.Name != "DveExport")
                    {
                        switch (XMLParser.Types[interfaceProperty.PropertyType])
                        {
                            // string
                            case 0:
                                string argName = interfaceProperty.Name;
                                args.Add($"\"{argName}\"");
                                properties.Add(interfaceProperty.Name, $"\"{argName}\"");
                                break;

                            // uint
                            case 1:
                            case 6:
                                args.Add("0");
                                properties.Add(interfaceProperty.Name, "0");
                                break;

                            // bool
                            case 2:
                                args.Add("false");
                                properties.Add(interfaceProperty.Name, "false");
                                break;

                            // Enums
                            case 3:
                            case 4:
                            case 5:
                            case 7:
                            case 8:
                                args.Add($"{interfaceProperty.Name}.Undefined");
                                properties.Add(interfaceProperty.Name, $"{interfaceProperty.Name}.Undefined");
                                break;
                        }
                    }
                }

                validateChecks.Add((args.Count != 0, String.Join(", ", args), properties, check));
            }

            foreach (var check in this.compares)
            {
                List<string> args = new List<string>();
                Dictionary<string, object> properties = new Dictionary<string, object>();

                foreach (var interfaceProperty in interfaceProperties)
                {
                    // Add the description with the correct parameters
                    if (interfaceProperty.Name == "Description")
                    {
                        string desc = TemplateHelper.MakeCSharpString(check.Description);
                        if (check.Parameters?.Count > 0)
                        {
                            // Parameters exists
                            int argCount = 0;
                            foreach (var parameter in check.Parameters)
                            {
                                if (String.IsNullOrWhiteSpace(parameter.Value))
                                {
                                    string argName = parameter.Text;
                                    args.Add($"\"{argName}\"");
                                    desc = desc.Replace($"{{{argCount}}}", $"{argName}");
                                }
                                else
                                {
                                    desc = desc.Replace($"{{{argCount}}}", $"{parameter.Value}");
                                }

                                argCount++;
                            }
                        }

                        properties.Add("Description", "\"" + desc + "\"");
                    }
                    else if (check.SettedProperties.Contains(interfaceProperty.Name) && XMLParser.Types.ContainsKey(interfaceProperty.PropertyType) && interfaceProperty.GetValue(check) != null)
                    {
                        switch (XMLParser.Types[interfaceProperty.PropertyType])
                        {
                            // string
                            case 0:
                                properties.Add(interfaceProperty.Name, ("\"" + TemplateHelper.MakeCSharpString(interfaceProperty.GetValue(check).ToString()) + "\"").Replace(Environment.NewLine, "\" + Environment.NewLine + \""));
                                break;

                            // uint
                            case 1:
                            case 6:
                                properties.Add(interfaceProperty.Name, interfaceProperty.GetValue(check));
                                break;

                            // bool
                            case 2:
                                properties.Add(interfaceProperty.Name, interfaceProperty.GetValue(check).ToString().ToLower());
                                break;

                            // Enums
                            case 3:
                            case 4:
                            case 5:
                            case 7:
                            case 8:
                                properties.Add(interfaceProperty.Name, interfaceProperty.Name + "." + interfaceProperty.GetValue(check).ToString());
                                break;
                        }
                    }
                    else if (interfaceProperty.Name != "Line" && interfaceProperty.Name != "DescriptionFormat" &&
                        interfaceProperty.Name != "ErrorId" && interfaceProperty.Name != "CheckId" &&
                        interfaceProperty.Name != "DescriptionParameters" && interfaceProperty.Name != "ReferenceNode" &&
                        interfaceProperty.Name != "PositionNode" && interfaceProperty.Name != "Position" &&
                        interfaceProperty.Name != "SubResults" && interfaceProperty.Name != "AutoFixWarnings" &&
                        interfaceProperty.Name != "DveExport")
                    {
                        switch (XMLParser.Types[interfaceProperty.PropertyType])
                        {
                            // string
                            case 0:
                                string argName = interfaceProperty.Name;
                                args.Add($"\"{argName}\"");
                                properties.Add(interfaceProperty.Name, $"\"{argName}\"");
                                break;

                            // uint
                            case 1:
                            case 6:
                                args.Add("0");
                                properties.Add(interfaceProperty.Name, "0");
                                break;

                            // bool
                            case 2:
                                args.Add("false");
                                properties.Add(interfaceProperty.Name, "false");
                                break;

                            // Enums
                            case 3:
                            case 4:
                            case 5:
                            case 7:
                            case 8:
                                args.Add($"{interfaceProperty.Name}.Undefined");
                                properties.Add(interfaceProperty.Name, $"{interfaceProperty.Name}.Undefined");
                                break;
                        }
                    }
                }

                compareChecks.Add((args.Count != 0, String.Join(", ", args), properties, check));
            }
        }
    }
}