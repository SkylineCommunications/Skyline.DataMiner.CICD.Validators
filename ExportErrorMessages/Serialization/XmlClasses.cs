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
        public string Value { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "InputParameters")]
    public class InputParameters
    {
        [XmlElement(ElementName = "InputParameter")]
        public List<InputParameter> InputParameter { get; set; }
    }

    [XmlRoot(ElementName = "DescriptionTemplate")]
    public class DescriptionTemplate
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "Format")]
        public string Format { get; set; }

        [XmlElement(ElementName = "InputParameters")]
        public InputParameters TemplateInputParameters { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public uint Id { get; set; }
    }

    [XmlRoot(ElementName = "DescriptionTemplates")]
    public class DescriptionTemplates
    {
        [XmlElement(ElementName = "DescriptionTemplate")]
        public List<DescriptionTemplate> DescriptionTemplate { get; set; }
    }

    [XmlRoot(ElementName = "Name")]
    public class Name
    {
        [XmlAttribute(AttributeName = "namespace")]
        public string Namespace { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Description")]
    public class Description
    {
        [XmlAttribute(AttributeName = "templateId")]
        public int? TemplateId { get; set; }

        [XmlElement(ElementName = "Format")]
        public string? Format { get; set; }

        [XmlElement(ElementName = "InputParameters")]
        public InputParameters? InputParameters { get; set; }
    }

    [XmlRoot(ElementName = "ErrorMessage")]
    public class ErrorMessage
    {
        private string? _howToFix;
        private string? _exampleCode;
        private string? _details;
        private string? _groupDescription = String.Empty;

        [XmlElement(ElementName = "Name")]
        public Name Name { get; set; }

        [XmlElement(ElementName = "GroupDescription", IsNullable = false)]
        public string? GroupDescription
        {
            get => _groupDescription ?? String.Empty;
            set => _groupDescription = value ?? String.Empty;
        }

        [XmlElement(ElementName = "Description")]
        public Description Description { get; set; }

        [XmlElement(ElementName = "Severity")]
        public Severity? Severity { get; set; }

        [XmlElement(ElementName = "Certainty")]
        public Certainty? Certainty { get; set; }

        [XmlElement(ElementName = "Source")]
        public Source? Source { get; set; }

        [XmlElement(ElementName = "FixImpact")]
        public FixImpact? FixImpact { get; set; }

        [XmlElement(ElementName = "HasCodeFix")]
        public bool? HasCodeFix { get; set; }

        [XmlElement(ElementName = "HowToFix")]
        public XmlCDataSection? HowToFix
        {
            get
            {
                if (_howToFix == null)
                {
                    return null;
                }

                XmlDocument doc = new XmlDocument();
                return doc.CreateCDataSection(_howToFix);
            }

            set
            {
                if (value != null)
                {
                    _howToFix = value.Value;
                }
            }
        }

        [XmlElement(ElementName = "ExampleCode")]
        public XmlCDataSection? ExampleCode
        {
            get
            {
                if (_exampleCode == null)
                {
                    return null;
                }

                XmlDocument doc = new XmlDocument();
                return doc.CreateCDataSection(_exampleCode);
            }

            set
            {
                if (value != null)
                {
                    _exampleCode = value.Value;
                }
            }
        }

        [XmlElement(ElementName = "Details")]
        public XmlCDataSection? Details
        {
            get
            {
                if (_details == null)
                {
                    return null;
                }

                XmlDocument doc = new XmlDocument();
                return doc.CreateCDataSection(_details);
            }

            set
            {
                if (value != null)
                {
                    _details = value.Value;
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
        [XmlAttribute(AttributeName = "autoFixPopup")]
        public bool AutoFixPopup { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "AutoFixWarnings")]
    public class AutoFixWarnings
    {
        [XmlElement(ElementName = "AutoFixWarning")]
        public List<AutoFixWarning> AutoFixWarning { get; set; }
    }

    [XmlRoot(ElementName = "ErrorMessages")]
    public class ErrorMessages
    {
        [XmlElement(ElementName = "ErrorMessage")]
        public List<ErrorMessage> ErrorMessage { get; set; }
    }

    [XmlRoot(ElementName = "Check")]
    public class Check
    {
        [XmlElement(ElementName = "Name")]
        public Name Name { get; set; }

        [XmlElement(ElementName = "ErrorMessages")]
        public ErrorMessages ErrorMessages { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }
    }

    [XmlRoot(ElementName = "Checks")]
    public class Checks
    {
        [XmlElement(ElementName = "Check")]
        public List<Check> Check { get; set; }
    }

    [XmlRoot(ElementName = "Category")]
    public class Category
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "Checks")]
        public Checks Checks { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }
    }

    [XmlRoot(ElementName = "Categories")]
    public class Categories
    {
        [XmlElement(ElementName = "Category")]
        public List<Category> Category { get; set; }
    }

    [XmlRoot(ElementName = "ValidationChecks")]
    public class ValidationChecks
    {
        [XmlElement(ElementName = "DescriptionTemplates")]
        public DescriptionTemplates DescriptionTemplates { get; set; }

        [XmlElement(ElementName = "Categories")]
        public Categories Categories { get; set; }
    }

    [XmlRoot(ElementName = "Validator")]
    public class Validator
    {
        [XmlElement(ElementName = "ValidationChecks")]
        public ValidationChecks ValidationChecks { get; set; }
    }
}