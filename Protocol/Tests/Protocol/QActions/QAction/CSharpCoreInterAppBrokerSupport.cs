namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpCoreInterAppBrokerSupport
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection.Metadata;
    using System.Xml.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using Skyline.DataMiner.CICD.CSharpAnalysis;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Classes;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Parsers.Common.VisualStudio.Projects;
    using Skyline.DataMiner.CICD.Parsers.Protocol.Xml.QActions;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    using static System.Net.Mime.MediaTypeNames;

    using Project = Parsers.Common.VisualStudio.Projects.Project;

    [Test(CheckId.CSharpCoreInterAppBrokerSupport, Category.QAction)]
    internal class CSharpCoreInterAppBrokerSupport : IValidate
    {
        // Please comment out the interfaces that aren't used together with the respective methods.

        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            // first make sure we are dealing with a Solution
            if (context?.ProtocolModel?.Protocol?.QActions == null ||
            context.InputData?.QActionCompilationModel == null ||
            !context.InputData.QActionCompilationModel.IsSolutionBased ||
            context.CompiledQActions == null)
            {
                // Early skip when no QActions are present or when it is not solution based.
                return results;
            }

            Debug.WriteLine("TEMP: Top lvl looping all QActions...");
            foreach ((IQActionsQAction qaction, SyntaxTree syntaxTree, SemanticModel semanticModel, CompiledQActionProject projectData) in context.EachQActionProjectsAndSyntaxTreesAndModelsAndProjectDatas(true))
            {
                Debug.WriteLine("TEMP: Top lvl Parsing a QAction.");
                // Load csproj
                var project = Project.Load(projectData.Project.FilePath, projectData.Project.Name);
                Debug.WriteLine("TEMP: Top lvl Loaded project.");
                if (!project.PackageReferences.Any())
                {
                    Debug.WriteLine("TEMP: found 0 references");
                    // No NuGet packages being used
                    continue;
                }

                // then check if the nuget version Skyline.DataMiner.Core.InterApp is >= 1.0.1.1
                bool hasHighEnoughInterApp = CheckInterAppNugetVersions(project);

                if (hasHighEnoughInterApp)
                {
                    Debug.WriteLine("TEMP: found a high nuget version");
                    // then check for invalid ways of replying with the interapp
                    // Protocol.SetParameter(9000001,)
                    // Protocol.SetParameter(ReturnAddress
                    // Message.Send(ReturnAddress

                    Solution solution = projectData.Project.Solution;
                    QActionAnalyzer analyzer = new QActionAnalyzer(this, qaction, results, context.ProtocolModel, semanticModel, solution);
                    RoslynVisitor parser = new RoslynVisitor(analyzer);
                    Debug.WriteLine("TEMP: visiting the parser");
                    parser.Visit(syntaxTree.GetRoot());
                }
            }




            return results;
        }

        private static bool CheckInterAppNugetVersions(Project project)
        {
            Debug.WriteLine("TEMP: checking package references");
            foreach (PackageReference packageReference in project.PackageReferences)
            {
                Debug.WriteLine($"TEMP: {packageReference.Name}  {packageReference.Version}");
                if (packageReference.Name.Equals("Skyline.DataMiner.Core.InterAppCalls.Common", StringComparison.InvariantCultureIgnoreCase))
                {
                    var thisVersion = packageReference.Version;
                    if (!thisVersion.StartsWith("1.0.0"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    internal class QActionAnalyzer : QActionAnalyzerBase
    {
        private readonly IProtocolModel protocolModel;

        public QActionAnalyzer(IValidate test, IQActionsQAction qAction, List<IValidationResult> results, IProtocolModel protocolModel, SemanticModel semanticModel, Solution solution)
            : base(test, results, qAction, semanticModel, solution)
        {
            this.protocolModel = protocolModel;
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
                    results.Add(Error.InvalidInterAppReplyLogic(test, qAction, qAction, "Protocol", "SetParameter(ReturnAddress", qAction.Id.RawValue));
                    return;
                }
                else
                {
                    return;
                }
            }

            if (!value.HasStaticValue)
            {
                // Uncertain about the value
                return;
            }

            int pid = value.AsInt32;

            if (pid == 9000001)
            {
                results.Add(Error.InvalidInterAppReplyLogic(test, qAction, qAction, "Protocol", "SetParameter(9000001", qAction.Id.RawValue));
            }
        }


        private bool IsArgumentPropertyFromReturnAddress(ISymbol symbol)
        {
            foreach (var reference in symbol.DeclaringSyntaxReferences)
            {
                var syntax = reference.GetSyntax();
                if (syntax is VariableDeclaratorSyntax variableDeclarator)
                {
                    var initializer = variableDeclarator.Initializer;
                    if (initializer != null)
                    {
                        if (initializer.Value is MemberAccessExpressionSyntax memberAccess)
                        {
                            // at this point we found a declaration to a property called AgentId
                            if (memberAccess.Name.Identifier.Text.Equals("AgentId"))
                            {
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
            }

            return false;
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
                var symbolInfo = semanticModel.GetSymbolInfo(identifierName).Symbol;

                if (IsArgumentPropertyFromReturnAddress(symbolInfo))
                {
                    results.Add(Error.InvalidInterAppReplyLogic(test, qAction, qAction, "Message", "Send(ReturnAddress", qAction.Id.RawValue));
                    return;
                }
            }
            else if (expressionOfArgument is MemberAccessExpressionSyntax memberAccess)
            {
                // Someone directly passing along the AgentId property.
                // This is currently performed using Syntax Analysis. As symbol parsing requires valid compilation.

                if (callingMethod.Arguments[1].RawValue.EndsWith(".ReturnAddress.AgentId", StringComparison.InvariantCultureIgnoreCase))
                {
                    results.Add(Error.InvalidInterAppReplyLogic(test, qAction, qAction, "Message", "Send(ReturnAddress", qAction.Id.RawValue));
                    return;
                }
            }
        }

        private static bool IsInterAppMessage(CallingMethodClass callingMethod, SemanticModel semanticModel)
        {
            string fullyQualifiedNameOfParent = callingMethod.GetFullyQualifiedNameOfParent(semanticModel);

            // Seems to also return "Message" as a 'fullyQualifiedNameOfParent'
            return string.Equals(fullyQualifiedNameOfParent, "Message", StringComparison.InvariantCultureIgnoreCase) || string.Equals(fullyQualifiedNameOfParent, "Skyline.DataMiner.Core.InterAppCalls.Common.Message", StringComparison.InvariantCultureIgnoreCase);
        }

        public override void CheckCallingMethod(CallingMethodClass callingMethod)
        {
            Debug.WriteLine("TEMP: Calling Method Check");
            CheckForSetParameter(callingMethod);
            CheckForSendMessage(callingMethod);
        }
    }
}