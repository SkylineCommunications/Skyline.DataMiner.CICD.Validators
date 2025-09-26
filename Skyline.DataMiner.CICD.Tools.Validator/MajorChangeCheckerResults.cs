using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Skyline.DataMiner.CICD.Tools.Validator
{
    [Serializable]
    [XmlRoot("MCCResults")]
    public class MajorChangeCheckerResults
    {
        public string NewProtocol { get; set; }
        public string NewVersion { get; set; }
        public string OldProtocol { get; set; }
        public string OldVersion { get; set; }
        public string MCCVersion { get; set; }
        public DateTime ValidationTimeStamp { get; set; }
        public int CriticalIssueCount { get; set; }
        public int SuppressedCriticalIssueCount { get; set; }
        public int MajorIssueCount { get; set; }
        public int SuppressedMajorIssueCount { get; set; }
        public int MinorIssueCount { get; set; }
        public int SuppressedMinorIssueCount { get; set; }
        public int WarningIssueCount { get; set; }
        public int SuppressedWarningIssueCount { get; set; }
        public bool SuppressedIssuesIncluded { get; set; }

        [XmlArray("Issues"), XmlArrayItem(typeof(MajorChangeCheckerResult), ElementName = "Issue")]
        public List<MajorChangeCheckerResult> Issues { get; set; }
    }
}
