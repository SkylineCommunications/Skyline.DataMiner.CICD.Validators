namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.Hierarchy.Table.CheckConditionAttribute
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

    [Test(CheckId.CheckConditionAttribute, Category.TreeControl)]
    internal class CheckConditionAttribute : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (ITreeControlsTreeControl treeControl in context.EachTreeControlWithValidParameterId())
            {
                if (context.ProtocolModel.IsExportedProtocolModel && !treeControl.MainParameterExists(context.ProtocolModel))
                {
                    continue;
                }

                if (treeControl.Hierarchy == null)
                {
                    continue;
                }

                foreach (ITreeControlsTreeControlHierarchyTable hierarchyTable in treeControl.Hierarchy)
                {
                    var condition = hierarchyTable.Condition;
                    if (condition == null)
                    {
                        continue;
                    }

                    ValidateHelper helper = new ValidateHelper(this, context, results, treeControl, hierarchyTable);
                    helper.Validate();
                }
            }

            return results;
        }
    }

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly ValidatorContext context;
        private readonly IProtocolModel model;
        private readonly List<IValidationResult> results;

        private readonly ITreeControlsTreeControl treeControl;
        private readonly ITreeControlsTreeControlHierarchyTable hierarchyTable;
        private readonly IValueTag<string> conditionAttribute;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results,
            ITreeControlsTreeControl treeControl, ITreeControlsTreeControlHierarchyTable hierarchyTable)
        {
            this.test = test;
            this.context = context;
            this.model = context.ProtocolModel;
            this.results = results;

            this.treeControl = treeControl;
            this.hierarchyTable = hierarchyTable;
            this.conditionAttribute = hierarchyTable.Condition;
        }

        public void Validate()
        {
            (GenericStatus status, string _, string _) = GenericTests.CheckBasics(conditionAttribute, isRequired: false);

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(test, treeControl, conditionAttribute, treeControl.ParameterId.RawValue));
                return;
            }

            var condition = hierarchyTable.GetCondition();
            if (condition == null)
            {
                return;
            }

            CheckColumnPid(condition);
            CheckFilter(condition);
        }

        private void CheckColumnPid(TableCondition condition)
        {
            string treeControlId = treeControl.ParameterId.RawValue;
            string columnPidRaw = condition.ColumnPidString;

            (GenericStatus status, uint columnPid) = GenericTests.CheckBasics<uint>(columnPidRaw);

            if (status.HasFlag(GenericStatus.Empty) || status.HasFlag(GenericStatus.Invalid))
            {
                IValidationResult error = Error.InvalidValue(test, treeControl, conditionAttribute, condition.ToString(), treeControlId)
                    .WithSubResults(Error.InvalidValueInAttribute_Sub(test, treeControl, conditionAttribute, "<columnPid>", treeControlId, columnPidRaw));

                if (condition.ConditionValue == null)
                {
                    error.WithSubResults(Error.MissingValueInAttribute_Sub(test, treeControl, conditionAttribute, "<filterValue>", treeControlId));
                }

                results.Add(error);
                return;
            }

            if (condition.ConditionValue == null)
            {
                IValidationResult error = Error.InvalidValue(test, treeControl, conditionAttribute, condition.ToString(), treeControlId)
                    .WithSubResults(Error.MissingValueInAttribute_Sub(test, treeControl, conditionAttribute, "<filterValue>", treeControlId));

                results.Add(error);
                return;
            }

            // Non existing column
            if (!model.TryGetObjectByKey(Mappings.ParamsById, Convert.ToString(columnPid), out IParamsParam conditionColumn))
            {
                results.Add(Error.NonExistingId(test, treeControl, conditionAttribute, condition.ColumnPidString));
                return;
            }

            // RTDisplay Expected
            IValidationResult rtDisplayError = Error.ReferencedColumnExpectingRTDisplay(test, treeControl, conditionAttribute, columnPidRaw, treeControlId);
            context.CrossData.RtDisplay.AddParam(conditionColumn, rtDisplayError);

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedColumnPid(test, treeControl, conditionAttribute, columnPidRaw, treeControlId));
            }
        }

        private void CheckFilter(TableCondition condition)
        {
            if (condition.Filter == null)
            {
                return;
            }

            string treeControlId = treeControl.ParameterId.RawValue;
            string columnPidRaw = condition.Filter.ColumnId;

            (GenericStatus status, uint columnPid) = GenericTests.CheckBasics<uint>(columnPidRaw);

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                IValidationResult error = Error.InvalidValue(test, treeControl, conditionAttribute, condition.Filter.ToString(), treeControlId)
                    .WithSubResults(Error.MissingValueInAttribute_Sub(test, treeControl, conditionAttribute, "<filterColumnPid>", treeControlId));

                results.Add(error);
                return;
            }

            // Empty
            if (status.HasFlag(GenericStatus.Invalid))
            {
                IValidationResult error = Error.InvalidValue(test, treeControl, conditionAttribute, condition.Filter.ToString(), treeControlId)
                    .WithSubResults(Error.InvalidValueInAttribute_Sub(test, treeControl, conditionAttribute, "<filterColumnPid>", treeControlId, columnPidRaw));

                results.Add(error);
                return;
            }

            // Non existing FK column
            if (!model.TryGetObjectByKey(Mappings.ParamsById, Convert.ToString(columnPid), out IParamsParam _))
            {
                results.Add(Error.NonExistingId(test, treeControl, conditionAttribute, columnPidRaw));
                return;
            }

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedColumnPid(test, treeControl, conditionAttribute, columnPidRaw, treeControlId));
            }
        }
    }
}