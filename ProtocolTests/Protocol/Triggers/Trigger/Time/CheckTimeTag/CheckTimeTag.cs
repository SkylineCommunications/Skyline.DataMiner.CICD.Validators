namespace ProtocolTests.Protocol.Triggers.Trigger.Time.CheckTimeTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Triggers.Trigger.Time.CheckTimeTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckTimeTag();

        #region Valid Checks

        [TestMethod]
        public void Trigger_CheckTimeTag_Valid()
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
        public void Trigger_CheckTimeTag_MultipleAfterStartup()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MultipleAfterStartup",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MultipleAfterStartup(null, null, null, "1, 2").WithSubResults(
                        Error.MultipleAfterStartup(null, null, null, "1"),
                        Error.MultipleAfterStartup(null, null, null, "2"))
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    ////[TestClass]
    ////public class CodeFix
    ////{
    ////    private readonly ICodeFix codeFix = new CheckTimeTag();

    ////    [TestMethod]
    ////    public void Param_CheckTimeTag_EmptyTag()
    ////    {
    ////        Generic.FixData data = new Generic.FixData
    ////        {
    ////            FileNameBase = "EmptyTag",
    ////        };

    ////        Generic.Fix(codeFix, data);
    ////    }

    ////    [TestMethod]
    ////    public void Param_CheckTimeTag_UntrimmedTag()
    ////    {
    ////        Generic.FixData data = new Generic.FixData
    ////        {
    ////            FileNameBase = "UntrimmedTag",
    ////        };

    ////        Generic.Fix(codeFix, data);
    ////    }
    ////}

    ////[TestClass]
    ////public class Compare
    ////{
    ////    private readonly ICompare compare = new CheckTimeTag();

    ////    #region Valid Checks

    ////    [TestMethod]
    ////    public void Protocol_CheckTimeTag_Valid()
    ////    {
    ////        Generic.CompareData data = new Generic.CompareData
    ////        {
    ////            TestType = Generic.TestType.Valid,
    ////            FileNameBase = "Valid",
    ////            ExpectedResults = new List<IValidationResult>()
    ////        };

    ////        Generic.Compare(compare, data);
    ////    }

    ////    #endregion Valid Checks

    ////    #region Invalid Checks

    ////    [TestMethod]
    ////    public void Protocol_CheckTimeTag_XXX()
    ////    {
    ////        Generic.CompareData data = new Generic.CompareData
    ////        {
    ////            TestType = Generic.TestType.Invalid,
    ////            FileNameBase = "XXX",
    ////            ExpectedResults = new List<IValidationResult>
    ////            {
    ////                ErrorCompare.XXX(null, null, "MotherChildASlot", "100", "Mother - ChildASlot"),
    ////            }
    ////        };

    ////        Generic.Compare(compare, data);
    ////    }

    ////    #endregion Invalid Checks
    ////}

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckTimeTag();

        [TestMethod]
        public void Trigger_CheckTimeTag_CheckCategory() => Generic.CheckCategory(root, Category.Trigger);

        [TestMethod]
        public void Trigger_CheckTimeTag_CheckId() => Generic.CheckId(root, CheckId.CheckTimeTag);
    }
}