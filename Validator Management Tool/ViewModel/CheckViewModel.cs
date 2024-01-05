namespace Validator_Management_Tool.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Threading;

    using Validator_Management_Tool.Common;
    using Validator_Management_Tool.Interfaces;
    using Validator_Management_Tool.Model;
    using Validator_Management_Tool.Views;

    /// <summary>
    /// The Viewmodel for the main check overview.
    /// </summary>
    public class CheckViewModel : BindableBase
    {
        private static ObservableCollection<Check> checks;
        private bool hasErrors;
        private bool categoryExpanderToggle = true;
        private bool namespaceExpanderToggle = true;
        private bool checkNameExpanderToggle = true;
        private bool hasChanges = false;
        private int currentErrorIndex = 0;
        private string searchString;
        private bool isCollapsed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckViewModel"/> class.
        /// Loads the checks with the <see cref="Serialization.Serializer"/> static methods.
        /// </summary>
        public CheckViewModel()
        {
            checks = new ObservableCollection<Check>();
            checks.CollectionChanged += Checks_CollectionChanged;

            ErrorChecks = new List<Check>();

            this.LoadChecks();
            this.GenerateFilesCommand = new MyCommand(this.OnGenerateFiles);
            this.ExpandCommand = new MyCommand(this.OnExpand);
            this.RefreshCommand = new MyCommand(this.OnRefresh);
            this.SaveCommand = new MyCommand(OnSave);
            this.AddCheckCommand = new MyCommand(this.OnAddCheck);
            this.DeleteCommand = new MyTCommand<Check>(this.OnDelete);
            this.OpenCheckCommand = new MyCommand(this.OnOpenCheck);
            this.UpArrowCommand = new MyCommand(this.OnUpArrow);
            this.DownArrowCommand = new MyCommand(this.OnDownArrow);
            this.GenerateExcelCommand = new MyCommand(this.OnGenerateExcel);

            Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);

            View = CollectionViewSource.GetDefaultView(Checks);
            View.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            View.GroupDescriptions.Add(new PropertyGroupDescription("Namespace"));
            View.GroupDescriptions.Add(new PropertyGroupDescription("CheckName"));
            View.Filter = Filter;
        }

        public ICollectionView View;

        public string CollapseButtonString
        {
            get
            {
                if (isCollapsed)
                {
                    return "Expand All";
                }
                else
                {
                    return "Collapse All";
                }
            }
        }

        public string SearchString
        {
            get
            {
                return searchString;
            }

            set
            {
                if (value != searchString)
                {
                    searchString = value;
                    OnPropertyChanged("SearchString");
                    View.Refresh();
                }
            }
        }

        /// <summary>
        /// Gets or sets the collection that holds all the error messages.
        /// </summary>
        public static ObservableCollection<Check> Checks
        {
            get
            {
                return checks;
            }

            set
            {
                checks.Clear();
                foreach (var check in value)
                {
                    checks.Add(check);
                }
            }
        }

        /// <summary>
        /// Gets or sets the collection that holds all the description templates.
        /// </summary>
        public static List<Serialization.DescriptionTemplate> Templates { get; set; }

        /// <summary>
        /// Gets or sets the collection that holds all the categories and checks.
        /// </summary>
        public static List<Serialization.Category> Categories { get; set; }

        /// <summary>
        /// Gets or sets the command to refresh the view.
        /// </summary>
        public MyCommand RefreshCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to generate the classes in files.
        /// </summary>
        public MyCommand GenerateFilesCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to collapse the tree-view.
        /// </summary>
        public MyCommand ExpandCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to save the checks.
        /// </summary>
        public MyCommand SaveCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to add a new error message
        /// </summary>
        public MyCommand AddCheckCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to delete a error message from the list.
        /// </summary>
        public MyTCommand<Check> DeleteCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to open the edit window for a error message.
        /// </summary>
        public MyCommand OpenCheckCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to go to the next error.
        /// </summary>
        public MyCommand UpArrowCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to go to the previous error.
        /// </summary>
        public MyCommand DownArrowCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to generate an excel worksheet.
        /// </summary>
        public MyCommand GenerateExcelCommand { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether one of the checks has changes or not.
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
                    OnPropertyChanged("HasChanges");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the category expanders are open or not.
        /// </summary>
        public bool CategoryExpanderToggle
        {
            get
            {
                return categoryExpanderToggle;
            }

            set
            {
                categoryExpanderToggle = value;
                OnPropertyChanged("CategoryExpanderToggle");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the namespace expanders are open or not.
        /// </summary>
        public bool NamespaceExpanderToggle
        {
            get
            {
                return namespaceExpanderToggle;
            }

            set
            {
                namespaceExpanderToggle = value;
                OnPropertyChanged("NamespaceExpanderToggle");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the check name expanders are open or not.
        /// </summary>
        public bool CheckNameExpanderToggle
        {
            get
            {
                return checkNameExpanderToggle;
            }

            set
            {
                checkNameExpanderToggle = value;
                OnPropertyChanged("CheckNameExpanderToggle");
            }
        }

        /// <summary>
        /// Gets the currently selected check with an error.
        /// </summary>
        public Check SelectedErrorCheck
        {
            get
            {
                if (ErrorChecks.Count > 0)
                {
                    return ErrorChecks[currentErrorIndex];
                }

                return new Check();
            }
        }

        /// <summary>
        /// Gets or sets the list with all the checks that have an error.
        /// </summary>
        public List<Check> ErrorChecks { get; set; }

        /// <summary>
        /// Gets a value indicating whether the up arrow on the error navigation is enabled or not.
        /// </summary>
        public bool UpArrowEnabled
        {
            get
            {
                return currentErrorIndex < ErrorChecks.Count - 1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the down arrow on the error navigation is enabled or not.
        /// </summary>
        public bool DownArrowEnabled
        {
            get
            {
                return currentErrorIndex > 0;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether one of the checks has an error or not.
        /// </summary>
        public bool HasErrors
        {
            get
            {
                return hasErrors;
            }

            set
            {
                if (value != hasErrors)
                {
                    hasErrors = value;
                    OnPropertyChanged("HasErrors");
                }
            }
        }

        /// <summary>
        /// Reads the checks from the XML and updates the display.
        /// </summary>
        public void LoadChecks()
        {
            Serialization.Serializer.ReadXml(Settings.XmlPath);

            foreach (var check in Serialization.Serializer.GetChecks())
            {
                Checks.Add(check);
            }

            Checks = new ObservableCollection<Check>(
                Checks.OrderBy(c => c.CategoryId).ThenBy(x => x.Namespace).ThenBy(c => c.CheckId).ThenBy(c => c.ErrorId).ToList());

            if (Serialization.Serializer.ParsingErrors.Count > 0)
            {
                StringBuilder error = new StringBuilder();
                foreach (var parsingError in Serialization.Serializer.ParsingErrors)
                {
                    error.Append(parsingError + "\n");
                }

                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    (Action)(() => { MessageBox.Show(error.ToString(), "Alert", MessageBoxButton.OK, MessageBoxImage.Information); }));
            }

            foreach (var check in Checks)
            {
                check.HasChanges = false;
            }

            Templates = Serialization.Serializer.GetTemplates().OrderBy(template => template.Id).ToList();
            Categories = Serialization.Serializer.GetCategories();

            GetErrorsAndChanges();
        }

        /// <summary>
        /// Checks if the checks have errors or changes.
        /// </summary>
        public void GetErrorsAndChanges()
        {
            List<Check> errorList = new List<Check>();
            foreach (var check in Checks)
            {
                if (check.Error)
                {
                    errorList.Add(check);
                }
            }

            ErrorChecks = errorList;
            if (ErrorChecks.Count != 0)
            {
                HasErrors = true;
                currentErrorIndex = 0;
                UpdateErrorNavigation();
            }
            else
            {
                HasErrors = false;
            }

            bool changes = false;
            int count = 0;
            while (!changes && count < Checks.Count)
            {
                if (Checks[count].HasChanges)
                {
                    changes = true;
                }

                count++;
            }

            HasChanges = changes;
        }

        /// <summary>
        /// Is executed on the main window closing event.
        /// </summary>
        public void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (HasChanges)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show(
                    "There are unsaved changed. Would you like to save them before we close the application?",
                    "Close Confirmation",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Exclamation);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    OnSave();
                }
                else if (messageBoxResult == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Is executed when the checks collection changes.
        /// </summary>
        private void Checks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            GetErrorsAndChanges();
            if (e.NewItems != null)
            {
                foreach (object check in e.NewItems)
                {
                    (check as INotifyPropertyChanged).PropertyChanged
                        += new PropertyChangedEventHandler(Check_PropertyChanged);
                }
            }

            if (e.OldItems != null)
            {
                foreach (object check in e.OldItems)
                {
                    (check as INotifyPropertyChanged).PropertyChanged
                        -= new PropertyChangedEventHandler(Check_PropertyChanged);
                }
            }
        }

        /// <summary>
        /// Is executed when a property of a check in the collection is changed.
        /// </summary>
        private void Check_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            GetErrorsAndChanges();
            OnPropertyChanged("Checks");
        }

        /// <summary>
        /// Notifies the View if the errors in the checks are changed.
        /// </summary>
        private void UpdateErrorNavigation()
        {
            OnPropertyChanged("UpArrowEnabled");
            OnPropertyChanged("DownArrowEnabled");
            OnPropertyChanged("SelectedErrorCheck");
        }

        /// <summary>
        /// Generates the classes into files.
        /// </summary>
        private void OnGenerateFiles()
        {
            if (HasChanges)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show(
                    "There are unsaved checks, do you want to Save first and Generate afterwards?",
                    "Unsaved Checks",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    OnSave();
                    TestGenerator.GenerateFiles(Checks);
                    MessageBox.Show("Generating Done!", "Generation Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                TestGenerator.GenerateFiles(Checks);
                MessageBox.Show("Generating Done!", "Generation Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Generates an excel worksheet with all the error messages.
        /// </summary>
        private void OnGenerateExcel()
        {
            if (HasChanges)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show(
                    "There are unsaved checks, do you want to Save first and Generate afterwards?",
                    "Unsaved Checks",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    OnSave();
                    ExportManager.ExportToExcel(Checks);
                    MessageBox.Show("Exporting Done!", "Export Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                ExportManager.ExportToExcel(Checks);
                MessageBox.Show("Exporting Done!", "Export Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Collapses or expands the grouping headers.
        /// </summary>
        private void OnExpand()
        {
            if (isCollapsed)
            {
                CategoryExpanderToggle = true;
                NamespaceExpanderToggle = true;
                CheckNameExpanderToggle = true;

                isCollapsed = false;
                OnPropertyChanged("CollapseButtonString");
            }
            else
            {
                CategoryExpanderToggle = false;
                NamespaceExpanderToggle = false;
                CheckNameExpanderToggle = false;

                isCollapsed = true;
                OnPropertyChanged("CollapseButtonString");
            }
        }

        /// <summary>
        /// Refreshes the view and reads the XML again.
        /// </summary>
        private void OnRefresh()
        {
            if (MessageBox.Show("Any unsaved changes will be removed and the file will be refreshed with the items from the XML.", "Warning", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Checks.Clear();
                LoadChecks();
            }
        }

        private void CheckDuplicates()
        {
            List<string> parsingErrors = new List<string>();

            List<string> namespaces = Checks.Select(x => x.Namespace).Distinct().ToList();

            foreach (var ns in namespaces)
            {
                var checkNamesInNamespace = new Dictionary<string, (HashSet<uint> Ids, Check CheckItself)>();
                var checksWithErrorMessages = new Dictionary<(string CheckName, uint CheckId), (Dictionary<string, Check> Names, Dictionary<string, Check> Descriptions)>();
                foreach (var check in Checks)
                {
                    if (check.Namespace == ns)
                    {
                        var checkCredentials = (check.CheckName, check.CheckId);

                        if (!checksWithErrorMessages.ContainsKey(checkCredentials))
                        {
                            checksWithErrorMessages.Add(checkCredentials, (new Dictionary<string, Check>(), new Dictionary<string, Check>()));
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

                            parsingErrors.Add(errorString);
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

                            parsingErrors.Add(errorString);
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
                        parsingErrors.Add(errorString);
                    }
                }
            }

            if (parsingErrors.Count > 0)
            {
                StringBuilder error = new StringBuilder();
                foreach (var parsingError in parsingErrors)
                {
                    error.Append(parsingError + "\n");
                }

                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    (Action)(() => { MessageBox.Show(error.ToString(), "Alert", MessageBoxButton.OK, MessageBoxImage.Information); }));
            }
        }

        private static void AddError(Check check, string propertyName, string errorMessage)
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
        /// Deletes a check from the list.
        /// </summary>
        /// <param name="check">The check that has to be deleted.</param>
        private void OnDelete(Check check)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show(
                "Are you sure?",
                "Delete Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Checks.Remove(check);
                GetErrorsAndChanges();
                HasChanges = true;
            }
        }

        /// <summary>
        /// Saves the current check list to the XML file.
        /// </summary>
        private void OnSave()
        {
            if (HasErrors)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show(
                "There are some issues detected, are you sure you want to save anyway?",
                "Save Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    Serialization.Serializer.SetChecks(Checks, Templates, Categories);
                    Serialization.Serializer.WriteXml(Settings.XmlPath);
                    foreach (var check in Checks)
                    {
                        check.HasChanges = false;
                    }

                    GetErrorsAndChanges();
                    MessageBox.Show("Saving Done!", "Saving Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                Serialization.Serializer.SetChecks(Checks, Templates, Categories);
                Serialization.Serializer.WriteXml(Settings.XmlPath);
                foreach (var check in Checks)
                {
                    check.HasChanges = false;
                }

                GetErrorsAndChanges();
                MessageBox.Show("Saving Done!", "Saving Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Opens the window to add a new check.
        /// </summary>
        private void OnAddCheck()
        {
            AddCheckView addCheckView = new AddCheckView();
            addCheckView.ShowDialog();
            GetErrorsAndChanges();
            CheckDuplicates();
        }

        /// <summary>
        /// Opens the edit window for the check that has an error and is selected.
        /// </summary>
        private void OnOpenCheck()
        {
            CheckEditView checkEditView = new CheckEditView(SelectedErrorCheck);
            checkEditView.ShowDialog();
        }

        /// <summary>
        /// Show the next check with an error.
        /// </summary>
        private void OnUpArrow()
        {
            if (currentErrorIndex < ErrorChecks.Count - 1)
            {
                currentErrorIndex++;
                UpdateErrorNavigation();
            }
        }

        /// <summary>
        /// Show the previous check with an error.
        /// </summary>
        private void OnDownArrow()
        {
            if (currentErrorIndex > 0)
            {
                currentErrorIndex--;
                UpdateErrorNavigation();
            }
        }

        private bool Filter(object item)
        {
            if (String.IsNullOrEmpty(SearchString))
            {
                return true;
            }
            else
            {
                List<string> filterWords = new List<string>();
                if (SearchString.StartsWith("\"") && SearchString.EndsWith("\""))
                {
                    // One group of filter words
                    filterWords.Add(SearchString.Split(new char[] { '\"' })[1]);
                }
                else
                {
                    // Different filter words
                    var splitFilter = SearchString.Split(new char[] { ' ' });
                    foreach (var word in splitFilter)
                    {
                        filterWords.Add(word);
                    }
                }


                Check checkItem = item as Check;

                List<string> stringProperties = new List<string>();
                var checkProperties = typeof(Check).GetProperties();

                // List all the properties as a string
                foreach (var property in checkProperties)
                {
                    if (property.Name == "DescriptionParameters" && checkItem.SettedProperties.Contains(property.Name))
                    {
                        foreach (string parameter in (object[])property.GetValue(checkItem))
                        {
                            stringProperties.Add(parameter);
                        }
                    }
                    else if (checkItem.SettedProperties.Contains(property.Name))
                    {
                        stringProperties.Add(property.GetValue(checkItem).ToString());
                    }
                }

                int matchCount = 0;

                // Find a match for each filter word
                foreach (var filterWord in filterWords)
                {
                    bool match = false;
                    int stringCount = 0;
                    while (!match && stringCount < stringProperties.Count)
                    {
                        if (stringProperties[stringCount].IndexOf(filterWord, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            match = true;
                        }

                        stringCount++;
                    }

                    if (match)
                    {
                        matchCount++;
                    }
                }

                // Check if each filter word has a match in the check, only then the check can be displayed
                return matchCount >= filterWords.Count;
            }
        }
    }
}