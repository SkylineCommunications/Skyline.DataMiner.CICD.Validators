namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckArrayOptionsTag
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

    [Test(CheckId.CheckArrayOptionsTag, Category.Param)]
    internal class CheckArrayOptionsTag : ICompare
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

                uint? oldDisplayColumn = oldArrayOptions.DisplayColumn?.Value;
                string oldNamingFormat = oldArrayOptions.NamingFormat?.Value;
                string newNamingFormat = newArrayOptions.NamingFormat?.Value;

                var newNaming = newArrayOptions.GetOptions()?.Naming;

                bool oldHasDisplayColumn = oldDisplayColumn != null;

                bool oldHasNaming = oldArrayOptions.GetOptions()?.Naming != null;
                bool newHasNaming = newNaming != null;
                bool oldHasNamingFormat = !String.IsNullOrEmpty(oldNamingFormat);
                bool newHasNamingFormat = !String.IsNullOrEmpty(newNamingFormat);

                if (!oldHasDisplayColumn || oldHasNaming || oldHasNamingFormat)
                {
                    continue;
                }

                // Old only has displayColumn
                // In priority of overwrite behavior. NamingFormat overrides naming, and naming overrides displayColumn.
                if (newHasNamingFormat)
                {
                    results.Add(ErrorCompare.DisplayColumnChangeToNamingFormat(newArrayOptions, newArrayOptions, Convert.ToString(oldDisplayColumn), newParam.Id?.RawValue, newNamingFormat));
                    continue;
                }

                if (newHasNaming)
                {
                    results.Add(ErrorCompare.DisplayColumnChangedToNaming(newArrayOptions, newArrayOptions, Convert.ToString(oldDisplayColumn), newParam.Id?.RawValue, newNaming.ToString()));
                    continue;
                }
            }

            return results;
        }
    }
}