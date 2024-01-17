namespace ProtocolTests.Protocol.QActions.QAction.CSharpQActionCompilation
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpQActionCompilation;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpQActionCompilation();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpQActionCompilation_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpQActionCompilation_Valid_CSharp4()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_CSharp4",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpQActionCompilation_Valid_CSharp7_3()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_CSharp7_3",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void QAction_CSharpQActionCompilation_CompilationFailure()
        {
            // List of expected results
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "CompilationFailure",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.CompilationFailure(null, null, null, "2").WithSubResults(
                        Error.CompilationFailure_Sub(null, null, null, "QAction_2.cs(9,19): error CS1001: Identifier expected"),
                        Error.CompilationFailure_Sub(null, null, null, "QAction_2.cs(9,19): error CS1002: ; expected")),
                    Error.CompilationFailure(null, null, null, "100").WithSubResults(
                        Error.CompilationFailure_Sub(null, null, null, "QAction_100.cs(16,12): error CS1061: 'SLProtocol' does not contain a definition for 'MyMistypedMethod' and no accessible extension method 'MyMistypedMethod' accepting a first argument of type 'SLProtocol' could be found (are you missing a using directive or an assembly reference?)")),
                    Error.CompilationFailure(null, null, null, "201").WithSubResults(
                        Error.CompilationFailure_Sub(null, null, null, "QAction_201.cs(17,26): error CS0117: 'MyClass' does not contain a definition for 'MyMistypedMethod1'"),
                        Error.CompilationFailure_Sub(null, null, null, "QAction_201.cs(18,26): error CS0117: 'MyClass' does not contain a definition for 'MyMistypedMethod2'"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Not possible until we update Microsoft.CodeAnalysis")]
        public void QAction_CSharpQActionCompilation_CompilationFailure_CSharp703()
        {
            // List of expected results
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "CompilationFailure_CSharp703",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.CompilationFailure(null, null, null, "100").WithSubResults(
                            Error.CompilationFailure_Sub(null, null, null, "QAction_100.cs(17,12): error CS1061: 'SLProtocol' does not contain a definition for 'MyMistypedMethod' and no accessible extension method 'MyMistypedMethod' accepting a first argument of type 'SLProtocol' could be found (are you missing a using directive or an assembly reference?)")/*,
                            Error.CompilationFailure_Sub(null, null, null, "QAction_100.cs(33,15): error CS8025: Feature 'not pattern' is not available in C# 7.3. Please use language version 9.0 or greater."),
                            Error.CompilationFailure_Sub(null, null, null, "QAction_100.cs(33,15): error CS8025: Feature 'type pattern' is not available in C# 7.3. Please use language version 9.0 or greater.")
                            */)
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Not possible until we update Microsoft.CodeAnalysis")]
        public void QAction_CSharpQActionCompilation_CompilationFailure_CSharp703_NoCompliancyTag()
        {
            // List of expected results
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "CompilationFailure_CSharp703_NoCompliancyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.CompilationFailure(null, null, null, "100").WithSubResults(
                            Error.CompilationFailure_Sub(null, null, null, "QAction_100.cs(17,12): error CS1061: 'SLProtocol' does not contain a definition for 'MyMistypedMethod' and no accessible extension method 'MyMistypedMethod' accepting a first argument of type 'SLProtocol' could be found (are you missing a using directive or an assembly reference?)")/*,
                            Error.CompilationFailure_Sub(null, null, null, "QAction_100.cs(33,15): error CS8025: Feature 'not pattern' is not available in C# 7.3. Please use language version 9.0 or greater."),
                            Error.CompilationFailure_Sub(null, null, null, "QAction_100.cs(33,15): error CS8025: Feature 'type pattern' is not available in C# 7.3. Please use language version 9.0 or greater.")
                            */)
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Keeping the code in case we have the same situation again later")]
        public void QAction_CSharpQActionCompilation_NoCSharpCodeAnalysisPerformed()
        {
            // QAction 100
            var result = Error.NoCSharpCodeAnalysisPerformed(null, null, null, "7.3", "< VS 2017");

            // List of expected results
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NoCSharpCodeAnalysisPerformed",
                ExpectedResults = new List<IValidationResult>
                {
                    result
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void QAction_CSharpQActionCompilation_CompilationFailure()
        {
            // Create ErrorMessage
            var message = Error.CompilationFailure(null, null, null, "1");

            string description = "C# compilation errors. QAction ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void QAction_CSharpQActionCompilation_CompilationFailure_Sub()
        {
            // Create ErrorMessage
            var message = Error.CompilationFailure_Sub(null, null, null, "QAction_2.cs(9,19): error CS1002: ; expected");

            string description = "QAction_2.cs(9,19): error CS1002: ; expected";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CSharpQActionCompilation();

        [TestMethod]
        public void QAction_CSharpQActionCompilation_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpQActionCompilation_CheckId() => Generic.CheckId(check, CheckId.CSharpQActionCompilation);
    }
}