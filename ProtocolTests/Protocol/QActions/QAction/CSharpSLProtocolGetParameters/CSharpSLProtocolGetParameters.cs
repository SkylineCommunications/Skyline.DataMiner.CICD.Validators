namespace ProtocolTests.Protocol.QActions.QAction.CSharpSLProtocolGetParameters
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpSLProtocolGetParameters;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpSLProtocolGetParameters();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpSLProtocolGetParameters_Valid()
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
        public void QAction_CSharpSLProtocolGetParameters_HardCodedPid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "HardCodedPid",
                ExpectedResults = new List<IValidationResult>
                {
                    // protocol.GetParameters()
                    Error.HardCodedPid(null, null, null, "100", "101"),
                    Error.HardCodedPid(null, null, null, "200", "101"),
                    Error.HardCodedPid(null, null, null, "300", "101"),
                    //Error.HardCodedPid(null, null, null, "350", "101"),   // Not yet supported

                    //// Wrappers (Not yet supported)
                    //Error.HardCodedPid(null, null, null, "100", "102"),
                    //Error.HardCodedPid(null, null, null, "200", "102"),
                    //Error.HardCodedPid(null, null, null, "300", "102"),
                    //Error.HardCodedPid(null, null, null, "350", "102"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolGetParameters_NonExistingParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingParam",
                ExpectedResults = new List<IValidationResult>
                {
                    #region QAction 101: protocol.GetParameters

                    Error.UnexpectedImplementation(null, null, null,
                        "(new uint[] { 9, Parameter.Write.writeparam_200 })",
                        "101").WithSubResults(
                            Error.HardCodedPid(null, null, null, "9", "101"),
                            Error.NonExistingParam(null, null, null, "9", "101")),

                    Error.UnexpectedImplementation(null, null, null,
                        "(new uint[] { Parameter.Write.writeparam_200, nonExisting_99, Parameter.readwriteparam_300_350 })",
                        "101").WithSubResults(
                        Error.HardCodedPid(null, null, null, "99", "101"),
                        Error.NonExistingParam(null, null, null, "99", "101")),

                    Error.UnexpectedImplementation(null, null, null,
                        "(pidsToGet)",
                        "101").WithSubResults(
                        Error.HardCodedPid(null, null, null, "199", "101"),
                        Error.NonExistingParam(null, null, null, "199", "101")),

                    //Error.UnexpectedImplementation(null, null, null,
                    //    "(pidsToGetAsList.ToArray())",
                    //    "101",
                    //    new List<IValidationResult>
                    //    {
                    //        Error.HardCodedPid(null, null, null, "1099", "102"),
                    //        Error.NonExistingParam(null, null, null, "1099", "102"),
                    //    }),   // Not yet supported

                    #endregion

                    #region QAction 102: Wrappers (Not yet supported)

                    //Error.UnexpectedImplementation(null, null, null,
                    //    "(new uint[] { 9, Parameter.Write.writeparam_200 })",
                    //    "101",
                    //    new List<IValidationResult>
                    //    {
                    //        Error.HardCodedPid(null, null, null, "9", "102"),
                    //        Error.NonExistingParam(null, null, null, "9", "102"),
                    //    }),

                    //Error.UnexpectedImplementation(null, null, null,
                    //    "(new uint[] { Parameter.Write.writeparam_200, nonExisting_99, Parameter.readwriteparam_300_350 })",
                    //    "101",
                    //    new List<IValidationResult>
                    //    {
                    //        Error.HardCodedPid(null, null, null, "99", "102"),
                    //        Error.NonExistingParam(null, null, null, "99", "102"),
                    //    }),

                    //Error.UnexpectedImplementation(null, null, null,
                    //    "(pidsToGet)",
                    //    "101",
                    //    new List<IValidationResult>
                    //    {
                    //        Error.HardCodedPid(null, null, null, "199", "102"),
                    //        Error.NonExistingParam(null, null, null, "199", "102"),
                    //    }),

                    //Error.UnexpectedImplementation(null, null, null,
                    //    "(pidsToGetAsList.ToArray())",
                    //    "101",
                    //    new List<IValidationResult>
                    //    {
                    //        Error.HardCodedPid(null, null, null, "1099", "102"),
                    //        Error.NonExistingParam(null, null, null, "1099", "102"),
                    //    }),

                    #endregion
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolGetParameters_UnexpectedImplementation()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnexpectedImplementation",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnsupportedArgumentTypeForIds(null, null, null, "object", "1"),

                    #region QAction 101: protocol.GetParameters

                    Error.UnexpectedImplementation(null, null, null,
                        "(new uint[] { pids, Parameter.readparam_100, MyParams.NonExisting_199 })",
                        "101").WithSubResults(
                            Error.HardCodedPid(null, null, null, "99", "101"),
                            Error.NonExistingParam(null, null, null, "99", "101"),
                            Error.HardCodedPid(null, null, null, "199", "101"),
                            Error.NonExistingParam(null, null, null, "199", "101")),

                    Error.UnexpectedImplementation(null, null, null, "(pidsToGet)", "101").WithSubResults(
                            Error.HardCodedPid(null, null, null, "1999", "101"),
                            Error.NonExistingParam(null, null, null, "1999", "101"),
                            Error.HardCodedPid(null, null, null, "1299", "101"),
                            Error.NonExistingParam(null, null, null, "1299", "101")),

                    #endregion
                    
                    #region QAction 102: Wrappers (Not yet supported)

                    //Error.UnexpectedImplementation(null, null, null,
                    //    "(new int[] { pids, Parameter.readparam_100, MyParams.NonExisting_199 })",
                    //    "102").WithSubResults(
                    //    new List<IValidationResult>{
                    //        Error.UnsupportedArgumentTypeForIds(null, null, null, "int[]", "102"),
                    //        Error.HardCodedPid(null, null, null, "99", "102"),
                    //        Error.NonExistingParam(null, null, null, "99", "102"),
                    //        Error.HardCodedPid(null, null, null, "199", "102"),
                    //        Error.NonExistingParam(null, null, null, "199", "102"),
                    //    }),

                    //Error.UnexpectedImplementation(null, null, null, "(pidsToGet)", "102").WithSubResults(
                    //    new List<IValidationResult>{
                    //        Error.UnsupportedArgumentTypeForIds(null, null, null, "int[]", "102"),
                    //        Error.HardCodedPid(null, null, null, "1999", "102"),
                    //        Error.NonExistingParam(null, null, null, "1999", "102"),
                    //        Error.HardCodedPid(null, null, null, "1299", "102"),
                    //        Error.NonExistingParam(null, null, null, "1299", "102"),
                    //    }),

                    #endregion
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolGetParameters_UnsupportedArgumentTypeForIds()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnsupportedArgumentTypeForIds",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnsupportedArgumentTypeForIds(null, null, null, "object", "1"),

                    // protocol.GetParameters()
                    Error.UnsupportedArgumentTypeForIds(null, null, null, "string", "100"),
                    Error.UnsupportedArgumentTypeForIds(null, null, null, "double", "100"),
                    Error.UnsupportedArgumentTypeForIds(null, null, null, "int", "100"),
                    Error.UnsupportedArgumentTypeForIds(null, null, null, "uint", "100"),
                    Error.UnsupportedArgumentTypeForIds(null, null, null, "uint", "100"),

                    Error.UnsupportedArgumentTypeForIds(null, null, null, "string[]", "101"),   // string[] to string[]
                    Error.UnsupportedArgumentTypeForIds(null, null, null, "double[]", "101"),   // double[] to object
                    Error.UnsupportedArgumentTypeForIds(null, null, null, "int[]", "101"),      // double[] to object

                    Error.UnsupportedArgumentTypeForIds(null, null, null, "int[]", "102"),      // int[] to var
                    Error.UnsupportedArgumentTypeForIds(null, null, null, "int[]", "102"),      // Implicit array type

                    //Error.UnsupportedArgumentTypeForIds(null, null, null, "int[]", "103"),        // List.ToArray // Not yet supported
                    //Error.UnsupportedArgumentTypeForIds(null, null, null, "List<int>", "103"),    // List<int>    // Not yet supported
                    
                    //Error.UnsupportedArgumentTypeForIds(null, null, null, "IEnumerable<int>", "104"), // Not yet supported

                    //Error.UnsupportedArgumentTypeForIds(null, null, null, "MyClass", "200"), // Not yet supported

                    //// Wrappers (Not yet supported)
                    //Error.UnsupportedArgumentTypeForIds(null, null, null, "string", "1000"),
                    //Error.UnsupportedArgumentTypeForIds(null, null, null, "double", "1000"),
                    //Error.UnsupportedArgumentTypeForIds(null, null, null, "int", "1000"),
                    //Error.UnsupportedArgumentTypeForIds(null, null, null, "uint", "1000"),
                    //Error.UnsupportedArgumentTypeForIds(null, null, null, "uint", "1000"),

                    //Error.UnsupportedArgumentTypeForIds(null, null, null, "string[]", "1001"),   // string[] to string[]
                    //Error.UnsupportedArgumentTypeForIds(null, null, null, "double[]", "1001"),   // double[] to object
                    //Error.UnsupportedArgumentTypeForIds(null, null, null, "int[]", "1001"),      // double[] to object

                    //Error.UnsupportedArgumentTypeForIds(null, null, null, "int[]", "1002"),      // int[] to var
                    //Error.UnsupportedArgumentTypeForIds(null, null, null, "int[]", "1002"),      // Implicit array type

                    //Error.UnsupportedArgumentTypeForIds(null, null, null, "int[]", "1003"),      // List.ToArray // Not yet supported
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
        public void QAction_CSharpSLProtocolGetParameters_HardCodedPid()
        {
            // Create ErrorMessage
            var message = Error.HardCodedPid(null, null, null, "2", "3");

            var expected = new ValidationResult()
            {
                ErrorId = 3,
                FullId = "3.33.3",
                Category = Category.QAction,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Unrecommended use of magic number '2', use 'Parameter' class instead. QAction ID '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolGetParameters_NonExistingParam()
        {
            // Create ErrorMessage
            var message = Error.NonExistingParam(null, null, null, "2", "3");

            var expected = new ValidationResult()
            {
                ErrorId = 2,
                FullId = "3.33.2",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Method 'SLProtocol.GetParameters' references a non-existing 'Param' with ID '2'. QAction ID '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolGetParameters_UnexpectedImplementation()
        {
            // Create ErrorMessage
            var message = Error.UnexpectedImplementation(null, null, null, "2", "3");

            var expected = new ValidationResult()
            {
                ErrorId = 1,
                FullId = "3.33.1",
                Category = Category.QAction,
                Severity = Severity.BubbleUp,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Method 'SLProtocol.GetParameters' with arguments '2' is not implemented as expected. QAction ID '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolGetParameters_UnsupportedArgumentTypeForIds()
        {
            // Create ErrorMessage
            var message = Error.UnsupportedArgumentTypeForIds(null, null, null, "2", "3");

            var expected = new ValidationResult()
            {
                ErrorId = 4,
                FullId = "3.33.4",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Invocation of method 'SLProtocol.GetParameters' has an invalid type '2' for the argument 'ids'. QAction ID '3'.",
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
        private readonly IRoot check = new CSharpSLProtocolGetParameters();

        [TestMethod]
        public void QAction_CSharpSLProtocolGetParameters_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpSLProtocolGetParameters_CheckId() => Generic.CheckId(check, CheckId.CSharpSLProtocolGetParameters);
    }
}