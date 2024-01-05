namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.CheckLoadSequenceAttribute
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckLoadSequenceAttribute, Category.Param)]
    internal class CheckLoadSequenceAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (context.ProtocolModel?.Protocol?.Params?.LoadSequence == null)
            {
                return results;
            }

            var model = context.ProtocolModel;
            var paramsElement = model.Protocol.Params;
            var loadSequence = paramsElement.LoadSequence;
            (GenericStatus status, string rawValue, string value) = GenericTests.CheckBasics(loadSequence, isRequired: false);

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(this, paramsElement, loadSequence));
                return results;
            }

            string[] loadSequencePids = value.Split(';');
            foreach (string pid in loadSequencePids)
            {
                // Non Existing Param
                if (!model.TryGetObjectByKey(Mappings.ParamsById, pid, out IParamsParam param))
                {
                    results.Add(Error.NonExistingId(this, paramsElement, loadSequence, pid));
                    continue;
                }

                // Unsaved Param
                if (param.Save?.Value != true)
                {
                    results.Add(Error.ReferencedParamSaveExpected(this, paramsElement, loadSequence, pid));
                }

                // Param Requiring RTDisplay
                IValidationResult rtDisplayError = Error.ReferencedParamRTDisplayExpected(this, paramsElement, loadSequence, pid);
                context.CrossData.RtDisplay.AddParam(param, rtDisplayError);
            }

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(this, paramsElement, loadSequence, rawValue));
                return results;
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    {
                        var readNode = (IParams)context.Result.ReferenceNode;
                        var editNode = context.Protocol.Params;

                        editNode.LoadSequence.Value = readNode.LoadSequence.Value;
                        result.Success = true;
                        break;
                    }

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }
}