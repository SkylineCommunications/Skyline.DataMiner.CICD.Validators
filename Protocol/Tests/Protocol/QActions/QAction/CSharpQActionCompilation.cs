namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpQActionCompilation
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CSharpQActionCompilation, Category.QAction)]
    internal class CSharpQActionCompilation : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (context.ProtocolModel?.Protocol?.QActions == null)
            {
                return results;
            }

            if (context.CompiledQActions == null)
            {
                return results;
            }

            // Keeping the code for when this would happen again.
            ////if (solutionSemanticModel.ConfiguredLanguageVersion == Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp4 && solutionSemanticModel.PotentialLanguageVersion == Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp7_3)
            ////{
            ////    results.Add(Error.NoCSharpCodeAnalysisPerformed(this, null, null, "7.3", "< VS 2017"));
            ////    return results;
            ////}

            foreach (var projectId in context.CompiledQActions.Keys)
            {
                var compiledQAction = context.CompiledQActions[projectId];

                // We only validate if the build of the project succeeded.
                if (compiledQAction.BuildSucceeded)
                {
                    continue;
                }

                var qaction = compiledQAction.QAction;
                if (qaction == null)
                {
                    continue;
                }

                List<IValidationResult> subResults = new List<IValidationResult>();
                foreach (var compilationError in compiledQAction.CompilationErrors)
                {
                    subResults.Add(Error.CompilationFailure_Sub(this, qaction, qaction, compilationError.ToString())
                        .WithCSharp(compilationError.Location));
                }

                results.Add(Error.CompilationFailure(this, qaction, qaction, qaction.Id.RawValue)
                    .WithSubResults(subResults.ToArray()));
            }

            return results;
        }
    }
}