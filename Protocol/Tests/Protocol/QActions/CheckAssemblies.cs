namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.CheckAssemblies
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Parsers.Common.VisualStudio.Projects;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckAssemblies, Category.QAction)]
    internal class CheckAssemblies : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (!context.HasQActionsAndIsSolution)
            {
                // Early skip when no QActions are present or when it is not solution based.
                return results;
            }

            Dictionary<string, List<(string Version, IQActionsQAction QAction)>> packagesWithVersions =
                new Dictionary<string, List<(string Version, IQActionsQAction QAction)>>();
            foreach (CompiledQActionProject compiledQActionProject in context.CompiledQActions.Values)
            {
                // Load csproj
                var project = Project.Load(compiledQActionProject.Project.FilePath, compiledQActionProject.Project.Name);

                if (!project.PackageReferences.Any())
                {
                    // No NuGet packages being used
                    continue;
                }

                if (!project.PackageReferences.Any(package => package.Name.Equals("Skyline.DataMiner.Utils.SecureCoding.Analyzers", System.StringComparison.OrdinalIgnoreCase)))
                {
                    var missingSecureCodingError = Error.MissingSecureCoding(
                        this,
                        compiledQActionProject.QAction,
                        compiledQActionProject.QAction,
                        compiledQActionProject.QAction.Id.RawValue);

                    results.Add(missingSecureCodingError);
                }

                foreach (PackageReference packageReference in project.PackageReferences)
                {
                    if (!packagesWithVersions.TryGetValue(packageReference.Name, out var list))
                    {
                        list = new List<(string Version, IQActionsQAction QAction)>();
                        packagesWithVersions.Add(packageReference.Name, list);
                    }

                    list.Add((packageReference.Version, compiledQActionProject.QAction));
                }
            }

            foreach (KeyValuePair<string, List<(string Version, IQActionsQAction)>> packagesWithVersion in packagesWithVersions)
            {
                string packageName = packagesWithVersion.Key;

                if (packagesWithVersion.Value.Select(tuple => tuple.Version).Distinct().Count() <= 1)
                {
                    // No multiple versions found.
                    continue;
                }

                IValidationResult mainError = Error.UnconsolidatedPackageReference(this, context.ProtocolModel.Protocol.QActions,
                    context.ProtocolModel.Protocol.QActions,
                    packageName);
                foreach ((string version, IQActionsQAction qAction) in packagesWithVersion.Value)
                {
                    mainError.WithSubResults(Error.UnconsolidatedPackageReference_Sub(this, qAction, qAction, qAction.Id.RawValue, packageName, version));
                }

                results.Add(mainError);
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
}