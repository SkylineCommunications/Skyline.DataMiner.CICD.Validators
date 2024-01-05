namespace Validator_Management_Tool.Templates.Tests
{
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    public partial class TestsClass
    {
        private string className;
        private string @namespace;
        private Category category;

        public TestsClass(string className, string @namespace, Category category)
        {
            this.className = className;
            this.@namespace = @namespace;
            this.category = category;
        }
    }
}