namespace Validator_Management_Tool.Views
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;

    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Validator_Management_Tool.ViewModel;

    /// <summary>
    /// Interaction logic for NewNamespaceView.xaml
    /// </summary>
    public partial class NewNamespaceView : Window
    {
        public NewNamespaceView(Category category, List<string> namespaces)
        {
            InitializeComponent();
            var vm = new NewNamespaceViewModel(category, namespaces);
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