namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    /// <summary>
    /// Represent a item that uses the feature.
    /// </summary>
    public class FeatureCheckResultItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureCheckResultItem"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <exception cref="System.ArgumentNullException">node</exception>
        public FeatureCheckResultItem(IReadable node)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        /// <summary>
        /// Gets the node where the feature is used.
        /// </summary>
        public IReadable Node { get; }
    }
}