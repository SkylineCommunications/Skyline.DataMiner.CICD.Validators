namespace Validator_Management_Tool.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Validator_Management_Tool.Interfaces;
    using Validator_Management_Tool.Model;
    using Validator_Management_Tool.Views;

    /// <summary>
    /// The Viewmodel for the Edit Error Message window.
    /// </summary>
    public class CheckEditViewModel : BindableBase
    {
        private readonly bool originalFromTemplate;
        private readonly Check newCheck;
        private Check check;
        private List<Severity> severities;
        private List<Certainty> certainties;
        private List<FixImpact> fixImpacts;
        private List<Category> categories;
        private List<Source> sources;
        private List<string> selectedNamespaces;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckEditViewModel"/> class.
        /// </summary>
        /// <param name="check">The check that has to be edited.</param>
        public CheckEditViewModel(Check check)
        {
            newCheck = check;
            this.check = new Check(check);
            CancelCommand = new MyCommand(OnCancel);
            SaveCommand = new MyCommand(OnSave);
            RefreshParametersCommand = new MyCommand(OnRefreshParameters);
            AddNewCheckCommand = new MyTCommand<string>(OnAddNewCheck);
            AddNewNamespaceCommand = new MyCommand(OnAddNewNamespace);

            severities = new List<Severity>(Enum.GetValues(typeof(Severity)).Cast<Severity>().ToList());
            certainties = new List<Certainty>(Enum.GetValues(typeof(Certainty)).Cast<Certainty>().ToList());
            fixImpacts = new List<FixImpact>(Enum.GetValues(typeof(FixImpact)).Cast<FixImpact>().ToList());
            categories = new List<Category>(Enum.GetValues(typeof(Category)).Cast<Category>().ToList());
            sources = new List<Source>(Enum.GetValues(typeof(Source)).Cast<Source>().ToList());

            GetExistingNamespace();
            CalculateNextIds();

            originalFromTemplate = Check.FromTemplate;
        }

        public List<string> SelectedNamespaces
        {
            get
            {
                selectedNamespaces.Sort();
                return selectedNamespaces;
            }

            set
            {
                if (value != selectedNamespaces)
                {
                    selectedNamespaces = value;
                    selectedNamespaces.Sort();
                    OnPropertyChanged("SelectedNamespaces");
                }
            }
        }

        public string SelectedNamespace
        {
            get
            {
                return Check.Namespace;
            }

            set
            {
                if (value != null && value != Check.Namespace)
                {
                    Check.Namespace = value;

                    OnPropertyChanged("SelectedNamespace");
                    OnPropertyChanged("SelectedCheck");
                    OnPropertyChanged("SelectedChecks");
                }

                if (value == newCheck.Namespace)
                {
                    Check.Namespace = newCheck.Namespace;
                }
            }
        }

        /// <summary>
        /// Gets or sets the check that is being edited.
        /// </summary>
        public Check Check
        {
            get
            {
                return check;
            }

            set
            {
                check = value;
                OnPropertyChanged("Check");
            }
        }

        /// <summary>
        /// Gets ors sets the currently selected checks that needs to appear in the combobox.
        /// </summary>
        public List<Serialization.Check> SelectedChecks
        {
            get
            {
                if (Check.CategoryId != 0)
                {
                    if (CategoriesCollection.FirstOrDefault(c => c.Id.ToString() == Check.CategoryId.ToString()) == null)
                    {
                        AddNewCategory(SelectedCategory);
                        return new List<Serialization.Check>();
                    }

                    return CategoriesCollection.FirstOrDefault(c => c.Id.ToString() == Check.CategoryId.ToString()).Checks.Check.Where(e => e.Name.Namespace == SelectedNamespace).ToList();
                }
                else
                {
                    return new List<Serialization.Check>();
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently selected category from the combobox.
        /// </summary>
        public Category SelectedCategory
        {
            get
            {
                return Check.Category;
            }

            set
            {
                if (value.ToString() != Check.Category.ToString())
                {
                    Check.Category = value;

                    if ((int)value == -1)
                    {
                        Check.CategoryId = 0;
                    }
                    else
                    {
                        Check.CategoryId = (uint)(int)value;
                    }

                    GetExistingNamespace();

                    OnPropertyChanged("SelectedCategory");
                    OnPropertyChanged("SelectedNamespace");
                }

                if (value.ToString() == newCheck.Category.ToString())
                {
                    Check.CheckName = newCheck.CheckName;
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently selected source from the combobox.
        /// </summary>
        public Source SelectedSource
        {
            get
            {
                return Check.Source;
            }

            set
            {
                if (value.ToString() != Check.Source.ToString())
                {
                    Check.Source = value;

                    OnPropertyChanged("SelectedSource");
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently selected check from the combobox.
        /// </summary>
        public Serialization.Check SelectedCheck
        {
            get
            {
                return new Serialization.Check()
                {
                    Name = new Serialization.Name()
                    {
                        Text = Check.CheckName
                    },
                    Id = Check.ErrorId.ToString()
                };
            }

            set
            {
                if (value != null && value.Name.Text != Check.CheckName)
                {
                    Check.CheckName = value.Name.Text;
                    Check.CheckId = UInt32.Parse(value.Id);

                    // Change the Id to the next available ID for that category
                    if (value.Name.Text == newCheck.CheckName.ToString() && SelectedCategory.ToString() == newCheck.Category.ToString())
                    {
                        Check.ErrorId = newCheck.ErrorId;
                    }
                    else
                    {
                        if (NextIds.ContainsKey(Check.CategoryId))
                        {
                            Check.ErrorId = NextIds[key: Check.CategoryId][key: Check.CheckId];
                        }
                        else
                        {
                            AddNewCategory(SelectedCategory);
                        }
                    }

                    OnPropertyChanged("SelectedCheck");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a check is using a description template or not.
        /// </summary>
        public bool FromTemplate
        {
            get
            {
                return Check.FromTemplate;
            }

            set
            {
                if (value != Check.FromTemplate)
                {
                    if (value)
                    {
                        if (!originalFromTemplate)
                        {
                            SelectedTemplate = Templates[0];
                        }
                        else
                        {
                            SelectedTemplate = Templates[(int)Check.TemplateId];
                        }
                    }

                    Check.FromTemplate = value;
                    OnPropertyChanged("FromTemplate");
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently selected description template from the combobox.
        /// </summary>
        public Serialization.DescriptionTemplate SelectedTemplate
        {
            get =>
                new Serialization.DescriptionTemplate
                {
                    Id = Check.TemplateId,
                    Format = Check.Description,
                    TemplateInputParameters = new Serialization.InputParameters()
                };

            set
            {
                if (value == null)
                {
                    return;
                }

                if (Check.Parameters == null)
                {
                    Check.Parameters = new ObservableCollection<Serialization.InputParameter>();
                    foreach (var inputParameter in value.TemplateInputParameters.InputParameter)
                    {
                        check.Parameters.Add(new Serialization.InputParameter
                        {
                            Id = inputParameter.Id,
                            Text = inputParameter.Text,
                        });
                    }
                }

                Check.Description = value.Format;
                Check.TemplateId = value.Id;
                OnPropertyChanged("SelectedTemplate");
            }
        }

        /// <summary>
        /// Gets a dictionary with the next error Id within a certain check and category.
        /// </summary>
        public Dictionary<uint, Dictionary<uint, uint>> NextIds { get; private set; }

        /// <summary>
        /// Gets the collection of the categories that are currently used.
        /// </summary>
        public List<Serialization.Category> CategoriesCollection
        {
            get
            {
                return CheckViewModel.Categories;
            }
        }

        /// <summary>
        /// Gets or sets the list of severities that needs to appear in the combobox.
        /// </summary>
        public List<Severity> Severities
        {
            get
            {
                return severities;
            }

            set
            {
                if (value != severities)
                {
                    severities = value;
                    OnPropertyChanged("Severities");
                }
            }
        }

        /// <summary>
        /// Gets or sets the list of categories that needs to appear in the combobox.
        /// </summary>
        public List<Category> Categories
        {
            get
            {
                return categories;
            }

            set
            {
                if (value != categories)
                {
                    categories = value;
                    OnPropertyChanged("Categories");
                }
            }
        }

        /// <summary>
        /// Gets or sets the list of sources that needs to appear in the combobox.
        /// </summary>
        public List<Source> Sources
        {
            get
            {
                return sources;
            }

            set
            {
                if (value != sources)
                {
                    sources = value;
                    OnPropertyChanged("Sources");
                }
            }
        }

        /// <summary>
        /// Gets or sets the list of certainties that needs to appear in the combobox.
        /// </summary>
        public List<Certainty> Certainties
        {
            get
            {
                return certainties;
            }

            set
            {
                if (value != certainties)
                {
                    certainties = value;
                    OnPropertyChanged("Certainties");
                }
            }
        }

        /// <summary>
        /// Gets or sets the list of breakingchanges that needs to appear in the combobox.
        /// </summary>
        public List<FixImpact> FixImpacts
        {
            get
            {
                return fixImpacts;
            }

            set
            {
                if (value != fixImpacts)
                {
                    fixImpacts = value;
                    OnPropertyChanged("FixImpacts");
                }
            }
        }

        /// <summary>
        /// Gets the list of description templates that needs to appear in the combobox.
        /// </summary>
        public List<Serialization.DescriptionTemplate> Templates
        {
            get
            {
                return CheckViewModel.Templates;
            }
        }

        /// <summary>
        /// Gets or sets the command to cancel the add check window.
        /// </summary>
        public MyCommand CancelCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to save the changes that are made to the list.
        /// </summary>
        public MyCommand SaveCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to refresh the errors on the description parameters.
        /// </summary>
        public MyCommand RefreshParametersCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to create a new namespace within a certain category.
        /// </summary>
        public MyCommand AddNewNamespaceCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to create a new Check name within a certain namespace.
        /// </summary>
        public MyTCommand<string> AddNewCheckCommand { get; set; }

        /// <summary>
        /// Gets or sets the close action.
        /// </summary>
        public Action CloseAction { get; set; }

        private void GetExistingNamespace()
        {
            if (Check.CategoryId != 0)
            {
                if (CategoriesCollection.FirstOrDefault(c => c.Id.ToString() == Check.CategoryId.ToString()) == null)
                {
                    AddNewCategory(SelectedCategory);
                    SelectedNamespaces = new List<string>();
                }
                else
                {
                    List<string> namespaceList = new List<string>();
                    foreach (var checkIterator in CategoriesCollection.FirstOrDefault(c => c.Id.ToString() == Check.CategoryId.ToString()).Checks.Check)
                    {
                        if (!namespaceList.Contains(checkIterator.Name.Namespace))
                        {
                            namespaceList.Add(checkIterator.Name.Namespace);
                        }
                    }

                    SelectedNamespaces = namespaceList;
                }
            }
            else
            {
                SelectedNamespaces = new List<string>();
            }
        }

        /// <summary>
        /// Adds a new category to the list of categories.
        /// </summary>
        /// <param name="category">The category that needs to be added.</param>
        private void AddNewCategory(Category category)
        {
            CheckViewModel.Categories.Add(new Serialization.Category()
            {
                Id = ((int)category).ToString(),
                Name = category.ToString(),
                Checks = new Serialization.Checks()
                {
                    Check = new List<Serialization.Check>()
                }
            });
        }

        /// <summary>
        /// Opens the window to create a new check within a certain namespace.
        /// </summary>
        /// <param name="namespace">The namespace where the new check has to be created.</param>
        private void OnAddNewCheck(string @namespace)
        {
            int amountOfChecksBefore = CategoriesCollection.SingleOrDefault(e => e.Name == check.Category.ToString()).Checks.Check.Count;
            var window = new NewCheckView(SelectedCategory, @namespace);
            window.ShowDialog();

            if (amountOfChecksBefore != CategoriesCollection.SingleOrDefault(e => e.Name == check.Category.ToString()).Checks.Check.Count)
            {
                CalculateNextIds();
                OnPropertyChanged("SelectedChecks");
                if (CategoriesCollection.SingleOrDefault(e => e.Name == check.Category.ToString()).Checks.Check.Count != 0)
                {
                    SelectedCheck = CategoriesCollection.SingleOrDefault(e => e.Name == check.Category.ToString()).Checks.Check.Last();
                }
            }
        }

        /// <summary>
        /// Opens the window to create a new namespace within a certain category.
        /// </summary>
        /// <param name="category">The category where the new check has to be created.</param>
        private void OnAddNewNamespace()
        {
            int amountOfNamespacesBefore = SelectedNamespaces.Count;
            var window = new NewNamespaceView(SelectedCategory, SelectedNamespaces);
            window.ShowDialog();

            if (amountOfNamespacesBefore < SelectedNamespaces.Count)
            {
                OnPropertyChanged("SelectedNamespace");
                SelectedNamespace = SelectedNamespaces[0];
                SelectedNamespaces = new List<string>(SelectedNamespaces);
            }
        }

        /// <summary>
        /// Closes the window without saving the changes.
        /// </summary>
        private void OnCancel()
        {
            CloseAction();
        }

        /// <summary>
        /// Saves the changes that are made to the list.
        /// </summary>
        private void OnSave()
        {
            Check.CheckDescriptionParameters();
            newCheck.Copy(Check);
            CheckViewModel.Checks = new ObservableCollection<Check>(
                CheckViewModel.Checks.OrderBy(c => c.CategoryId).ThenBy(x => x.Namespace).ThenBy(c => c.CheckId).ThenBy(c => c.ErrorId).ToList());
            CloseAction();
        }

        /// <summary>
        /// Refreshes the errors from the description parameters.
        /// </summary>
        private void OnRefreshParameters()
        {
            Check.RefreshParameters();
        }

        /// <summary>
        /// Calculates the next id for each check in each category.
        /// </summary>
        private void CalculateNextIds()
        {
            NextIds = new Dictionary<uint, Dictionary<uint, uint>>();
            foreach (var category in CategoriesCollection)
            {
                Dictionary<uint, uint> nextErrorIds = new Dictionary<uint, uint>();
                foreach (var idCheck in category.Checks.Check)
                {
                    uint maxErrorId = 0;

                    foreach (var errorMessage in CheckViewModel.Checks)
                    {
                        if (errorMessage.Category.ToString() == category.Name
                            && errorMessage.CheckName == idCheck.Name.Text
                            && errorMessage.Namespace == idCheck.Name.Namespace
                            && errorMessage.ErrorId > maxErrorId)
                        {
                            maxErrorId = errorMessage.ErrorId;
                        }
                    }

                    if (!nextErrorIds.ContainsKey(UInt32.Parse(idCheck.Id)))
                    {
                        nextErrorIds.Add(UInt32.Parse(idCheck.Id), maxErrorId + 1);
                    }
                }

                if (!NextIds.ContainsKey(UInt32.Parse(category.Id)))
                {
                    NextIds.Add(UInt32.Parse(category.Id), nextErrorIds);
                }
            }
        }
    }
}