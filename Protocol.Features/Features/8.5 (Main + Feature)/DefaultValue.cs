namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    //[MinDataMinerVersions("9.0-7282", "8.5.3.7-3052")]
    internal class DefaultValue : IFeatureCheck
    {
        public string Title => "Default Parameter Value";

        public string Description => "Use this tag to specify the default value to be assigned to the parameter if it is empty after startup.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 8776 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            // Doesn't take in account the DefaultValues on columns that currently don't work at all (DCP33710)
            var items = context?.Model?.Protocol?.Params
                            ?.Where(x => x?.Interprete?.DefaultValue != null)
                            .Select(x => (IReadable)x)
                            .ToList();

            return new FeatureCheckResult(items);
        }
    }
}