namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpCheckEntryPoints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.CSharpAnalysis;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Classes;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Enums;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CSharpCheckEntryPoints, Category.QAction)]
    internal class CSharpCheckEntryPoints : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((CompiledQActionProject projectData, IQActionsQAction qaction) in context.EachQActionProject())
            {
                // If no triggers are defined then there is no entry point.
                if (qaction.Triggers == null)
                {
                    continue;
                }

                var entryPoints = qaction.GetEntryPoints().EntryPoints;

                QActionAnalyzer analyzer = new QActionAnalyzer(this, qaction, results, entryPoints);
                foreach ((SyntaxTree syntaxTree, SemanticModel semanticModel) in projectData.EachQActionSyntaxTreesAndModels())
                {
                    analyzer.UpdateSemanticModel(semanticModel);
                    RoslynVisitor parser = new RoslynVisitor(analyzer);
                    parser.Visit(syntaxTree.GetRoot());
                }

                analyzer.CheckRemainingEntryPoints();
                analyzer.CheckAccessModifiersEntryPoints();
                analyzer.CheckArg0TypeEntryPoints();
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();
            switch (context.Result.ErrorId)
            {

                default:
                    result.Message = String.Format("This error ({0}) isn't implemented.", context.Result.ErrorId.ToString());
                    break;
            }

            return result;
        }
    }

    internal class QActionAnalyzer : CSharpAnalyzerBase
    {
        private SemanticModel semanticModel;
        private readonly List<IValidationResult> results;
        private readonly IValidate test;
        private readonly IQActionsQAction qAction;
        private readonly IDictionary<string, QActionEntryPoints.EntryPoint> remainingEntryPoints;
        private readonly List<ClassClass> classesNotPublic;
        private readonly IDictionary<MethodClass, ClassClass> methodsNotPublic;
        private readonly IDictionary<MethodClass, ClassClass> protocolArguments;
        private readonly List<string> classOfEntrypoints;

        public QActionAnalyzer(IValidate test, IQActionsQAction qAction, List<IValidationResult> results, IReadOnlyList<QActionEntryPoints.EntryPoint> entryPoints)
        {
            this.test = test;
            this.qAction = qAction;
            this.results = results;

            remainingEntryPoints = new Dictionary<string, QActionEntryPoints.EntryPoint>();
            methodsNotPublic = new Dictionary<MethodClass, ClassClass>();
            protocolArguments = new Dictionary<MethodClass, ClassClass>();
            classesNotPublic = new List<ClassClass>();
            classOfEntrypoints = new List<string>();

            foreach (var entryPoint in entryPoints)
            {
                // In case 2 triggers point to the same entrypoint => duplicate entrypoint
                string key = $"{entryPoint.GetClassNameOrDefault()}.{entryPoint.GetMethodNameOrDefault()}";
                remainingEntryPoints[key] = entryPoint;

                // Classes that should be public
                if (!classOfEntrypoints.Contains(entryPoint.GetClassNameOrDefault()))
                {
                    classOfEntrypoints.Add(entryPoint.GetClassNameOrDefault());
                }
            }
        }

        public override void CheckClass(ClassClass classClass)
        {
            if (classOfEntrypoints.Contains(classClass.Name) && classClass.Access != AccessModifier.Public)
            {
                classesNotPublic.Add(classClass);
            }

            foreach (var method in classClass.Methods)
            {
                string path = $"{classClass.Name}.{method.Name}";

                if (!remainingEntryPoints.TryGetValue(path, out _))
                {
                    continue;
                }

                remainingEntryPoints.Remove(path);

                if (method.Access != AccessModifier.Public)
                {
                    methodsNotPublic[method] = classClass;
                }

                if (method.Parameters == null || !method.Parameters.Any())
                {
                    protocolArguments[method] = classClass;
                }
                else if (!method.Parameters[0].IsSLProtocol(semanticModel))
                {
                    protocolArguments[method] = classClass;
                }
            }
        }

        public void CheckRemainingEntryPoints()
        {
            foreach (var entryPoint in remainingEntryPoints.Values)
            {
                results.Add(Error.MissingEntryPoint(test, qAction, qAction, entryPoint.GetClassNameOrDefault(), entryPoint.GetMethodNameOrDefault(), qAction.Id.RawValue));
            }
        }

        public void CheckAccessModifiersEntryPoints()
        {
            foreach (var classClass in classesNotPublic)
            {
                results.Add(Error.UnexpectedAccessModifierForEntryPointClass(test, qAction, qAction, classClass.Name, classClass.Access.ToString().ToLower(), qAction.Id.RawValue).WithCSharp(classClass));
            }

            foreach (KeyValuePair<MethodClass, ClassClass> kvClass in methodsNotPublic)
            {
                results.Add(Error.UnexpectedAccessModifierForEntryPointMethod(test, qAction, qAction, kvClass.Value.Name, kvClass.Key.Name, kvClass.Key.Access.ToString().ToLower(), qAction.Id.RawValue).WithCSharp(kvClass.Key));
            }
        }

        public void CheckArg0TypeEntryPoints()
        {
            foreach (KeyValuePair<MethodClass, ClassClass> kvClass in protocolArguments)
            {
                if (kvClass.Key.Parameters == null || !kvClass.Key.Parameters.Any())
                {
                    results.Add(Error.UnexpectedArg0TypeForEntryPointMethod(test, qAction, qAction, kvClass.Value.Name, kvClass.Key.Name, "", qAction.Id.RawValue).WithCSharp(kvClass.Key));
                }
                else
                {
                    results.Add(Error.UnexpectedArg0TypeForEntryPointMethod(test, qAction, qAction, kvClass.Value.Name, kvClass.Key.Name, kvClass.Key.Parameters[0].Type, qAction.Id.RawValue).WithCSharp(kvClass.Key.Parameters[0]));
                }
            }
        }

        public void UpdateSemanticModel(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }
    }
}