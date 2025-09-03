namespace ProtocolTests.Protocol.QActions.QAction.CSharpCheckUnrecommendedMethod
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpCheckUnrecommendedMethod;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpCheckUnrecommendedMethod();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyDataMinerNTGetRemoteTrend()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyDataMinerNTGetRemoteTrend",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyDataMinerNTGetRemoteTrend(null, null, null, "101"),
                    Error.UnrecommendedNotifyDataMinerNTGetRemoteTrend(null, null, null, "101"),
                    Error.UnrecommendedNotifyDataMinerNTGetRemoteTrend(null, null, null, "101"),
                    Error.UnrecommendedNotifyDataMinerNTGetRemoteTrend(null, null, null, "101"),
                    Error.UnrecommendedNotifyDataMinerNTGetRemoteTrend(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyDataMinerNTGetRemoteTrendAvg()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyDataMinerNTGetRemoteTrendAvg",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyDataMinerNTGetRemoteTrendAvg(null, null, null, "102"),
                    Error.UnrecommendedNotifyDataMinerNTGetRemoteTrendAvg(null, null, null, "102"),
                    Error.UnrecommendedNotifyDataMinerNTGetRemoteTrendAvg(null, null, null, "102"),
                    Error.UnrecommendedNotifyDataMinerNTGetRemoteTrendAvg(null, null, null, "102"),
                    Error.UnrecommendedNotifyDataMinerNTGetRemoteTrendAvg(null, null, null, "102"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_ARRAY_ROW_COUNT()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_ARRAY_ROW_COUNT",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_ARRAY_ROW_COUNT(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_ARRAY_ROW_COUNT(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_CHECK_TRIGGER()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_CHECK_TRIGGER",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_CHECK_TRIGGER(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_CHECK_TRIGGER(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_GET_DATA()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_GET_DATA",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_GET_DATA(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_GET_DATA(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_GET_DESCRIPTION()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_GET_DESCRIPTION",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_GET_DESCRIPTION(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_GET_DESCRIPTION(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_GET_ITEM_DATA()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_GET_ITEM_DATA",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_GET_ITEM_DATA(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_GET_ITEM_DATA(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_GET_KEY_POSITION()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_GET_KEY_POSITION",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_GET_KEY_POSITION(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_GET_KEY_POSITION(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_GET_PARAMETER()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_GET_PARAMETER",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_GET_PARAMETER(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_GET_PARAMETER(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_GET_PARAMETER_BY_DATA()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_GET_PARAMETER_BY_DATA",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_GET_PARAMETER_BY_DATA(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_GET_PARAMETER_BY_DATA(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_GET_PARAMETER_BY_NAME()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_GET_PARAMETER_BY_NAME",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_GET_PARAMETER_BY_NAME(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_GET_PARAMETER_BY_NAME(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_GET_PARAMETER_INDEX()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_GET_PARAMETER_INDEX",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_GET_PARAMETER_INDEX(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_GET_PARAMETER_INDEX(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_GET_ROW()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_GET_ROW",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_GET_ROW(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_GET_ROW(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_NOTIFY_DISPLAY()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_NOTIFY_DISPLAY",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_NOTIFY_DISPLAY(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_NOTIFY_DISPLAY(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_SET_DESCRIPTION()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_SET_DESCRIPTION",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_SET_DESCRIPTION(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_SET_DESCRIPTION(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_SET_ITEM_DATA()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_SET_ITEM_DATA",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_SET_ITEM_DATA(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_SET_ITEM_DATA(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_SET_PARAMETER_BY_DATA()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_SET_PARAMETER_BY_DATA",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_SET_PARAMETER_BY_DATA(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_SET_PARAMETER_BY_DATA(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_SET_PARAMETER_BY_NAME()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_SET_PARAMETER_BY_NAME",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_SET_PARAMETER_BY_NAME(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_SET_PARAMETER_BY_NAME(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_SET_PARAMETER_WITH_HISTORY()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_SET_PARAMETER_WITH_HISTORY",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_SET_PARAMETER_WITH_HISTORY(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_SET_PARAMETER_WITH_HISTORY(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_SET_ROW()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNT_SET_ROW",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNT_SET_ROW(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNT_SET_ROW(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNTAddRow()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNTAddRow",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNTAddRow(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNTAddRow(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNTDeleteRow()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNotifyProtocolNTDeleteRow",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNotifyProtocolNTDeleteRow(null, null, null, "101"),
                    Error.UnrecommendedNotifyProtocolNTDeleteRow(null, null, null, "101"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedSlProtocolGetParameterIndex()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedSlProtocolGetParameterIndex",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedSlProtocolGetParameterIndex(null, null, null, "103"),
                    Error.UnrecommendedSlProtocolGetParameterIndex(null, null, null, "103"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedSlProtocolSetParameterIndex()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedSlProtocolSetParameterIndex",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedSlProtocolSetParameterIndex(null, null, null, "104"),
                    Error.UnrecommendedSlProtocolSetParameterIndex(null, null, null, "104"),
                    Error.UnrecommendedSlProtocolSetParameterIndex(null, null, null, "104"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedSlProtocolSetParametersIndex()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedSlProtocolSetParametersIndex",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedSlProtocolSetParametersIndex(null, null, null, "105"),
                    Error.UnrecommendedSlProtocolSetParametersIndex(null, null, null, "105"),

                    Error.UnrecommendedSlProtocolSetParametersIndex(null, null, null, "105"),
                    Error.UnrecommendedSlProtocolSetParametersIndex(null, null, null, "105"),

                    Error.UnrecommendedSlProtocolSetParametersIndex(null, null, null, "105"),

                    Error.UnrecommendedSlProtocolSetParametersIndex(null, null, null, "105"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedThreadAbort()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedThreadAbort",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedThreadAbort(null, null, null, "106"),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CSharpCheckUnrecommendedMethod();

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_ARRAY_ROW_COUNT()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNT_ARRAY_ROW_COUNT",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_CHECK_TRIGGER()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNT_CHECK_TRIGGER",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_GET_DATA()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNT_GET_DATA",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_GET_DESCRIPTION()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNT_GET_DESCRIPTION",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_GET_ITEM_DATA()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNT_GET_ITEM_DATA",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_GET_PARAMETER()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNT_GET_PARAMETER",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_GET_PARAMETER_BY_DATA()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNT_GET_PARAMETER_BY_DATA",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_GET_PARAMETER_BY_NAME()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNT_GET_PARAMETER_BY_NAME",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_GET_ROW()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNT_GET_ROW",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_NOTIFY_DISPLAY()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNT_NOTIFY_DISPLAY",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_SET_DESCRIPTION()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNT_SET_DESCRIPTION",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_SET_ITEM_DATA()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNT_SET_ITEM_DATA",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_SET_PARAMETER_BY_DATA()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNT_SET_PARAMETER_BY_DATA",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_SET_PARAMETER_BY_NAME()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNT_SET_PARAMETER_BY_NAME",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_SET_PARAMETER_WITH_HISTORY()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNT_SET_PARAMETER_WITH_HISTORY",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNT_SET_ROW()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNT_SET_ROW",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNTAddRow()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNTAddRow",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyProtocolNTDeleteRow()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNotifyProtocolNTDeleteRow",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyDataMinerNTGetRemoteTrend()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedNotifyDataMinerNTGetRemoteTrend(null, null, null, "1");

            var expected = new ValidationResult()
            {
                ErrorId = 5,
                FullId = "3.15.5",
                Category = Category.QAction,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Method 'SLProtocol.NotifyDataMiner(216/*NT_GET_REMOTE_TREND*/, ...)' is unrecommended. QAction ID '1'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedNotifyDataMinerNTGetRemoteTrendAvg()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedNotifyDataMinerNTGetRemoteTrendAvg(null, null, null, "1");

            var expected = new ValidationResult()
            {
                ErrorId = 6,
                FullId = "3.15.6",
                Category = Category.QAction,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Method 'SLProtocol.NotifyDataMiner(260/*NT_GET_REMOTE_TREND_AVG*/, ...)' is unrecommended. QAction ID '1'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedSlProtocolGetParameterIndex()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedSlProtocolGetParameterIndex(null, null, null, "1");

            var expected = new ValidationResult()
            {
                ErrorId = 2,
                FullId = "3.15.2",
                Category = Category.QAction,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Method 'SLProtocol.GetParameterIndex' is unrecommended. QAction ID '1'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedSlProtocolSetParameterIndex()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedSlProtocolSetParameterIndex(null, null, null, "1");

            var expected = new ValidationResult()
            {
                ErrorId = 3,
                FullId = "3.15.3",
                Category = Category.QAction,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Method 'SLProtocol.SetParameterIndex' is unrecommended. QAction ID '1'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedSlProtocolSetParametersIndex()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedSlProtocolSetParametersIndex(null, null, null, "1");

            var expected = new ValidationResult()
            {
                ErrorId = 4,
                FullId = "3.15.4",
                Category = Category.QAction,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Method 'SLProtocol.SetParametersIndex' is unrecommended. QAction ID '1'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_UnrecommendedThreadAbort()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedThreadAbort(null, null, null, "1");

            var expected = new ValidationResult()
            {
                ErrorId = 1,
                FullId = "3.15.1",
                Category = Category.QAction,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Method 'System.Threading.Thread.Abort' is unrecommended. QAction ID '1'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CSharpCheckUnrecommendedMethod();

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedMethod_CheckId() => Generic.CheckId(check, CheckId.CSharpCheckUnrecommendedMethod);
    }
}