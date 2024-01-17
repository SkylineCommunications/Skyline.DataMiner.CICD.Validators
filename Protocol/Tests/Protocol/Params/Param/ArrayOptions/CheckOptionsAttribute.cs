namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckOptionsAttribute
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckOptionsAttribute, Category.Param)]
    internal class CheckOptionsAttribute : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.ArrayOptions == null)
                {
                    continue;
                }

                ValidateHelper helper = new ValidateHelper(this, context, results, param);
                helper.Validate();
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol?.Params == null)
            {
                result.Message = "No Params found!";
                return result;
            }

            var paramReadNode = (IParamsParam)context.Result.ReferenceNode;
            var paramEditNode = context.Protocol.Params.Get(paramReadNode);
            if (paramEditNode == null)
            {
                result.Message = "No Param Edit Node found!";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.EmptyAttribute:
                    if (paramEditNode.ArrayOptions?.Options == null)
                    {
                        result.Message = "Options was already removed.";
                        result.Success = true;
                        break;
                    }

                    paramEditNode.ArrayOptions.Options = null;
                    result.Success = true;
                    break;
                case ErrorIds.UntrimmedAttribute:
                    paramEditNode.ArrayOptions.Options = paramReadNode.ArrayOptions.Options.Value;
                    result.Success = true;
                    break;
                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IParamsParam oldParam, IParamsParam newParam) in context.EachMatchingParam())
            {
                var oldArrayOptions = oldParam?.ArrayOptions;
                var newArrayOptions = newParam?.ArrayOptions;
                if (oldArrayOptions == null || newArrayOptions == null)
                {
                    continue;
                }

                results.AddIfNotNull(CheckDatabaseOption(oldArrayOptions, newArrayOptions, newParam));
            }

            return results;

            IValidationResult CheckDatabaseOption(IParamsParamArrayOptions oldArrayOptions, IParamsParamArrayOptions newArrayOptions, IParamsParam newParam)
            {
                var oldDatabaseOption = oldArrayOptions?.GetOptions()?.Database;
                var newDatabaseOption = newArrayOptions?.GetOptions()?.Database;

                if (oldDatabaseOption != null && newDatabaseOption == null)
                {
                    return ErrorCompare.RemovedLoggerTableDatabaseLink(newParam, newParam, newParam.Id?.RawValue);
                }

                return null;
            }
        }
    }

    internal class ValidateHelper : ValidateHelperBase
    {
        private readonly IProtocolModel model;

        private readonly IParamsParam tableParam;
        private readonly ArrayOptionsOptions options;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam tableParam)
            : base(test, context, results)
        {
            model = context.ProtocolModel;

            this.tableParam = tableParam;
            options = tableParam.ArrayOptions.GetOptions();
        }

        public void Validate()
        {
            (GenericStatus status, string optionsRaw, string _) = GenericTests.CheckBasics(tableParam.ArrayOptions.Options, isRequired: false);

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(test, tableParam, tableParam.ArrayOptions, tableParam.Id.RawValue));
                return;
            }

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(test, tableParam, tableParam.ArrayOptions, tableParam.Id?.RawValue, optionsRaw));
            }

            if (options == null)
            {
                return;
            }

            ValidateNamingOption();
            ValidatePreserveStateOption();
            ValidateViewOption();
            ValidateFilterChangeOption();
            ValidateDirectViewOption();
        }

        private void ValidateNamingOption()
        {
            if (options.Naming == null)
            {
                return;
            }

            // Empty
            if (!options.Naming.Separator.HasValue
                || options.Naming.OriginalValue.Length <= "naming=/".Length)
            {
                // If the length of it is only 1, it means that only a separator was defined which can be considered as empty naming option.
                results.Add(Error.NamingEmpty(test, tableParam, tableParam.ArrayOptions, tableParam.Id.RawValue));
                return;
            }

            string[] namingParts = options.Naming.OriginalValue.Substring("naming=/".Length).Split(',');
            foreach (var namingPart in namingParts)
            {
                // Non-Existing reference
                if (!model.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, namingPart, out _))
                {
                    results.Add(Error.NamingRefersToNonExistingParam(test, tableParam, tableParam.ArrayOptions, namingPart, tableParam.Id.RawValue));
                }
            }
        }

        private void ValidatePreserveStateOption()
        {
            if (options.HasPreserveState)
            {
                results.Add(Error.PreserveStateShouldBeAvoided(test, tableParam, tableParam.ArrayOptions, tableParam.Id?.RawValue));
            }
        }

        private void ValidateViewOption()
        {
            if (options.View == null)
            {
                return;
            }

            // Syntax: view=1000 or view=1000,remoteId
            // TODO: Check if there is an id and if uint.
            // TODO: use View.IsValid to check if no extra stuff has been added (added , but no remoteId or something)

            if (options.View.HasRemoteId)
            {
                // In this case the ID refers to a table in another protocol
                return;
            }

            string viewId = Convert.ToString(options.View.Pid);
            if (!model.TryGetObjectByKey(Mappings.ParamsById, viewId, out IParamsParam param))
            {
                results.Add(Error.ViewTableInvalidReference(test, tableParam, tableParam.ArrayOptions, Severity.Major, viewId, tableParam.Id?.RawValue));
            }
            else if (Equals(param, tableParam))
            {
                results.Add(Error.ViewTableInvalidReference(test, tableParam, tableParam.ArrayOptions, Severity.Critical, viewId, tableParam.Id?.RawValue));
            }
        }

        private void ValidateFilterChangeOption()
        {
            if (options.FilterChange == null)
            {
                return;
            }

            // Syntax: filterChange=19901-1501,19902-1502,19903-1503,19904-1504,19905-1505
            // First part of each tuple must be a column of the table

            // Retrieve all column pids
            HashSet<uint> allColumnPids = new HashSet<uint>();

            foreach (var columnOption in tableParam.ArrayOptions)
            {
                uint? pid = columnOption.Pid?.Value;
                if (pid != null)
                {
                    allColumnPids.Add(pid.Value);
                }
            }

            // Compare
            List<string> invalidColumnPids = new List<string>();

            foreach ((uint? localId, uint? _, bool _) in options.FilterChange.Pairs)
            {
                string columnPid = String.Empty;
                if (localId == null)
                {
                    invalidColumnPids.Add(columnPid);
                }
                else if (!allColumnPids.Contains(localId.Value))
                {
                    columnPid = Convert.ToString(localId);
                    invalidColumnPids.Add(columnPid);
                }
            }

            // Results
            if (invalidColumnPids.Count > 0)
            {
                IValidationResult viewTableFilterChangeInvalidColumns = Error.ViewTableFilterChangeInvalidColumns(test, tableParam, tableParam.ArrayOptions, String.Join(", ", invalidColumnPids), tableParam.Id?.RawValue);

                if (invalidColumnPids.Count > 1)
                {
                    foreach (string item in invalidColumnPids)
                    {
                        IValidationResult subResult = Error.ViewTableFilterChangeInvalidColumns(test, tableParam, tableParam.ArrayOptions, item, tableParam.Id?.RawValue);
                        viewTableFilterChangeInvalidColumns.WithSubResults(subResult);
                    }
                }

                results.Add(viewTableFilterChangeInvalidColumns);
            }
        }

        private void ValidateDirectViewOption()
        {
            if (options.DirectView == null)
            {
                return;
            }

            // Syntax: directView=5361
            string directViewPid = String.Empty;
            if (options.DirectView.Pid != null)
            {
                directViewPid = Convert.ToString(options.DirectView.Pid);
            }

            if (!ParamHelper.TryFindTableParamForColumnPid(model, directViewPid, out IParamsParam directViewBaseTableParam) || directViewBaseTableParam == tableParam)
            {
                results.Add(Error.ViewTableDirectViewInvalidColumn(test, tableParam, tableParam.ArrayOptions, directViewPid, tableParam.Id?.RawValue));
            }
        }
    }
}