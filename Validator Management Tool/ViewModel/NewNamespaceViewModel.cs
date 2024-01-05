namespace Validator_Management_Tool.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Validator_Management_Tool.Common;
    using Validator_Management_Tool.Interfaces;

    public class NewNamespaceViewModel : BindableBase
    {
        private readonly Category category;
        private readonly List<string> existingNamespaces;
        private string errorMessage = String.Empty;
        private bool hasError;
        private string @namespace;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewNamespaceViewModel"/> class.
        /// </summary>
        /// <param name="category">The category to which the new check needs to be added.</param>
        public NewNamespaceViewModel(Category category, List<string> namespaceList)
        {
            this.category = category;
            Namespace = String.Empty;

            existingNamespaces = namespaceList;

            AddNamespaceCommand = new MyCommand(OnAddNamespace);
            CancelCommand = new MyCommand(OnCancel);
        }


        /// <summary>
        /// Gets or sets the name for the new check.
        /// It contains logic to check whether the name is acceptable or not.
        /// </summary>
        public string Namespace
        {
            get
            {
                return @namespace;
            }

            set
            {
                if (value == String.Empty || value == null)
                {
                    ErrorMessage = "The namespace can't be empty!";
                    HasError = true;
                }
                else if (existingNamespaces.Contains(value))
                {
                    ErrorMessage = String.Format(
                        "The namespace {0} already exists in this category!",
                        value);
                    HasError = true;
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
                                ErrorMessage = String.Format(
                                            "The namespace-part '{0}' is a forbidden string!",
                                            namespacePart);
                                HasError = true;
                            }
                            else if (!Char.IsUpper(namespacePart[0]))
                            {
                                ErrorMessage = String.Format(
                                            "The namespace-part '{0}' has to start with a capital!",
                                            namespacePart);
                                HasError = true;
                            }
                            else if (namespacePart.IndexOf(" ") != -1)
                            {
                                ErrorMessage = String.Format(
                                            "The namespace-part '{0}' can't contain a space!",
                                            namespacePart);
                                HasError = true;
                            }
                            else if (namespacePart.Any(c => !Char.IsLetter(c) && c != '.'))
                            {
                                ErrorMessage = String.Format(
                                            "The namespace-part '{0}' can't contain a symbol!",
                                            namespacePart);
                                HasError = true;
                            }
                            else
                            {
                                HasError = false;
                                ErrorMessage = String.Empty;
                            }
                        }
                        else
                        {
                            ErrorMessage = "A part of the namespace can't be empty!";
                            HasError = true;
                        }
                    }
                }

                @namespace = value;
                OnPropertyChanged("Namespace");
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
        /// Gets or sets the command to add the new check.
        /// </summary>
        public MyCommand AddNamespaceCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to cancel and close the window.
        /// </summary>
        public MyCommand CancelCommand { get; set; }

        /// <summary>
        /// Gets or sets the close action.
        /// </summary>
        public Action CloseAction { get; set; }

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
        private void OnAddNamespace()
        {
            existingNamespaces.Add(Namespace);

            CloseAction();
        }
    }
}
