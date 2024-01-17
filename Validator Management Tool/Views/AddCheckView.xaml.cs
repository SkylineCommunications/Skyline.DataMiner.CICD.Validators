namespace Validator_Management_Tool.Views
{
    using System;
    using System.Windows;

    using Validator_Management_Tool.ViewModel;

    /// <summary>
    /// Interaction logic for AddCheckView.xaml
    /// </summary>
    public partial class AddCheckView : Window
    {
        public AddCheckView()
        {
            InitializeComponent();
            var vm = new AddCheckViewModel();
            this.DataContext = vm;
            if (vm.CloseAction == null)
                vm.CloseAction = new Action(this.Close);


        }
    }
}
