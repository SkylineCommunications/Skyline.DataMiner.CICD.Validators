namespace Validator_Management_Tool
{
    using System;

    using Validator_Management_Tool.Common;

    public class EntryPoint
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                ExportManager.ExportToExcelPipeline(args[0]);
            }
            else
            {
                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }
    }
}