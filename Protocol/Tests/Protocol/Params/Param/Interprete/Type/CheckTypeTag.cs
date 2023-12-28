namespace SLDisValidator2.Tests.Protocol.Params.Param.Interprete.Type.CheckTypeTag
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Interfaces;

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

                var oldInterprete = oldParam?.Interprete;
                var newInterprete = newParam?.Interprete;

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
                    results.Add(ErrorCompare.RemovedTag(newInterprete, newInterprete, newParam.Id?.RawValue));
                    continue;
                }

                {
                    string oldTypeStringified = EnumParamInterpretTypeConverter.ConvertBack(oldType.Value);
                    string newTypeStringified = EnumParamInterpretTypeConverter.ConvertBack(newType.Value);
                    results.Add(ErrorCompare.UpdatedValue(newInterprete, newInterprete, newParam.Id?.RawValue, oldTypeStringified, newTypeStringified));
                }
            }

            return results;
        }
    }
}