namespace ProtocolTests.Protocol.DVEs.DVEProtocols.DVEProtocol.CheckNameAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.DVEs.DVEProtocols.DVEProtocol.CheckNameAttribute;

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckNameAttribute();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckNameAttribute_Valid()
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
        public void Protocol_CheckNameAttribute_NoTag()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "NoTag",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Protocol_CheckNameAttribute_UpdatedValue()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "UpdatedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.UpdatedValue(null, null, "Mother - Child2", "200", "Mother - NewChild"),
                    ErrorCompare.UpdatedValue(null, null, "MotherChild2", "400", "MotherChild 2"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckNameAttribute_RemovedItem()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "RemovedItem",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.RemovedItem(null, null, "Mother - Child2", "200"),
                    ErrorCompare.RemovedItem(null, null, "MotherChild", "300"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Protocol_CheckNameAttribute_UpdatedValue()
        {
            // Create ErrorMessage
            var message = ErrorCompare.UpdatedValue(null, null, "0", "1", "2");

            string description = "DVE Protocol with Name '0' for Table '1' was changed into '2'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Protocol_CheckNameAttribute_RemovedItem()
        {
            // Create ErrorMessage
            var message = ErrorCompare.RemovedItem(null, null, "0", "1");

            string description = "DVE Protocol with Name '0' for Table '1' was removed.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckNameAttribute();

        [TestMethod]
        public void Protocol_CheckNameAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckNameAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckNameAttribute);
    }
}