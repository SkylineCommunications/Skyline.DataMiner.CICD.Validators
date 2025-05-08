namespace Validator_Management_Tool.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    using Validator_Management_Tool.Common;
    using Validator_Management_Tool.Model;

    public static class TemplateHelper
    {
        /// <summary>
        /// Makes a list of arguments for the method and a list of properties that are assigned in the method body.
        /// This method will use the SettedProperties list of the <see cref="Check"/> object to decide wither or not
        /// a property has to be asked as argument.
        /// </summary>
        internal static (Dictionary<string, Dictionary<string, string>> Arguments, Dictionary<string, Dictionary<string, object>> Properties) MakeLists(IEnumerable<Check> allChecks)
        {
            var argumentLists = new Dictionary<string, Dictionary<string, string>>();
            var propertyLists = new Dictionary<string, Dictionary<string, object>>();

            var interfaceProperties = typeof(IValidationResult).GetProperties();

            foreach (var check in allChecks)
            {
                Dictionary<string, string> argumentList = new Dictionary<string, string>();
                Dictionary<string, object> propertyList = new Dictionary<string, object>();

                foreach (var interfaceProperty in interfaceProperties)
                {
                    if (interfaceProperty.Name == "Details" || interfaceProperty.Name == "ExampleCode")
                    {
                        // Skip the properties that are obsolete
                        continue;
                    }

                    // Add the description with the correct parameters
                    if (interfaceProperty.Name == "Description")
                    {
                        if (check.Parameters == null || check.Parameters.Count == 0)
                        {
                            // The description needs no parameters
                            propertyList.Add("Description", "\"" + MakeCSharpString(check.Description) + "\"");
                        }
                        else
                        {
                            // Parameters exists
                            StringBuilder description = new StringBuilder("String.Format(\"");
                            description.Append(MakeCSharpString(check.Description) + "\"");
                            foreach (var parameter in check.Parameters)
                            {
                                if (String.IsNullOrWhiteSpace(parameter.Value))
                                {
                                    description.Append(", " + parameter.Text);
                                    argumentList.Add(parameter.Text, "string");
                                }
                                else
                                {
                                    string stringified = String.Format("\"{0}\"", MakeCSharpString(parameter.Value));
                                    description.Append(", " + stringified);
                                }
                            }

                            description.Append(")");
                            propertyList.Add("Description", description.ToString());
                        }

                        continue;
                    }

                    if (interfaceProperty.Name == "ErrorId")
                    {
                        // If the property is the ID, then it has to be added to the ErrorsIds static class
                        propertyList.Add(interfaceProperty.Name, "ErrorIds." + check.Name);
                        continue;
                    }

                    if (interfaceProperty.Name == "CheckId")
                    {
                        propertyList.Add(interfaceProperty.Name, "CheckId." + check.CheckName);
                        continue;
                    }

                    if (check.SettedProperties.Contains(interfaceProperty.Name) && XMLParser.Types.ContainsKey(interfaceProperty.PropertyType) && interfaceProperty.GetValue(check) != null)
                    {
                        // If it is a property that is set when parsing the XML, the correct value is given to the property
                        switch (XMLParser.Types[interfaceProperty.PropertyType])
                        {
                            // string
                            case 0:
                                propertyList.Add(interfaceProperty.Name, ("\"" + MakeCSharpString(interfaceProperty.GetValue(check).ToString()) + "\"").Replace(Environment.NewLine, "\" + Environment.NewLine + \""));
                                break;

                            // uint
                            // int
                            case 1:
                            case 6:
                                propertyList.Add(interfaceProperty.Name, interfaceProperty.GetValue(check));
                                break;

                            // bool
                            case 2:
                                propertyList.Add(interfaceProperty.Name, interfaceProperty.GetValue(check).ToString().ToLower());
                                break;

                            // Enums
                            case 3:
                            case 4:
                            case 5:
                            case 7:
                            case 8:
                                propertyList.Add(interfaceProperty.Name, interfaceProperty.Name + "." + interfaceProperty.GetValue(check).ToString());
                                break;
                        }

                        continue;
                    }

                    if (interfaceProperty.Name == "GroupDescription")
                    {
                        propertyList.Add(interfaceProperty.Name, $"\"{MakeCSharpString(interfaceProperty.GetValue(check)?.ToString())}\"");
                        continue;
                    }

                    if (interfaceProperty.Name != "Line" && interfaceProperty.Name != "DescriptionFormat" &&
                        interfaceProperty.Name != "DescriptionParameters" && interfaceProperty.Name != "ReferenceNode" &&
                        interfaceProperty.Name != "PositionNode" && interfaceProperty.Name != "Position" &&
                        interfaceProperty.Name != "SubResults" && interfaceProperty.Name != "AutoFixWarnings" &&
                        interfaceProperty.Name != "DveExport")
                    {
                        // If the property was not set, it will be added to the argument list of the method
                        string lowerCaseObject = MakeCSharpString(Char.ToLowerInvariant(interfaceProperty.Name[0]) + interfaceProperty.Name.Substring(1));
                        propertyList.Add(interfaceProperty.Name, lowerCaseObject);

                        switch (XMLParser.Types[interfaceProperty.PropertyType])
                        {
                            // string
                            case 0:
                                argumentList.Add(lowerCaseObject, "string");
                                break;

                            // uint
                            case 1:
                                argumentList.Add(lowerCaseObject, "uint");
                                break;

                            // bool
                            case 2:
                                argumentList.Add(lowerCaseObject, "bool");
                                break;

                            // Enums
                            case 3:
                            case 4:
                            case 5:
                            case 7:
                            case 8:
                                argumentList.Add(lowerCaseObject, interfaceProperty.Name);
                                break;

                            // int
                            case 6:
                                argumentList.Add(lowerCaseObject, "int");
                                break;
                        }
                    }
                }

                argumentLists.Add(check.Name, argumentList);
                propertyLists.Add(check.Name, propertyList);
            }

            return (argumentLists, propertyLists);
        }

        internal static Dictionary<string, Dictionary<string, string>> GetWarnings(IEnumerable<Check> checks)
        {
            Dictionary<string, Dictionary<string, string>> allWarningsPerCheck = new Dictionary<string, Dictionary<string, string>>();

            foreach (Check check in checks)
            {
                Dictionary<string, string> warnings = new Dictionary<string, string>();

                if (check.AutoFixWarnings != null)
                {
                    foreach ((string message, bool autoFixPopup) in check.AutoFixWarnings)
                    {
                        warnings.Add(MakeCSharpString(message), autoFixPopup.ToString().ToLowerInvariant());
                    }
                }

                allWarningsPerCheck.Add(check.Name, warnings);
            }

            return allWarningsPerCheck;
        }

        /// <summary>
        /// Converts the input string to a C# proof string.
        /// </summary>
        /// <param name="input">Input string that needs to be converted.</param>
        /// <returns>The converted C# proof string</returns>
        internal static string MakeCSharpString(string input)
        {
            if (input == null)
            {
                return null;
            }

            input = input.Replace("\\", "\\" + "\\");
            int previousIndex = 0;
            while (input.IndexOf('"', previousIndex) != -1)
            {
                int index = input.IndexOf('"', previousIndex);
                input = input.Insert(index, @"\");
                previousIndex = index + 2;
            }

            return input;
        }
    }
}