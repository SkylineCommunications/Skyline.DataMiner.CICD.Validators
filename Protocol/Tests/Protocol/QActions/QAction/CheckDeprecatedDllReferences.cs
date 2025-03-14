namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CheckDeprecatedDllReferences
{
    using System;
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckDeprecatedDllReferences, Category.QAction)]
    internal class CheckDeprecatedDllReferences : IValidate /*, ICodeFix, ICompare */
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            ValidateHelper helper = new ValidateHelper(this, context, results);

            helper.ValidateIfNonSolution();
            helper.ValidateIfSolution();
            
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
        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results) : base(test, context, results)
        {
        }

        public void ValidateIfNonSolution()
        {
            if (!context.HasQActionsAndIsNotSolution)
            {
                // Will be covered by ValidateIfSolution
                return;
            }

            foreach (IQActionsQAction qaction in context.EachQActionWithValidId())
            {
                if (String.IsNullOrWhiteSpace(qaction.DllImport?.Value))
                {
                    continue;
                }

                foreach (string dll in qaction.GetDllImports())
                {
                    if (QActionHelper.DeprecatedDlls.Contains(dll, StringComparer.OrdinalIgnoreCase))
                    {
                        results.Add(Error.DeprecatedDll(test, qaction, qaction.DllImport, dll, qaction.Id.RawValue));
                    }
                }
            }
        }

        public void ValidateIfSolution()
        {
            if (!context.HasQActionsAndIsSolution)
            {
                // Will be covered by ValidateIfNonSolution
                return;
            }

            foreach ((CompiledQActionProject compiledQActionProject, IQActionsQAction qaction) in context.EachQActionProject(allowBuildErrors: true))
            {
                // Load csproj of the QAction
                Project qactionProject = Project.Load(compiledQActionProject.Project.FilePath, compiledQActionProject.Project.Name);

                string[] referencedDlls = qactionProject.References.Select(reference => reference.GetDllName()).ToArray();
                foreach (string deprecatedDll in QActionHelper.DeprecatedDlls)
                {
                    if (referencedDlls.Contains(deprecatedDll, StringComparer.OrdinalIgnoreCase))
                    {
                        results.Add(Error.DeprecatedDll(test, qaction, qaction, deprecatedDll, qaction.Id.RawValue));
                    }
                }
            }
        }
    }
}