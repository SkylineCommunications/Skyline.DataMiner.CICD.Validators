namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    //[MinDataMinerVersions("8.5-2859", "8.0.8.4-2556")]
    internal class ParameterDateTime : IFeatureCheck
    {
        public string Title => "Parameter DateTime";

        public string Description => "The parameter will be displayed as a datetime. The value represents a decimal number indicating the total number of days that have passed since midnight 1899-12-30. The Interprete/Decimals tag of this parameter needs to be set to 8 to avoid rounding errors.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 6046 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var items = context?.Model?.Protocol?.Params
                            ?.Where(x => x?.IsDateTime() == true)
                            .Select(x => (IReadable)x)
                            .ToList();

            return new FeatureCheckResult(items);
        }
    }
}