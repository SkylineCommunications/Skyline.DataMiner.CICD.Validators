namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;
    using System.IO;

    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Parsers.Common.VisualStudio.Projects;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    [MinDataMinerVersions("10.2.0.0-11517", "10.1.11.0-11027")]
    internal class DotNetFramework : IFeatureCheck
    {
        public string Title => ".NET Framework 4.8";

        public string Description => "DataMiner has .NET Framework 4.8 as minimum requirement.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 31120 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            if (context?.Model?.Protocol?.QActions == null || context.CompiledQActions == null ||
                !context.IsSolution)
            {
                return new FeatureCheckResult();
            }

            var items = new List<FeatureCheckResultItem>();

            foreach ((CompiledQActionProject projectData, IQActionsQAction qAction) in context.EachQActionProject(true))
            {
                try
                {
                    Project project = Project.Load(projectData.Project.FilePath, projectData.Project.Name);

                    if (project.TargetFrameworkMoniker != ".NETFramework,Version=v4.6.2")
                    {
                        // If TargetFramework is not .NET Framework 4.6.2 (e.g.: 4.7 or higher), then it requires DM 10.2.
                        items.Add(new FeatureCheckResultItem(qAction));
                    }
                }
                catch (FileNotFoundException)
                {
                    // Can happen in legacy style projects. In case a file mentioned in the csproj does not exist.
                }
            }

            return new FeatureCheckResult(items);
        }
    }
}
