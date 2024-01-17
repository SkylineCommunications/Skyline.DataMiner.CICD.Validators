namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    //[MinDataMinerVersions("10.1.0.0-9966", "10.0.8.0-9300")]
    internal class MatrixLayout : IFeatureCheck
    {
        public string Title => "Matrix Layout";

        public string Description => "It is now possible to change the layout of a matrix, so the inputs are on the top, and the outputs on the left (instead of inputs left, outputs top).";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 25456, 25892 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            // TODO: Also check in C# if it's changed?

            var items = context?.Model?.Protocol?.Params
                               ?.Where(param => param?.Measurement?.Discreets?.MatrixLayout != null)
                               .Select(param => (IReadable)param)
                               .ToList();

            return new FeatureCheckResult(items);
        }
    }
}