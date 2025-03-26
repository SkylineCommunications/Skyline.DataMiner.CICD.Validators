namespace Skyline.DataMiner.CICD.Tools.ValidatorErrorsToMarkdown
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// A helper to get all the info in the element check.
    /// </summary>
    public class XDocCheckHelper
    {
        private readonly XElement check;
        private readonly DescriptionTemplates descriptionTemplates;

        /// <summary>
        /// Creates an instance of class <see cref="XDocCheckHelper"/>.
        /// </summary>
        /// <param name="check"></param>
        /// <param name="descriptionTemplates"></param>
        public XDocCheckHelper(XElement check, DescriptionTemplates descriptionTemplates)
        {
            this.check = check;
            this.descriptionTemplates = descriptionTemplates;
        }

        /// <summary>
        /// Gets the id of the <see cref="check"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> checkId.</returns>
        public string GetCheckId() => check?.Attribute("id")?.Value;

        /// <summary>
        /// Gets the Name of the <see cref="check"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> checkName.</returns>
        public string GetCheckName() => check?.Element("Name")?.Value;

        /// <summary>
        /// Gets the namespace of the <see cref="check"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> checkNamespace.</returns>
        public string GetCheckNamespace() => check?.Element("Name")?.Attribute("namespace")?.Value;

        /// <summary>
        /// Gets the id of the <see cref="XElement"/> errorMessage.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>A <see cref="string"/> errorMessageId.</returns>
        public static string GetCheckErrorMessageId(XElement errorMessage) => errorMessage?.Attribute("id")?.Value;

        /// <summary>
        /// Gets the name of the <see cref="XElement"/> errorMessage.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>A <see cref="string"/> errorMessageName.</returns>
        public static string GetCheckErrorMessageName(XElement errorMessage) => errorMessage?.Element("Name")?.Value;

        /// <summary>
        /// Gets the group description of the <see cref="XElement"/> errorMessage.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>A <see cref="string"/> groupDescription.</returns>
        public static string GetCheckGroupDescription(XElement errorMessage) => errorMessage?.Element("GroupDescription")?.Value;

        /// <summary>
        /// Gets the description of the <see cref="XElement"/> errorMessage.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>A <see cref="string"/> description.</returns>
        public string GetCheckDescription(XElement errorMessage)
        {
            XElement templateInputs = null;
            var templateId = errorMessage?.Element("Description")?.Attribute("templateId");
            var inputParameters = errorMessage?.Element("Description")?.Element("InputParameters");
            string format;
            if (templateId != null)
            {
                format = descriptionTemplates.GetFormat(templateId);
                templateInputs = descriptionTemplates.GetTemplateInputs(templateId);
            }
            else
            {
                format = errorMessage?.Element("Description")?.Element("Format")?.Value;
            }

            if (format is null)
            {
                throw new InvalidOperationException($"No format found for {errorMessage}");
            }

            var input = GetInputParams(inputParameters, templateInputs);
            return String.Format(format, input);
        }

        /// <summary>
        /// Gets the severity of the <see cref="XElement"/> errorMessage.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>A <see cref="string"/> severity.</returns>
        public static string GetCheckSeverity(XElement errorMessage) => errorMessage?.Element("Severity")?.Value;

        /// <summary>
        /// Gets the certainty of the <see cref="XElement"/> errorMessage.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>A <see cref="string"/> certainty.</returns>
        public static string GetCheckCertainty(XElement errorMessage) => errorMessage?.Element("Certainty")?.Value;

        /// <summary>
        /// Gets the source of the <see cref="XElement"/> errorMessage.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>A <see cref="string"/> source.</returns>
        public static string GetCheckSource(XElement errorMessage) => errorMessage?.Element("Source")?.Value;

        /// <summary>
        /// Gets the fix impact of the <see cref="XElement"/> errorMessage.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>A <see cref="string"/> fixImpact.</returns>
        public static string GetCheckFixImpact(XElement errorMessage) => errorMessage?.Element("FixImpact")?.Value;

        /// <summary>
        /// Has the <see cref="XElement"/> errorMessage a code fix.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>A <see cref="bool"/> hasCodeFix.</returns>
        public static bool HasCheckErrorMessageCodeFix(XElement errorMessage) => errorMessage?.Element("HasCodeFix")?.Value is "True";

        /// <summary>
        /// Gets the how to fix of the <see cref="XElement"/> errorMessage.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>A <see cref="string"/> howToFix.</returns>
        public static string GetCheckHowToFix(XElement errorMessage) => SetNewLines(errorMessage?.Element("HowToFix")?.Value);

        /// <summary>
        /// Gets the example code of the <see cref="XElement"/> errorMessage.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>A <see cref="string"/> exampleCode.</returns>
        public static string GetCheckExampleCode(XElement errorMessage) => SetNewLines(errorMessage?.Element("ExampleCode")?.Value);

        /// <summary>
        /// Gets the details of the <see cref="XElement"/> errorMessage.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>A <see cref="string"/> details.</returns>
        public static string GetCheckDetails(XElement errorMessage) => SetNewLines(errorMessage?.Element("Details")?.Value);

        /// <summary>
        /// Gets the auto fix warnings of the <see cref="XElement"/> errorMessage.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>A <see cref="List{T}"/> with autoFixWarnings.</returns>
        public static List<string> GetCheckAutoFixWarnings(XElement errorMessage) => errorMessage?.Element("AutoFixWarnings")?.Elements("AutoFixWarning")?.Select(AutoFixWarning => AutoFixWarning.Value).ToList();

        private static string[] GetInputParams(XElement inputParameters, XElement templateInput)
        {
            IEnumerable<XElement> inputParams = inputParameters?.Elements("InputParameter").ToList();
            string[] inputParamsArray;

            if (templateInput is not null)
            {
                IEnumerable<XElement> templateInputs = templateInput.Elements("InputParameter").ToList();
                inputParamsArray = new string[templateInputs.Count()];
                inputParamsArray = CheckValueOverrides(templateInputs, inputParamsArray);
            }
            else
            {
                inputParamsArray = new string[inputParams?.Count() ?? 0];
            }

            if (inputParams is not null)
            {
                inputParamsArray = CheckValueOverrides(inputParams, inputParamsArray);
            }

            return inputParamsArray;
        }

        /// <summary>
        /// Checks every <see cref="XElement"/> parameter in <see cref="T:string[]"/> inputParamsArray if the <see cref="XAttribute"/> value overrides the parameter value.
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="inputParamsArray"></param>
        /// <returns>A <see cref="T:string[]"/> with the correct values</returns>
        private static string[] CheckValueOverrides(IEnumerable<XElement> inputs, string[] inputParamsArray)
        {
            int index = 0;
            foreach (var parameter in inputs)
            {
                string valueAttribute = parameter.Attribute("value")?.Value;
                if (String.IsNullOrEmpty(valueAttribute))
                {
                    inputParamsArray[index] = "{" + parameter.Value + "}";
                }
                else
                {
                    inputParamsArray[index] = valueAttribute;
                }

                index++;
            }

            return inputParamsArray;
        }

        private static string SetNewLines(string data)
        {
            return data.Replace("[NewLine]", Environment.NewLine);
        }
    }
}
