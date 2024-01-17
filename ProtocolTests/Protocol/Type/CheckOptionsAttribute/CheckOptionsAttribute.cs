namespace ProtocolTests.Protocol.Type.CheckOptionsAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Type.CheckOptionsAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckOptionsAttribute();

        #region Valid Checks

        [TestMethod]
        public void Trigger_CheckOptionsAttribute_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Trigger_CheckOptionsAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckOptionsAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1000"),
                    Error.NonExistingId(null, null, null, "2000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void Trigger_CheckOptionsAttribute_ReferencedParamExpectingRTDisplay()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ReferencedParamExpectingRTDisplay",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ReferencedParamExpectingRTDisplay(null, null, null, "1000"),
                    Error.ReferencedParamExpectingRTDisplay(null, null, null, "2000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckOptionsAttribute_ReferencedParamWrongType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ReferencedParamWrongType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ReferencedParamWrongType(null, null, null, "read", "1000"),
                    Error.ReferencedParamWrongType(null, null, null, "bus", "2000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Trigger_CheckOptionsAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "  unicode;disableViewRefresh;exportProtocol:DVE Parent - Child 1:1000:noElementPrefix;exportProtocol:DVE Parent - Child 2:2000  "),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckOptionsAttribute();

        [TestMethod]
        public void Param_CheckOptionsAttribute_EmptyAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "EmptyAttribute",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_UntrimmedAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedAttribute",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckOptionsAttribute();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckOptionsAttribute_Valid()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckOptionsAttribute_ValidUnicode()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "ValidUnicode",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckOptionsAttribute_ValidNoUnicode()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "ValidNoUnicode",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        #endregion Valid Checks

        #region Invalid Checks

        [TestMethod]
        public void Protocol_CheckOptionsAttribute_UpdatedDveExportProtocolName()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "UpdatedDveExportProtocolName",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.UpdatedDveExportProtocolName(null, null, "MotherChildASlot", "100", "Mother - ChildASlot"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckOptionsAttribute_UpdatedDveExportProtocolNameOneDve()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "UpdatedDveExportProtocolNameOneDve",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.UpdatedDveExportProtocolName(null, null, "Mother ChildA - Slot", "100", "Mother DifferentChild - Slot"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckOptionsAttribute_UpdatedDveExportProtocolNameMultiDve()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "UpdatedDveExportProtocolNameMultiDve",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.UpdatedDveExportProtocolName(null, null, "Mother ChildA - Slot", "100", "Mother ChildA Slot"),
                    ErrorCompare.UpdatedDveExportProtocolName(null, null, "Mother ChildA OldName", "150", "Mother ChildA - NewName"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckOptionsAttribute_UpdatedDveExportProtocolNameElementPrefixDve()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "UpdatedDveExportProtocolNameElementPrefixDve",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.UpdatedDveExportProtocolName(null, null, "MotherChildAOldName", "150", "MotherChildA - OldName"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckOptionsAttribute_RemovedDveExportProtocolName()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "RemovedDveExportProtocolName",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.RemovedDveExportProtocolName(null, null, "MotherChildASlot", "100"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckOptionsAttribute_RemovedDveExportProtocolNameFullPropertyRemoval()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "RemovedDveExportProtocolNameFullPropertyRemoval",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.RemovedDveExportProtocolName(null, null, "MotherChildASlot", "100"),
                    ErrorCompare.RemovedDveExportProtocolName(null, null, "MotherChildAOldName", "150"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckOptionsAttribute_RemovedDveExportProtocolNameFullPropertyEmpty()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "RemovedDveExportProtocolNameFullPropertyEmpty",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.RemovedDveExportProtocolName(null, null, "MotherChildASlot", "100"),
                    ErrorCompare.RemovedDveExportProtocolName(null, null, "MotherChildAOldName", "150"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckOptionsAttribute_AddedNoElementPrefix()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "AddedNoElementPrefix",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.AddedNoElementPrefix(null, null, "MotherChildASlot", "100"),
                    ErrorCompare.AddedNoElementPrefix(null, null, "MotherChildAOldName", "150"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckOptionsAttribute_RemovedNoElementPrefix()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "RemovedNoElementPrefix",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.RemovedNoElementPrefix(null, null, "MotherChildAOldName", "150"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckOptionsAttribute_AddedUnicode()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "AddedUnicode",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.AddedUnicode(null, null),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckOptionsAttribute_RemovedUnicode()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "RemovedUnicode",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.RemovedUnicode(null, null),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion Invalid Checks
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckOptionsAttribute();

        [TestMethod]
        public void Protocol_CheckOptionsAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckOptionsAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckOptionsAttribute);
    }
}