namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Parsers.Common.VisualStudio.Projects;

    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    //[MinDataMinerVersions("10.1.0.0-9966", "10.0.10.0-9454")]
    internal class NuGetPackages : IFeatureCheck
    {
        public string Title => "NuGet packages";

        public string Description => "NuGet packages";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 26605 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            if (!context.IsSolution)
            {
                // FilePaths are not available for the solution & projects when being run on the xml file only.
                return new FeatureCheckResult();
            }

            List<IReadable> qActions = new List<IReadable>();
            foreach ((CompiledQActionProject projectData, IQActionsQAction qaction) in context.EachQActionProject())
            {
                var project = Project.Load(projectData.Project.FilePath, projectData.Project.Name);

                bool packageReferencesFound = project.PackageReferences.Any(IsNotDevPackOrStyleCop);

                if (packageReferencesFound)
                {
                    qActions.Add(qaction);
                }
            }

            return new FeatureCheckResult(qActions);

            bool IsNotDevPackOrStyleCop(PackageReference packageReference)
            {
                return !packageReference.Name.StartsWith("Skyline.DataMiner.Dev.") &&
                       !packageReference.Name.StartsWith("Skyline.DataMiner.Files.") &&
                       !packageReference.Name.Equals("StyleCop.Analyzers");
            }
        }
    }
}