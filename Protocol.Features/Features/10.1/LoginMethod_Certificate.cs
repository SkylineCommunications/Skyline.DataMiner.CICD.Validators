namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    //[MinDataMinerVersions("10.1.0.0-9966", "10.0.5.0-9164")]
    internal class LoginMethodCertificate : IFeatureCheck
    {
        public string Title => "Login Method - Certificate";

        public string Description => "It's now possible to use client certificate authentication with a webserver in a DataMiner protocol";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 25243 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var items = context?.Model?.Protocol?.HTTP
                               ?.Where(session => session?.LoginMethod?.Value == EnumHttpLoginMethod.Certificate)
                               .Select(session => (IReadable)session)
                               .ToList();

            return new FeatureCheckResult(items);
        }
    }
}