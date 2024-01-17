namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// Holds the features that are used.
    /// </summary>
    public interface IDmVersionCheckResults
    {
        /// <summary>
        /// Gets the features that are used.
        /// </summary>
        IReadOnlyCollection<Feature> Features { get; }
    }
}