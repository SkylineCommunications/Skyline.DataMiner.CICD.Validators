namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.ColumnOption.CheckIdxAttribute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckIdxAttribute, Category.Param)]
    internal class CheckIdxAttribute : /*IValidate, ICodeFix, */ICompare
    {
        ////public List<IValidationResult> Validate(ValidatorContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {
        ////        default:
        ////            result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
        ////            break;
        ////    }

        ////    return result;
        ////}

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IParamsParam oldParam, IParamsParam newParam) in context.EachMatchingParam())
            {
                string tablePid = newParam.Id?.RawValue;

                bool oldRtDisplay = oldParam.GetRTDisplay();
                bool newRtDisplay = newParam.GetRTDisplay();
                if (!oldRtDisplay || !newRtDisplay)
                {
                    continue;
                }

                if (oldParam.Type?.Value != EnumParamType.Array ||
                    newParam.Type?.Value != EnumParamType.Array)
                {
                    continue;
                }

                var oldSlProtocolColumns = CompareHelper.GetSlProtocolColumns(oldParam);
                var newSlProtocolColumns = CompareHelper.GetSlProtocolColumns(newParam);

                List<IValidationResult> subResults = new List<IValidationResult>();

                foreach ((uint pos, uint pid, string rawPid) in oldSlProtocolColumns)
                {
                    var newColumn = newSlProtocolColumns.FirstOrDefault(c => c.pid == pid);
                    if (String.IsNullOrEmpty(newColumn.rawPid))
                    {
                        continue;
                    }

                    if (pos != newColumn.pos)
                    {
                        subResults.Add(ErrorCompare.UpdatedIdxValue(newParam, newParam, rawPid,
                            Convert.ToString(pos), Convert.ToString(newColumn.pos), tablePid));
                    }
                }

                if (subResults.Count == 1)
                {
                    results.Add(subResults[0]);
                }
                else if (subResults.Count > 1)
                {
                    results.Add(ErrorCompare.UpdatedIdxValue_Parent(newParam, newParam, tablePid).WithSubResults(subResults.ToArray()));
                }
            }

            return results;
        }
    }

    internal static class CompareHelper
    {
        /// <summary>
        /// Retrieves columns known by SLProtocol (ignores columns only known by SLElement such as displaykey columns)
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        internal static (uint pos, uint pid, string rawPid)[] GetSlProtocolColumns(IParamsParam table)
        {
            List<(uint pos, uint pid, string rawPid)> columns = new List<(uint pos, uint pid, string rawPid)>();

            // Get columns included via Param.Type@id
            if (table?.Type?.Id?.Value != null)
            {
                string[] columnPids = table.Type.Id.Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (uint i = 0; i < columnPids.Length; i++)
                {
                    if (!UInt32.TryParse(columnPids[i], out uint columnPid))
                    {
                        continue;
                    }

                    columns.Add((i, columnPid, columnPids[i]));
                }
            }

            // Get columns included via ColumnOption tags
            uint columnsToIgnoreCount = 0;
            var columnOptions = table?.ArrayOptions;
            if (columnOptions == null)
            {
                return columns.OrderBy(c => c.pos).ToArray();
            }

            foreach (var columnOption in columnOptions)
            {
                if (columnOption?.Idx?.Value == null || columnOption.Pid?.Value == null)
                {
                    continue;
                }

                if (columnOption.Type?.Value == EnumColumnOptionType.Displaykey)
                {
                    columnsToIgnoreCount++;
                    continue;
                }

                columns.Add((columnOption.Idx.Value.Value - columnsToIgnoreCount, columnOption.Pid.Value.Value, columnOption.Pid.RawValue));
            }

            return columns.OrderBy(c => c.pos).ToArray();
        }
    }
}