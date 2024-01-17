namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    //[MinDataMinerVersions("10.1.0.0-9966", "10.0.9.0-9385")]
    internal class DynamicUnits : IFeatureCheck
    {
        public string Title => "Dynamic Units";

        public string Description => "Some parameters require dynamic units. This way it is possible to represent parameter values with the most adequate units, despite the fact user being able to override them. Driver support for defining dynamic units was now added.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 18321, 26318, 26330 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var items = context?.Model?.Protocol?.Params
                               ?.Where(param => param?.Display?.DynamicUnits != null)
                               .Select(param => (IReadable)param)
                               .ToList();

            return new FeatureCheckResult(items);
        }
    }
}