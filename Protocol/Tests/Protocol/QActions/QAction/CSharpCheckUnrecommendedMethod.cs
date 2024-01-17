namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpCheckUnrecommendedMethod
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

    [Test(CheckId.CSharpCheckUnrecommendedMethod, Category.QAction)]
    internal class CSharpCheckUnrecommendedMethod : IValidate, ICodeFix
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

        public ICodeFixResult Fix(CodeFixContext context)
        {
            throw new NotImplementedException();
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

        public override void CheckCallingMethod(CallingMethodClass callingMethod)
        {
            CheckThreadAbort(callingMethod);

            CheckSetParameterIndex(callingMethod);
            CheckSetParametersIndex(callingMethod);
            CheckGetParameterIndex(callingMethod);

            CheckGetRemoteTrend(callingMethod);
            CheckGetRemoteTrendAvg(callingMethod);

            CheckNtArrayRowCount(callingMethod);
            CheckNtTrigger(callingMethod);

            CheckNtGetData(callingMethod);
            CheckNtGetDescription(callingMethod);
            CheckNtGetItemData(callingMethod);
            CheckNtkGetKeyPosition(callingMethod);

            CheckNtGetParameter(callingMethod);
            CheckNtGetParameterByData(callingMethod);
            CheckNtGetParameterByName(callingMethod);
            CheckNtGetParameterIndex(callingMethod);

            CheckNtGetRow(callingMethod);

            CheckNtNotifyDisplay(callingMethod);

            CheckNtSetDescription(callingMethod);
            CheckNtSetItemData(callingMethod);

            CheckNtSetParameterByData(callingMethod);
            CheckNtSetParameterByName(callingMethod);
            CheckNtSetParameterWithHistory(callingMethod);

            CheckNtSetRow(callingMethod);

            CheckNtAddRow(callingMethod);
            CheckNtDeleteRow(callingMethod);
        }

        private void CheckThreadAbort(CallingMethodClass callingMethod)
        {
            string fqn = callingMethod.GetFullyQualifiedNameOfParent(semanticModel);

            if (!String.Equals(fqn, "System.Threading.Thread") || !String.Equals(callingMethod.Name, "Abort"))
            {
                return;
            }

            if (callingMethod.Arguments.Count == 0)
            {
                results.Add(Error.UnrecommendedThreadAbort(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
            }
        }

        private void CheckSetParameterIndex(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsSLProtocol(semanticModel) || !String.Equals(callingMethod.Name, "SetParameterIndex"))
            {
                return;
            }

            if (CheckIfNotMatrix(callingMethod.Arguments[0]))
            {
                results.Add(Error.UnrecommendedSlProtocolSetParameterIndex(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
            }
        }

        private void CheckSetParametersIndex(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsSLProtocol(semanticModel) || !String.Equals(callingMethod.Name, "SetParametersIndex"))
            {
                return;
            }

            if (CheckIfNotMatrices(callingMethod.Arguments[0]))
            {
                results.Add(Error.UnrecommendedSlProtocolSetParametersIndex(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
            }
        }

        private void CheckGetParameterIndex(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsSLProtocol(semanticModel) || !String.Equals(callingMethod.Name, "GetParameterIndex"))
            {
                return;
            }

            if (CheckIfNotMatrix(callingMethod.Arguments[0]))
            {
                results.Add(Error.UnrecommendedSlProtocolGetParameterIndex(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
            }
        }

        private void CheckGetRemoteTrend(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyDataMiner(semanticModel, solution, 216))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyDataMinerNTGetRemoteTrend(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckGetRemoteTrendAvg(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyDataMiner(semanticModel, solution, 260))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyDataMinerNTGetRemoteTrendAvg(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtArrayRowCount(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 195))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_ARRAY_ROW_COUNT(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtTrigger(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 134))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_CHECK_TRIGGER(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtGetData(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 60))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_GET_DATA(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtGetDescription(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 77))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_GET_DESCRIPTION(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtGetItemData(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 88))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_GET_ITEM_DATA(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtkGetKeyPosition(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 163))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_GET_KEY_POSITION(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtGetParameter(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 73))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_GET_PARAMETER(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtGetParameterByData(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 87))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_GET_PARAMETER_BY_DATA(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtGetParameterByName(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 85))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_GET_PARAMETER_BY_NAME(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtGetParameterIndex(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 122))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_GET_PARAMETER_INDEX(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtGetRow(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 215))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_GET_ROW(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtNotifyDisplay(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 123))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_NOTIFY_DISPLAY(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtSetDescription(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 131))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_SET_DESCRIPTION(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtSetItemData(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 89))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_SET_ITEM_DATA(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtSetParameterByData(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 86))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_SET_PARAMETER_BY_DATA(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtSetParameterByName(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 84))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_SET_PARAMETER_BY_NAME(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtSetParameterWithHistory(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 256))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_SET_PARAMETER_WITH_HISTORY(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtSetRow(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 225))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNT_SET_ROW(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtAddRow(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 149))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNTAddRow(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private void CheckNtDeleteRow(CallingMethodClass callingMethod)
        {
            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 156))
            {
                return;
            }

            results.Add(Error.UnrecommendedNotifyProtocolNTDeleteRow(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
        }

        private bool CheckIfNotMatrix(Argument argument)
        {
            if (!argument.TryParseToValue(semanticModel, solution, out Value value))
            {
                // Was unable to parse the argument
                return false;
            }

            if (!value.IsNumeric() || !value.HasStaticValue)
            {
                // Couldn't detect a number or value could have changed
                return false;
            }

            if (!protocolModel.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, Convert.ToString(value.Object), out var param))
            {
                // Non existing parameter
                return false;
            }

            if (param.IsMatrix())
            {
                // Exclude matrices
                return false;
            }

            return true;
        }

        private bool CheckIfNotMatrices(Argument argument)
        {
            if (!argument.TryParseToValue(semanticModel, solution, out Value value) || value.Type != Value.ValueType.Array || !value.HasStaticValue)
            {
                // Argument couldn't be parsed OR argument isn't an array.
                return false;
            }

            foreach (Value subValue in value.Array)
            {
                if (subValue == null || !subValue.IsNumeric() || !value.HasStaticValue)
                {
                    continue;
                }

                if (!protocolModel.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, Convert.ToString(subValue.Object), out var param))
                {
                    // Non existing parameter
                    continue;
                }

                if (param.IsMatrix())
                {
                    // Exclude matrices
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}