namespace Validator_Management_Tool.Views
{
    using System;
    using System.Windows;

    using Validator_Management_Tool.Model;
    using Validator_Management_Tool.ViewModel;

    /// <summary>
    /// Interaction logic for CheckEditView.xaml
    /// </summary>
    public partial class CheckEditView : Window
    {
        public CheckEditView(Check check)
        {
            InitializeComponent();
            var vm = new CheckEditViewModel(check);
            this.DataContext = vm;
            if (vm.CloseAction == null)
                vm.CloseAction = new Action(this.Close);
        }
    }
}
