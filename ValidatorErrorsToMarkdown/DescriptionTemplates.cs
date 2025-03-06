namespace Skyline.DataMiner.CICD.Tools.ValidatorErrorsToMarkdown
{
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Class to work with the description templates
    /// </summary>
    public class DescriptionTemplates
    {
        private readonly XElement descriptionTemplates;

        /// <summary>
        /// Creates an instance off class <see cref="DescriptionTemplates"/>.
        /// </summary>
        /// <param name="descriptionTemplates"></param>
        public DescriptionTemplates(XElement descriptionTemplates)
        {
            this.descriptionTemplates = descriptionTemplates;
        }

        /// <summary>
        /// Gets the format from the template with the template id.
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns>A <see cref="string"/> format from the template that matches the specified <see cref="XAttribute"/> template id.</returns>
        public string GetFormat(XAttribute templateId)
        {
            var template = GetTemplateById(templateId);
            return template?.Element("Format")?.Value;
        }

        /// <summary>
        /// Gets the name of the template with the template id.
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns>An <see cref="XElement"/> TemplateInputs from the template that matches the specified <see cref="XAttribute"/> template id.</returns>
        public XElement GetTemplateInputs(XAttribute templateId)
        {
            var template = GetTemplateById(templateId);
            return template?.Element("InputParameters");
        }

        /// <summary>
        /// Gets the template by id.
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns>An <see cref="XElement"/> Template that matches the specified <see cref="XAttribute"/> template id.</returns>
        private XElement GetTemplateById(XAttribute templateId)
        {
            return descriptionTemplates?.Elements("DescriptionTemplate").FirstOrDefault(template => template?.Attribute("id")?.Value == templateId?.Value);
        }
    }
}
