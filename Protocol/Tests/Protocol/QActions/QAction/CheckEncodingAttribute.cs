namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CheckEncodingAttribute
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckEncodingAttribute, Category.QAction)]
    internal class CheckEncodingAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IQActionsQAction qaction in context.EachQActionWithValidId())
            {
                (GenericStatus status, _, EnumQActionEncoding? value) = GenericTests.CheckBasics(qaction.Encoding, isRequired: true);

                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingAttribute(this, qaction, qaction, qaction.Id.RawValue));
                    continue;
                }

                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, qaction, qaction, qaction.Id.RawValue));
                    continue;
                }

                if (status.HasFlag(GenericStatus.Invalid))
                {
                    results.Add(Error.InvalidValue(this, qaction, qaction, qaction.Encoding.RawValue, qaction.Id.RawValue));
                    continue;
                }

                if (value != EnumQActionEncoding.Csharp)
                {
                    results.Add(Error.UnsupportedValue(this, qaction, qaction.Encoding, qaction.Encoding.RawValue, qaction.Id.RawValue));
                }

                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedAttribute(this, qaction, qaction, qaction.Id.RawValue, qaction.Encoding.RawValue));
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    if (!(context.Result.ReferenceNode is IQActionsQAction readQAction))
                    {
                        break;
                    }

                    var editQAction = context.Protocol?.QActions?.Get(readQAction);
                    if (editQAction == null)
                    {
                        break;
                    }

                    editQAction.Encoding.Value = editQAction.Encoding.Value;
                    result.Success = true;
                    break;

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