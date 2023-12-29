namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ParameterGroups.Group.CheckDynamicIdAttribute
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckDynamicIdAttribute, Category.ParameterGroup)]
    internal class CheckDynamicIdAttribute : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParameterGroupsGroup parameterGroup in context.EachParameterGroupWithValidId())
            {
                (GenericStatus status, string rawValue, uint? _) = GenericTests.CheckBasics(parameterGroup.DynamicId, isRequired: true);

                if (status.HasFlag(GenericStatus.Missing))
                {
                    continue;
                }

                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, parameterGroup, parameterGroup, parameterGroup.Id.RawValue));
                    continue;
                }

                if (status.HasFlag(GenericStatus.Invalid))
                {
                    results.Add(Error.InvalidAttribute(this, parameterGroup, parameterGroup, rawValue, parameterGroup.Id.RawValue));
                    continue;
                }

                // it's normal that parameter doesn't exist if model is for exported protocol
                if (!context.ProtocolModel.IsExportedProtocolModel)
                {
                    if (!context.ProtocolModel.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, rawValue, out _))
                    {
                        results.Add(Error.NonExistingId(this, parameterGroup, parameterGroup, rawValue, parameterGroup.Id.RawValue));
                        continue;
                    }

                    // TODO: Referred param should either be a table or a matrix

                    // TODO: Referred table/matrix should have RTDisplay true 
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