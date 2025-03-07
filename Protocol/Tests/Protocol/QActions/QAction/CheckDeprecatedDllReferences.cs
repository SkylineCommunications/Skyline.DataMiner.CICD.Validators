namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CheckDeprecatedDllReferences
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Parsers.Common.VisualStudio.Projects;
    
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckDeprecatedDllReferences, Category.QAction)]
    internal class CheckDeprecatedDllReferences : IValidate /*, ICodeFix, ICompare */
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (!context.HasQActionsAndIsSolution)
            {
                // Early skip when no QActions are present or when it is not solution based.
                return results;
            }
           
            foreach (CompiledQActionProject compiledQActionProject in context.CompiledQActions.Values)
            {
                // Load csproj of the QAction
                Project qactionProject = Project.Load(compiledQActionProject.Project.FilePath, compiledQActionProject.Project.Name);

                string[] referencedDlls = qactionProject.References.Select(reference => reference.GetDllName()).ToArray();
                foreach (string deprecatedDll in QActionHelper.DeprecatedDlls)
                {
                    if (referencedDlls.Contains(deprecatedDll, StringComparer.InvariantCultureIgnoreCase))
                    {
                        results.Add(Error.DeprecatedDll(this, compiledQActionProject.QAction, compiledQActionProject.QAction, deprecatedDll, compiledQActionProject.QAction.Id.RawValue));
                    }
                }
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