namespace Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    internal static class ValidatorContextExtensions
    {
        public static IEnumerable<IParameterGroupsGroup> EachParameterGroupWithValidId(this ValidatorContext context)
        {
            return context?.ProtocolModel?.EachParameterGroupWithValidId() ?? Enumerable.Empty<IParameterGroupsGroup>();
        }

        public static IEnumerable<IParamsParam> EachParamWithValidId(this ValidatorContext context)
        {
            return context?.ProtocolModel?.EachParamWithValidId() ?? Enumerable.Empty<IParamsParam>();
        }

        public static IEnumerable<IQActionsQAction> EachQActionWithValidId(this ValidatorContext context)
        {
            return context?.ProtocolModel?.EachQActionWithValidId() ?? Enumerable.Empty<IQActionsQAction>();
        }

        public static IEnumerable<IHTTPSession> EachHttpSessionWithValidId(this ValidatorContext context)
        {
            return context?.ProtocolModel?.EachHttpSessionWithValidId() ?? Enumerable.Empty<IHTTPSession>();
        }

        public static IEnumerable<IHTTPSessionConnection> EachHttpConnectionWithValidId(this ValidatorContext context, IHTTPSession httpSession)
        {
            return context?.ProtocolModel?.EachHttpConnectionWithValidId(httpSession) ?? Enumerable.Empty<IHTTPSessionConnection>();
        }

        public static IEnumerable<ICommandsCommand> EachCommandWithValidId(this ValidatorContext context)
        {
            return context?.ProtocolModel?.EachCommandWithValidId() ?? Enumerable.Empty<ICommandsCommand>();
        }

        public static IEnumerable<IResponsesResponse> EachResponseWithValidId(this ValidatorContext context)
        {
            return context?.ProtocolModel?.EachResponseWithValidId() ?? Enumerable.Empty<IResponsesResponse>();
        }

        public static IEnumerable<IPairsPair> EachPairWithValidId(this ValidatorContext context)
        {
            return context?.ProtocolModel?.EachPairWithValidId() ?? Enumerable.Empty<IPairsPair>();
        }

        public static IEnumerable<IGroupsGroup> EachGroupWithValidId(this ValidatorContext context)
        {
            return context?.ProtocolModel?.EachGroupWithValidId() ?? Enumerable.Empty<IGroupsGroup>();
        }

        public static IEnumerable<ITriggersTrigger> EachTriggerWithValidId(this ValidatorContext context)
        {
            return context?.ProtocolModel?.EachTriggerWithValidId() ?? Enumerable.Empty<ITriggersTrigger>();
        }

        public static IEnumerable<IActionsAction> EachActionWithValidId(this ValidatorContext context)
        {
            return context?.ProtocolModel?.EachActionWithValidId() ?? Enumerable.Empty<IActionsAction>();
        }

        public static IEnumerable<ITimersTimer> EachTimerWithValidId(this ValidatorContext context)
        {
            return context?.ProtocolModel?.EachTimerWithValidId() ?? Enumerable.Empty<ITimersTimer>();
        }

        public static IEnumerable<ITreeControlsTreeControl> EachTreeControlWithValidParameterId(this ValidatorContext context)
        {
            return context?.ProtocolModel?.EachTreeControlWithValidParameterId() ?? Enumerable.Empty<ITreeControlsTreeControl>();
        }

        public static IEnumerable<(CompiledQActionProject projectData, IQActionsQAction qaction)> EachQActionProject(this ValidatorContext context)
        {
            var model = context.ProtocolModel;
            if (model?.Protocol?.QActions == null)
            {
                yield break;
            }

            if (context.CompiledQActions == null)
            {
                yield break;
            }

            foreach (KeyValuePair<ProjectId, CompiledQActionProject> kvp in context.CompiledQActions)
            {
                var projectData = kvp.Value;

                var projectName = projectData.Project.Name;
                if (projectName == "QAction_Helper" || projectName == "QAction_ClassLibrary")
                {
                    continue;
                }

                var qaction = projectData.QAction;

                // Don't consider old generated class library projects 
                if (qaction.Name?.RawValue == "** Auto-generated Class Library **")
                {
                    continue;
                }

                if (qaction.Id?.Value == null)
                {
                    // Invalid QAction. Has no ID.
                    continue;
                }

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
        
        public static IEnumerable<(IQActionsQAction qaction, SyntaxTree syntaxTree, SemanticModel semanticModel, CompiledQActionProject projectData)> EachQActionProjectsAndSyntaxTreesAndModelsAndProjectDatas(this ValidatorContext context)
        {
            foreach ((var projectData, var qaction) in context.EachQActionProject())
            {
                foreach ((var syntaxTree, var semanticModel) in projectData.EachQActionSyntaxTreesAndModels())
                {
                    yield return (qaction, syntaxTree, semanticModel, projectData);
                }
            }
        }
    }
}