namespace SLDisDmFeatureCheck.Common.Results
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    public class FeatureCheckResultItem
    {
        public IReadable Node { get; }

        public FeatureCheckResultItem(IReadable node)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }
    }
}