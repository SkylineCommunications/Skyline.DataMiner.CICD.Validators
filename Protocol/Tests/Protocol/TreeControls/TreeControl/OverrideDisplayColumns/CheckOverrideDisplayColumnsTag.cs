namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.OverrideDisplayColumns.CheckOverrideDisplayColumnsTag
{
    using System;
    using System.Collections.Generic;

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

    [Test(CheckId.CheckOverrideDisplayColumnsTag, Category.TreeControl)]
    internal class CheckOverrideDisplayColumnsTag : IValidate, ICodeFix
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            IProtocolModel model = context.ProtocolModel;

            ValidateHelper helper = new ValidateHelper(this, model, results);
            foreach (ITreeControlsTreeControl treeControl in context.EachTreeControlWithValidParameterId())
            {
                if (context.ProtocolModel.IsExportedProtocolModel && !treeControl.MainParameterExists(context.ProtocolModel))
                {
                    continue;
                }

                if (treeControl.OverrideDisplayColumns == null)
                {
                    continue;
                }

                (GenericStatus status, string rawValue, string _) = GenericTests.CheckBasics(treeControl.OverrideDisplayColumns, false);

                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyTag(this, treeControl, treeControl, treeControl.ParameterId.RawValue));
                    continue;
                }

                var tableIds = treeControl.GetDisplayedTablesPids(model);
                helper.BuildData(treeControl, tableIds);

                string[] ids = rawValue.Split(',');
                foreach (string id in ids)
                {
                    ValidateHelper.Column c = helper.BuildColumn(id);

                    if (c == null)
                    {
                        continue;
                    }

                    helper.CheckColumnId(c);
                    helper.CheckSortId(c);
                }

                helper.AddResults(rawValue);
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

                        List<string> trimmed = new List<string>();

                        foreach (var untrimmed in readNode.OverrideDisplayColumns.Value.Split(','))
                        {
                            string[] parts = untrimmed.Split('|');

                            List<string> trimmedParts = new List<string>(parts.Length);
                            foreach (var part in parts)
                            {
                                trimmedParts.Add(part.Trim());
                            }

                            trimmed.Add(String.Join("|", trimmedParts));
                        }

                        string trimmedValue = String.Join(",", trimmed);

                        editNode.OverrideDisplayColumns.Value = trimmedValue;
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

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly IProtocolModel model;
        private readonly List<IValidationResult> results;

        private Data data;

        public ValidateHelper(IValidate test, IProtocolModel model, List<IValidationResult> results)
        {
            this.test = test;
            this.model = model;
            this.results = results;
        }

        public void BuildData(ITreeControlsTreeControl treeControl, ICollection<uint> tableIds)
        {
            data = new Data
            {
                TreeControl = treeControl,
                SubResultsInvalid = new List<IValidationResult>(),
                SubResultsNonExisting = new List<IValidationResult>(),
                SubResultsUntrimmed = new List<IValidationResult>(),
                TablePids = tableIds,
                DuplicateIds = new HashSet<string>(),
                DuplicateTableIds = new Dictionary<uint, List<string>>()
            };
        }

        public Column BuildColumn(string id)
        {
            string[] temp = id.Split('|');
            Column c = new Column();

            switch (temp.Length)
            {
                case 1:
                    c.HasSortId = false;
                    c.Id = temp[0];
                    break;
                case 2:
                    c.HasSortId = true;
                    c.Id = temp[0];
                    c.SortId = temp[1];
                    break;
                default:
                    data.SubResultsInvalid.Add(Error.InvalidValueInTag_Sub(test, data.TreeControl, data.TreeControl.OverrideDisplayColumns, id));
                    return null;
            }

            return c;
        }

        public void CheckSortId(Column c)
        {
            if (!c.HasSortId)
            {
                return;
            }

            var treeControl = data.TreeControl;
            string treeControlId = data.TreeControl.ParameterId.RawValue;

            (GenericStatus idStatus, uint idValue) = GenericTests.CheckBasics<uint>(c.SortId);

            if (idStatus.HasFlag(GenericStatus.Empty) || idStatus.HasFlag(GenericStatus.Invalid))
            {
                data.SubResultsInvalid.Add(Error.InvalidValueInTag_Sub(test, treeControl, treeControl.OverrideDisplayColumns, c.SortId));
                return;
            }

            if (data.DuplicateIds.Contains(c.SortId))
            {
                results.Add(Error.DuplicateId(test, treeControl, treeControl.OverrideDisplayColumns, c.SortId, treeControlId));
                return;
            }

            data.DuplicateIds.Add(c.SortId);

            // Check if parameter links to an existing Table.
            var paramIdString = Convert.ToString(idValue);
            if (!model.TryGetObjectByKey(Mappings.ParamsById, paramIdString, out IParamsParam column))
            {
                data.SubResultsNonExisting.Add(Error.NonExistingIds_Sub(test, treeControl, treeControl.OverrideDisplayColumns, paramIdString, treeControlId));
                return;
            }

            // ID exists, but does it belong to a table that is part of the TreeControl?
            if (!column.TryGetTable(model.RelationManager, out IParamsParam table) || table.Id.Value == null || !data.TablePids.Contains(table.Id.Value.Value))
            {
                data.SubResultsInvalid.Add(Error.InvalidValueInTag_Sub(test, treeControl, treeControl.OverrideDisplayColumns, c.SortId));
                return;
            }

            if (idStatus.HasFlag(GenericStatus.Untrimmed))
            {
                data.SubResultsUntrimmed.Add(Error.UntrimmedValueInTag_Sub(test, treeControl, treeControl.OverrideDisplayColumns, c.SortId));
            }
        }

        public void CheckColumnId(Column c)
        {
            var treeControl = data.TreeControl;
            string treeControlId = data.TreeControl.ParameterId.RawValue;
            (GenericStatus idStatus, uint idValue) = GenericTests.CheckBasics<uint>(c.Id);

            if (idStatus.HasFlag(GenericStatus.Empty) || idStatus.HasFlag(GenericStatus.Invalid))
            {
                data.SubResultsInvalid.Add(Error.InvalidValueInTag_Sub(test, treeControl, treeControl.OverrideDisplayColumns, c.Id));
                return;
            }

            if (data.DuplicateIds.Contains(c.Id))
            {
                results.Add(Error.DuplicateId(test, treeControl, treeControl.OverrideDisplayColumns, c.Id, treeControlId));
                return;
            }

            data.DuplicateIds.Add(c.Id);

            // Check if parameter links to an existing Table.
            var paramIdString = Convert.ToString(idValue);
            if (!model.TryGetObjectByKey(Mappings.ParamsById, paramIdString, out IParamsParam column))
            {
                data.SubResultsNonExisting.Add(Error.NonExistingIds_Sub(test, treeControl, treeControl.OverrideDisplayColumns, paramIdString, treeControlId));
                return;
            }

            // ID exists, but does it belong to a table that is part of the TreeControl?
            if (!column.TryGetTable(model.RelationManager, idValue, out IParamsParam table) || table.Id.Value == null || !data.TablePids.Contains(table.Id.Value.Value))
            {
                results.Add(Error.IrrelevantColumn(test, treeControl, treeControl.OverrideDisplayColumns, c.Id, treeControlId));
                return;
            }

            if (!data.DuplicateTableIds.TryGetValue(table.Id.Value.Value, out List<string> duplicateIds))
            {
                duplicateIds = new List<string>
                {
                    c.Id
                };
                data.DuplicateTableIds.Add(table.Id.Value.Value, duplicateIds);
            }
            else
            {
                duplicateIds.Add(c.Id);
            }

            if (idStatus.HasFlag(GenericStatus.Untrimmed))
            {
                data.SubResultsUntrimmed.Add(Error.UntrimmedValueInTag_Sub(test, treeControl, treeControl.OverrideDisplayColumns, c.Id));
            }
        }

        public void AddResults(string rawValue)
        {
            var treeControl = data.TreeControl;
            string treeControlId = data.TreeControl.ParameterId.RawValue;
            foreach (var item in data.DuplicateTableIds)
            {
                if (item.Value.Count <= 1)
                {
                    continue;
                }

                IValidationResult error = Error.DuplicateOverrideDisplayColumn(test, treeControl, treeControl.OverrideDisplayColumns, Convert.ToString(item.Key), treeControlId);
                foreach (string sub in item.Value)
                {
                    IValidationResult subResult = Error.DuplicateOverrideDisplayColumns_Sub(test, treeControl, treeControl.OverrideDisplayColumns, sub);
                    error.WithSubResults(subResult);
                }

                results.Add(error);
            }

            if (data.SubResultsInvalid.Count > 0)
            {
                IValidationResult error = Error.InvalidValue(null, null, null, rawValue, treeControlId);
                error.WithSubResults(data.SubResultsInvalid.ToArray());
                results.Add(error);
            }

            if (data.SubResultsNonExisting.Count > 0)
            {
                IValidationResult error = Error.NonExistingIds(test, treeControl, treeControl.OverrideDisplayColumns, treeControlId);
                error.WithSubResults(data.SubResultsNonExisting.ToArray());
                results.Add(error);
            }

            if (data.SubResultsUntrimmed.Count > 0)
            {
                IValidationResult error = Error.UntrimmedTag(test, treeControl, treeControl.OverrideDisplayColumns, treeControlId, rawValue);
                error.WithSubResults(data.SubResultsUntrimmed.ToArray());
                results.Add(error);
            }
        }

        internal class Column
        {
            public string Id { get; set; }

            public string SortId { get; set; }

            public bool HasSortId { get; set; }
        }

        private sealed class Data
        {
            public List<IValidationResult> SubResultsUntrimmed { get; set; }

            public List<IValidationResult> SubResultsInvalid { get; set; }

            public List<IValidationResult> SubResultsNonExisting { get; set; }

            public ICollection<uint> TablePids { get; set; }

            public Dictionary<uint, List<string>> DuplicateTableIds { get; set; }

            public HashSet<string> DuplicateIds { get; set; }

            public ITreeControlsTreeControl TreeControl { get; set; }
        }
    }
}