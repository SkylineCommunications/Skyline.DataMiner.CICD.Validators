namespace Validator_Management_Tool.Templates.Error_Messages
{
    using System.Collections.Generic;
    using System.Linq;

    using Validator_Management_Tool.Model;

    public partial class ErrorMessagesClass
    {
        private List<Check> allChecks;

        private List<Check> checks;
        private List<Check> compares;

        /// <summary>
        /// Contains a key for each check name (string) in the checklist.
        /// The value is another dictionary whose keys are the argument names and the values are the argument types (string).
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> argumentLists;

        /// <summary>
        /// Contains a key for each check name (string) in the checklist.
        /// The value is another dictionary whose keys are the property names and the values are the property values (string).
        /// </summary>
        private Dictionary<string, Dictionary<string, object>> propertyLists;

        /// <summary>
        /// Contains a key for each check name (string) in the checklist.
        /// The value is another dictionary whose keys are the property names and the values are the property values (string).
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> warningLists;

        private string @namespace;

        public ErrorMessagesClass(List<Check> checks)
        {
            this.allChecks = checks;

            this.checks = checks.Where(x => x.Source != Skyline.DataMiner.CICD.Validators.Common.Model.Source.MajorChangeChecker).ToList();
            this.compares = checks.Where(x => x.Source == Skyline.DataMiner.CICD.Validators.Common.Model.Source.MajorChangeChecker).ToList();

            var (Arguments, Properties) = TemplateHelper.MakeLists(allChecks);
            argumentLists = Arguments;
            propertyLists = Properties;
            warningLists = TemplateHelper.GetWarnings(allChecks);
            @namespace = checks[0].FullNamespace;
        }
    }
}