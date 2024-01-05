namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ParameterGroups.Group.Params.Param.CheckIdAttribute
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckIdAttribute, Category.ParameterGroup)]
    internal class CheckIdAttribute : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var model = context.ProtocolModel;

            var parameterGroups = model.Protocol?.ParameterGroups;
            if (parameterGroups == null)
            {
                return results;
            }

            foreach (IParameterGroupsGroup parameterGroup in context.EachParameterGroupWithValidId())
            {
                var pgParams = parameterGroup.Params;
                if (pgParams == null)
                {
                    continue;
                }

                // Check if all parameters in a ParameterGroup link to an existing Param.
                foreach (var pgParam in pgParams)
                {
                    var paramIdString = Convert.ToString(pgParam.Id?.Value);
                    if (!model.TryGetObjectByKey(Mappings.ParamsById, paramIdString, out IParamsParam _))
                    {
                        results.Add(Error.NonExistingId(this, parameterGroup, pgParam, paramIdString, parameterGroup.Id.RawValue));
                    }

                    // TODO: param should have Alarm/Monitored="true"

                    // TODO: Should we add it to the params requiring RTDisplay ?
                    // - In a way yes, cause it does require RTDisplay=true
                    // - In a way no, cause it would already be covered by cascade effect since it requires Alarm/Monitored="true"
                    // => After discussing it, we decide to also check the RTDisplay. It will provide more info to the user in one go.
                    // => And the "duplicate results" won't hurt as all reasons for RTDisplay are grouped in a parent result.
                }

                // Check if the same Param is not added more than once in a ParameterGroup.
                foreach (var duplicates in pgParams.GroupBy(p => p.Id?.Value).Where(g => g.Count() > 1))
                {
                    var paramIdString = Convert.ToString(duplicates.Key);

                    IValidationResult duplicateParamInParameterGroup = Error.DuplicateParamInParameterGroup(this, parameterGroup, parameterGroup, paramIdString, parameterGroup.Id.RawValue);

                    foreach (var pgParam in duplicates)
                    {
                        IValidationResult subResult = Error.DuplicateParamInParameterGroup(this, parameterGroup, pgParam, paramIdString, parameterGroup.Id.RawValue);
                        duplicateParamInParameterGroup.WithSubResults(subResult);
                    }

                    results.Add(duplicateParamInParameterGroup);
                }
            }

            return results;
        }

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {

        ////        default:
        ////            result.Message = String.Format("This error ({0}) isn't implemented.", context.Result.ErrorId.ToString());
        ////            break;
        ////    }

        ////    return result;
        ////}

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }
}