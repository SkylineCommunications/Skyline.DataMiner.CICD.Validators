namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    [MinDataMinerVersions("10.2.0.0-11517", "10.1.4.0-10077")]
    internal class FlushPerDatagram : IFeatureCheck
    {
        public string Title => "Flush Per Datagram";

        public string Description => "FlushPerDatagram-option for SmartSerial UDP connections: When set to true the connection will send any received datagram straight to SLProtocol to be put on the response parameter.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 28999 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var items = context?.Model?.Protocol?.Ports
                               ?.Where(settings => settings.FlushPerDatagram != null)
                               .Select(settings => (IReadable)settings)
                               .ToList() ?? new List<IReadable>();

            if (context?.Model?.Protocol?.PortSettings?.FlushPerDatagram != null)
            {
                items.Add(context.Model.Protocol.PortSettings);
            }

            return new FeatureCheckResult(items);
        }
    }
}