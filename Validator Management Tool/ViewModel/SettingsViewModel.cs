namespace Validator_Management_Tool.ViewModel
{
    using System.Windows.Forms;

    using Validator_Management_Tool.Common;
    using Validator_Management_Tool.Interfaces;

    /// <summary>
    /// The Viewmodel for the settings view.
    /// </summary>
    public class SettingsViewModel : BindableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel()
        {
            BrowseFolderCommand = new MyTCommand<string>(OnBrowseFolder);
            BrowseFileCommand = new MyCommand(OnBrowseFile);
            DefaultPathCommand = new MyTCommand<string>(OnDefaultPath);
            DefaultAllCommand = new MyCommand(OnDefaultAll);
        }

        /// <summary>
        /// Gets or sets the command to browse for a folder.
        /// </summary>
        public MyTCommand<string> BrowseFolderCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to browse for a file.
        /// </summary>
        public MyCommand BrowseFileCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to restore a path to default.
        /// </summary>
        public MyTCommand<string> DefaultPathCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to restore all settings to default.
        /// </summary>
        public MyCommand DefaultAllCommand { get; set; }

        /// <summary>
        /// Gets or sets the path where the error message classes will be generated.
        /// </summary>
        public string ErrorPath
        {
            get
            {
                return Settings.ErrorMessagesPath;
            }

            set
            {
                if (value != Settings.ErrorMessagesPath)
                {
                    Settings.ErrorMessagesPath = value;
                    OnPropertyChanged("ErrorPath");
                }
            }
        }

        /// <summary>
        /// Gets or sets the path where the test classes will be generated.
        /// </summary>
        public string TestPath
        {
            get
            {
                return Settings.TestPath;
            }

            set
            {
                if (value != Settings.TestPath)
                {
                    Settings.TestPath = value;
                    OnPropertyChanged("TestPath");
                }
            }
        }

        /// <summary>
        /// Gets or sets the path where the unit test classes will be generated.
        /// </summary>
        public string UnitTestPath
        {
            get
            {
                return Settings.UnitTestPath;
            }

            set
            {
                if (value != Settings.UnitTestPath)
                {
                    Settings.UnitTestPath = value;
                    OnPropertyChanged("UnitTestPath");
                }
            }
        }

        /// <summary>
        /// Gets or sets the path where the XML file is located.
        /// </summary>
        public string XmlPath
        {
            get
            {
                return Settings.XmlPath;
            }

            set
            {
                if (value != Settings.XmlPath)
                {
                    Settings.XmlPath = value;
                    OnPropertyChanged("XmlPath");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the error message classes are generated in one single file or not.
        /// </summary>
        public bool AllClassesInOneFile
        {
            get
            {
                return Settings.ErrorClassesInOneFile;
            }

            set
            {
                if (value != Settings.ErrorClassesInOneFile)
                {
                    Settings.ErrorClassesInOneFile = value;
                    OnPropertyChanged("AllClassesInOneFile");
                }
            }
        }

        /// <summary>
        /// Opens a folder browse dialog to select the path for a certain browser.
        /// </summary>
        /// <param name="index">The path that is being changed.</param>
        private void OnBrowseFolder(string index)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string path = dialog.SelectedPath.Replace("\\", "/") + "/";
                    switch (index)
                    {
                        case "TEST":
                            TestPath = path;
                            break;
                        case "ERROR":
                            ErrorPath = path;
                            break;
                        case "UNIT":
                            UnitTestPath = path;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Restores a certain path to the default path.
        /// </summary>
        /// <param name="index">The path that is being restored.</param>
        private void OnDefaultPath(string index)
        {
            switch (index)
            {
                case "TEST":
                    TestPath = Settings.DefaultTestPath;
                    break;
                case "ERROR":
                    ErrorPath = Settings.DefaultErrorMessagesPath;
                    break;
                case "UNIT":
                    UnitTestPath = Settings.DefaultUnitTestPath;
                    break;
                case "XML":
                    XmlPath = Settings.DefaultXmlPath;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Restores all settings to default.
        /// </summary>
        private void OnDefaultAll()
        {
            TestPath = Settings.DefaultTestPath;
            ErrorPath = Settings.DefaultErrorMessagesPath;
            UnitTestPath = Settings.DefaultUnitTestPath;
            XmlPath = Settings.DefaultXmlPath;
            AllClassesInOneFile = Settings.DefaultErrorClassesInOneFile;
        }

        /// <summary>
        /// Opens a file browser dialog to select the location of the XML file.
        /// </summary>
        private void OnBrowseFile()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".xml",
                Filter = "XML Files (*.xml)|*.xml"
            };

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName.Replace("\\", "/");
                XmlPath = path;
            }
        }
    }
}