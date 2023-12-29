namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpSLProtocolGetParameters
{
    using System;
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.CSharpAnalysis;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Classes;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CSharpSLProtocolGetParameters, Category.QAction)]
    internal class CSharpSLProtocolGetParameters : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IQActionsQAction qaction, SyntaxTree syntaxTree, SemanticModel semanticModel, CompiledQActionProject projectData) in context.EachQActionProjectsAndSyntaxTreesAndModelsAndProjectDatas())
            {
                Solution solution = projectData.Project.Solution;
                QActionAnalyzer analyzer = new QActionAnalyzer(this, qaction, results, context.ProtocolModel, semanticModel, solution);
                RoslynVisitor parser = new RoslynVisitor(analyzer);

                parser.Visit(syntaxTree.GetRoot());
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

    internal class QActionAnalyzer : QActionAnalyzerBase
    {
        private readonly IProtocolModel protocolModel;

        public QActionAnalyzer(IValidate test, IQActionsQAction qAction, List<IValidationResult> results, IProtocolModel protocolModel, SemanticModel semanticModel, Solution solution)
            : base(test, results, qAction, semanticModel, solution)
        {
            this.protocolModel = protocolModel;
        }

        public override void CheckCallingMethod(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsSLProtocol(semanticModel) || !String.Equals(callingMethod.Name, "GetParameters"))
            {
                return;
            }

            if (callingMethod.Arguments.Count != 1)
            {
                // Invalid amount of arguments (GetParameters always has 1 argument)
                return;
            }

            if (!callingMethod.Arguments[0].TryParseToValue(semanticModel, solution, out Value value))
            {
                // Argument couldn't be parsed
                return;
            }

            if (value.Type == Value.ValueType.Unknown)
            {
                // Couldn't be properly parsed yet (Classes, List, ...)
                // Because we can't properly make a string with the type we skip these cases for now.
                return;
            }

            if (value.Type != Value.ValueType.Array || value.ArrayType != Value.ValueType.UInt32)
            {
                // GetParameters always need to be an UInt32 Array
                results.Add(Error.UnsupportedArgumentTypeForIds(test, qAction, qAction, value.GetValueTypeAsString(), qAction.Id.RawValue).WithCSharp(value));
                return;
            }

            if (value.IsMethodArgument)
            {
                // Can't parse the array.
                return;
            }

            List<IValidationResult> subResults = new List<IValidationResult>();
            foreach (Value element in value.Array)
            {
                if (element == null)
                {
                    continue;
                }

                if (!element.HasStaticValue)
                {
                    continue;
                }

                int pid = element.AsInt32;

                // Internal parameters don't have to be explicitly in driver and are allowed to be hard-coded
                if (ParamHelper.IsInternalPid(pid))
                {
                    continue;
                }

                // Check if the parameter exists.
                if (!protocolModel.TryGetObjectByKey(Mappings.ParamsById, pid.ToString(), out IParamsParam param))
                {
                    subResults.Add(Error.NonExistingParam(test, qAction, qAction, pid.ToString(), qAction.Id.RawValue).WithCSharp(element));
                }

                if (element.IsParameterClass(semanticModel, solution))
                {
                    continue;
                }

                if (param != null && !param.IsRead() && !param.IsWrite())
                {
                    // Example: polling ip
                    continue;
                }

                subResults.Add(Error.HardCodedPid(test, qAction, qAction, pid.ToString(), qAction.Id.RawValue).WithCSharp(element));
            }

            if (subResults.Count > 1)
            {
                string arguments = $"({callingMethod.Arguments[0].RawValue})";
                ICSharpValidationResult unexpectedImplementation = Error.UnexpectedImplementation(test, qAction, qAction, arguments, qAction.Id.RawValue)
                                                                        .WithSubResults(subResults.ToArray())
                                                                        .WithCSharp(callingMethod);
                results.Add(unexpectedImplementation);
            }
            else
            {
                results.AddRange(subResults);
            }
        }
    }
}