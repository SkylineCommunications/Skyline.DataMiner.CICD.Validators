namespace SLDisValidator2.Tests.Protocol.QActions.QAction.CSharpCheckUnrecommendedConstructor
{
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.CSharpAnalysis;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Classes;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Interfaces;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    [Test(CheckId.CSharpCheckUnrecommendedConstructor, Category.QAction)]
    public class CSharpCheckUnrecommendedConstructor : IValidate /*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IQActionsQAction qaction, SyntaxTree syntaxTree, SemanticModel semanticModel, CompiledQActionProject projectData) in context.EachQActionProjectsAndSyntaxTreesAndModelsAndProjectDatas())
            {
                Solution solution = projectData.Project.Solution;
                QActionAnalyzer analyzer = new QActionAnalyzer(this, qaction, results, semanticModel, solution);
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

        internal class QActionAnalyzer : CSharpAnalyzerBase
        {
            private readonly List<IValidationResult> results;
            private readonly IValidate test;
            private readonly IQActionsQAction qAction;
            private readonly SemanticModel semanticModel;
            private readonly Solution solution;

            public QActionAnalyzer(IValidate test, IQActionsQAction qAction, List<IValidationResult> results, SemanticModel semanticModel, Solution solution)
            {
                this.test = test;
                this.qAction = qAction;
                this.results = results;
                this.semanticModel = semanticModel;
                this.solution = solution;
            }

            public override void CheckObjectCreation(ObjectCreationClass objectCreation)
            {
                CheckXmlSerializerConstructor(objectCreation);
            }

            private void CheckXmlSerializerConstructor(ObjectCreationClass objectCreation)
            {
                if (objectCreation.Name != "XmlSerializer")
                {
                    return;
                }

                var symbol = RoslynHelper.GetSymbol(objectCreation.SyntaxNode.Type, semanticModel);
                
                if (!RoslynHelper.CheckIfCertainClass(symbol, semanticModel, solution, "System.Xml", "System.Xml.Serialization"))
                {
                    return;
                }

                // Check if new XmlSerializer(Type) is called.
                if (objectCreation.Arguments.Count == 1 && objectCreation.Arguments[0].GetFullyQualifiedName(semanticModel).Equals("System.Type"))
                {
                    return;
                }

                // Check if new XmlSerializer(Type, String) is called.
                if (objectCreation.Arguments.Count == 2 &&
                    objectCreation.Arguments[0].GetFullyQualifiedName(semanticModel).Equals("System.Type") &&
                    objectCreation.Arguments[1].GetFullyQualifiedName(semanticModel).Equals("System.String"))
                {
                    return;
                }
                
                string constructor = GetConstructorDisplayValue(objectCreation.Arguments);

                results.Add(Error.UnrecommendedXmlSerializerConstructor(test, qAction, qAction, "System.Xml.Serialization", constructor, qAction.Id.RawValue).WithCSharp(objectCreation));
            }

            private string GetConstructorDisplayValue(IList<Argument> arguments)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("XmlSerializer(");

                for (int i = 0; i < arguments.Count; i++)
                {
                    string fqn = arguments[i].GetFullyQualifiedName(semanticModel);
                    int idx = fqn.LastIndexOf('.');

                    if (idx > -1)
                    {
                        sb.Append(fqn.Substring(idx + 1));
                    }
                    else
                    {
                        sb.Append(fqn);
                    }

                    if (i < arguments.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }

                sb.Append(")");

                return sb.ToString();
            }
        }
    }
}