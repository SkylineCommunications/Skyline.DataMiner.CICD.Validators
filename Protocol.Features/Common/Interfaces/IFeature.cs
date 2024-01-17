namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a feature.
    /// </summary>
    public interface IFeature
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the release notes.
        /// </summary>
        IReadOnlyCollection<uint> ReleaseNotes { get; }
    }
}