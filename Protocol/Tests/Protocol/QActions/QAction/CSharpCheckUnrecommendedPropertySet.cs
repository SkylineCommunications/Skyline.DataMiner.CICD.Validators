namespace SLDisValidator2.Tests.Protocol.QActions.QAction.CSharpCheckUnrecommendedPropertySet
{
    using System.Collections.Generic;
    using System.Threading;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using Skyline.DataMiner.CICD.CSharpAnalysis;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Classes;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CSharpCheckUnrecommendedPropertySet, Category.QAction)]
    public class CSharpCheckUnrecommendedPropertySet : IValidate /*, ICodeFix, ICompare*/
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

    internal class QActionAnalyzer : CSharpAnalyzerBase
    {
        private static readonly System.Type ThreadType = typeof(Thread);
        private static readonly string ThreadAssemblyName = ThreadType.Assembly.GetName().Name;

        private readonly List<IValidationResult> results;
        private readonly IValidate test;
        private readonly IQActionsQAction qAction;
        private readonly IProtocolModel protocolModel;
        private readonly SemanticModel semanticModel;
        private readonly Solution solution;

        public QActionAnalyzer(IValidate test, IQActionsQAction qAction, List<IValidationResult> results, IProtocolModel protocolModel, SemanticModel semanticModel, Solution solution)
        {
            this.test = test;
            this.qAction = qAction;
            this.results = results;
            this.protocolModel = protocolModel;
            this.semanticModel = semanticModel;
            this.solution = solution;
        }

        public override void CheckAssignmentExpression(AssignmentExpressionClass assignment)
        {
            CheckCultureInfo_DefaultThreadCurrentCulture(assignment);
            CheckCultureInfo_Thread_CurrentThread_CurrentCulture(assignment);
        }

        private void CheckCultureInfo_DefaultThreadCurrentCulture(AssignmentExpressionClass assignmentExpression)
        {
            string fqn = assignmentExpression.GetFullyQualifiedNameOfLeftOperand(semanticModel);

            if (fqn != "System.Globalization.CultureInfo" || assignmentExpression.MemberName == null || assignmentExpression.MemberName != "DefaultThreadCurrentCulture")
            {
                return;
            }

            results.Add(Error.UnrecommendedCultureInfoDefaultThreadCurrentCulture(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(assignmentExpression));
        }

        private void CheckCultureInfo_Thread_CurrentThread_CurrentCulture(AssignmentExpressionClass assignmentExpression)
        {
            var symbol = RoslynHelper.GetSymbol(assignmentExpression.SyntaxNode.Left, semanticModel);
            if (!RoslynHelper.CheckIfCertainClass(symbol, semanticModel, solution, ThreadAssemblyName, ThreadType.Namespace))
            {
                // Not System.Threading.Thread
                return;
            }

            string propertyPath = assignmentExpression.PropertyPath;

            // Try to figure out when it's a variable what is assigned to the variable.
            if (RoslynHelper.TryGetVariableAssignment(assignmentExpression.SyntaxNode, semanticModel, solution, out var varAssignment) &&
                varAssignment is MemberAccessExpressionSyntax maes)
            {
                propertyPath = maes.ToString();
            }
            
            if (propertyPath != "Thread.CurrentThread" &&
                propertyPath != "System.Threading.Thread.CurrentThread")
            {
                return;
            }

            if (assignmentExpression.MemberName == "CurrentCulture")
            {
                results.Add(Error.UnrecommendedThreadCurrentThreadCurrentCulture(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(assignmentExpression));
            }
            else if (assignmentExpression.MemberName == "CurrentUICulture")
            {
                results.Add(Error.UnrecommendedThreadCurrentThreadCurrentUICulture(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(assignmentExpression));
            }
            else
            {
                // Do nothing.
            }
        }
    }
}