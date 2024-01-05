namespace Validator_Management_Tool.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Validator_Management_Tool.Common;
    using Validator_Management_Tool.Interfaces;

    /// <summary>
    /// The Viewmodel for the Create new check window.
    /// </summary>
    public class NewCheckViewModel : BindableBase
    {
        private readonly Category category;
        private readonly List<Serialization.Check> existingChecks;
        private string errorMessage = String.Empty;
        private bool hasError;
        private string checkName;
        private readonly string @namespace;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewCheckViewModel"/> class.
        /// </summary>
        /// <param name="category">The category to which the new check needs to be added.</param>
        public NewCheckViewModel(Category category, string @namespace)
        {
            CategoryId = (int)category;
            this.category = category;
            CheckName = String.Empty;
            this.@namespace = @namespace;

            CalculateNextCheckId();
            existingChecks = CheckViewModel.Categories.SingleOrDefault(e => e.Name == category.ToString()).Checks.Check.Where(e => e.Name.Namespace == @namespace).ToList();

            AddCheckCommand = new MyCommand(OnAddCheck);
            CancelCommand = new MyCommand(OnCancel);
        }

        /// <summary>
        /// Gets or sets the name for the new check.
        /// It contains logic to check whether the name is acceptable or not.
        /// </summary>
        public string CheckName
        {
            get
            {
                return checkName;
            }

            set
            {
                if (checkName != value)
                {
                    if (value == String.Empty)
                    {
                        ErrorMessage = "The name can't be empty!";
                        HasError = true;
                    }
                    else if (Settings.ForbiddenStrings.Contains(value))
                    {
                        ErrorMessage = "The name is a forbidden string!";
                        HasError = true;
                    }
                    else if (!Char.IsUpper(value[0]))
                    {
                        ErrorMessage = "The name has to start with a capital!";
                        HasError = true;
                    }
                    else if (value.IndexOf(" ") != -1)
                    {
                        ErrorMessage = "The name can't contain a space!";
                        HasError = true;
                    }
                    else if (value.Any(c => !Char.IsLetter(c)))
                    {
                        ErrorMessage = "The name can't contain symbols!";
                        HasError = true;
                    }
                    else if (existingChecks.Any(e => e.Name.Text == value))
                    {
                        ErrorMessage = "The name already exists in the category!";
                        HasError = true;
                    }
                    else
                    {
                        HasError = false;
                        ErrorMessage = String.Empty;
                    }

                    checkName = value;
                    OnPropertyChanged("CheckName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the error message that is displayed when there is an error.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }

            set
            {
                if (value != errorMessage)
                {
                    errorMessage = value;
                    OnPropertyChanged("ErrorMessage");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether there is an error in the check name or not.
        /// </summary>
        public bool HasError
        {
            get
            {
                return hasError;
            }

            set
            {
                if (value != hasError)
                {
                    hasError = value;
                    OnPropertyChanged("HasError");
                }
            }
        }

        /// <summary>
        /// Gets the category id from the category to which to check is added.
        /// </summary>
        public int CategoryId { get; }

        /// <summary>
        /// Gets the next check id that can be used.
        /// </summary>
        public int NextCheckId { get; private set; }

        /// <summary>
        /// Gets or sets the command to add the new check.
        /// </summary>
        public MyCommand AddCheckCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to cancel and close the window.
        /// </summary>
        public MyCommand CancelCommand { get; set; }

        /// <summary>
        /// Gets or sets the close action.
        /// </summary>
        public Action CloseAction { get; set; }

        /// <summary>
        /// Calculates the next check id within an category.
        /// </summary>
        private void CalculateNextCheckId()
        {
            List<Serialization.Check> checks = CheckViewModel.Categories.SingleOrDefault(e => e.Name == category.ToString()).Checks.Check;
            int maxId = 0;
            foreach (var check in checks)
            {
                if (Int32.Parse(check.Id) > maxId)
                {
                    maxId = Int32.Parse(check.Id);
                }
            }

            NextCheckId = maxId + 1;
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        private void OnCancel()
        {
            CloseAction();
        }

        /// <summary>
        /// Adds the new check to the category and closes the window.
        /// </summary>
        private void OnAddCheck()
        {
            CheckViewModel.Categories.SingleOrDefault(e => e.Name == category.ToString()).Checks.Check.Add(new Serialization.Check()
            {
                Name = new Serialization.Name()
                {
                    Text = CheckName,
                    Namespace = @namespace
                },
                Id = NextCheckId.ToString()
            });

            CloseAction();
        }
    }
}