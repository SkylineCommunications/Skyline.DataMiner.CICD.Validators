namespace ProtocolTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Common.Testing;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Parsers.Common.Xml;
    using Skyline.DataMiner.CICD.Validators.Common.Data;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;

    [TestClass]
    public class ValidatorTests
    {
        #region Old Run Validate

        [TestMethod]
        public void RunValidate_Old_MultipleResults()
        {
            var validator = new Validator();

            var protocolCode = String.Empty;

            QActionCompilationModel semanticModel = null; // not needed for old validator
            ProtocolInputData input = new ProtocolInputData(protocolCode, semanticModel);
            var results = validator.RunValidate(input, new ValidatorSettings(), new CancellationToken());

            results.Should().NotBeEmpty();
        }

        [TestMethod]
        public void RunValidate_Old_MultipleResults2()
        {
            var validator = new Validator();

            var protocolCode = String.Empty;

            var list = new List<(Category Category, uint CheckId)>();

            QActionCompilationModel semanticModel = null; // not needed for old validator
            ProtocolInputData input = new ProtocolInputData(protocolCode, semanticModel);
            var results = validator.RunValidate(input, new ValidatorSettings().WithTestToExecute(list), new CancellationToken());

            results.Should().NotBeEmpty();
        }

        [TestMethod]
        public void RunValidate_Old_NoResults()
        {
            var validator = new Validator();

            var protocolCode = String.Empty;

            var list = new List<(Category Category, uint CheckId)>()
            {
                // Shouldn't give an error message
                (Category.Undefined, 0),
            };

            QActionCompilationModel semanticModel = null; // not needed for old validator
            ProtocolInputData input = new ProtocolInputData(protocolCode, semanticModel);
            var results = validator.RunValidate(input, new ValidatorSettings().WithTestToExecute(list), new CancellationToken());

            results.Should().BeEmpty();
        }

        #endregion

        #region New Run Validate

        [TestMethod]
        public void RunValidate_New_MultipleResults()
        {
            var validator = new Validator();

            var protocolCode = String.Empty;
            var parser = new Parser(protocolCode);
            var model = new ProtocolModel(parser.Document);

            var semanticModel = ProtocolTestsHelper.GetQActionCompilationModel(model, protocolCode);

            var input = new ProtocolInputData(model, parser.Document, semanticModel);
            var results = validator.RunValidate(input, new ValidatorSettings(), new CancellationToken());

            results.Should().NotBeEmpty();
        }

        [TestMethod]
        public void RunValidate_New_MultipleResults2()
        {
            var validator = new Validator();

            var protocolCode = String.Empty;
            var parser = new Parser(protocolCode);
            var model = new ProtocolModel(parser.Document);

            var list = new List<(Category Category, uint CheckId)>();

            var semanticModel = ProtocolTestsHelper.GetQActionCompilationModel(model, protocolCode);

            var input = new ProtocolInputData(model, parser.Document, semanticModel);
            var results = validator.RunValidate(input, new ValidatorSettings().WithTestToExecute(list), new CancellationToken());

            results.Should().NotBeEmpty();
        }

        [TestMethod]
        public void RunValidate_New_NoResults()
        {
            var validator = new Validator();

            var protocolCode = String.Empty;
            var parser = new Parser(protocolCode);
            var model = new ProtocolModel(parser.Document);

            var list = new List<(Category Category, uint CheckId)>()
            {
                // Shouldn't give an error message
                (Category.Undefined, 0),
            };

            var semanticModel = ProtocolTestsHelper.GetQActionCompilationModel(model, protocolCode);

            var input = new ProtocolInputData(model, parser.Document, semanticModel);
            var results = validator.RunValidate(input, new ValidatorSettings().WithTestToExecute(list), new CancellationToken());

            results.Should().BeEmpty();
        }

        #endregion

        #region Old Run Compare

        // Currently not possible due to issues in Compare methods (Doesn't take in account an empty protocol or empty file)

        #endregion

        #region New Run Compare

        // Currently not possible due to issues in Compare methods (Doesn't take in account an empty protocol or empty file)

        #endregion

        #region Execute CodeFix

        // Not really testable yet.

        #endregion

        #region Group Results

        [TestMethod]
        public void GroupResults_NoGenericDescription()
        {
            List<IValidationResult> input = new List<IValidationResult>();
            List<IValidationResult> expectedOutput = new List<IValidationResult>();
            for (int i = 0; i < 20; i++)
            {
                var result = new ValidationResult()
                {
                    GroupDescription = "",
                };

                input.Add(result);
                expectedOutput.Add(result);
            }

            List<IValidationResult> output = Validator.GroupResults(input).ToList();

            output.Should().BeEquivalentTo(expectedOutput);
        }

        [TestMethod]
        public void GroupResults_TooFewToGroup()
        {
            List<IValidationResult> input = new List<IValidationResult>();
            List<IValidationResult> expectedOutput = new List<IValidationResult>();
            for (int i = 0; i < 9; i++)
            {
                var result = new ValidationResult()
                {
                    GroupDescription = "myGroupDescription",
                };

                input.Add(result);
                expectedOutput.Add(result);
            }

            List<IValidationResult> output = Validator.GroupResults(input).ToList();

            output.Should().BeEquivalentTo(expectedOutput);
        }

        [TestMethod]
        public void GroupResults_EnoughToGroup()
        {
            List<IValidationResult> input = new List<IValidationResult>();
            List<IValidationResult> expectedChildren = new List<IValidationResult>();
            for (int i = 0; i < 11; i++)
            {
                var result = new ValidationResult()
                {
                    GroupDescription = "myGroupDescription",
                };

                input.Add(result);
                expectedChildren.Add(result);
            }

            List<IValidationResult> expectedOutput = new List<IValidationResult>()
            {
                new ValidationResult()
                {
                    Description = "myGroupDescription",
                    SubResults = expectedChildren,
                },
            };

            List<IValidationResult> output = Validator.GroupResults(input).ToList();

            output.Should().BeEquivalentTo(expectedOutput);
        }

        #endregion
    }
}