namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpCheckPreprocessorDirective
{
    using System;
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using Skyline.DataMiner.CICD.CSharpAnalysis;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CSharpCheckPreprocessorDirective, Category.QAction)]
    internal class CSharpCheckPreprocessorDirective : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((CompiledQActionProject projectData, IQActionsQAction qaction) in context.EachQActionProject())
            {
                QActionAnalyzer analyzer = new QActionAnalyzer(this, qaction, results);
                RoslynVisitor parser = new RoslynVisitor(analyzer);

                foreach ((SyntaxTree syntaxTree, _) in projectData.EachQActionSyntaxTreesAndModels())
                {
                    parser.Visit(syntaxTree.GetRoot());
                }
            }

            return results;
        }
    }

    internal class QActionAnalyzer : CSharpAnalyzerBase
    {
        private readonly List<IValidationResult> results;
        private readonly IValidate test;
        private readonly IQActionsQAction qAction;

        public QActionAnalyzer(IValidate test, IQActionsQAction qAction, List<IValidationResult> results)
        {
            this.test = test;
            this.qAction = qAction;
            this.results = results;
        }

        public override void CheckDefineDirective(string directiveName, DefineDirectiveTriviaSyntax directive) => CheckDirective(directiveName, directive);

        public override void CheckIfDirective(string directiveName, IfDirectiveTriviaSyntax directive) => CheckDirective(directiveName, directive);

        public override void CheckCommentLine(string commentLine, SyntaxTrivia trivia)
        {
            if (String.IsNullOrWhiteSpace(commentLine))
            {
                return;
            }

            if (commentLine.Contains("DCFv1"))
            {
                results.Add(Error.ObsoleteDcfV1(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(trivia));
            }
        }

        public void CheckDirective(string directiveName, DirectiveTriviaSyntax directive)
        {
            if (String.Equals(directiveName, "DCFv1", StringComparison.OrdinalIgnoreCase))
            {
                results.Add(Error.ObsoleteDcfV1(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(directive.ParentTrivia));
            }
            // Add extra cases when needed
        }
    }
}