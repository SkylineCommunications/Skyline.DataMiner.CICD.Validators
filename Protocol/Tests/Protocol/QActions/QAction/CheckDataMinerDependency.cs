namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CheckDataMinerDependency
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Common;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Parsers.Common.VisualStudio.Projects;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckDataMinerDependency, Category.QAction)]
    internal class CheckDataMinerDependency : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (!context.HasQActionsAndIsSolution)
            {
                // Early skip when no QActions are present or when it is not solution based.
                return results;
            }

            ValidateHelper helper = new ValidateHelper(this, context, results);
            foreach ((CompiledQActionProject projectData, IQActionsQAction qaction) in context.EachQActionProject(true))
            {
                // Load csproj
                var project = Project.Load(projectData.Project.FilePath, projectData.Project.Name);
                if (!project.PackageReferences.Any())
                {
                    // No NuGet packages being used
                    // Should not happen, but is covered by the banner in DIS.
                    continue;
                }

                helper.CheckDevPack(project.PackageReferences, qaction);
            }

            return results;
        }

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {

        ////        default:
        ////            result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
        ////            break;
        ////    }

        ////    return result;
        ////}

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }

    internal class ValidateHelper : ValidateHelperBase
    {
        private readonly string minReqVersionTag;
        private readonly Version minVersion;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results) : base(test, context, results)
        {
            minReqVersionTag = context.ProtocolModel.Protocol?.Compliancies?.MinimumRequiredVersion?.Value;
            if (!DataMinerVersion.TryParse(minReqVersionTag, out DataMinerVersion version))
            {
                // Default in case tag is not present.
                version = context.ValidatorSettings.MinimumSupportedDataMinerVersion;
            }

            minVersion = new Version(version.Major, version.Minor, version.Build);
        }

        public void CheckDevPack(IEnumerable<PackageReference> packageReferences, IQActionsQAction qAction)
        {
            // Filter out DevPacks & the Files
            var dmPacks = packageReferences.Where(IsDevPackOrPartOf).ToList();

            foreach (PackageReference dmPack in dmPacks)
            {
                if (!Version.TryParse(dmPack.Version, out Version v))
                {
                    // Unable to parse the version of the DevPack.
                    continue;
                }

                Version packageVersion = new Version(v.Major, v.Minor, v.Build);

                /*
                 * MinimumRequiredVersion = 10.2.0.0 - XXXXX
                 *  minVersion == 10.2.0
                 *
                 * Package Reference = 10.2.0.5 (CU or rerun of package creation)
                 *  packageVersion == 10.2.0
                 */

                if (minVersion < packageVersion)
                {
                    results.Add(Error.MismatchDevPack(test, qAction, qAction, dmPack.Name, dmPack.Version, minReqVersionTag, qAction.Id?.RawValue));
                }
            }

            return;
            bool IsDevPackOrPartOf(PackageReference packageReference)
            {
                return packageReference.Name.StartsWith("Skyline.DataMiner.Dev.") ||
                       packageReference.Name.StartsWith("Skyline.DataMiner.Files");
            }
        }
    }
}