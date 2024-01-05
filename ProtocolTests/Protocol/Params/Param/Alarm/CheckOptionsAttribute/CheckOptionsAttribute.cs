namespace ProtocolTests.Protocol.Params.Param.Alarm.CheckOptionsAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Alarm.CheckOptionsAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckOptionsAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckOptionsAttribute_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.FullValidate(check, data);
        }
        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckOptionsAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "100"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    // Threshold
                    Error.NonExistingId(null, null, null, "1000", "100"),
                    Error.NonExistingId(null, null, null, "1001", "100"),

                    // Properties
                    Error.NonExistingId(null, null, null, "1002", "100"),
                    Error.NonExistingId(null, null, null, "1003", "100"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay check")]
        public void Param_CheckOptionsAttribute_ReferencedParamRTDisplayExpected()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ReferencedParamRTDisplayExpected",
                ExpectedResults = new List<IValidationResult>
                {
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "100", " threshold:1000,1001;propertyNames:Prop1,Prop2,Prop3;properties:|aaa|1002|*value:*1003 "),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckOptionsAttribute();

        [TestMethod]
        public void Param_CheckOptionsAttribute_EmptyAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "EmptyAttribute",
            };

            Generic.Fix(check, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_UntrimmedAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedAttribute",
            };

            Generic.Fix(check, data);
        }
    }

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckOptionsAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckOptionsAttribute_Valid()
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
        public void Param_CheckOptionsAttribute_UpdatedThresholdAlarmType()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "UpdatedThresholdAlarmType",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.UpdatedThresholdAlarmType(null, null, "102,2206", "1", "104,2206"),
                    ErrorCompare.UpdatedThresholdAlarmType(null, null, "102,2206", "2", "105,2206"),
                    ErrorCompare.UpdatedThresholdAlarmType(null, null, "102,2206", "3", "106,2208"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_AddedThresholdAlarmType()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "AddedThresholdAlarmType",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.AddedThresholdAlarmType(null, null, "102,2206", "1"),
                    ErrorCompare.AddedThresholdAlarmType(null, null, "102,2206", "2"),
                    ErrorCompare.AddedThresholdAlarmType(null, null, "102,2206", "3"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_RemovedThresholdAlarmType()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "RemovedThresholdAlarmType",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.RemovedThresholdAlarmType(null, null, "102,2206", "1"),
                    ErrorCompare.RemovedThresholdAlarmType(null, null, "102,2206", "2"),
                    ErrorCompare.RemovedThresholdAlarmType(null, null, "102,2206", "3"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckOptionsAttribute();

        [TestMethod]
        public void Param_CheckOptionsAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckOptionsAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckOptionsAttribute);
    }
}