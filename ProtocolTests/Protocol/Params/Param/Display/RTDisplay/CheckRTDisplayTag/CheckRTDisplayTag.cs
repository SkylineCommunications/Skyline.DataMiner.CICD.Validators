namespace ProtocolTests.Protocol.Params.Param.Display.RTDisplay.CheckRTDisplayTag
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.RTDisplay.CheckRTDisplayTag;

    using PageOrderError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Display.CheckPageOrderAttribute.Error;
    using PageVisibilityError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Display.Pages.Page.Visibility.CheckOverridePidAttribute.Error;
    using ParamAlarmingDisabledIfError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Alarm.Monitored.CheckDisabledIfAttribute.Error;
    using ParamAlarmingError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Alarm.Monitored.CheckMonitoredTag.Error;
    using ParamAlarmOptionsError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Alarm.CheckOptionsAttribute.Error;
    using ParamColumnOptionsError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.ColumnOption.CheckOptionsAttribute.Error;
    using ParamDependenciesError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Dependencies.Id.CheckIdTag.Error;
    using ParamDiscreetDependencyValuesError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Discreets.Discreet.CheckDependencyValuesAttribute.Error;
    using ParamDiscreetsDependencyIdError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Discreets.CheckDependencyId.Error;
    using ParamIdError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.CheckIdAttribute.Error;
    using ParamMeasTypeOptionsError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Type.CheckOptionsAttribute.Error;
    using ParamNameError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Name.CheckNameTag.Error;
    using ParamPositionsError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Positions.CheckPositionsTag.Error;
    using ParamsLoadSequenceError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.CheckLoadSequenceAttribute.Error;
    using ParamTrendingError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.CheckTrendingAttribute.Error;
    using ParamVirtualIdError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Type.CheckVirtualAttribute.Error;
    using ProtocolTypeOptionsError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Type.CheckOptionsAttribute.Error;
    using RelationsError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Relations.Relation.CheckPathAttribute.Error;
    using TrapMapAlarmError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.SNMP.TrapOID.CheckMapAlarmAttribute.Error;
    using TreeControlExtraDetailsColumnIdError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.ExtraDetails.LinkedDetails.CheckDiscreetColumnIdAttribute.Error;
    using TreeControlExtraDetailsTableIdError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.ExtraDetails.LinkedDetails.CheckDetailsTableIdAttribute.Error;
    using TreeControlExtraTabParameterError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.ExtraTab.Tab.CheckParameterAttribute.Error;
    using TreeControlHierarchyPathError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.Hierarchy.CheckPathAttribute.Error;
    using TreeControlHierarchyTableConditionError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.Hierarchy.Table.CheckConditionAttribute.Error;
    using TreeControlHierarchyTableIdError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.Hierarchy.Table.CheckIdAttribute.Error;
    using TreeControlParameterIdError = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.CheckParameterIdAttribute.Error;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckRTDisplayTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_DveTables()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_DveTables",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_Pages()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_Pages",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_PageVisibility()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_PageVisibility",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_ParamAlarming()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_ParamAlarming",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_ParamAlarmingDisabledIf()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_ParamAlarmingDisabledIf",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_ParamAlarmOptions()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_ParamAlarmOptions",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_ParamDependencies()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_ParamDependencies",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_ParamDiscreetDependencyValues()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_ParamDiscreetDependencyValues",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_ParamDiscreetsDependencyId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_ParamDiscreetsDependencyId",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_ParamPositions()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_ParamPositions",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_ParamsLoadSequence()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_ParamsLoadSequence",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_ParamTrending()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_ParamTrending",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_ParamVirtualSource()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_ParamVirtualSource",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_Relations()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_Relations",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_Spectrum()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_Spectrum",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_TableColumnOptions()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_TableColumnOptions",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_TableContextMenu()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_TableContextMenu",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_TableDisplayedColumns()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_TableDisplayedColumns",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_TableViewColumns()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_TableViewColumns",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_TrapMapAlarm()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_TrapMapAlarm",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_TreeControl()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_TreeControl",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_TreeControlExtraDetails()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_TreeControlExtraDetails",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_TreeControlExtraTabs()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_TreeControlExtraTabs",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_TreeControlHierarchyPath()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_TreeControlHierarchyPath",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_TreeControlHierarchyTable()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_TreeControlHierarchyTable",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_Valid_TreeControlParameterId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_TreeControlParameterId",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckRTDisplayTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "1"),
                    Error.EmptyTag(null, null, null, "2"),
                    Error.EmptyTag(null, null, null, "3"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "InvalidValue", "1"),
                    Error.InvalidValue(null, null, null, "  InvalidValue_Untrimmed   ", "2"),
                }
            };

            Generic.Validate(check, data);
        }


        #region RTDisplay Expected
        [TestMethod]
        [Ignore("todo when several (combine-able) types of RTDisplayExpected results are implemented.")]
        public void Param_CheckRTDisplayTag_RTDisplayExpected()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected",
                ExpectedResults = new List<IValidationResult>
                {
                    //Error.RTDisplayExpected(null, null, null, "101").WithSubResults(
                    //    TreeControlParameterIdError.ParamRTDisplayExpected(null, null, null, "101"))
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_DveTables()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_DveTables",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "1000").WithSubResults(
                        ProtocolTypeOptionsError.ReferencedParamExpectingRTDisplay(null, null, null, "1000")),

                    Error.RTDisplayExpected(null, null, null, "2000").WithSubResults(
                        ProtocolTypeOptionsError.ReferencedParamExpectingRTDisplay(null, null, null, "2000")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_Pages()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_Pages",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "100").WithSubResults(
                        PageOrderError.ReferencedParamRTDisplayExpected(null, null, null, "100")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_PageVisibility()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_PageVisibility",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "10").WithSubResults(
                        PageVisibilityError.ReferencedParamExpectingRTDisplay(null, null, null, "10", "RTDisplay_False")),

                    Error.RTDisplayExpected(null, null, null, "11").WithSubResults(
                        PageVisibilityError.ReferencedParamExpectingRTDisplay(null, null, null, "11", "NoRTDisplay")),

                    Error.RTDisplayExpected(null, null, null, "12").WithSubResults(
                        PageVisibilityError.ReferencedParamExpectingRTDisplay(null, null, null, "12", "NoDisplay")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_ParamAlarming()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_ParamAlarming",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "101").WithSubResults(
                        ParamAlarmingError.RTDisplayExpected(null, null, null, "101")),

                    Error.RTDisplayExpected(null, null, null, "102").WithSubResults(
                        ParamAlarmingError.RTDisplayExpected(null, null, null, "102")),

                    Error.RTDisplayExpected(null, null, null, "103").WithSubResults(
                        ParamAlarmingError.RTDisplayExpected(null, null, null, "103")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_ParamAlarmingDisabledIf()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_ParamAlarmingDisabledIf",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "1001").WithSubResults(
                        ParamAlarmingDisabledIfError.ReferencedParamRTDisplayExpected(null, null, null, "1001", "1")),

                    Error.RTDisplayExpected(null, null, null, "1002").WithSubResults(
                        ParamAlarmingDisabledIfError.ReferencedParamRTDisplayExpected(null, null, null, "1002", "2")),

                    Error.RTDisplayExpected(null, null, null, "1003").WithSubResults(
                        ParamAlarmingDisabledIfError.ReferencedParamRTDisplayExpected(null, null, null, "1003", "3")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_ParamAlarmOptions()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_ParamAlarmOptions",
                ExpectedResults = new List<IValidationResult>
                {
                    #region standalone
                    // Thresholds
                    Error.RTDisplayExpected(null, null, null, "1000").WithSubResults(
                        ParamAlarmOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "1000", "100")),

                    Error.RTDisplayExpected(null, null, null, "1001").WithSubResults(
                        ParamAlarmOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "1001", "100")),

                    // Properties
                    Error.RTDisplayExpected(null, null, null, "1002").WithSubResults(
                        ParamAlarmOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "1002", "100")),

                    Error.RTDisplayExpected(null, null, null, "1003").WithSubResults(
                        ParamAlarmOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "1003", "100")),
                    #endregion
                    
                    #region Tables
                    Error.RTDisplayExpected(null, null, null, "10000").WithSubResults(
                        ParamAlarmOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "10000", "502"),
                        ParamAlarmOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "10000", "502"),
                        ParamAlarmOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "10000", "502"),
                        ParamAlarmOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "10000", "502")),

                    // Thresholds
                    Error.RTDisplayExpected(null, null, null, "10001").WithSubResults(
                        ParamAlarmOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "10001", "502")),

                    Error.RTDisplayExpected(null, null, null, "10002").WithSubResults(
                        ParamAlarmOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "10002", "502")),

                    // Properties
                    Error.RTDisplayExpected(null, null, null, "10003").WithSubResults(
                        ParamAlarmOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "10003", "502")),

                    Error.RTDisplayExpected(null, null, null, "10004").WithSubResults(
                        ParamAlarmOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "10004", "502")),
                    #endregion
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_ParamDependencies()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_ParamDependencies",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "10").WithSubResults(
                        ParamDependenciesError.RTDisplayExpected(null, null, null, "10")),
                    Error.RTDisplayExpected(null, null, null, "11").WithSubResults(
                        ParamDependenciesError.RTDisplayExpectedOnReferencedParam(null, null, null, "11", "10")),
                    Error.RTDisplayExpected(null, null, null, "12").WithSubResults(
                        ParamDependenciesError.RTDisplayExpectedOnReferencedParam(null, null, null, "12", "10")),
                    Error.RTDisplayExpected(null, null, null, "13").WithSubResults(
                        ParamDependenciesError.RTDisplayExpectedOnReferencedParam(null, null, null, "13", "10")),

                    Error.RTDisplayExpected(null, null, null, "20").WithSubResults(
                        ParamDependenciesError.RTDisplayExpected(null, null, null, "20")),
                    Error.RTDisplayExpected(null, null, null, "21").WithSubResults(
                        ParamDependenciesError.RTDisplayExpectedOnReferencedParam(null, null, null, "21", "20")),
                    Error.RTDisplayExpected(null, null, null, "22").WithSubResults(
                        ParamDependenciesError.RTDisplayExpectedOnReferencedParam(null, null, null, "22", "20")),
                    Error.RTDisplayExpected(null, null, null, "23").WithSubResults(
                        ParamDependenciesError.RTDisplayExpectedOnReferencedParam(null, null, null, "23", "20")),

                    Error.RTDisplayExpected(null, null, null, "30").WithSubResults(
                        ParamDependenciesError.RTDisplayExpected(null, null, null, "30")),
                    Error.RTDisplayExpected(null, null, null, "31").WithSubResults(
                        ParamDependenciesError.RTDisplayExpectedOnReferencedParam(null, null, null, "31", "30")),
                    Error.RTDisplayExpected(null, null, null, "32").WithSubResults(
                        ParamDependenciesError.RTDisplayExpectedOnReferencedParam(null, null, null, "32", "30")),
                    Error.RTDisplayExpected(null, null, null, "33").WithSubResults(
                        ParamDependenciesError.RTDisplayExpectedOnReferencedParam(null, null, null, "33", "30")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_ParamDiscreetDependencyValues()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_ParamDiscreetDependencyValues",
                ExpectedResults = new List<IValidationResult>
                {
                    // Discreet 1
                    Error.RTDisplayExpected(null, null, null, "1002").WithSubResults(
                        ParamDiscreetDependencyValuesError.ReferencedParamExpectingRTDisplay(null, null, null, "1002", "999")),

                    Error.RTDisplayExpected(null, null, null, "1003").WithSubResults(
                        ParamDiscreetDependencyValuesError.ReferencedParamExpectingRTDisplay(null, null, null, "1003", "999")),

                    Error.RTDisplayExpected(null, null, null, "1004").WithSubResults(
                        ParamDiscreetDependencyValuesError.ReferencedParamExpectingRTDisplay(null, null, null, "1004", "999")),

                    Error.RTDisplayExpected(null, null, null, "1005").WithSubResults(
                        ParamDiscreetDependencyValuesError.ReferencedParamExpectingRTDisplay(null, null, null, "1005", "999")),

                    Error.RTDisplayExpected(null, null, null, "1006").WithSubResults(
                        ParamDiscreetDependencyValuesError.ReferencedParamExpectingRTDisplay(null, null, null, "1006", "999")),

                    Error.RTDisplayExpected(null, null, null, "1007").WithSubResults(
                        ParamDiscreetDependencyValuesError.ReferencedParamExpectingRTDisplay(null, null, null, "1007", "999")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_ParamDiscreetsDependencyId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_ParamDiscreetsDependencyId",
                ExpectedResults = new List<IValidationResult>
                {
                    // Read Param
                    Error.RTDisplayExpected(null, null, null, "100").WithSubResults(
                        ParamDiscreetsDependencyIdError.ReferencedParamRTDisplayExpected(null, null, null, "100", "1000")),

                    Error.RTDisplayExpected(null, null, null, "101").WithSubResults(
                        ParamDiscreetsDependencyIdError.ReferencedParamRTDisplayExpected(null, null, null, "101", "1001")),

                    Error.RTDisplayExpected(null, null, null, "102").WithSubResults(
                        ParamDiscreetsDependencyIdError.ReferencedParamRTDisplayExpected(null, null, null, "102", "1002")),

                    // ReadBit param
                    Error.RTDisplayExpected(null, null, null, "200").WithSubResults(
                        ParamDiscreetsDependencyIdError.ReferencedParamRTDisplayExpected(null, null, null, "200", "2000")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_ParamPositions()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_ParamPositions",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "1").WithSubResults(
                        ParamPositionsError.RTDisplayExpected(null, null, null, "1")),

                    Error.RTDisplayExpected(null, null, null, "2").WithSubResults(
                        ParamPositionsError.RTDisplayExpected(null, null, null, "2")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_ParamsLoadSequence()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_ParamsLoadSequence",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "1").WithSubResults(
                        ParamsLoadSequenceError.ReferencedParamRTDisplayExpected(null, null, null, "1")),

                    Error.RTDisplayExpected(null, null, null, "2").WithSubResults(
                        ParamsLoadSequenceError.ReferencedParamRTDisplayExpected(null, null, null, "2")),

                    Error.RTDisplayExpected(null, null, null, "3").WithSubResults(
                        ParamsLoadSequenceError.ReferencedParamRTDisplayExpected(null, null, null, "3")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_ParamTrending()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_ParamTrending",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "100").WithSubResults(
                        ParamTrendingError.RTDisplayExpected(null, null, null, "100")),

                    Error.RTDisplayExpected(null, null, null, "101").WithSubResults(
                        ParamTrendingError.RTDisplayExpected(null, null, null, "101")),

                    Error.RTDisplayExpected(null, null, null, "102").WithSubResults(
                        ParamTrendingError.RTDisplayExpected(null, null, null, "102")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_ParamVirtualSource()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_ParamVirtualSource",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "100").WithSubResults(
                        ParamVirtualIdError.RTDisplayExpected(null, null, null, "100")),

                    Error.RTDisplayExpected(null, null, null, "110").WithSubResults(
                        ParamVirtualIdError.RTDisplayExpected(null, null, null, "110")),

                    Error.RTDisplayExpected(null, null, null, "120").WithSubResults(
                        ParamVirtualIdError.RTDisplayExpected(null, null, null, "120")),

                    Error.RTDisplayExpected(null, null, null, "130").WithSubResults(
                        ParamVirtualIdError.RTDisplayExpected(null, null, null, "130")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_Relations()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_Relations",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "1000").WithSubResults(
                        RelationsError.ReferencedParamExpectingRTDisplay(null, null, null, "1000"),
                        RelationsError.ReferencedParamExpectingRTDisplay(null, null, null, "1000")),

                    Error.RTDisplayExpected(null, null, null, "2000").WithSubResults(
                        RelationsError.ReferencedParamExpectingRTDisplay(null, null, null, "2000"),
                        RelationsError.ReferencedParamExpectingRTDisplay(null, null, null, "2000")),

                    Error.RTDisplayExpected(null, null, null, "3000").WithSubResults(
                        RelationsError.ReferencedParamExpectingRTDisplay(null, null, null, "3000")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_Spectrum()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_Spectrum",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "64000").WithSubResults(
                        ParamIdError.RTDisplayExpectedOnSpectrumParam(null, null, null, "64000")),

                    Error.RTDisplayExpected(null, null, null, "64001").WithSubResults(
                        ParamIdError.RTDisplayExpectedOnSpectrumParam(null, null, null, "64001")),

                    Error.RTDisplayExpected(null, null, null, "64002").WithSubResults(
                        ParamIdError.RTDisplayExpectedOnSpectrumParam(null, null, null, "64002")),


                    Error.RTDisplayExpected(null, null, null, "64299").WithSubResults(
                        ParamIdError.RTDisplayExpectedOnSpectrumParam(null, null, null, "64299")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_TableColumnOptions()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_TableColumnOptions",
                ExpectedResults = new List<IValidationResult>
                {
                    // Displayed Table
                    Error.RTDisplayExpected(null, null, null, "1003").WithSubResults(
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "1003", "enableHeaderAvg", "1000"),
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "1003", "enableHeatmap", "1000")),
                    Error.RTDisplayExpected(null, null, null, "1053").WithSubResults(
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "1053", "enableHeaderAvg", "1000"),
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "1053", "enableHeatmap", "1000")),

                    Error.RTDisplayExpected(null, null, null, "1004").WithSubResults(
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "1004", "rowTextColoring", "1000"),
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "1004", "selectionSetVar", "1000")),
                    Error.RTDisplayExpected(null, null, null, "1054").WithSubResults(
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "1054", "rowTextColoring", "1000"),
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "1054", "selectionSetVar", "1000")),

                    Error.RTDisplayExpected(null, null, null, "1005").WithSubResults(
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "1005", "xpos", "1000")),
                    Error.RTDisplayExpected(null, null, null, "1055").WithSubResults(
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "1055", "xpos", "1000")),
                    
                    // Exported Table (no errors since all columns have export="true" which is validated by another check)
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_TableContextMenu()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_TableContextMenu",
                ExpectedResults = new List<IValidationResult>
                {
                    // QAction Feedback
                    Error.RTDisplayExpected(null, null, null, "998").WithSubResults(
                        ParamNameError.RTDisplayExpectedOnQActionFeedback(null, null, null, "998")),
                    
                    // Context Menu
                    Error.RTDisplayExpected(null, null, null, "999").WithSubResults(
                        ParamNameError.RTDisplayExpectedOnContextMenu(null, null, null, "999")),
}
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_TableDisplayedColumns()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_TableDisplayedColumns",
                ExpectedResults = new List<IValidationResult>
                {
                    // Displayed Table
                    Error.RTDisplayExpected(null, null, null, "1003").WithSubResults(
                        ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "1003", "1000")),
                    Error.RTDisplayExpected(null, null, null, "1053").WithSubResults(
                        ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "1053", "1000")),

                    Error.RTDisplayExpected(null, null, null, "1004").WithSubResults(
                        ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "1004", "1000")),
                    Error.RTDisplayExpected(null, null, null, "1054").WithSubResults(
                        ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "1054", "1000")),

                    Error.RTDisplayExpected(null, null, null, "1005").WithSubResults(
                        ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "1005", "1000")),
                    Error.RTDisplayExpected(null, null, null, "1055").WithSubResults(
                        ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "1055", "1000")),

                    // Exported Table (no errors since all columns have export="true" which is validated by another check)
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_TableViewColumns()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_TableViewColumns",
                ExpectedResults = new List<IValidationResult>
                {
                    // Normal Table
                    Error.RTDisplayExpected(null, null, null, "1001").WithSubResults(
                        ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "1001", "1000"),
                        ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "2001", "2000"),
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "2001", "view=1001", "2000")),

                    Error.RTDisplayExpected(null, null, null, "1002").WithSubResults(
                        ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "1002", "1000"),
                        ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "2002", "2000"),
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "2002", "view=1002", "2000")),
                    Error.RTDisplayExpected(null, null, null, "1052").WithSubResults(
                        ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "1052", "1000"),
                        ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "1052", "2000"),
                        // Ideally, below one should replace above one.
                        //ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "2052", "2000"),
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "1052", "view=1002", "2000")),

                    Error.RTDisplayExpected(null, null, null, "1003").WithSubResults(
                        ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "1003", "1000"),
                        ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "2003", "2000"),
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "2003", "view=1003", "2000")),
                    Error.RTDisplayExpected(null, null, null, "1053").WithSubResults(
                        ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "1053", "1000"),
                        ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "1053", "2000"),
                        // Ideally, below one should replace above one.
                        //ParamMeasTypeOptionsError.ReferencedParamRTDisplayExpected(null, null, null, "2053", "2000"),
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "1053", "view=1003", "2000")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_TrapMapAlarm()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_TrapMapAlarm",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "100").WithSubResults(
                        TrapMapAlarmError.RTDisplayExpected(null, null, null, "100")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_TreeControlExtraDetails()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_TreeControlExtraDetails",
                ExpectedResults = new List<IValidationResult>
                {
                    // detailsTableId
                    Error.RTDisplayExpected(null, null, null, "1100").WithSubResults(
                        TreeControlExtraDetailsTableIdError.ReferencedTableExpectingRTDisplay(null, null, null, "1100")),

                    Error.RTDisplayExpected(null, null, null, "2100").WithSubResults(
                        TreeControlExtraDetailsTableIdError.ReferencedTableExpectingRTDisplay(null, null, null, "2100")),

                    // discreetColumnId
                    Error.RTDisplayExpected(null, null, null, "1002").WithSubResults(
                        TreeControlExtraDetailsColumnIdError.ReferencedColumnExpectingRTDisplay(null, null, null, "1002")),

                    Error.RTDisplayExpected(null, null, null, "2002").WithSubResults(
                        TreeControlExtraDetailsColumnIdError.ReferencedColumnExpectingRTDisplay(null, null, null, "2002"),
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "2002", "foreignKey=1000", "2000")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_TreeControlExtraTabs()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_TreeControlExtraTabs",
                ExpectedResults = new List<IValidationResult>
                {
                    // Tabs on table 1000 level
                    Error.RTDisplayExpected(null, null, null, "1000").WithSubResults(
                        TreeControlHierarchyPathError.ReferencedParamExpectingRTDisplay(null, null, null, "1000")/*,
                        TreeControlExtraTabTableIdError.ReferencedParamExpectingRTDisplay(null, null, null, "1000"),
                        TreeControlExtraTabTableIdError.ReferencedParamExpectingRTDisplay(null, null, null, "1000"),
                        TreeControlExtraTabTableIdError.ReferencedParamExpectingRTDisplay(null, null, null, "1000"),
                        TreeControlExtraTabTableIdError.ReferencedParamExpectingRTDisplay(null, null, null, "1000")*/),

                    Error.RTDisplayExpected(null, null, null, "1102").WithSubResults(
                        TreeControlExtraTabParameterError.ReferencedParamExpectingRTDisplay(null, null, null, "1102"),
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "1102", "foreignKey=1000", "1100")),
                    Error.RTDisplayExpected(null, null, null, "1100").WithSubResults(
                        TreeControlExtraTabParameterError.ReferencedParamExpectingRTDisplay(null, null, null, "1100")),

                    Error.RTDisplayExpected(null, null, null, "2").WithSubResults(
                        TreeControlExtraTabParameterError.ReferencedParamExpectingRTDisplay(null, null, null, "2")),

                    Error.RTDisplayExpected(null, null, null, "4000").WithSubResults(
                        TreeControlExtraTabParameterError.ReferencedParamExpectingRTDisplay(null, null, null, "4000")),


                    // Tabs on table 2000 level
                    Error.RTDisplayExpected(null, null, null, "2000").WithSubResults(
                        TreeControlHierarchyPathError.ReferencedParamExpectingRTDisplay(null, null, null, "2000")/*,
                        TreeControlExtraTabTableIdError.ReferencedParamExpectingRTDisplay(null, null, null, "2000"),
                        TreeControlExtraTabTableIdError.ReferencedParamExpectingRTDisplay(null, null, null, "2000"),
                        TreeControlExtraTabTableIdError.ReferencedParamExpectingRTDisplay(null, null, null, "2000")*/),

                    Error.RTDisplayExpected(null, null, null, "2102").WithSubResults(
                        TreeControlExtraTabParameterError.ReferencedParamExpectingRTDisplay(null, null, null, "2102"),
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "2102", "foreignKey=2000", "2100")),
                    Error.RTDisplayExpected(null, null, null, "2100").WithSubResults(
                        TreeControlExtraTabParameterError.ReferencedParamExpectingRTDisplay(null, null, null, "2100")),

                    Error.RTDisplayExpected(null, null, null, "3").WithSubResults(
                        TreeControlExtraTabParameterError.ReferencedParamExpectingRTDisplay(null, null, null, "3")),

                    
                    // Tabs on table 3000 level
                    Error.RTDisplayExpected(null, null, null, "3000").WithSubResults(
                        TreeControlHierarchyPathError.ReferencedParamExpectingRTDisplay(null, null, null, "3000")/*,
                        TreeControlExtraTabTableIdError.ReferencedParamExpectingRTDisplay(null, null, null, "3000"),
                        TreeControlExtraTabTableIdError.ReferencedParamExpectingRTDisplay(null, null, null, "3000"),
                        TreeControlExtraTabTableIdError.ReferencedParamExpectingRTDisplay(null, null, null, "3000")*/),

                    Error.RTDisplayExpected(null, null, null, "3102").WithSubResults(
                        TreeControlExtraTabParameterError.ReferencedParamExpectingRTDisplay(null, null, null, "3102"),
                        ParamColumnOptionsError.ColumnOptionExpectingRTDisplay(null, null, null, "3102", "foreignKey=3000", "3100")),
                    Error.RTDisplayExpected(null, null, null, "3100").WithSubResults(
                        TreeControlExtraTabParameterError.ReferencedParamExpectingRTDisplay(null, null, null, "3100")),

                    Error.RTDisplayExpected(null, null, null, "4").WithSubResults(
                        TreeControlExtraTabParameterError.ReferencedParamExpectingRTDisplay(null, null, null, "4")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_TreeControlHierarchyPath()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_TreeControlHierarchyPath",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "1000").WithSubResults(
                        TreeControlHierarchyPathError.ReferencedParamExpectingRTDisplay(null, null, null, "1000")),

                    Error.RTDisplayExpected(null, null, null, "2000").WithSubResults(
                        TreeControlHierarchyPathError.ReferencedParamExpectingRTDisplay(null, null, null, "2000")),

                    Error.RTDisplayExpected(null, null, null, "3000").WithSubResults(
                        TreeControlHierarchyPathError.ReferencedParamExpectingRTDisplay(null, null, null, "3000")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_TreeControlHierarchyTableCondition()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_TreeControlHierarchyTableCondition",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "1002").WithSubResults(
                        TreeControlHierarchyTableConditionError.ReferencedColumnExpectingRTDisplay(null, null, null, "1002", "101"),
                        TreeControlHierarchyTableConditionError.ReferencedColumnExpectingRTDisplay(null, null, null, "1002", "101")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_TreeControlHierarchyTableId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_TreeControlHierarchyTableId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "1000").WithSubResults(
                        TreeControlHierarchyTableIdError.ReferencedParamExpectingRTDisplay(null, null, null, "1000")),

                    Error.RTDisplayExpected(null, null, null, "2000").WithSubResults(
                        TreeControlHierarchyTableIdError.ReferencedParamExpectingRTDisplay(null, null, null, "2000")),

                    Error.RTDisplayExpected(null, null, null, "3000").WithSubResults(
                        TreeControlHierarchyTableIdError.ReferencedParamExpectingRTDisplay(null, null, null, "3000")),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected_TreeControlParameterId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected_TreeControlParameterId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "101").WithSubResults(
                        TreeControlParameterIdError.ReferencedParamExpectingRTDisplay(null, null, null, "101")),

                    Error.RTDisplayExpected(null, null, null, "102").WithSubResults(
                        TreeControlParameterIdError.ReferencedParamExpectingRTDisplay(null, null, null, "102")),

                    Error.RTDisplayExpected(null, null, null, "103").WithSubResults(
                        TreeControlParameterIdError.ReferencedParamExpectingRTDisplay(null, null, null, "103")),
                }
            };

            Generic.FullValidate(check, data);
        }
        #endregion


        #region RTDisplay Unexpected

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayUnexpected()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayUnexpected",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayUnexpected(null, null, null, "1000"),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayUnexpected_TableColumns()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayUnexpected_TableColumns",
                ExpectedResults = new List<IValidationResult>
                {
                    // RTDisplay true on table
                    Error.RTDisplayUnexpected(null, null, null, "1001"),
                    Error.RTDisplayUnexpected(null, null, null, "1002"),
                    Error.RTDisplayUnexpected(null, null, null, "1052"),
                    
                    // RTDisplay false on table
                    Error.RTDisplayUnexpected(null, null, null, "2001"),
                    Error.RTDisplayUnexpected(null, null, null, "2002"),
                    Error.RTDisplayUnexpected(null, null, null, "2052"),
                }
            };

            Generic.FullValidate(check, data);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayUnexpected_TrapMapAlarm()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayUnexpected_TrapMapAlarm",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayUnexpected(null, null, null, "200"),
                }
            };

            Generic.FullValidate(check, data);
        }

        #endregion


        [TestMethod]
        public void Param_CheckRTDisplayTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    // RTDisplay not needed
                    Error.UntrimmedTag(null, null, null, "1000", " false "),

                    // RTDisplay with onAppLevel
                    Error.UntrimmedTag(null, null, null, "2000", " true "),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckRTDisplayTag();

        [TestMethod]
        public void Param_CheckRTDisplayTag_UntrimmedTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedTag",
            };

            Generic.Fix(check, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Param_CheckRTDisplayTag_EmptyTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyTag(null, null, null, "1");

            var expected = new ValidationResult()
            {
                ErrorId = 1,
                FullId = "2.7.1",
                Category = Category.Param,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                Description = "Empty tag 'RTDisplay' in Param '1'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "invalidValue", "100");

            var expected = new ValidationResult()
            {
                ErrorId = 3,
                FullId = "2.7.3",
                Category = Category.Param,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                Description = "Invalid value 'invalidValue' in tag 'RTDisplay'. Possible values 'true, false'. Param ID '100'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayExpected()
        {
            // Create ErrorMessage
            var message = Error.RTDisplayExpected(null, null, null, "100");

            var expected = new ValidationResult()
            {
                ErrorId = 4,
                FullId = "2.7.4",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                Description = "RTDisplay(true) expected on Param '100'.",
                HowToFix = "Double check the subresults to evaluate if the features requiring RTDisplay are to be removed or if RTDisplay actually has to be set to true.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_RTDisplayUnexpected()
        {
            // Create ErrorMessage
            var message = Error.RTDisplayUnexpected(null, null, null, "100");

            var expected = new ValidationResult()
            {
                ErrorId = 5,
                FullId = "2.7.5",
                Category = Category.Param,
                Severity = Severity.Minor,
                Certainty = Certainty.Uncertain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                Description = "Unexpected RTDisplay(true) on Param '100'.",
                HowToFix = "Double check if this Param requires RTDisplay for reasons that are outside the scope of this driver (Visios, automation scripts, etc)." + Environment.NewLine + "- If so, suppress this result and explain why RTDisplay is required via the suppression comment." + Environment.NewLine + "- If not, remove the full Display tag containing this RTDisplay tag.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckRTDisplayTag_UntrimmedTag()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedTag(null, null, null, "1", " true ");

            var expected = new ValidationResult()
            {
                ErrorId = 2,
                FullId = "2.7.2",
                Category = Category.Param,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                Description = "Untrimmed tag 'RTDisplay' in Param '1'. Current value ' true '.",
                HowToFix = "",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckRTDisplayTag();

        [TestMethod]
        public void Param_CheckRTDisplayTag_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckRTDisplayTag_CheckId() => Generic.CheckId(check, CheckId.CheckRTDisplayTag);
    }
}