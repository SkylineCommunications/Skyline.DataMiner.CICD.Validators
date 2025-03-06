namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.Type.CheckTypeTag
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckTypeTag, Category.Param)]
    internal class CheckTypeTag : ICompare
    {
        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IParamsParam oldParam, IParamsParam newParam) in context.EachMatchingParam())
            {
                // Exclude Dummy and write Parameters
                if (oldParam.Type?.Value == EnumParamType.Array ||
                    oldParam.Type?.Value == EnumParamType.Dummy ||
                    oldParam.Type?.Value == EnumParamType.Write ||
                    oldParam.Type?.Value == EnumParamType.WriteBit)
                {
                    continue;
                }

                var oldInterprete = oldParam.Interprete;
                var newInterprete = newParam.Interprete;

                EnumParamInterpretType? oldType = oldInterprete?.Type?.Value;
                EnumParamInterpretType? newType = newInterprete?.Type?.Value;

                if (newType == oldType)
                {
                    continue;
                }

                if (oldType == null)
                {
                    string newTypeStringified = EnumParamInterpretTypeConverter.ConvertBack(newType.Value);
                    results.Add(ErrorCompare.AddedTag(newInterprete, newInterprete, newTypeStringified, newParam.Id?.RawValue));
                    continue;
                }

                if (newType == null)
                {
                    IReadable referenceNode = (IReadable)newInterprete ?? newParam;
                    results.Add(ErrorCompare.RemovedTag(referenceNode, referenceNode, newParam.Id?.RawValue));
                    continue;
                }

                string oldTypeString = EnumParamInterpretTypeConverter.ConvertBack(oldType.Value);
                string newTypeString = EnumParamInterpretTypeConverter.ConvertBack(newType.Value);
                results.Add(ErrorCompare.UpdatedValue(newInterprete, newInterprete, newParam.Id?.RawValue, oldTypeString, newTypeString));
            }

            return results;
        }
    }
}