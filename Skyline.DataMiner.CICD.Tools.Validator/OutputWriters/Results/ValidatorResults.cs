namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.Results
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    /// <summary>
    /// Represents the validator results.
    /// </summary>
    [Serializable]
    [XmlRoot("ValidatorResults")]
    public class ValidatorResults(IProtocolInputData inputData)
    {
        /// <summary>
        /// Represents the default value used to indicate an unknown state or condition.
        /// </summary>
        protected const string Unknown = "Unknown";

        /// <summary>
        /// Gets or sets the name of the validated protocol.
        /// </summary>
        public string Protocol { get; } = inputData.Model.Protocol?.Name?.Value ?? Unknown;

        /// <summary>
        /// Gets or sets the version of the validated protocol.
        /// </summary>
        public string Version { get; } = inputData.Model.Protocol?.Version?.Value ?? Unknown;

        /// <summary>
        /// Gets or sets the version of the validator used.
        /// </summary>
        public string ValidatorVersion { get; } = typeof(ValidatorResults).Assembly.GetName().Version?.ToString() ?? Unknown;

        /// <summary>
        /// Gets or sets the validation time stamp.
        /// </summary>
        public DateTime ValidationTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the number of non-suppressed critical issues.
        /// </summary>
        public int CriticalIssueCount { get; set; }

        /// <summary>
        /// Gets or sets the number of suppressed critical issues.
        /// </summary>
        public int SuppressedCriticalIssueCount { get; set; }

        /// <summary>
        /// Gets or sets the number of non-suppressed major issues.
        /// </summary>
        public int MajorIssueCount { get; set; }

        /// <summary>
        /// Gets or sets the number of suppressed major issues.
        /// </summary>
        public int SuppressedMajorIssueCount { get; set; }

        /// <summary>
        /// Gets or sets the number of non-suppressed minor issues.
        /// </summary>
        public int MinorIssueCount { get; set; }

        /// <summary>
        /// Gets or sets the number of suppressed minor issues.
        /// </summary>
        public int SuppressedMinorIssueCount { get; set; }

        /// <summary>
        /// Gets or sets the number of non-suppressed warning issues.
        /// </summary>
        public int WarningIssueCount { get; set; }

        /// <summary>
        /// Gets or sets the number of suppressed warning issues.
        /// </summary>
        public int SuppressedWarningIssueCount { get; set; }

        /// <summary>
        /// Gets a value indicating whether the suppressed issues are included in the issue list.
        /// </summary>
        public bool SuppressedIssuesIncluded { get; set; }

        /// <summary>
        /// Gets or sets the detected issues.
        /// </summary>
        [XmlArray("Issues"), XmlArrayItem(typeof(ValidatorResult), ElementName = "Issue")]
        public List<ValidatorResult> Issues { get; set; } = [];
    }
}
