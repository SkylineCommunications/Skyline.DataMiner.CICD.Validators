namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.ReadonlyColumns.CheckReadonlyColumnsTag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckReadonlyColumnsTag, Category.TreeControl)]
    internal class CheckReadonlyColumnsTag : IValidate, ICodeFix
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

                if (treeControl.ReadonlyColumns == null)
                {
                    continue;
                }

                (GenericStatus status, string rawValue, _) = GenericTests.CheckBasics(treeControl.ReadonlyColumns, false);

                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyTag(this, treeControl, treeControl, treeControl.ParameterId.RawValue));
                    continue;
                }

                string[] readonlyColumnPids = rawValue.Split(',');

                List<IValidationResult> subResultsInvalid = new List<IValidationResult>();
                List<IValidationResult> subResultsNonExisting = new List<IValidationResult>();
                List<IValidationResult> subResultsUntrimmed = new List<IValidationResult>();
                ICollection<uint> tablePids = treeControl.GetDisplayedTablesPids(model);
                List<string> tempIds = new List<string>();
                foreach (string readonlyColumnPid in readonlyColumnPids)
                {
                    (GenericStatus idStatus, uint idValue) = GenericTests.CheckBasics<uint>(readonlyColumnPid);

                    if (idStatus.HasFlag(GenericStatus.Empty) || idStatus.HasFlag(GenericStatus.Invalid))
                    {
                        subResultsInvalid.Add(Error.InvalidValueInTag_Sub(this, treeControl, treeControl.ReadonlyColumns, readonlyColumnPid));
                        continue;
                    }

                    if (tempIds.Contains(readonlyColumnPid))
                    {
                        results.Add(Error.DuplicateId(this, treeControl, treeControl.ReadonlyColumns, readonlyColumnPid, treeControl.ParameterId.RawValue));
                        continue;
                    }

                    tempIds.Add(readonlyColumnPid);

                    // Check if parameter links to an existing Table.
                    var paramIdString = Convert.ToString(idValue);
                    if (!model.TryGetObjectByKey(Mappings.ParamsById, paramIdString, out IParamsParam column))
                    {
                        subResultsNonExisting.Add(Error.NonExistingIds_Sub(this, treeControl, treeControl.ReadonlyColumns, paramIdString, treeControl.ParameterId.RawValue));
                        continue;
                    }

                    // ID exists, but does it belong to a table that is part of the TreeControl?
                    if (!column.TryGetTable(model.RelationManager, idValue, out IParamsParam table) ||
                        table.Id.Value == null || !tablePids.Contains(table.Id.Value.Value))
                    {
                        results.Add(Error.IrrelevantColumn(this, treeControl, treeControl.ReadonlyColumns, readonlyColumnPid, treeControl.ParameterId.RawValue));
                        continue;
                    }

                    if (idStatus.HasFlag(GenericStatus.Untrimmed))
                    {
                        subResultsUntrimmed.Add(Error.UntrimmedValueInTag_Sub(this, treeControl, treeControl.ReadonlyColumns, readonlyColumnPid));
                    }
                }

                if (subResultsInvalid.Count > 0)
                {
                    IValidationResult error = Error.InvalidValue(null, null, null, rawValue, treeControl.ParameterId.RawValue);
                    error.WithSubResults(subResultsInvalid.ToArray());
                    results.Add(error);
                }

                if (subResultsNonExisting.Count > 0)
                {
                    IValidationResult error = Error.NonExistingIds(this, treeControl, treeControl.ReadonlyColumns, treeControl.ParameterId.RawValue);
                    error.WithSubResults(subResultsNonExisting.ToArray());
                    results.Add(error);
                }

                if (subResultsUntrimmed.Count > 0)
                {
                    IValidationResult error = Error.UntrimmedTag(this, treeControl, treeControl.ReadonlyColumns, treeControl.ParameterId.RawValue, rawValue);
                    error.WithSubResults(subResultsUntrimmed.ToArray());
                    results.Add(error);
                }
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
                case ErrorIds.UntrimmedTag:
                    {
                        var readNode = (ITreeControlsTreeControl)context.Result.ReferenceNode;
                        var editNode = context.Protocol.TreeControls.Get(readNode);

                        editNode.ReadonlyColumns.Value = String.Join(",", readNode.ReadonlyColumns.Value.Split(',').Select(x => x.Trim()));
                        result.Success = true;
                        break;
                    }

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }
    }
}