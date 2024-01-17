namespace ProtocolTests.Protocol.Params.Param.Alarm.CheckTypeAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Alarm.CheckTypeAttribute;

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckTypeAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckTypeAttribute_Valid()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckTypeAttribute_RemovedNormalizationAlarmType()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "RemovedNormalizationAlarmType",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.RemovedNormalizationAlarmType(null, null, "absolute:2214,100", "1"),
                    ErrorCompare.RemovedNormalizationAlarmType(null, null, "absolute:2214", "2"),
                    ErrorCompare.RemovedNormalizationAlarmType(null, null, "relative:2214,100", "3"),
                    ErrorCompare.RemovedNormalizationAlarmType(null, null, "relative:2214", "4"),
                    ErrorCompare.RemovedNormalizationAlarmType(null, null, "relative", "5"),
                    ErrorCompare.RemovedNormalizationAlarmType(null, null, "absolute", "6"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckTypeAttribute_UpdatedNormalizationAlarmType()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "UpdatedNormalizationAlarmType",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.UpdatedNormalizationAlarmType(null, null, "absolute:2214,100", "1", "relative:2214,100"),
                    ErrorCompare.UpdatedNormalizationAlarmType(null, null, "absolute:2214", "2", "relative:2214"),
                    ErrorCompare.UpdatedNormalizationAlarmType(null, null, "absolute", "3", "relative"),
                    ErrorCompare.UpdatedNormalizationAlarmType(null, null, "relative", "4", "absolute"),
                    ErrorCompare.UpdatedNormalizationAlarmType(null, null, "relative:2214,100", "5", "relative:2214"),
                    ErrorCompare.UpdatedNormalizationAlarmType(null, null, "relative:2214", "6", "relative:2214,100"),
                    ErrorCompare.UpdatedNormalizationAlarmType(null, null, "relative:2214,100", "7", "relative"),
                    ErrorCompare.UpdatedNormalizationAlarmType(null, null, "absolute:2214", "8", "absolute"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckTypeAttribute_AddedNormalizationAlarmType()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "AddedNormalizationAlarmType",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.AddedNormalizationAlarmType(null, null, "absolute:205", "1"),
                    ErrorCompare.AddedNormalizationAlarmType(null, null, "relative:1000001", "2"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckTypeAttribute();

        [TestMethod]
        public void Param_CheckTypeAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckTypeAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckTypeAttribute);
    }
}