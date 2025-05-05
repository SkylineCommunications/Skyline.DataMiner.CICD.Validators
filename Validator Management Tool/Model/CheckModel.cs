namespace Validator_Management_Tool.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Validator_Management_Tool.Common;
    using Validator_Management_Tool.Interfaces;
    using Validator_Management_Tool.Views;

    /// <summary>
    /// The Check Model that represents an error message.
    /// </summary>
    public class Check : INotifyPropertyChanged, IValidationResult
    {
        private uint checkId;
        private uint errorId;
        private string checkName;
        private string name;
        private string @namespace;
        private Certainty certainty;
        private FixImpact fixImpact;
        private string groupDescription;
        private string description;
        private bool fromTemplate;
        private uint templateId;
        private string howToFix;
        private string exampleCode;
        private string details;
        private Category category;
        private Source source;
        private uint categoryId;
        private Severity severity;
        private bool hasCodeFix;
        private int line;
        private string descriptionFormat;
        private HashSet<string> settedProperties;
        private Dictionary<string, string> errorMessages;
        private Dictionary<string, bool> propertyHasError;
        private bool error;
        private bool hasChanges;
        private ObservableCollection<Serialization.InputParameter> parameters;
        private List<(string Message, bool AutoFixPopup)> autoFixWarnings;

        /// <summary>
        /// Initializes a new instance of the <see cref="Check"/> class.
        /// </summary>
        public Check()
        {
            SettedProperties = new HashSet<string>();
            ErrorMessages = new Dictionary<string, string>();
            Parameters = new ObservableCollection<Serialization.InputParameter>();
            parameters.CollectionChanged += Parameters_CollectionChanged;

            PropertyHasError = new Dictionary<string, bool>();
            foreach (var property in typeof(Check).GetProperties())
            {
                PropertyHasError.Add(property.Name, false);
            }

            autoFixWarnings = new List<(string, bool)>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Check"/> class.
        /// The copy constructor will copy all the properties of the parameter object. 
        /// </summary>
        /// <param name="check">Object from which the new object has to copy his properties.</param>
        public Check(Check check)
        {
            Copy(check);
        }

        /// <summary>
        /// Handles when a property calls an property changed event, to notify the UI.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the ID of the check to which to error message belongs.
        /// </summary>
        public uint CheckId
        {
            get
            {
                return checkId;
            }

            set
            {
                if (checkId != value)
                {
                    checkId = value;
                    RaisePropertyChanged("CheckCollectionId");
                    RaisePropertyChanged("FullId");
                }

                SettedProperties.Add("CheckCollectionId");
                SettedProperties.Add("FullId");

                HasChanges = true;
            }
        }

        /// <summary>
        /// Gets or sets the ID of the error message.
        /// </summary>
        public uint ErrorId
        {
            get
            {
                return errorId;
            }

            set
            {
                if (errorId != value)
                {
                    errorId = value;
                    RaisePropertyChanged("ErrorId");
                    RaisePropertyChanged("FullId");
                }

                SettedProperties.Add("ErrorId");

                HasChanges = true;
            }
        }

        /// <summary>
        /// Gets or sets the name of the check to which the error message belongs.
        /// There is logic that will check if the name is acceptable.
        /// </summary>
        public string CheckName
        {
            get
            {
                return checkName;
            }

            set
            {
                if (value != null)
                {
                    if (ErrorMessages.ContainsKey("CheckName"))
                    {
                        PropertyHasError["CheckName"] = false;
                        ErrorMessages.Remove("CheckName");
                        if (ErrorMessages.Count == 0)
                        {
                            Error = false;
                        }

                        NotifyErrorUI();
                    }

                    if (value == String.Empty)
                    {
                        AddError("CheckName", "Choose a correct check name!");
                    }
                    else if (Settings.ForbiddenStrings.Contains(value))
                    {
                        string errorString = String.Format(
                                    "The checkName '{0}' is a forbidden string!",
                                    value);
                        AddError("CheckName", errorString);
                    }
                    else if (!Char.IsUpper(value[0]))
                    {
                        string errorString = String.Format(
                                    "The checkName '{0}' has to start with a capital!",
                                    value);
                        AddError("CheckName", errorString);
                    }
                    else if (value.IndexOf(" ") != -1)
                    {
                        string errorString = String.Format(
                                    "The checkName '{0}' can't contain a space!",
                                    value);
                        AddError("CheckName", errorString);
                    }
                    else if (value.Any(c => !Char.IsLetter(c)))
                    {
                        string errorString = String.Format(
                                    "The checkName '{0}' can't contain symbols!",
                                    value);
                        AddError("CheckName", errorString);
                    }

                    checkName = value;
                    RaisePropertyChanged("CheckName");
                }
                else
                {
                    AddError("CheckName", "Choose a correct check name!");
                    checkName = String.Empty;
                    RaisePropertyChanged("CheckName");
                }

                SettedProperties.Add("CheckName");

                HasChanges = true;
            }
        }

        /// <summary>
        /// Gets or sets the name of the error message.
        /// There is logic that will check if the name is acceptable.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (name != value)
                {
                    if (ErrorMessages.ContainsKey("Name"))
                    {
                        PropertyHasError["Name"] = false;
                        ErrorMessages.Remove("Name");
                        if (ErrorMessages.Count == 0)
                        {
                            Error = false;
                        }

                        NotifyErrorUI();
                    }


                    if (value == String.Empty)
                    {
                        string errorString = "The name can't be empty!";
                        AddError("Name", errorString);
                    }
                    else if (Settings.ForbiddenStrings.Contains(value))
                    {
                        string errorString = String.Format(
                                    "The name '{0}' is a forbidden string!",
                                    value);
                        AddError("Name", errorString);
                    }
                    else if (!Char.IsUpper(value[0]))
                    {
                        string errorString = String.Format(
                                    "The name '{0}' has to start with a capital!",
                                    value);
                        AddError("Name", errorString);
                    }
                    else if (value.IndexOf(" ") != -1)
                    {
                        string errorString = String.Format(
                                    "The name '{0}' can't contain a space!",
                                    value);
                        AddError("Name", errorString);
                    }
                    else if (Char.IsNumber(value.First()))
                    {
                        string errorString = String.Format(
                                    "The name '{0}' can't start with a number!",
                                    value);
                        AddError("Name", errorString);
                    }
                    else if (value.Any(c => !(Char.IsLetter(c) || Char.IsNumber(c) || Char.Equals(c, '_'))))
                    {
                        string errorString = String.Format(
                                    "The name '{0}' can't contain symbols!",
                                    value);
                        AddError("Name", errorString);
                    }

                    name = value;
                    RaisePropertyChanged("Name");
                }

                SettedProperties.Add("Name");

                HasChanges = true;
            }
        }

        /// <summary>
        /// Gets the concatenation of the namespace and the check name.
        /// </summary>
        public string FullNamespace
        {
            get
            {
                return @namespace + "." + checkName;
            }
        }

        /// <summary>
        /// Gets the full Id of the error message.
        /// </summary>
        public string FullId
        {
            get
            {
                return categoryId.ToString() + "." + checkId.ToString() + "." + errorId.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the namespace to which the check belongs.
        /// There is logic that will check if the namespace is acceptable.
        /// </summary>
        public string Namespace
        {
            get
            {
                return @namespace;
            }

            set
            {
                if (@namespace != value)
                {
                    if (ErrorMessages.ContainsKey("Namespace"))
                    {
                        ErrorMessages.Remove("Namespace");
                        PropertyHasError["Namespace"] = false;
                        if (ErrorMessages.Count == 0)
                        {
                            Error = false;
                        }

                        NotifyErrorUI();
                    }

                    if (String.IsNullOrEmpty(value))
                    {
                        string errorString = "The namespace can't be empty!";
                        AddError("Namespace", errorString);
                    }
                    else
                    {
                        var splitNamespace = value.Split(new string[] { "." }, StringSplitOptions.None);
                        foreach (var namespacePart in splitNamespace)
                        {
                            if (namespacePart != String.Empty)
                            {
                                if (Settings.ForbiddenStrings.Contains(namespacePart))
                                {
                                    string errorString = String.Format(
                                                "The namespace-part '{0}' is a forbidden string!",
                                                namespacePart);
                                    AddError("Namespace", errorString);
                                }
                                else if (!Char.IsUpper(namespacePart[0]))
                                {
                                    string errorString = String.Format(
                                                "The namespace-part '{0}' has to start with a capital!",
                                                namespacePart);
                                    AddError("Namespace", errorString);
                                }
                                else if (namespacePart.IndexOf(" ") != -1)
                                {
                                    string errorString = String.Format(
                                                "The namespace-part '{0}' can't contain a space!",
                                                namespacePart);
                                    AddError("Namespace", errorString);
                                }
                                else if (namespacePart.Any(c => !Char.IsLetter(c) && c != '.'))
                                {
                                    string errorString = String.Format(
                                                "The namespace-part '{0}' can't contain a symbol!",
                                                namespacePart);
                                    AddError("Namespace", errorString);
                                }
                            }
                            else
                            {
                                string errorString = "A part of the namespace can't be empty!";
                                AddError("Namespace", errorString);
                            }
                        }
                    }

                    @namespace = value;
                    RaisePropertyChanged("Namespace");
                }

                SettedProperties.Add("Namespace");

                HasChanges = true;
            }
        }

        /// <summary>
        /// Gets or sets the certainty for the error message.
        /// </summary>
        public Certainty Certainty
        {
            get
            {
                return certainty;
            }

            set
            {
                certainty = value;
                RaisePropertyChanged("Certainty");
                RaisePropertyChanged("CertaintyEnabled");

                SettedProperties.Add("Certainty");

                HasChanges = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a error message is breaking change or not.
        /// </summary>
        public FixImpact FixImpact
        {
            get
            {
                return fixImpact;
            }

            set
            {
                fixImpact = value;
                RaisePropertyChanged("FixImpact");
                RaisePropertyChanged("FixImpactEnabled");

                SettedProperties.Add("FixImpact");

                HasChanges = true;
            }
        }

        /// <summary>
        /// Generic description to be used when grouping multiple instances of an error message.
        /// </summary>
        public string GroupDescription
        {
            get
            {
                return groupDescription;
            }

            set
            {
                groupDescription = value ?? "";

                SettedProperties.Add("GroupDescription");
            }
        }

        /// <summary>
        /// Gets or sets the description string that describes the error message.
        /// There is logic that will check if the description is acceptable.
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                if (ErrorMessages.ContainsKey("Description"))
                {
                    ErrorMessages.Remove("Description");
                    PropertyHasError["Description"] = false;
                    if (ErrorMessages.Count == 0)
                    {
                        Error = false;
                    }

                    NotifyErrorUI();
                }

                if (value != String.Empty)
                {
                    List<int> parameterIndexes = new List<int>();

                    // Get all the positions of the parameters
                    for (int i = value.IndexOf('{'); i > -1; i = value.IndexOf('{', i + 1))
                    {
                        if (i + 2 < value.Length && value[i + 2] == '}' && Int32.TryParse(value[i + 1].ToString(), out int number))
                        {
                            bool exist = false;
                            foreach (var item in parameterIndexes)
                            {
                                if (value[item] == value[i + 1])
                                {
                                    exist = true;
                                }
                            }

                            if (!exist)
                            {
                                parameterIndexes.Add(i + 1);
                            }
                        }
                    }

                    int amountOfParameters = parameterIndexes.Count;

                    // Check if the numbers of the indexes make sense
                    bool incorrect = false;
                    int count = 0;
                    while (!incorrect && count < amountOfParameters)
                    {
                        char index = value[parameterIndexes[count]];
                        if (Int32.Parse(index.ToString()) > amountOfParameters - 1)
                        {
                            incorrect = true;
                        }

                        count++;
                    }

                    if (incorrect)
                    {
                        string errorString = String.Format(
                            "The parameter indexes in the description : {0} are incorrect!",
                            value);
                        AddError("Description", errorString);
                    }
                    else
                    {
                        if (Parameters == null)
                        {
                            if (parameterIndexes.Count > 0)
                            {
                                ObservableCollection<Serialization.InputParameter> parameterCollection = new ObservableCollection<Serialization.InputParameter>();
                                for (int i = 0; i < parameterIndexes.Count; i++)
                                {
                                    parameterCollection.Add(new Serialization.InputParameter { Id = i.ToString(), Text = "newParameter" });
                                }

                                Parameters = parameterCollection;
                            }
                        }
                        else
                        {
                            // Check if the amount of parameters matches with the format string
                            if (Parameters.Count < parameterIndexes.Count)
                            {
                                ObservableCollection<Serialization.InputParameter> parameterCollection = new ObservableCollection<Serialization.InputParameter>();
                                int paramCount = 0;
                                foreach (var param in Parameters)
                                {
                                    parameterCollection.Add(new Serialization.InputParameter { Id = paramCount.ToString(), Text = param.Text });
                                    paramCount++;
                                }

                                int difference = parameterIndexes.Count - parameterCollection.Count;

                                for (int i = 0; i < difference; i++)
                                {
                                    parameterCollection.Add(new Serialization.InputParameter { Id = (paramCount + i).ToString(), Text = "newParameter" });
                                }

                                Parameters = parameterCollection;
                            }
                            else if (Parameters.Count > parameterIndexes.Count)
                            {
                                ObservableCollection<Serialization.InputParameter> parameterCollection = new ObservableCollection<Serialization.InputParameter>();
                                foreach (var param in Parameters)
                                {
                                    parameterCollection.Add(new Serialization.InputParameter { Id = param.Id, Text = param.Text });
                                }

                                int difference = parameterCollection.Count - parameterIndexes.Count;
                                for (int i = 0; i < difference; i++)
                                {
                                    parameterCollection.RemoveAt(parameterCollection.Count - 1);
                                }

                                Parameters = parameterCollection;
                            }
                        }
                    }
                }
                else
                {
                    AddError("Description", "The description can't be empty!");
                }

                description = value;

                SettedProperties.Add("Description");

                CheckDescriptionParameters();
                RaisePropertyChanged("Description");
                HasChanges = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the description is from a description template or not.
        /// </summary>
        public bool FromTemplate
        {
            get
            {
                return fromTemplate;
            }

            set
            {
                if (value != fromTemplate)
                {
                    fromTemplate = value;
                    RaisePropertyChanged("FromTemplate");
                    CheckDescriptionParameters();
                }
            }
        }

        /// <summary>
        /// Gets or sets the template id if the description is from a description template.
        /// </summary>
        public uint TemplateId
        {
            get
            {
                return templateId;
            }

            set
            {
                if (value != templateId)
                {
                    templateId = value;
                    RaisePropertyChanged("TemplateId");
                }
            }
        }

        /// <summary>
        /// Gets or sets the description of how to fix the error message.
        /// </summary>
        public string HowToFix
        {
            get
            {
                return howToFix;
            }

            set
            {
                if (howToFix != value)
                {
                    howToFix = value;
                    RaisePropertyChanged("HowToFix");
                    RaisePropertyChanged("HowToFixEnabled");

                    if (value == null)
                    {
                        SettedProperties.Remove("HowToFix");
                    }
                    else
                    {
                        SettedProperties.Add("HowToFix");
                    }

                    HasChanges = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets a piece of example code to fix the error.
        /// </summary>
        public string ExampleCode
        {
            get
            {
                return exampleCode;
            }

            set
            {
                if (exampleCode != value)
                {
                    exampleCode = value;
                    RaisePropertyChanged("ExampleCode");
                }

                if (value == null)
                {
                    SettedProperties.Remove("ExampleCode");
                }
                else
                {
                    SettedProperties.Add("ExampleCode");
                }

                HasChanges = true;
            }
        }

        /// <summary>
        /// Gets or sets some details about the error message.
        /// </summary>
        public string Details
        {
            get
            {
                return details;
            }

            set
            {
                if (details != value)
                {
                    details = value;
                    RaisePropertyChanged("Details");
                }

                if (value == null)
                {
                    SettedProperties.Remove("Details");
                }
                else
                {
                    SettedProperties.Add("Details");
                }

                HasChanges = true;
            }
        }

        /// <summary>
        /// Gets or sets the category of the error message.
        /// </summary>
        public Category Category
        {
            get
            {
                return category;
            }

            set
            {
                if (category != value)
                {
                    if (ErrorMessages.ContainsKey("Category"))
                    {
                        PropertyHasError["Category"] = false;
                        ErrorMessages.Remove("Category");
                        if (ErrorMessages.Count == 0)
                        {
                            Error = false;
                        }

                        NotifyErrorUI();
                    }

                    if (value == Category.Undefined)
                    {
                        AddError("Category", "Choose a correct Category!");
                    }

                    category = value;
                    RaisePropertyChanged("Category");
                }

                SettedProperties.Add("Category");

                HasChanges = true;
            }
        }

        /// <summary>
        /// Gets or sets the source of the error message.
        /// </summary>
        public Source Source
        {
            get
            {
                return source;
            }

            set
            {
                if (source != value)
                {
                    if (ErrorMessages.ContainsKey("Source"))
                    {
                        PropertyHasError["Source"] = false;
                        ErrorMessages.Remove("Source");
                        if (ErrorMessages.Count == 0)
                        {
                            Error = false;
                        }

                        NotifyErrorUI();
                    }

                    if (value == Source.Undefined)
                    {
                        AddError("Source", "Choose a correct Source!");
                    }

                    source = value;
                    RaisePropertyChanged("Source");
                }

                SettedProperties.Add("Source");

                HasChanges = true;
            }
        }

        /// <summary>
        /// Gets or sets the ID of the category to which the error message belongs.
        /// </summary>
        public uint CategoryId
        {
            get
            {
                return categoryId;
            }

            set
            {
                if (value != categoryId)
                {
                    categoryId = value;
                    RaisePropertyChanged("CategoryId");
                    RaisePropertyChanged("FullId");
                }
            }
        }

        /// <summary>
        /// Gets or sets the severity of the error message.
        /// </summary>
        public Severity Severity
        {
            get
            {
                return severity;
            }

            set
            {
                severity = value;
                RaisePropertyChanged("Severity");
                RaisePropertyChanged("SeverityEnabled");

                SettedProperties.Add("Severity");

                HasChanges = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the error message has a code fix or not.
        /// </summary>
        public bool HasCodeFix
        {
            get
            {
                return hasCodeFix;
            }

            set
            {
                if (hasCodeFix != value)
                {
                    hasCodeFix = value;
                    RaisePropertyChanged("HasCodeFix");
                }

                SettedProperties.Add("HasCodeFix");

                HasChanges = true;
            }
        }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        public int Line
        {
            get
            {
                return line;
            }

            set
            {
                if (line != value)
                {
                    line = value;
                    RaisePropertyChanged("Line");
                }
            }
        }

        /// <summary>
        /// Gets or sets the format string for the description of the error message.
        /// </summary>
        public string DescriptionFormat
        {
            get
            {
                return descriptionFormat;
            }

            set
            {
                if (descriptionFormat != value)
                {
                    descriptionFormat = value;
                    RaisePropertyChanged("DescriptionFormat");
                }
            }
        }

        /// <summary>
        /// Gets or sets a collection of serialization input parameters that is used for dynamic binding to the View.
        /// </summary>
        public ObservableCollection<Serialization.InputParameter> Parameters
        {
            get
            {
                return parameters;
            }

            set
            {
                parameters = value;
                parameters.CollectionChanged += Parameters_CollectionChanged;
                HasChanges = true;
                RaisePropertyChanged("Parameters");
            }
        }

        /// <summary>
        /// Gets or sets a list that holds the names of the properties that are set while parsing the XML file
        /// or when editing the error messages in the UI. This list is used to know which properties
        /// needs to be asked as a parameter in the generated classes and also to determine whether a
        /// tag needs to be written to the XML.
        /// </summary>
        public HashSet<string> SettedProperties
        {
            get
            {
                return settedProperties;
            }

            set
            {
                if (settedProperties != value)
                {
                    settedProperties = value;
                    RaisePropertyChanged("SettedProperties");
                }
            }
        }

        /// <summary>
        /// Gets or sets a dictionary that contains the property name as key and the error message(s) as value if there is an error.
        /// </summary>
        public Dictionary<string, string> ErrorMessages
        {
            get
            {
                return errorMessages;
            }

            set
            {
                if (errorMessages != value)
                {
                    errorMessages = value;
                    RaisePropertyChanged("ErrorMessage");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the error message has an error or not.
        /// </summary>
        public bool Error
        {
            get
            {
                return error;
            }

            set
            {
                if (error != value)
                {
                    error = value;
                    RaisePropertyChanged("Error");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the error message has changes or not.
        /// </summary>
        public bool HasChanges
        {
            get
            {
                return hasChanges;
            }

            set
            {
                if (value != hasChanges)
                {
                    hasChanges = value;
                    RaisePropertyChanged("HasChanges");
                }
            }
        }

        /// <summary>
        /// Gets or sets a dictionary that has all the properties as key and for each property has a boolean
        /// as value that decides whether a property has an error.
        /// </summary>
        public Dictionary<string, bool> PropertyHasError
        {
            get
            {
                return propertyHasError;
            }

            set
            {
                if (propertyHasError != value)
                {
                    propertyHasError = value;
                    RaisePropertyChanged("PropertyHasError");
                }
            }
        }

        /// <summary>
        /// Gets the command that opens a new detail view window when called from the View.
        /// </summary>
        public MyCommand EditCommand
        {
            get
            {
                return new MyCommand(OnEdit);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the how to fix tag should be included in the XML or not.
        /// </summary>
        public bool HowToFixEnabled
        {
            get
            {
                return SettedProperties.Contains("HowToFix");
            }

            set
            {
                if (value)
                {
                    if (HowToFix == null)
                    {
                        HowToFix = String.Empty;
                    }

                    SettedProperties.Add("HowToFix");
                }
                else
                {
                    HowToFix = null;
                    SettedProperties.Remove("HowToFix");
                }

                RaisePropertyChanged("HowToFixEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the certainty tag should be included in the XML or not.
        /// </summary>
        public bool CertaintyEnabled
        {
            get
            {
                return SettedProperties.Contains("Certainty");
            }

            set
            {
                if (value)
                {
                    SettedProperties.Add("Certainty");
                }
                else
                {
                    SettedProperties.Remove("Certainty");
                }

                RaisePropertyChanged("CertaintyEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the severity tag should be included in the XML or not.
        /// </summary>
        public bool SeverityEnabled
        {
            get
            {
                return SettedProperties.Contains("Severity");
            }

            set
            {
                if (value)
                {
                    SettedProperties.Add("Severity");
                }
                else
                {
                    SettedProperties.Remove("Severity");
                }

                RaisePropertyChanged("SeverityEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the has code fix tag should be included in the XML or not.
        /// </summary>
        public bool HasCodeFixEnabled
        {
            get
            {
                return SettedProperties.Contains("HasCodeFix");
            }

            set
            {
                if (value)
                {
                    SettedProperties.Add("HasCodeFix");
                }
                else
                {
                    SettedProperties.Remove("HasCodeFix");
                }

                RaisePropertyChanged("HasCodeFixEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the is breaking change tag should be included in the XML or not.
        /// </summary>
        public bool FixImpactEnabled
        {
            get
            {
                return SettedProperties.Contains("FixImpact");
            }

            set
            {
                if (value)
                {
                    SettedProperties.Add("FixImpact");
                }
                else
                {
                    SettedProperties.Remove("FixImpact");
                }

                RaisePropertyChanged("FixImpactEnabled");
            }
        }

        public IReadable ReferenceNode => null;

        public IReadable PositionNode => null;

        public int Position => -1;

        public List<IValidationResult> SubResults => null;

        public object[] DescriptionParameters => throw new NotImplementedException();

        public List<(string Message, bool AutoFixPopup)> AutoFixWarnings
        {
            get => autoFixWarnings;

            set
            {
                autoFixWarnings = value;
                RaisePropertyChanged("AutoFixWarnings");

                SettedProperties.Add("AutoFixWarnings");

                HasChanges = true;
            }
        }

        public (int TablePid, string Name)? DveExport => throw new NotImplementedException();

        /// <summary>
        /// Copies all the properties of the parameter object.
        /// </summary>
        /// <param name="check">Object from which the called object has to copy his properties.</param>
        public void Copy(Check check)
        {
            ErrorMessages = new Dictionary<string, string>(check.ErrorMessages);
            Error = check.Error;
            SettedProperties = new HashSet<string>(check.SettedProperties);
            PropertyHasError = new Dictionary<string, bool>(check.PropertyHasError);
            HasChanges = check.HasChanges;

            Parameters = new ObservableCollection<Serialization.InputParameter>();
            foreach (var item in check.Parameters)
            {
                // Creation of a new InputParameter to stop the sharing of data by reference
                Parameters.Add(new Serialization.InputParameter { Id = item.Id, Text = item.Text, Value = item.Value });
            }

            var checkProperties = typeof(Check).GetProperties();
            foreach (var property in checkProperties)
            {
                if (check.SettedProperties.Contains(property.Name) && property.GetSetMethod() != null)
                {
                    property.SetValue(this, property.GetValue(check));
                }
            }

            CategoryId = check.CategoryId;
            CheckId = check.CheckId;
            Line = check.Line;
            DescriptionFormat = check.DescriptionFormat;
            FromTemplate = check.FromTemplate;
            TemplateId = check.TemplateId;
            AutoFixWarnings = new List<(string, bool)>(check.AutoFixWarnings ?? new List<(string, bool)>());
        }

        /// <summary>
        /// Checks if the parameters meet the requirements.
        /// If not, errors are generated.
        /// Is called whenever the description parameters are changed.
        /// </summary>
        public void CheckDescriptionParameters()
        {
            if (ErrorMessages.ContainsKey("Parameters"))
            {
                ErrorMessages.Remove("Parameters");
                PropertyHasError["Parameters"] = false;
                if (ErrorMessages.Count == 0)
                {
                    Error = false;
                }

                NotifyErrorUI();
            }

            List<string> parameterList = new List<string>();
            foreach (var parameter in Parameters)
            {
                if (parameter.Text == String.Empty)
                {
                    AddError("Parameters", "The description parameter can't be empty!");
                    continue;
                }

                if (parameterList.Contains(parameter.Text))
                {
                    AddError("Parameters", $"The error message {Name} has duplicate description parameters: {parameter.Text}!");
                }
                else if (!Char.IsLower(parameter.Text[0]))
                {
                    AddError("Parameters", $"The description parameter '{parameter.Text}' has to start with a lowercase!");
                }
                else if (parameter.Text.IndexOf(" ") != -1)
                {
                    AddError("Parameters", $"The description parameter '{parameter.Text}' can't contain a space!");
                }
                else if (Settings.ForbiddenStrings.Contains(parameter.Text))
                {
                    AddError("Parameters", $"The description parameter '{parameter.Text}' contains a forbidden string!");
                }
                else if (parameter.Text.Any(c => !(Char.IsLetter(c) || Char.IsNumber(c))) && !Char.IsNumber(parameter.Text[0]))
                {
                    string errorString = $"The description parameter '{parameter.Text}' can't contain symbols!";
                    AddError("Parameters", errorString);
                }
                else if (parameter.Text == "newParameter")
                {
                    AddError("Parameters", $"The description parameter {parameter.Id} has to be named!");
                }

                parameterList.Add(parameter.Text);
            }
        }

        /// <summary>
        /// Refreshes the error for the description parameters.
        /// </summary>
        public void RefreshParameters()
        {
            CheckDescriptionParameters();
        }

        /// <summary>
        /// Notifies the UI and is called whenever a error from a property is changed/added.
        /// </summary>
        private void NotifyErrorUI()
        {
            RaisePropertyChanged("PropertyHasError");
        }

        /// <summary>
        /// Adds a error to the error message. It will set and edit all the required
        /// values and list so that the error will be shown in the View.
        /// </summary>
        /// <param name="propertyName">The name of the property that has an error.</param>
        /// <param name="errorMessage">The error message that describes the error for the user.</param>
        private void AddError(string propertyName, string errorMessage)
        {
            if (ErrorMessages.TryGetValue(propertyName, out string previousError))
            {
                ErrorMessages.Remove(propertyName);
                PropertyHasError[propertyName] = true;
                ErrorMessages.Add(propertyName, previousError + "; " + errorMessage);
                NotifyErrorUI();
                RaisePropertyChanged("ErrorMessages");
            }
            else
            {
                ErrorMessages.Add(propertyName, errorMessage);
                PropertyHasError[propertyName] = true;
                Error = true;
                NotifyErrorUI();
                RaisePropertyChanged("ErrorMessages");
            }
        }

        /// <summary>
        /// Is added to the parameter list to notify the UI when the list is changed.
        /// </summary>
        private void Parameters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CheckDescriptionParameters();
            if (e.NewItems != null)
            {
                foreach (object parameter in e.NewItems)
                {
                    (parameter as INotifyPropertyChanged).PropertyChanged
                        += new PropertyChangedEventHandler(Parameter_PropertyChanged);
                }
            }

            if (e.OldItems != null)
            {
                foreach (object parameter in e.OldItems)
                {
                    (parameter as INotifyPropertyChanged).PropertyChanged
                        -= new PropertyChangedEventHandler(Parameter_PropertyChanged);
                }
            }
        }

        /// <summary>
        /// Is added to the parameter list to get notified when a property of a member of the list is changed.
        /// </summary>
        private void Parameter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CheckDescriptionParameters();
        }

        /// <summary>
        /// Notifies the UI that the given property is changed.
        /// </summary>
        /// <param name="property">The name of the property that is changed.</param>
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Is executed when the EditCommand is called through the View.
        /// Will open a new edit view window for this error message.
        /// </summary>
        private void OnEdit()
        {
            CheckEditView checkEditView = new CheckEditView(this);
            checkEditView.ShowDialog();
        }
    }
}