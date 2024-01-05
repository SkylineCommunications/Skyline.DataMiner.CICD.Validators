namespace Validator_Management_Tool.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Xml;
    using System.Xml.Serialization;

    using Skyline.DataMiner.CICD.Validators.Common.Model;

    /// <summary>
    /// Static class that contains all the methods to read and write to and from the XML file.
    /// Also the methods to convert the XML class hierarchy to a list of check objects, is located in here.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// List that holds the error messages from the errors that occur while parsing.
        /// </summary>
        public static List<string> ParsingErrors = new List<string>();

        /// <summary>
        /// Static <see cref="Validator"/> object that holds all the data from the XML file.
        /// </summary>
        public static Validator xmlValidator;

        /// <summary>
        /// Gets the different data types that are used in a dictionary, to switch between them.
        /// </summary>
        public static Dictionary<Type, int> Types { get; } = new Dictionary<Type, int>
        {
            { typeof(string), 0 },
            { typeof(uint), 1 },
            { typeof(bool), 2 },
            { typeof(Severity), 3 },
            { typeof(Certainty), 4 },
            { typeof(Category), 5 },
            { typeof(int), 6 },
            { typeof(Source), 7 },
            { typeof(FixImpact), 8 },
        };

        /// <summary>
        /// Reads the XML file and converts the content in a <see cref="Validator"/> object.
        /// </summary>
        /// <param name="filePath">The relative or absolute path to the XML file.</param>
        /// <returns>All the data from the XML file in the corresponding class hierarchy.</returns>
        public static Validator ReadXml(string filePath)
        {
            Validator validator = null;
            ParsingErrors.Clear();
            XmlSerializer serializer = new XmlSerializer(typeof(Validator));

            serializer.UnknownAttribute += Serializer_UnknownAttribute;
            serializer.UnknownElement += Serializer_UnknownElement;

            using (StreamReader reader = new StreamReader(filePath))
            {
                validator = (Validator)serializer.Deserialize(reader);
                reader.Close();
            }

            ValidateErrorMessage(validator);

            if (ParsingErrors.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                ParsingErrors.Sort();
                foreach (var error in ParsingErrors)
                {
                    sb.AppendLine(error);
                }

                Console.Error.WriteLine(sb.ToString());
            }

            xmlValidator = validator;

            return validator;
        }

        private static void ValidateErrorMessage(Validator validator)
        {
            foreach (var cat in validator.ValidationChecks.Categories.Category)
            {
                foreach (var check in cat.Checks.Check)
                {
                    foreach (var error in check.ErrorMessages.ErrorMessage)
                    {
                        if (error.Description.TemplateId != null && error.Description.Format != null)
                        {
                            string fullId = $"{cat.Id}.{check.Id}.{error.Id}";

                            ParsingErrors.Add($"[{fullId}] Description@TemplateId and Description\\Format can't be used together!");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Writes the static <see cref="Validator"/> object to a XML file.
        /// </summary>
        /// <param name="filePath">The relative or absolute path to the XML file.</param>
        public static void WriteXml(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Validator));

            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            using (XmlWriter xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true, IndentChars = "\t" }))
            {
                serializer.Serialize(xmlWriter, xmlValidator);
            }
        }

        /// <summary>
        /// Converts a list of error messages from the UI to a <see cref="Validator"/> object, and saves it in the static <see cref="Validator"/> object.
        /// </summary>
        /// <param name="checks">List of <see cref="Check"/>s that contains the different error messages.</param>
        /// <param name="templates">List of <see cref="DescriptionTemplate"/>s that contains the different templates that are used in the error messages.</param>
        /// <param name="categoryListing">List of <see cref="Category"/> that are currently used in the error messages, the categories also contain the corresponding checks that are used.</param>
        /// <returns>A <see cref="Validator"/> object that contains all the content in a XML class hierarchy.</returns>
        public static Validator SetChecks(ObservableCollection<Model.Check> checks, List<DescriptionTemplate> templates, List<Category> categoryListing)
        {
            var validator = new Validator();
            var categories = new Categories();
            List<Category> categoryList = new List<Category>();

            var lookupTemplates = templates.ToDictionary(key => key.Id);

            // Loop through all the existing categories and checks and add them to the new Categories object
            foreach (var category in categoryListing)
            {
                Category newCategory = new Category()
                {
                    Name = category.Name,
                    Id = category.Id,
                    Checks = new Checks()
                };

                List<Check> checkList = new List<Check>();
                foreach (var check in category.Checks.Check)
                {
                    checkList.Add(new Check()
                    {
                        Id = check.Id,
                        Name = check.Name,
                        ErrorMessages = new ErrorMessages()
                        {
                            ErrorMessage = new List<ErrorMessage>()
                        }
                    });
                }

                newCategory.Checks.Check = checkList;
                categoryList.Add(newCategory);
            }

            foreach (var errorMessage in checks)
            {
                foreach (var category in categoryList)
                {
                    foreach (var check in category.Checks.Check)
                    {
                        if (errorMessage.Category.ToString() == category.Name && errorMessage.CheckName == check.Name.Text && errorMessage.Namespace == check.Name.Namespace)
                        {
                            // Namespace from the check
                            check.Name.Namespace = errorMessage.Namespace;

                            // Name
                            ErrorMessage newError = new ErrorMessage
                            {
                                Name = new Name
                                {
                                    Text = errorMessage.Name
                                }
                            };

                            // ErrorId
                            newError.Id = errorMessage.ErrorId.ToString();

                            // Description
                            if (errorMessage.FromTemplate)
                            {
                                // If it is from a template
                                newError.Description = new Description
                                {
                                    TemplateId = errorMessage.TemplateId.ToString()
                                };

                                if (errorMessage.Parameters.Any(x => !String.IsNullOrWhiteSpace(x.Value)))
                                {
                                    // Some have overridden values
                                    newError.Description.InputParameters = new InputParameters
                                    {
                                        InputParameter = new List<InputParameter>(errorMessage.Parameters.Count)
                                    };

                                    // Get DescriptionTemplate to check if they are all there...? Or only on code generation
                                    DescriptionTemplate descriptionTemplate = lookupTemplates[errorMessage.TemplateId];

                                    List<string> coveredIds = new List<string>(errorMessage.Parameters.Count);
                                    foreach (var item in errorMessage.Parameters)
                                    {
                                        coveredIds.Add(item.Id);
                                        newError.Description.InputParameters.InputParameter.Add(item);
                                    }

                                    if (descriptionTemplate.TemplateInputParameters.InputParameter.Count != coveredIds.Count)
                                    {
                                        foreach (var item in descriptionTemplate.TemplateInputParameters.InputParameter)
                                        {
                                            // Only for the ones that aren't covered
                                            if (coveredIds.Contains(item.Id))
                                            {
                                                continue;
                                            }

                                            // Add remaining parameters
                                            newError.Description.InputParameters.InputParameter.Add(item);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // If it is a hard coded description
                                newError.Description = new Description
                                {
                                    Format = errorMessage.Description,
                                    InputParameters = new InputParameters(),
                                };
                                newError.Description.InputParameters.InputParameter = new List<InputParameter>();

                                if (errorMessage.Parameters != null)
                                {
                                    newError.Description.InputParameters.InputParameter = new List<InputParameter>(errorMessage.Parameters);
                                }
                            }

                            // AutoFixWarnings
                            if (errorMessage.AutoFixWarnings.Count != 0)
                            {
                                newError.AutoFixWarnings = new AutoFixWarnings
                                {
                                    AutoFixWarning = errorMessage.AutoFixWarnings.Select(x => new AutoFixWarning { AutoFixPopup = x.AutoFixPopup, Text = x.Message }).ToList()
                                };
                            }

                            var checkProperties = typeof(Model.Check).GetProperties();
                            var newCheckProperties = typeof(ErrorMessage).GetProperties();

                            foreach (var property in checkProperties)
                            {
                                if (property.Name == "Description" || property.Name == "Name" || property.Name == "AutoFixWarnings" ||
                                    !errorMessage.SettedProperties.Contains(property.Name))
                                {
                                    continue;
                                }

                                foreach (var newProperty in newCheckProperties)
                                {
                                    if (property.Name != newProperty.Name)
                                    {
                                        continue;
                                    }

                                    if (newProperty.PropertyType == typeof(XmlCDataSection))
                                    {
                                        if (property.GetValue(errorMessage) != null)
                                        {
                                            XmlDocument xdoc = new XmlDocument();
                                            newProperty.SetValue(newError, xdoc.CreateCDataSection(property.GetValue(errorMessage).ToString().Replace(Environment.NewLine, "[NewLine]")));
                                        }
                                    }
                                    else
                                    {
                                        newProperty.SetValue(newError, property.GetValue(errorMessage).ToString());
                                    }
                                }
                            }

                            check.ErrorMessages.ErrorMessage.Add(newError);
                        }
                    }
                }
            }

            validator.ValidationChecks = new ValidationChecks();
            categories.Category = categoryList;
            validator.ValidationChecks.Categories = categories;
            validator.ValidationChecks.DescriptionTemplates = new DescriptionTemplates
            {
                DescriptionTemplate = templates.OrderBy(x => x.Id).ToList()
            };

            xmlValidator = validator;

            return validator;
        }

        /// <summary>
        /// Gets the categories that are currently used.
        /// </summary>
        /// <returns>A list of <see cref="Category"/>. </returns>
        public static List<Category> GetCategories()
        {
            return xmlValidator.ValidationChecks.Categories.Category;
        }

        /// <summary>
        /// Gets the templates that are currently used.
        /// </summary>
        /// <returns>A list of <see cref="DescriptionTemplate"/>.</returns>
        public static List<DescriptionTemplate> GetTemplates()
        {
            return xmlValidator.ValidationChecks.DescriptionTemplates.DescriptionTemplate;
        }

        /// <summary>
        /// Converts the XML class hierarchy into a collection of error messages that can be used to generate code.
        /// </summary>
        /// <returns>A list of <see cref="Model.Check"/>.</returns>
        public static ObservableCollection<Model.Check> GetChecks()
        {
            ObservableCollection<Model.Check> checks = new ObservableCollection<Model.Check>();

            // Get all the properties of the interface with Reflection
            var checkProperties = typeof(Model.Check).GetProperties();

            // Check for duplicate templateID's
            HasDuplicateTemplateIDs(xmlValidator);

            List<string> namespaces = new List<string>();

            // List with all the category Id's to prevent duplicates
            List<uint> categoryIds = new List<uint>();

            // Loop through all the categories
            foreach (var category in xmlValidator.ValidationChecks.Categories.Category)
            {
                // List with all the check Id's within a category to prevent duplicates
                List<uint> checkIds = new List<uint>();
                bool categoryIdError = false;

                if (category?.Id != null && UInt32.TryParse(category.Id, out uint id))
                {
                    if (!categoryIds.Contains(id))
                    {
                        categoryIds.Add(id);
                    }
                    else
                    {
                        categoryIdError = true;
                    }
                }

                if (category?.Checks == null)
                {
                    continue;
                }

                // Loop through all the checks within a category
                foreach (var check in category.Checks.Check)
                {
                    bool checkIdError = false;
                    if (check.Id != null && UInt32.TryParse(check.Id, out uint checkId))
                    {
                        if (!checkIds.Contains(checkId))
                        {
                            checkIds.Add(checkId);
                        }
                        else
                        {
                            checkIdError = true;
                        }
                    }

                    // List with all the error Id's within a check to prevent duplicates
                    List<uint> errorIds = new List<uint>();

                    if (check.ErrorMessages == null)
                    {
                        continue;
                    }

                    // Loop through all the error messages within a check
                    foreach (var errorMessage in check.ErrorMessages.ErrorMessage)
                    {
                        Model.Check validationCheck = new Model.Check
                        {
                            CheckId = UInt32.Parse(check.Id)
                        };

                        if (categoryIdError)
                        {
                            string errorString = String.Format(
                                "The Category {0} has a duplicate ID: {1}, please edit the XML file manually!",
                                category.Name,
                                category.Id.ToString());
                            AddError(validationCheck, "CheckId", errorString);
                        }

                        if (checkIdError)
                        {
                            string errorString = String.Format(
                                "The Check {0} has a duplicate ID: {1}, please edit the XML file manually!",
                                check.Name.Text,
                                check.Id.ToString());
                            AddError(validationCheck, "CheckId", errorString);
                        }

                        if (check.Name != null)
                        {
                            if (check.Name.Text != null)
                            {
                                validationCheck.CheckName = check.Name.Text;
                            }
                            else
                            {
                                validationCheck.CheckName = String.Empty;
                            }

                            if (check.Name.Namespace != null)
                            {
                                string @namespace = check.Name.Namespace;

                                validationCheck.Namespace = @namespace;

                                if (!namespaces.Contains(@namespace))
                                {
                                    namespaces.Add(@namespace);
                                }
                            }
                            else
                            {
                                validationCheck.Namespace = String.Empty;
                                string errorString = String.Format(
                                    "There is no Namespace attribute present in check {0}, a namespace is mandatory!",
                                    check.Name.Text);
                                AddError(validationCheck, "Namespace", errorString);
                            }
                        }
                        else
                        {
                            validationCheck.CheckName = String.Empty;
                            validationCheck.Namespace = String.Empty;
                            string errorString = String.Format(
                                "There is no Name tag present for check {0}, a name is mandatory!",
                                check.Id);
                            AddError(validationCheck, "Name", errorString);
                        }

                        // Add the Category
                        if (category != null)
                        {
                            if (category.Id != null && category.Name != null)
                            {
                                if (UInt32.TryParse(category.Id, out uint catId))
                                {
                                    validationCheck.CategoryId = catId;
                                }
                                else
                                {
                                    string errorString = String.Format(
                                        "The type of value {0} for {1} is not supported, expected type: {2}",
                                        category.Id,
                                        "Category ID",
                                        "UInt32");

                                    AddError(validationCheck, "Category", errorString);
                                }

                                if (Enum.TryParse(category.Name.ToString(), out Skyline.DataMiner.CICD.Validators.Common.Model.Category categoryName))
                                {
                                    validationCheck.Category = categoryName;
                                }
                                else
                                {
                                    validationCheck.Category = Skyline.DataMiner.CICD.Validators.Common.Model.Category.Undefined;
                                    string errorString = String.Format(
                                        "The type of value {0} for {1} is not supported, expected type: {2}",
                                        category.Name,
                                        "Category",
                                        "Category");

                                    AddError(validationCheck, "Category", errorString);
                                }
                            }
                            else
                            {
                                validationCheck.Category = Skyline.DataMiner.CICD.Validators.Common.Model.Category.Undefined;
                                string errorString = String.Format(
                                    "The {0} id or Name tag is missing!",
                                    "Category");

                                AddError(validationCheck, "Category", errorString);
                            }
                        }
                        else
                        {
                            validationCheck.Category = Skyline.DataMiner.CICD.Validators.Common.Model.Category.Undefined;
                            string errorString = String.Format("The {0} tag is missing!", "Category");

                            AddError(validationCheck, "Category", errorString);
                        }

                        // Add the Source
                        if (errorMessage.Source != null)
                        {
                            // Check if the Source is not empty
                            if (errorMessage.Source != String.Empty)
                            {
                                if (Enum.TryParse(errorMessage.Source.ToString(), out Source sourceName))
                                {
                                    if (sourceName != Source.Undefined)
                                    {
                                        validationCheck.Source = sourceName;
                                    }
                                    else
                                    {
                                        validationCheck.Source = Source.Undefined;
                                        string errorString = "A correct Source is mandatory!";

                                        AddError(validationCheck, "Source", errorString);
                                    }
                                }
                                else
                                {
                                    validationCheck.Source = Source.Undefined;
                                    string errorString = String.Format(
                                        "The type of value {0} for {1} is not supported, expected type: {2}",
                                        errorMessage.Source,
                                        "Category",
                                        "Category");

                                    AddError(validationCheck, "Source", errorString);
                                }
                            }
                            else
                            {
                                validationCheck.Source = Source.Undefined;
                                string errorString = "A correct Source is mandatory!";

                                AddError(validationCheck, "Source", errorString);
                            }
                        }
                        else
                        {
                            validationCheck.Source = Source.Undefined;
                            string errorString = String.Format("The {0} tag is missing!", "Source");

                            AddError(validationCheck, "Source", errorString);
                        }

                        // Add the ErrorId
                        if (errorMessage.Id != null)
                        {
                            // Check if the id is not empty
                            if (errorMessage.Id != String.Empty)
                            {
                                // Check if Id is correct type
                                if (UInt32.TryParse(errorMessage.Id, out uint errorId))
                                {
                                    // Check if the Id is unique
                                    if (errorIds.Contains(errorId))
                                    {
                                        // Duplicate Id
                                        string errorString = String.Format("The error id {0} is a duplicate!", errorId);

                                        AddError(validationCheck, "ErrorId", errorString);
                                    }

                                    validationCheck.ErrorId = errorId;
                                    errorIds.Add(errorId);
                                }
                                else
                                {
                                    validationCheck.ErrorId = 0;
                                    string errorString = String.Format("The type of value {0} for {1} is not supported, expected type: {2}", errorMessage.Id, "id", "UInt32");

                                    AddError(validationCheck, "ErrorId", errorString);
                                }
                            }
                            else
                            {
                                validationCheck.ErrorId = 0;
                                string errorString = "A correct error ID is mandatory!";

                                AddError(validationCheck, "ErrorId", errorString);
                            }
                        }
                        else
                        {
                            validationCheck.ErrorId = 0;
                            string errorString = "There is no ID attribute present, ID is mandatory!";

                            AddError(validationCheck, "CheckId", errorString);
                        }

                        // Check if a name tag exists
                        if (errorMessage.Name != null)
                        {
                            // Check if name is not null
                            if (errorMessage.Name.Text != null)
                            {
                                validationCheck.Name = errorMessage.Name.Text;
                            }
                            else
                            {
                                validationCheck.Name = String.Empty;
                                string errorString = String.Format(
                                    "There is no Name tag present in error {0}, a name is mandatory!",
                                    validationCheck.ErrorId);
                                AddError(validationCheck, "Name", errorString);
                            }
                        }
                        else
                        {
                            validationCheck.Name = String.Empty;
                            validationCheck.Namespace = String.Empty;
                            string errorString = String.Format(
                                "There is no Name tag present in error {0}, a name is mandatory!",
                                validationCheck.ErrorId);
                            AddError(validationCheck, "Name", errorString);
                        }

                        // Check if a description is present, a description is mandatory!
                        if (errorMessage.Description != null)
                        {
                            // Check if description is a template or a new description
                            if (errorMessage.Description.TemplateId != null)
                            {
                                // Existing Template
                                if (UInt32.TryParse(errorMessage.Description.TemplateId, out uint templateId))
                                {
                                    validationCheck.TemplateId = templateId;
                                    validationCheck.FromTemplate = true;

                                    // Find the corresponding template and add the description and the input parameters to the ValidationCheck
                                    DescriptionTemplate template = null;
                                    int count = 0;
                                    while (template == null && count < xmlValidator.ValidationChecks.DescriptionTemplates.DescriptionTemplate.Count)
                                    {
                                        if (xmlValidator.ValidationChecks.DescriptionTemplates.DescriptionTemplate[count].Id == templateId)
                                        {
                                            template = xmlValidator.ValidationChecks.DescriptionTemplates.DescriptionTemplate[count];
                                        }

                                        count++;
                                    }

                                    if (template != null)
                                    {
                                        if (errorMessage.Description.InputParameters == null)
                                        {
                                            IsCorrectDescription(template, validationCheck);
                                        }
                                        else
                                        {
                                            IsCorrectDescription(template, validationCheck, errorMessage.Description);
                                        }
                                    }
                                    else
                                    {
                                        string errorString = String.Format("The template with the templateId {0} was not found!", templateId.ToString());
                                        AddError(validationCheck, "Description", errorString);
                                        validationCheck.Description = String.Empty;
                                        validationCheck.Parameters = new ObservableCollection<InputParameter>();
                                        if (!ParsingErrors.Contains(errorString))
                                        {
                                            ParsingErrors.Add(errorString);
                                        }
                                    }
                                }
                                else
                                {
                                    string errorString = String.Format(
                                        "The type of value {0} for {1} is not supported, expected type: {2}",
                                        errorMessage.Description.TemplateId,
                                        "templateID",
                                        "UInt32");
                                    AddError(validationCheck, "Description", errorString);

                                    validationCheck.Description = String.Empty;
                                    validationCheck.Parameters = new ObservableCollection<InputParameter>();
                                    if (!ParsingErrors.Contains(errorString))
                                    {
                                        ParsingErrors.Add(errorString);
                                    }
                                }
                            }
                            else
                            {
                                // Create a new description
                                validationCheck.FromTemplate = false;
                                IsCorrectDescription(errorMessage.Description, validationCheck);
                            }
                        }
                        else
                        {
                            // No description tag
                            string errorString = String.Format(
                                "A description is mandatory in the check with ID {0}!",
                                validationCheck.ErrorId);
                            AddError(validationCheck, "Description", errorString);

                            validationCheck.Description = String.Empty;
                            validationCheck.Parameters = new ObservableCollection<InputParameter>();
                        }

                        // Check the warnings
                        if (errorMessage.AutoFixWarnings != null)
                        {
                            foreach (var warning in errorMessage.AutoFixWarnings.AutoFixWarning)
                            {
                                validationCheck.AutoFixWarnings.Add((warning.Text, warning.AutoFixPopup));
                            }
                        }
                        else
                        {
                            validationCheck.AutoFixWarnings = new List<(string, bool)>();
                        }

                        // List with all the properties in the Serialization.Check class
                        var serializationProperties = typeof(ErrorMessage).GetProperties();

                        // Loop through all the properties that are obtained by Reflection on the interface
                        foreach (var prop in checkProperties)
                        {
                            // Checks if the property is already set
                            if (validationCheck.SettedProperties.Contains(prop.Name) || !Types.ContainsKey(prop.PropertyType))
                            {
                                continue;
                            }

                            bool foundMatch = false;
                            int count = 0;
                            while (!foundMatch && count < serializationProperties.Count())
                            {
                                if (serializationProperties[count].Name == prop.Name)
                                {
                                    foundMatch = true;
                                    if (serializationProperties[count].GetValue(errorMessage) != null)
                                    {
                                        if (serializationProperties[count].PropertyType == typeof(XmlCDataSection))
                                        {
                                            var data = (XmlCDataSection)serializationProperties[count].GetValue(errorMessage);
                                            prop.SetValue(validationCheck, data.Value.Replace("[NewLine]", Environment.NewLine));
                                        }
                                        else
                                        {
                                            // Determines the data type and choose the correct action
                                            switch (Types[prop.PropertyType])
                                            {
                                                // string
                                                case 0:
                                                    // Check if XML contains the corresponding Element
                                                    prop.SetValue(validationCheck, serializationProperties[count].GetValue(errorMessage).ToString());
                                                    break;

                                                // uint
                                                case 1:
                                                    if (UInt32.TryParse(serializationProperties[count].GetValue(errorMessage).ToString(), out uint uintResult))
                                                    {
                                                        prop.SetValue(validationCheck, uintResult);
                                                    }
                                                    else
                                                    {
                                                        string errorString = String.Format(
                                                            "The type of value {0} for {1} is not supported, expected type: {2}",
                                                            serializationProperties[count].GetValue(errorMessage).ToString(),
                                                            prop.Name,
                                                            "UInt32");
                                                        AddError(validationCheck, prop.Name, errorString);
                                                    }

                                                    break;

                                                // bool
                                                case 2:
                                                    if (Boolean.TryParse(serializationProperties[count].GetValue(errorMessage).ToString(), out bool boolResult))
                                                    {
                                                        prop.SetValue(validationCheck, boolResult);
                                                    }
                                                    else
                                                    {
                                                        string errorString = String.Format(
                                                            "The type of value {0} for {1} is not supported, expected type: {2}",
                                                            serializationProperties[count].GetValue(errorMessage).ToString(),
                                                            prop.Name,
                                                            "Boolean");
                                                        AddError(validationCheck, prop.Name, errorString);
                                                    }

                                                    break;

                                                // Severity
                                                case 3:
                                                    if (Enum.TryParse(serializationProperties[count].GetValue(errorMessage).ToString(), out Severity severityResult))
                                                    {
                                                        prop.SetValue(validationCheck, severityResult);
                                                    }
                                                    else
                                                    {
                                                        string errorString = String.Format(
                                                            "The type of value {0} for {1} is not supported, expected type: {2}",
                                                            serializationProperties[count].GetValue(errorMessage).ToString(),
                                                            prop.Name,
                                                            "Severity");
                                                        AddError(validationCheck, prop.Name, errorString);
                                                    }

                                                    break;

                                                // Certainty
                                                case 4:
                                                    if (Enum.TryParse(serializationProperties[count].GetValue(errorMessage).ToString(), out Certainty certaintyResult))
                                                    {
                                                        prop.SetValue(validationCheck, certaintyResult);
                                                    }
                                                    else
                                                    {
                                                        string errorString = String.Format(
                                                            "The type of value {0} for {1} is not supported, expected type: {2}",
                                                            serializationProperties[count].GetValue(errorMessage).ToString(),
                                                            prop.Name,
                                                            "Certainty");
                                                        AddError(validationCheck, prop.Name, errorString);
                                                    }

                                                    break;

                                                // FixImpact
                                                case 8:
                                                    if (Enum.TryParse(serializationProperties[count].GetValue(errorMessage).ToString(), out FixImpact fixImpactResult))
                                                    {
                                                        prop.SetValue(validationCheck, fixImpactResult);
                                                    }
                                                    else
                                                    {
                                                        string errorString = String.Format(
                                                            "The type of value {0} for {1} is not supported, expected type: {2}",
                                                            serializationProperties[count].GetValue(errorMessage).ToString(),
                                                            prop.Name,
                                                            "FixImpact");
                                                        AddError(validationCheck, prop.Name, errorString);
                                                    }

                                                    break;

                                                default:
                                                    MessageBox.Show($"Default: {prop.PropertyType.Name}");
                                                    break;
                                            }
                                        }
                                    }
                                }

                                count++;
                            }
                        }

                        checks.Add(validationCheck);
                    }
                }
            }

            HasDuplicates(checks, namespaces);
            return checks;
        }

        /// <summary>
        /// Adds an error message to the check object.
        /// </summary>
        /// <param name="check">The check object to which to error has to be added.</param>
        /// <param name="propertyName">The name of the property of the check where the error occurred.</param>
        /// <param name="errorMessage">The error message that describes the error.</param>
        private static void AddError(Model.Check check, string propertyName, string errorMessage)
        {
            if (check.ErrorMessages.TryGetValue(propertyName, out string previousError))
            {
                check.ErrorMessages.Remove(propertyName);
                check.PropertyHasError[propertyName] = false;
                check.ErrorMessages.Add(propertyName, previousError + "; " + errorMessage);
            }
            else
            {
                check.ErrorMessages.Add(propertyName, errorMessage);
                check.PropertyHasError[propertyName] = true;
                check.Error = true;
            }
        }

        /// <summary>
        /// Checks if a description is complete and if the parameters correspond to the format.
        /// </summary>
        /// <param name="description">The XElement that contains the Description tag from the XML file.</param>
        /// <param name="validationCheck">The check object to which the description applies.</param>
        private static void IsCorrectDescription(Description description, Model.Check validationCheck)
        {
            string formatDescription = String.Empty;

            // Check if description contains the format tag
            if (description.Format != null)
            {
                formatDescription = description.Format;
            }
            else
            {
                string errorString = String.Format(
                    "The description of check {0}, does not contain a Format tag",
                    validationCheck.ErrorId);
                AddError(validationCheck, "Description", errorString);
            }

            // Check if Inputparameters tag exists
            if (description.InputParameters != null)
            {
                // Add the parameters
                validationCheck.Parameters = new ObservableCollection<InputParameter>(description.InputParameters.InputParameter);
            }
            else
            {
                string errorString = String.Format(
                    "The description of check {0}, does not contain a InputParameters tag",
                    validationCheck.ErrorId);
                AddError(validationCheck, "Description", errorString);
            }

            validationCheck.Description = formatDescription;
        }

        /// <summary>
        /// Checks if a description is complete and if the parameters correspond to the format.
        /// </summary>
        /// <param name="description">The XElement that contains the Description tag from the XML file.</param>
        /// <param name="validationCheck">The check object to which the description applies.</param>
        private static void IsCorrectDescription(DescriptionTemplate description, Model.Check validationCheck, Description originalDescription = null)
        {
            string formatDescription = String.Empty;

            // Check if description contains the format tag
            if (description.Format != null)
            {
                formatDescription = description.Format;
            }
            else
            {
                string errorString = String.Format(
                    "The description of check {0}, does not contain a Format tag",
                    validationCheck.ErrorId);
                AddError(validationCheck, "Description", errorString);
            }

            // Check if Inputparameters tag exists
            if (description.TemplateInputParameters != null)
            {
                // Add the parameters
                if (originalDescription == null)
                {
                    validationCheck.Parameters = new ObservableCollection<InputParameter>(description.TemplateInputParameters.InputParameter);
                }
                else
                {
                    validationCheck.Parameters = new ObservableCollection<InputParameter>(originalDescription.InputParameters.InputParameter);

                    IEnumerable<string> coveredIds = originalDescription.InputParameters.InputParameter.Select(x => x.Id);
                    if (description.TemplateInputParameters.InputParameter.Count != originalDescription.InputParameters.InputParameter.Count)
                    {
                        foreach (var item in description.TemplateInputParameters.InputParameter)
                        {
                            // Only for the ones that aren't covered
                            if (coveredIds.Contains(item.Id))
                            {
                                continue;
                            }

                            // Add remaining parameters
                            validationCheck.Parameters.Add(item);
                        }
                    }
                }
            }
            else
            {
                string errorString = String.Format(
                    "The description of check {0}, does not contain a InputParameters tag",
                    validationCheck.ErrorId);
                AddError(validationCheck, "Description", errorString);
            }

            validationCheck.Description = formatDescription;
        }

        /// <summary>
        /// Checks if there are duplicate name's or duplicate descriptions within a namespace.
        /// </summary>
        /// <param name="checks">A list that contains all the checks as check object.</param>
        /// <param name="namespaces">A list that contains all the different namespace's as a string.</param>
        private static void HasDuplicates(ObservableCollection<Model.Check> checks, List<string> namespaces)
        {
            foreach (var ns in namespaces)
            {
                var checkNamesInNamespace = new Dictionary<string, (HashSet<uint> Ids, Model.Check CheckItself)>();
                var checksWithErrorMessages = new Dictionary<(string CheckName, uint CheckId), (Dictionary<string, Model.Check> Names, Dictionary<string, Model.Check> Descriptions)>();
                foreach (var check in checks)
                {
                    if (check.Namespace == ns)
                    {
                        var checkCredentials = (check.CheckName, check.CheckId);

                        if (!checksWithErrorMessages.ContainsKey(checkCredentials))
                        {
                            checksWithErrorMessages.Add(checkCredentials, (new Dictionary<string, Model.Check>(), new Dictionary<string, Model.Check>()));
                        }

                        if (!checkNamesInNamespace.ContainsKey(check.CheckName))
                        {
                            checkNamesInNamespace.Add(check.CheckName, (new HashSet<uint>(), check));
                        }

                        checkNamesInNamespace[check.CheckName].Ids.Add(check.CheckId);

                        // Duplicate error names
                        if (!checksWithErrorMessages[checkCredentials].Names.Keys.Contains(check.Name))
                        {
                            checksWithErrorMessages[checkCredentials].Names.Add(check.Name, check);
                        }
                        else
                        {
                            string errorString = String.Format(
                                "The name : {0}, is a duplicate in the namespace {1}",
                                check.Name,
                                ns);

                            // Current check
                            AddError(check, "Name", errorString);
                            foreach (var item in checksWithErrorMessages[checkCredentials].Names)
                            {
                                if (String.Equals(item.Key, check.Name, StringComparison.Ordinal))
                                {
                                    // Other checks
                                    AddError(item.Value, "Name", errorString);
                                }
                            }

                            ParsingErrors.Add(errorString);
                        }

                        // Duplicate descriptions

                        // Create filled in description to check (with the hard-coded values)
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

                        if (!checksWithErrorMessages[checkCredentials].Descriptions.Keys.Contains(description))
                        {
                            checksWithErrorMessages[checkCredentials].Descriptions.Add(description, check);
                        }
                        else
                        {
                            string errorString = String.Format(
                                "The description : {0}, is a duplicate in the namespace {1}",
                                check.Description,
                                ns);

                            // Current check
                            AddError(check, "Description", errorString);
                            foreach (var item in checksWithErrorMessages[checkCredentials].Descriptions)
                            {
                                if (String.Equals(item.Key, description, StringComparison.Ordinal))
                                {
                                    // Other checks
                                    AddError(item.Value, "Description", errorString);
                                }
                            }

                            ParsingErrors.Add(errorString);
                        }
                    }
                }

                foreach (var item in checkNamesInNamespace)
                {
                    if (item.Value.Ids.Count > 1)
                    {
                        string errorString = String.Format(
                            "The name : {0}, is a duplicate in the namespace {1}",
                            item.Key,
                            ns);
                        AddError(item.Value.CheckItself, "CheckName", errorString);
                        ParsingErrors.Add(errorString);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if there are duplicate templateId's in the XML file.
        /// </summary>
        /// <param name="validator">The deserialized XML file as a Validator object</param>
        private static void HasDuplicateTemplateIDs(Validator validator)
        {
            List<uint> templateIds = new List<uint>();
            if (validator.ValidationChecks.DescriptionTemplates.DescriptionTemplate == null)
            {
                return;
            }

            foreach (var template in validator.ValidationChecks.DescriptionTemplates.DescriptionTemplate)
            {
                if (!templateIds.Contains(template.Id))
                {
                    templateIds.Add(template.Id);
                }
                else
                {
                    ParsingErrors.Add($"The template ID {template.Id} is a duplicate, please edit the XML file manually!");
                }
            }
        }

        /// <summary>
        /// Adds an error message when there are parsing errors with attributes from the XML.
        /// </summary>
        private static void Serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            ParsingErrors.Add($"[{e.LineNumber}] XML Attribute {e.Attr.Name}, not found in the interface");
        }

        /// <summary>
        /// Adds an error message when there are parsing errors with elements from the XML.
        /// </summary>
        private static void Serializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            ParsingErrors.Add($"[{e.LineNumber}] XML Element {e.Element.Name}, not found in the interface");
        }
    }
}