namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.Hierarchy.CheckPathAttribute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckPathAttribute, Category.TreeControl)]
    internal class CheckPathAttribute : IValidate, ICodeFix
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var model = context.ProtocolModel;

            foreach (ITreeControlsTreeControl treeControl in context.EachTreeControlWithValidParameterId())
            {
                if (context.ProtocolModel.IsExportedProtocolModel && !treeControl.MainParameterExists(context.ProtocolModel))
                {
                    continue;
                }

                if (treeControl.Hierarchy?.Path == null)
                {
                    continue;
                }

                ValidateHelper helper = new ValidateHelper(this, context, model, results, treeControl);
                helper.Validate();
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol.TreeControls == null)
            {
                result.Message = "No TreeControls found!";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    {
                        if (context.Protocol?.TreeControls != null)
                        {
                            var readNode = (ITreeControlsTreeControl)context.Result.ReferenceNode;
                            var editNode = context.Protocol.TreeControls.Get(readNode);

                            editNode.Hierarchy.Path.Value = String.Join(",", readNode.Hierarchy.Path.Value.Split(',').Select(x => x.Trim()));
                            result.Success = true;
                        }

                        break;
                    }

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }
    }

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly ValidatorContext context;
        private readonly IProtocolModel model;
        private readonly List<IValidationResult> results;

        private readonly ITreeControlsTreeControl treeControl;
        private readonly IValueTag<string> path;

        private readonly List<IValidationResult> subResultsInvalid = new List<IValidationResult>();
        private readonly List<IValidationResult> subResultsNonExisting = new List<IValidationResult>();
        private readonly List<IValidationResult> subResultsUntrimmed = new List<IValidationResult>();

        internal ValidateHelper(IValidate test, ValidatorContext context, IProtocolModel model, List<IValidationResult> results, ITreeControlsTreeControl treeControl)
        {
            this.test = test;
            this.context = context;
            this.model = model;
            this.results = results;

            this.treeControl = treeControl;
            path = treeControl.Hierarchy.Path;
        }

        internal void Validate()
        {
            (GenericStatus status, string rawValue, string _) = GenericTests.CheckBasics(path, false);

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(test, treeControl, treeControl, treeControl.ParameterId.RawValue));
                return;
            }

            HashSet<string> idsForUniquenessCheck = new HashSet<string>();

            string[] ids = rawValue.Split(',');
            foreach (string id in ids)
            {
                (GenericStatus idStatus, uint idValue) = GenericTests.CheckBasics<uint>(id);

                // Valid PID
                if (idStatus.HasFlag(GenericStatus.Empty) || idStatus.HasFlag(GenericStatus.Invalid) || !GenericTests.IsPlainNumbers(rawValue))
                {
                    subResultsInvalid.Add(Error.InvalidValueInAttribute_Sub(test, treeControl, path, id));
                    continue;
                }

                // Unique PID
                if (!idsForUniquenessCheck.Add(id))
                {
                    results.Add(Error.DuplicateId(test, treeControl, path, id, treeControl.ParameterId.RawValue));
                    continue;
                }

                CheckTableParam(tablePid: Convert.ToString(idValue));

                // Untrimmed ID
                if (idStatus.HasFlag(GenericStatus.Untrimmed))
                {
                    subResultsUntrimmed.Add(Error.UntrimmedValueInAttribute_Sub(test, treeControl, path, id));
                }
            }

            // Invalid
            if (subResultsInvalid.Count > 0)
            {
                IValidationResult error = Error.InvalidValue(test, treeControl, treeControl, rawValue, treeControl.ParameterId.RawValue);
                error.WithSubResults(subResultsInvalid.ToArray());
                results.Add(error);
            }

            // NonExisting
            if (subResultsNonExisting.Count > 0)
            {
                IValidationResult error = Error.NonExistingIdsInAttribute(test, treeControl, path, treeControl.ParameterId.RawValue);
                error.WithSubResults(subResultsNonExisting.ToArray());
                results.Add(error);
            }

            // Untrimmed
            if (subResultsUntrimmed.Count > 0)
            {
                IValidationResult error = Error.UntrimmedAttribute(test, treeControl, path, treeControl.ParameterId.RawValue, rawValue);
                error.WithSubResults(subResultsUntrimmed.ToArray());
                results.Add(error);
            }
        }

        private void CheckTableParam(string tablePid)
        {
            // Non Existing Param
            if (!model.TryGetObjectByKey(Mappings.ParamsById, tablePid, out IParamsParam tableParam))
            {
                subResultsNonExisting.Add(Error.NonExistingIdsInAttribute_Sub(test, treeControl, path, tablePid, treeControl.ParameterId.RawValue));
                return;
            }

            // TODO: Param Wrong Type
            //if (!tableParam.IsTable())
            //{
            //    results.Add(Error.ReferencedParamWrongType(test, treeControl, path, tableParam.Type?.RawValue, tablePid));
            //}

            // Param Requiring RTDisplay
            IValidationResult rtDisplayError = Error.ReferencedParamExpectingRTDisplay(test, treeControl, path, tableParam.Id.RawValue);
            context.CrossData.RtDisplay.AddParam(tableParam, rtDisplayError);
        }
    }
}