using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters
{
    /// <summary>
    /// Interface for writing Major Change Checker results.
    /// </summary>
    internal interface IMajorChangeCheckerResultWriter
    {
        /// <summary>
        /// Writes the specified MCC results.
        /// </summary>
        /// <param name="majorChangeCheckerResults">The MCC results.</param>
        void WriteResults(MajorChangeCheckerResults majorChangeCheckerResults);
    }
}
