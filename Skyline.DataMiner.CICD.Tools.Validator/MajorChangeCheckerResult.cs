using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skyline.DataMiner.CICD.Validators.Common.Model;

namespace Skyline.DataMiner.CICD.Tools.Validator
{
    [Serializable]
    public class MajorChangeCheckerResult
    {
        public Certainty Certainty { get; set; }
        public FixImpact FixImpact { get; set; }
        public Category Category { get; set; }
        public string Id { get; set; }
        public Severity Severity { get; set; }
        public string Description { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public string Dve { get; set; }
        public bool Suppressed { get; set; }
        public List<MajorChangeCheckerResult> SubResults { get; set; }
    }
}
