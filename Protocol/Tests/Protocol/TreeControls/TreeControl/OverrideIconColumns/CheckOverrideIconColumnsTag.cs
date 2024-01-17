namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.OverrideIconColumns.CheckOverrideIconColumnsTag
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

    [Test(CheckId.CheckOverrideIconColumnsTag, Category.TreeControl)]
    internal class CheckOverrideIconColumnsTag : IValidate, ICodeFix
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

                if (treeControl.OverrideIconColumns == null)
                {
                    continue;
                }

                (GenericStatus status, string rawValue, string _) = GenericTests.CheckBasics(treeControl.OverrideIconColumns, false);

                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyTag(this, treeControl, treeControl, treeControl.ParameterId.RawValue));
                    continue;
                }

                string[] overrideIconPids = rawValue.Split(',');

                List<IValidationResult> subResultsInvalid = new List<IValidationResult>();
                List<IValidationResult> subResultsNonExisting = new List<IValidationResult>();
                List<IValidationResult> subResultsUntrimmed = new List<IValidationResult>();
                Dictionary<uint, List<string>> duplicateTableIds = new Dictionary<uint, List<string>>();
                ICollection<uint> tablePids = treeControl.GetDisplayedTablesPids(model);
                List<string> tempIds = new List<string>();
                foreach (var overrideIconPid in overrideIconPids)
                {
                    (GenericStatus idStatus, uint idValue) = GenericTests.CheckBasics<uint>(overrideIconPid);

                    if (idStatus.HasFlag(GenericStatus.Empty) || idStatus.HasFlag(GenericStatus.Invalid))
                    {
                        subResultsInvalid.Add(Error.InvalidValueInTag_Sub(this, treeControl, treeControl.OverrideIconColumns, overrideIconPid));
                        continue;
                    }

                    if (tempIds.Contains(overrideIconPid))
                    {
                        results.Add(Error.DuplicateId(this, treeControl, treeControl.OverrideIconColumns, overrideIconPid, treeControl.ParameterId.RawValue));
                        continue;
                    }

                    tempIds.Add(overrideIconPid);

                    // Check if parameter links to an existing column.
                    var paramIdString = Convert.ToString(idValue);
                    if (!model.TryGetObjectByKey(Mappings.ParamsById, paramIdString, out IParamsParam column))
                    {
                        subResultsNonExisting.Add(Error.NonExistingIds_Sub(this, treeControl, treeControl.OverrideIconColumns, paramIdString, treeControl.ParameterId.RawValue));
                        continue;
                    }

                    // ID exists, but does it belong to a table that is part of the TreeControl?
                    if (!column.TryGetTable(model.RelationManager, idValue, out IParamsParam table) ||
                        table.Id.Value == null || !tablePids.Contains(table.Id.Value.Value))
                    {
                        results.Add(Error.IrrelevantColumn(this, treeControl, treeControl.OverrideIconColumns, overrideIconPid, treeControl.ParameterId.RawValue));
                        continue;
                    }

                    if (!duplicateTableIds.TryGetValue(table.Id.Value.Value, out List<string> duplicateIds))
                    {
                        duplicateIds = new List<string>();
                        duplicateTableIds.Add(table.Id.Value.Value, duplicateIds);
                    }

                    duplicateIds.Add(overrideIconPid);

                    if (idStatus.HasFlag(GenericStatus.Untrimmed))
                    {
                        subResultsUntrimmed.Add(Error.UntrimmedValueInTag_Sub(this, treeControl, treeControl.OverrideIconColumns, overrideIconPid));
                    }
                }

                foreach (var item in duplicateTableIds)
                {
                    if (item.Value.Count > 1)
                    {
                        IValidationResult error = Error.DuplicateOverrideIconColumns(this, treeControl, treeControl.OverrideIconColumns, Convert.ToString(item.Key), treeControl.ParameterId.RawValue);
                        foreach (string sub in item.Value)
                        {
                            IValidationResult subResult = Error.DuplicateOverrideIconColumns_Sub(this, treeControl, treeControl.OverrideIconColumns, sub);
                            error.WithSubResults(subResult);
                        }

                        results.Add(error);
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
                    IValidationResult error = Error.NonExistingIds(this, treeControl, treeControl.OverrideIconColumns, treeControl.ParameterId.RawValue);
                    error.WithSubResults(subResultsNonExisting.ToArray());
                    results.Add(error);
                }

                if (subResultsUntrimmed.Count > 0)
                {
                    IValidationResult error = Error.UntrimmedTag(this, treeControl, treeControl.OverrideIconColumns, treeControl.ParameterId.RawValue, rawValue);
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

                        editNode.OverrideIconColumns.Value = String.Join(",", readNode.OverrideIconColumns.Value.Split(',').Select(x => x.Trim()));
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