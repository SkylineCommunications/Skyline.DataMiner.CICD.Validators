namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Common
{
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    internal static class FeatureCheckContextExtensions
    {
        public static IEnumerable<(IQActionsQAction qaction, SyntaxTree syntaxTree, SemanticModel semanticModel)> EachQActionProjectsAndSyntaxTreesAndModels(this FeatureCheckContext context)
        {
            foreach ((var projectData, var qaction) in context.EachQActionProject())
            {
                foreach ((var syntaxTree, var semanticModel) in projectData.EachQActionSyntaxTreesAndModels())
                {
                    yield return (qaction, syntaxTree, semanticModel);
                }
            }
        }

        public static IEnumerable<(IQActionsQAction qaction, SyntaxTree syntaxTree, SemanticModel semanticModel, CompiledQActionProject projectData)> EachQActionProjectsAndSyntaxTreesAndModelsAndProjectDatas(this FeatureCheckContext context)
        {
            foreach ((var projectData, var qaction) in context.EachQActionProject())
            {
                foreach ((var syntaxTree, var semanticModel) in projectData.EachQActionSyntaxTreesAndModels())
                {
                    yield return (qaction, syntaxTree, semanticModel, projectData);
                }
            }
        }

        public static IEnumerable<(CompiledQActionProject projectData, IQActionsQAction qaction)> EachQActionProject(this FeatureCheckContext context)
        {
            var model = context.Model;
            if (model?.Protocol?.QActions == null)
            {
                yield break;
            }

            var solutionSemanticModel = context.CompiledQActions;
            if (solutionSemanticModel == null)
            {
                yield break;
            }

            foreach (KeyValuePair<ProjectId, CompiledQActionProject> kvp in context.CompiledQActions)
            {
                var projectData = kvp.Value;

                var qaction = projectData.QAction;

                // We only validate if the build of the project succeeded.
                if (!projectData.BuildSucceeded)
                {
                    continue;
                }

                yield return (projectData, qaction);
            }
        }

        public static IEnumerable<(SyntaxTree syntaxTree, SemanticModel semanticModel)> EachQActionSyntaxTreesAndModels(this CompiledQActionProject projectData)
        {
            var treesAndModels = projectData.TreesAndModels;
            foreach (var treeAndModel in treesAndModels)
            {
                var syntaxTree = treeAndModel.SyntaxTree;
                var semanticModel = treeAndModel.SemanticModel;

                if (syntaxTree == null || semanticModel == null)
                {
                    continue;
                }

                yield return (syntaxTree, semanticModel);
            }
        }
    }
}