namespace ExportErrorMessages.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;

    using Skyline.DataMiner.CICD.Validators.Common.Model;

    [XmlRoot(ElementName = "InputParameter")]
    public class InputParameter
    {
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; } = null!;

        [XmlText]
        public string Text { get; set; } = null!;
    }

    [XmlRoot(ElementName = "InputParameters")]
    public class InputParameters
    {
        [XmlElement(ElementName = "InputParameter")]
        public List<InputParameter> InputParameter { get; set; } = null!;
    }

    [XmlRoot(ElementName = "DescriptionTemplate")]
    public class DescriptionTemplate
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; } = null!;

        [XmlElement(ElementName = "Format")]
        public string Format { get; set; } = null!;

        [XmlElement(ElementName = "InputParameters")]
        public InputParameters TemplateInputParameters { get; set; } = null!;

        [XmlAttribute(AttributeName = "id")]
        public uint Id { get; set; }
    }

    [XmlRoot(ElementName = "DescriptionTemplates")]
    public class DescriptionTemplates
    {
        [XmlElement(ElementName = "DescriptionTemplate")]
        public List<DescriptionTemplate> DescriptionTemplate { get; set; } = null!;
    }

    [XmlRoot(ElementName = "Name")]
    public class Name
    {
        [XmlAttribute(AttributeName = "namespace")]
        public string Namespace { get; set; } = null!;

        [XmlText]
        public string Text { get; set; } = null!;
    }

    [XmlRoot(ElementName = "Description")]
    public class Description
    {
        [XmlIgnore]
        public int? TemplateId { get; set; }

        [XmlAttribute(AttributeName = "templateId")]
        public string? TemplateIdAsString
        {
            get => TemplateId?.ToString();
            set
            {
                if (Int32.TryParse(value, out int temp))
                {
                    TemplateId = temp;
                }
                else
                {
                    TemplateId = null;
                }
            }
        }

        [XmlElement(ElementName = "Format")]
        public string? Format { get; set; }

        [XmlElement(ElementName = "InputParameters")]
        public InputParameters? InputParameters { get; set; }
    }

    [XmlRoot(ElementName = "ErrorMessage")]
    public class ErrorMessage
    {
        private string? howToFix;
        private string? exampleCode;
        private string? details;
        private string? groupDescription = String.Empty;

        [XmlElement(ElementName = "Name")]
        public Name Name { get; set; } = null!;

        [XmlElement(ElementName = "GroupDescription", IsNullable = false)]
        public string? GroupDescription
        {
            get => groupDescription ?? String.Empty;
            set => groupDescription = value ?? String.Empty;
        }

        [XmlElement(ElementName = "Description")]
        public Description Description { get; set; } = null!;

        [XmlElement(ElementName = "Severity")]
        public Severity? Severity { get; set; }

        [XmlElement(ElementName = "Certainty")]
        public Certainty? Certainty { get; set; }

        [XmlElement(ElementName = "Source")]
        public Source? Source { get; set; }

        [XmlElement(ElementName = "FixImpact")]
        public FixImpact? FixImpact { get; set; }

        [XmlIgnore]
        public bool? HasCodeFix { get; set; }

        [XmlElement(ElementName = "HasCodeFix")]
        public string? HasCodeFixAsString
        {
            get => HasCodeFix?.ToString();
            set
            {
                if (value == null)
                {
                    HasCodeFix = null;
                    return;
                }

                HasCodeFix = value.ToUpperInvariant() == "TRUE";
            }
        }

        [XmlElement(ElementName = "HowToFix")]
        public XmlCDataSection? HowToFix
        {
            get
            {
                if (howToFix == null)
                {
                    return null;
                }

                XmlDocument doc = new XmlDocument();
                return doc.CreateCDataSection(howToFix);
            }

            set
            {
                if (value != null)
                {
                    howToFix = value.Value;
                }
            }
        }

        [XmlElement(ElementName = "ExampleCode")]
        public XmlCDataSection? ExampleCode
        {
            get
            {
                if (exampleCode == null)
                {
                    return null;
                }

                XmlDocument doc = new XmlDocument();
                return doc.CreateCDataSection(exampleCode);
            }

            set
            {
                if (value != null)
                {
                    exampleCode = value.Value;
                }
            }
        }

        [XmlElement(ElementName = "Details")]
        public XmlCDataSection? Details
        {
            get
            {
                if (details == null)
                {
                    return null;
                }

                XmlDocument doc = new XmlDocument();
                return doc.CreateCDataSection(details);
            }

            set
            {
                if (value != null)
                {
                    details = value.Value;
                }
            }
        }

        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        [XmlElement(ElementName = "AutoFixWarnings")]
        public AutoFixWarnings? AutoFixWarnings { get; set; }
    }

    [XmlRoot(ElementName = "AutoFixWarning")]
    public class AutoFixWarning
    {
        [XmlIgnore]
        public bool? AutoFixPopup { get; set; }

        [XmlAttribute(AttributeName = "autoFixPopup")]
        public string? AutoFixPopupAsString
        {
            get => AutoFixPopup?.ToString();
            set
            {
                if (value == null)
                {
                    AutoFixPopup = null;
                    return;
                }

                AutoFixPopup = value.ToUpperInvariant() == "TRUE";
            }
        }

        [XmlText]
        public string? Text { get; set; }
    }

    [XmlRoot(ElementName = "AutoFixWarnings")]
    public class AutoFixWarnings
    {
        [XmlElement(ElementName = "AutoFixWarning")]
        public List<AutoFixWarning> AutoFixWarning { get; set; } = null!;
    }

    [XmlRoot(ElementName = "ErrorMessages")]
    public class ErrorMessages
    {
        [XmlElement(ElementName = "ErrorMessage")]
        public List<ErrorMessage> ErrorMessage { get; set; } = null!;
    }

    [XmlRoot(ElementName = "Check")]
    public class Check
    {
        [XmlElement(ElementName = "Name")]
        public Name Name { get; set; } = null!;

        [XmlElement(ElementName = "ErrorMessages")]
        public ErrorMessages ErrorMessages { get; set; } = null!;

        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }
    }

    [XmlRoot(ElementName = "Checks")]
    public class Checks
    {
        [XmlElement(ElementName = "Check")]
        public List<Check> Check { get; set; } = null!;
    }

    [XmlRoot(ElementName = "Category")]
    public class Category
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; } = null!;

        [XmlElement(ElementName = "Checks")]
        public Checks Checks { get; set; } = null!;

        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }
    }

    [XmlRoot(ElementName = "Categories")]
    public class Categories
    {
        [XmlElement(ElementName = "Category")]
        public List<Category> Category { get; set; } = null!;
    }

    [XmlRoot(ElementName = "ValidationChecks")]
    public class ValidationChecks
    {
        [XmlElement(ElementName = "DescriptionTemplates")]
        public DescriptionTemplates DescriptionTemplates { get; set; } = null!;

        [XmlElement(ElementName = "Categories")]
        public Categories Categories { get; set; } = null!;
    }

    [XmlRoot(ElementName = "Validator")]
    public class Validator
    {
        [XmlElement(ElementName = "ValidationChecks")]
        public ValidationChecks ValidationChecks { get; set; } = null!;
    }
}