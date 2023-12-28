namespace SLDisDmFeatureCheck.Features
{
    using System.Collections.Generic;
    using System.Linq;
    using SLDisDmFeatureCheck.Common;
    using SLDisDmFeatureCheck.Common.Attributes;
    using SLDisDmFeatureCheck.Common.Interfaces;
    using SLDisDmFeatureCheck.Common.Results;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    //[MinDataMinerVersions("9.0-7282", "8.5.3.7-3052")]
    public class DefaultValue : IFeatureCheck
    {
        public string Title => "Default Parameter Value";

        public string Description => "Use this tag to specify the default value to be assigned to the parameter if it is empty after startup.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 8776 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            // Doesn't take in account the DefaultValues on columns that currently don't work at all (DCP33710)
            var items = context?.Model?.Protocol?.Params
                            ?.Where(x => x?.Interprete?.DefaultValue != null)
                            ?.Select(x => (IReadable)x)
                            .ToList();

            return new FeatureCheckResult(items);
        }
    }
}