namespace Skyline.DataMiner.CICD.Tools.Validator
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the validator results.
    /// </summary>
    [Serializable]
    [XmlRoot("ValidatorResults")]
    public class ValidatorResults
    {
        /// <summary>
        /// Gets or sets the name of the validated protocol.
        /// </summary>
        /// <value>The name of the validated protocol.</value>
        public string Protocol { get; set; }

        /// <summary>
        /// Gets or sets the version of the validated protocol.
        /// </summary>
        /// <value>The version of the validated protocol.</value>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the version of the validator used to validate the protocol.
        /// </summary>
        /// <value>The version of the validator used to validate the protocol.</value>
        public string ValidatorVersion { get; set; }

        /// <summary>
        /// Gets or sets the validation time stamp.
        /// </summary>
        /// <value>The validation time stamp.</value>
        public DateTime ValidationTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the number of non-suppressed critical issues.
        /// </summary>
        /// <value>The number of non-suppressed critical issues.</value>
        public int CriticalIssueCount { get; set; }

        /// <summary>
        /// Gets or sets the number of suppressed critical issues.
        /// </summary>
        /// <value>The number of suppressed critical issues.</value>
        public int SuppressedCriticalIssueCount { get; set; }

        /// <summary>
        /// Gets or sets the number of non-suppressed major issues.
        /// </summary>
        /// <value>The number of non-suppressed major issues.</value>
        public int MajorIssueCount { get; set; }

        /// <summary>
        /// Gets or sets the number of suppressed major issues.
        /// </summary>
        /// <value>The number of suppressed major issues.</value>
        public int SuppressedMajorIssueCount { get; set; }

        /// <summary>
        /// Gets or sets the number of non-suppressed minor issues.
        /// </summary>
        /// <value>The number of non-suppressed minor issues.</value>
        public int MinorIssueCount { get; set; }

        /// <summary>
        /// Gets or sets the number of suppressed minor issues.
        /// </summary>
        /// <value>The number of suppressed minor issues.</value>
        public int SuppressedMinorIssueCount { get; set; }

        /// <summary>
        /// Gets or sets the number of non-suppressed warning issues.
        /// </summary>
        /// <value>The number of non-suppressed warning issues.</value>
        public int WarningIssueCount { get; set; }

        /// <summary>
        /// Gets or sets the number of suppressed warning issues.
        /// </summary>
        /// <value>The number of suppressed warning issues.</value>
        public int SuppressedWarningIssueCount { get; set; }

        /// <summary>
        /// Gets a value indicating whether the suppressed issues are included in the issue list.
        /// </summary>
        /// <value><c>true</c> if the suppressed issues are included in the issue list; otherwise, <c>false</c>.</value>
        public bool SuppressedIssuesIncluded { get; set; }

        /// <summary>
        /// Gets or sets the detected issues.
        /// </summary>
        /// <value>The detected issues.</value>
        [XmlArray("Issues"), XmlArrayItem(typeof(ValidatorResult), ElementName = "Issue")]
        public List<ValidatorResult> Issues { get; set; }
    }
}
