namespace Skyline.DataMiner.CICD.Validators.Protocol.Legacy
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    /// <summary>
    /// Validation Result properties.
    /// </summary>
    /// <seealso cref="IValidationResult" />
    [DataContract]
    internal class ValidationResult : IValidationResult
    {
        public List<IValidationResult> SubResults { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// Information, Warning or Error.
        /// </summary>
        [DataMember(Order = 3)]
        public Severity Severity { get; set; }

        public Source Source
        {
            get
            {
                return Source.Validator;
            }
        }

        public List<(string Message, bool AutoFixPopup)> AutoFixWarnings { get; } = new List<(string Message, bool AutoFixPopup)>(0);

        /// <summary>
        /// Gets or sets the line number in XML file.
        /// </summary>
        [DataMember(Order = 4)]
        public int Line { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the test.
        /// </summary>
        public string FullId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the error type.
        /// </summary>
        [DataMember(Order = 0)]
        public uint ErrorId { get; set; }

        public uint CheckId { get; set; }

        /// <summary>
        /// Gets or sets the format for the error description.
        /// </summary>
        [DataMember(Order = 1)]
        public string DescriptionFormat { get; set; }

        /// <summary>
        /// Gets or sets the name of the test.
        /// </summary>
        public string TestName { get; set; }

        /// <summary>
        /// Gets or sets the parameters to fill in DescriptionFormat.
        /// </summary>
        [DataMember(Order = 2)]
        public object[] DescriptionParameters { get; set; }

        /// <summary>
        /// Gets or sets the position of the result in XML file.
        /// </summary>
        public int Position { get; set; }

        public Category Category
        {
            get
            {
                return Category.Undefined;
            }
        }

        public Certainty Certainty
        {
            get
            {
                return Certainty.Certain;
            }
        }

        public FixImpact FixImpact
        {
            get
            {
                return FixImpact.Undefined;
            }
        }

        public string GroupDescription
        {
            get
            {
                return String.Empty;
            }
        }

        public string Description
        {
            get
            {
                return String.Empty;
            }
        }

        public string HowToFix
        {
            get
            {
                return String.Empty;
            }
        }

        [Obsolete("Has been moved to the dedicated markdown file about the error. Will be removed in the next major change update.")]
        public string ExampleCode
        {
            get
            {
                return String.Empty;
            }
        }

        [Obsolete("Has been moved to the dedicated markdown file about the error. Will be removed in the next major change update.")]
        public string Details
        {
            get
            {
                return String.Empty;
            }
        }

        public IReadable ReferenceNode
        {
            get
            {
                return null;
            }
        }

        public IReadable PositionNode
        {
            get
            {
                return null;
            }
        }

        public bool HasCodeFix
        {
            get
            {
                return false;
            }
        }

        [DataMember(Order = 5)]
        public (int TablePid, string Name)? DveExport { get; set; }
    }
}