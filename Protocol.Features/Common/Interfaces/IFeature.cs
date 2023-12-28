namespace SLDisDmFeatureCheck.Common.Interfaces
{
    using System.Collections.Generic;

    public interface IFeature
    {
        string Title { get; }

        string Description { get; }

        IReadOnlyCollection<uint> ReleaseNotes { get; }
    }
}