namespace Validator_Management_Tool.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Interaction logic for CheckView.xaml
    /// </summary>
    public partial class CheckView : UserControl
    {
        public CheckView()
        {
            InitializeComponent();

            //Set the source for the icons
            var error = System.Drawing.SystemIcons.Warning;
            var image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(error.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            ErrorIcon.Source = image;
        }
    }
}
