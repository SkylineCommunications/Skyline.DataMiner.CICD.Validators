namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckDisplayColumnAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckDisplayColumnAttribute, Category.Param)]
    internal class CheckDisplayColumnAttribute : ICompare
    {
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

                string newPid = newParam?.Id?.RawValue;

                uint? oldDisplayColumn = oldArrayOptions.DisplayColumn?.Value;
                uint? newDisplayColumn = newArrayOptions.DisplayColumn?.Value;
                uint? oldIndex = oldArrayOptions.Index?.Value;
                uint? newIndex = oldArrayOptions.Index?.Value;

                bool oldHasDisplayColumn = oldDisplayColumn != null;
                bool newHasDisplayColumn = newDisplayColumn != null;
                bool oldHasIndex = oldIndex != null;
                bool newHasIndex = newIndex != null;

                bool hasIndexesThatDidntChange = oldHasIndex && newHasIndex && oldIndex == newIndex;

                if (oldHasDisplayColumn)
                {
                    if (!newHasDisplayColumn && hasIndexesThatDidntChange && oldDisplayColumn != oldIndex)
                    {
                        results.Add(ErrorCompare.DisplayColumnRemoved(newArrayOptions, newArrayOptions, Convert.ToString(oldDisplayColumn), newPid));
                        continue;
                    }

                    if (newHasDisplayColumn && oldDisplayColumn != newDisplayColumn)
                    {
                        results.Add(ErrorCompare.DisplayColumnContentChanged(newArrayOptions, newArrayOptions, Convert.ToString(oldDisplayColumn), newPid, Convert.ToString(newDisplayColumn)));
                        continue;
                    }
                }
                else if (newHasDisplayColumn && hasIndexesThatDidntChange && newDisplayColumn != oldIndex)
                {
                    results.Add(ErrorCompare.DisplayColumnAdded(newArrayOptions, newArrayOptions, Convert.ToString(newDisplayColumn), Convert.ToString(newPid)));
                    continue;
                }
            }

            return results;
        }
    }
}