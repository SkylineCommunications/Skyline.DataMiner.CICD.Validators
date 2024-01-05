namespace Validator_Management_Tool.ViewModel
{
    using Validator_Management_Tool.Interfaces;

    /// <summary>
    /// The main window view model.
    /// This window contains the menu buttons to switch between the check overview and the settings view.
    /// </summary>
    public class MainWindowModel : BindableBase
    {
        public MainWindowModel()
        {
            NavCommand = new MyTCommand<string>(OnNav);
            CurrentViewModel = checkViewModel;
        }

        private readonly CheckViewModel checkViewModel = new CheckViewModel();

        private readonly SettingsViewModel settingsViewModel = new SettingsViewModel();

        private BindableBase _CurrentViewModel;

        public BindableBase CurrentViewModel
        {
            get { return _CurrentViewModel; }
            set { SetProperty(ref _CurrentViewModel, value); }
        }

        public MyTCommand<string> NavCommand { get; private set; }

        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "settings":
                    CurrentViewModel = settingsViewModel;
                    break;

                case "checks":
                default:
                    CurrentViewModel = checkViewModel;
                    break;
            }
        }
    }
}