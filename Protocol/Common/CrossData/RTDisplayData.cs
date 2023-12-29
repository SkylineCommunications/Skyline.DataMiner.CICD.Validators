namespace Skyline.DataMiner.CICD.Validators.Protocol.Common.CrossData
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    /// <summary>
    /// Holds the data needed for <see cref="Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.RTDisplay.CheckRTDisplayTag.CheckRTDisplayTag"/>.
    /// </summary>
    internal class RtDisplayData
    {
        private readonly Dictionary<int, (IParamsParam param, List<IValidationResult> results)> parameters = new Dictionary<int, (IParamsParam, List<IValidationResult>)>();

        public IReadOnlyList<(IParamsParam param, List<IValidationResult> expectedRTDisplayResults)> GetParamsAllowingRtDisplay() => parameters.Values.ToList();

        /// <summary>
        /// Add parameter for which RTDisplay tag is allowed or required to be true.
        /// </summary>
        /// <param name="param">Param for which RTDisplay tag is allowed or required to be true.</param>
        /// <param name="result">Optional subResult in case RTDisplay tag is required to be true.</param>
        public void AddParam(IParamsParam param, IValidationResult result = null)
        {
            // We use the FirstCharOffset in the case of parameters with a duplicate id
            // This will also cover parameters without an id or an invalid one, but technically this shouldn't happen as the checks should only cover items with valid ids.

            // In case of exported protocol: param.ReadNode is XmlElementExportOverride class where Token is null. FirstCharOffset is overriden, but in the regular XmlElement, the FirstCharOffset relies on the Token property.

            if (!parameters.TryGetValue(param.ReadNode.FirstCharOffset, out (IParamsParam param, List<IValidationResult> expectedRTDisplayResults) temp))
            {
                temp = (param, new List<IValidationResult>());
                parameters.Add(param.ReadNode.FirstCharOffset, temp);
            }

            if (result != null)
            {
                temp.expectedRTDisplayResults.Add(result);
            }
        }
    }
}