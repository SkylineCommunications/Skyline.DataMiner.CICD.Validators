namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpCoreInterAppBrokerSupport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using Skyline.DataMiner.CICD.CSharpAnalysis;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Classes;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    using Project = Parsers.Common.VisualStudio.Projects.Project;

    [Test(CheckId.CSharpCoreInterAppBrokerSupport, Category.QAction)]
    internal class CSharpCoreInterAppBrokerSupport : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (!context.HasQActionsAndIsSolution)
            {
                // Early skip when no QActions are present or when it is not solution based.
                return results;
            }

            foreach ((IQActionsQAction qaction, SyntaxTree syntaxTree, SemanticModel semanticModel, CompiledQActionProject projectData) in context.EachQActionProjectsAndSyntaxTreesAndModelsAndProjectDatas(true))
            {
                // Load csproj
                var project = Project.Load(projectData.Project.FilePath, projectData.Project.Name);
                if (!project.PackageReferences.Any())
                {
                    // No NuGet packages being used
                    continue;
                }

                // Then check if the nuget version Skyline.DataMiner.Core.InterApp is >= 1.0.1.1
                bool hasHighEnoughInterApp = CheckInterAppNugetVersions(project);
                if (hasHighEnoughInterApp)
                {
                    // Then check for invalid ways of replying with the interapp
                    // Protocol.SetParameter(9000001,)
                    // Protocol.SetParameter(ReturnAddress
                    // Message.Send(ReturnAddress

                    Solution solution = projectData.Project.Solution;
                    QActionAnalyzer analyzer = new QActionAnalyzer(this, qaction, results, semanticModel, solution);
                    RoslynVisitor parser = new RoslynVisitor(analyzer);
                    parser.Visit(syntaxTree.GetRoot());
                }
            }

            return results;
        }

        private static bool CheckInterAppNugetVersions(Project project)
        {
            return project.PackageReferences.Any(packageReference =>
                packageReference.Name.Equals("Skyline.DataMiner.Core.InterAppCalls.Common", StringComparison.InvariantCultureIgnoreCase) &&
                !packageReference.Version.StartsWith("1.0.0"));
        }
    }

    internal class QActionAnalyzer : QActionAnalyzerBase
    {
        public QActionAnalyzer(IValidate test, IQActionsQAction qAction, List<IValidationResult> results, SemanticModel semanticModel, Solution solution)
            : base(test, results, qAction, semanticModel, solution)
        {
        }

        public override void CheckCallingMethod(CallingMethodClass callingMethod)
        {
            CheckForSetParameter(callingMethod);
            CheckForSendMessage(callingMethod);
        }

        private static bool IsInterAppMessage(CallingMethodClass callingMethod, SemanticModel semanticModel)
        {
            string fullyQualifiedNameOfParent = callingMethod.GetFullyQualifiedNameOfParent(semanticModel);

            return String.Equals(fullyQualifiedNameOfParent, "Message", StringComparison.InvariantCultureIgnoreCase) || // .NET 8
                   String.Equals(fullyQualifiedNameOfParent, "Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle.Message", StringComparison.InvariantCultureIgnoreCase); // .NET Framework, .NET 6
        }

        private void CheckForSendMessage(CallingMethodClass callingMethod)
        {
            if (!IsInterAppMessage(callingMethod, semanticModel) || !String.Equals(callingMethod.Name, "Send"))
            {
                return;
            }

            var agentArgument = callingMethod.Arguments[1];
            var expressionOfArgument = agentArgument.SyntaxNode.Expression;

            // Trace the origin of the argument
            if (expressionOfArgument is IdentifierNameSyntax identifierName)
            {
                // Someone passing along a different variable possibly defined earlier as the AgentId.
                var symbolInfo = RoslynHelper.GetSymbol(identifierName, semanticModel);
                
                if (IsArgumentPropertyFromReturnAddress(symbolInfo))
                {
                    results.Add(Error.InvalidInterAppReplyLogic(test, qAction, qAction, "Message", "Send(ReturnAddress", qAction.Id.RawValue)
                                     .WithCSharp(callingMethod));
                }
            }
            else if (expressionOfArgument is MemberAccessExpressionSyntax maes)
            {
                string fullyQualifiedName = RoslynHelper.GetFullyQualifiedName(semanticModel, maes);

                /*
                 * var returnAddress = message.ReturnAddress;
                 * Send(..., returnAddress.AgentId, ...);
                 * => Covered by the first 2 statements.
                 *
                 *
                 * Send(..., message.ReturnAddress.AgentId, ...);
                 * => Covered in .NET 8 by the last statement. In .NET 6 & Framework, it's covered by the first 2 statements.
                 */
                if (String.Equals(fullyQualifiedName, "ReturnAddress") || // .NET 8
                    String.Equals(fullyQualifiedName, "Skyline.DataMiner.Core.InterAppCalls.Common.Shared.ReturnAddress") || // .NET Framework, .NET 6
                    agentArgument.RawValue.EndsWith(".ReturnAddress.AgentId", StringComparison.InvariantCultureIgnoreCase)) // Backup for .NET 8
                {
                    results.Add(Error.InvalidInterAppReplyLogic(test, qAction, qAction, "Message", "Send(ReturnAddress", qAction.Id.RawValue)
                                     .WithCSharp(callingMethod));
                }
            }
        }

        private void CheckForSetParameter(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsSLProtocol(semanticModel) || !String.Equals(callingMethod.Name, "SetParameter"))
            {
                return;
            }

            if (!callingMethod.Arguments[0].TryParseToValue(semanticModel, solution, out Value value) || !value.IsNumeric())
            {
                if (callingMethod.Arguments[0].RawValue.EndsWith(".ReturnAddress.ParameterId", StringComparison.InvariantCultureIgnoreCase))
                {
                    results.Add(Error.InvalidInterAppReplyLogic(test, qAction, qAction, "Protocol", "SetParameter(ReturnAddress", qAction.Id.RawValue)
                                     .WithCSharp(callingMethod));
                    return;
                }

                return;
            }

            if (!value.HasStaticValue)
            {
                // Uncertain about the value
                return;
            }

            int pid = value.AsInt32;

            if (pid == 9000001)
            {
                results.Add(Error.InvalidInterAppReplyLogic(test, qAction, qAction, "Protocol", "SetParameter(9000001", qAction.Id.RawValue)
                                 .WithCSharp(callingMethod));
            }
        }

        private bool IsArgumentPropertyFromReturnAddress(ISymbol symbol)
        {
            foreach (SyntaxNode syntax in symbol.DeclaringSyntaxReferences.Select(reference => reference.GetSyntax()))
            {
                if (!(syntax is VariableDeclaratorSyntax variableDeclarator))
                {
                    continue;
                }

                var initializer = variableDeclarator.Initializer;
                if (!(initializer?.Value is MemberAccessExpressionSyntax memberAccess))
                {
                    continue;
                }

                if (!memberAccess.Name.Identifier.Text.Equals("AgentId"))
                {
                    continue;
                }

                var fullyQualifiedName = RoslynHelper.GetFullyQualifiedName(semanticModel, memberAccess);
                if (String.Equals(fullyQualifiedName, "Skyline.DataMiner.Core.InterAppCalls.Common.Shared.ReturnAddress"))
                {
                    // .NET Framework & .NET 6
                    return true;
                }

                // At this point we found a declaration to a property called AgentId
                // Double check the Class this property comes from is our ReturnAddress class.
                // This is currently performed using Syntax Analysis. As symbol parsing requires valid compilation.
                // Get the full text of the expression leading to 'AgentId'
                var instanceExpression = memberAccess.Expression.ToString();
                
                if (instanceExpression.EndsWith("ReturnAddress"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}