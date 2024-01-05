namespace Validator_Management_Tool.Views
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Validator_Management_Tool.ViewModel;

    /// <summary>
    /// Interaction logic for NewCheckView.xaml
    /// </summary>
    public partial class NewCheckView : Window
    {
        public NewCheckView(Category category, string @namespace)
        {
            InitializeComponent();
            var vm = new NewCheckViewModel(category, @namespace);
            this.DataContext = vm;
            if (vm.CloseAction == null)
            {
                vm.CloseAction = new Action(this.Close);
            }

            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }
}