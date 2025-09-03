namespace Validator_Management_Tool.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "InputParameter")]
    public class InputParameter : BindableBase
    {
        private string id;
        private string valueAttr;
        private string text;

        [XmlAttribute(AttributeName = "id")]
        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                if (value != id)
                {
                    id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        [XmlAttribute(AttributeName = "value")]
        public string Value
        {
            get
            {
                return valueAttr;
            }

            set
            {
                if (value != valueAttr)
                {
                    valueAttr = value;
                    OnPropertyChanged("Value");
                }
            }
        }

        [XmlText]
        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                if (value != text)
                {
                    text = value;
                    OnPropertyChanged("Text");
                }
            }
        }
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
        public string TemplateId { get; set; }

        [XmlElement(ElementName = "Format")]
        public string Format { get; set; }

        [XmlElement(ElementName = "InputParameters")]
        public InputParameters InputParameters { get; set; }
    }

    [XmlRoot(ElementName = "ErrorMessage")]
    public class ErrorMessage
    {
        private string _howToFix = null;
        private string _groupDescription = String.Empty;

        [XmlElement(ElementName = "Name")]
        public Name Name { get; set; }

        [XmlElement(ElementName = "GroupDescription", IsNullable = false)]
        public string GroupDescription
        {
            get
            {
                return _groupDescription ?? String.Empty;
            }
            set
            {
                _groupDescription = value ?? String.Empty;
            }
        }

        [XmlElement(ElementName = "Description")]
        public Description Description { get; set; }

        [XmlElement(ElementName = "Severity")]
        public string Severity { get; set; }

        [XmlElement(ElementName = "Certainty")]
        public string Certainty { get; set; }

        [XmlElement(ElementName = "Source")]
        public string Source { get; set; }

        [XmlElement(ElementName = "FixImpact")]
        public string FixImpact { get; set; }

        [XmlElement(ElementName = "HasCodeFix")]
        public string HasCodeFix { get; set; }

        [XmlElement(ElementName = "HowToFix")]
        public XmlCDataSection HowToFix
        {
            get
            {
                if (_howToFix != null)
                {
                    XmlDocument doc = new XmlDocument();
                    return doc.CreateCDataSection(_howToFix);
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value != null)
                {
                    _howToFix = value.Value;
                }
            }
        }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "AutoFixWarnings")]
        public AutoFixWarnings AutoFixWarnings { get; set; }
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
        public string Id { get; set; }
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
        public string Id { get; set; }
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