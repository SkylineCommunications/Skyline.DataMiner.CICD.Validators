namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Actions.Action.On.CheckNrAttribute
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckNrAttribute, Category.Action)]
    internal class CheckNrAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var action in context.EachActionWithValidId())
            {
                if (action.Type?.Value == null || action.On?.Value == null)
                {
                    // Invalid Type handled by check on Action/Type
                    // Invalid On handled by check on Action/On
                    continue;
                }

                (GenericStatus status, _, _) = GenericTests.CheckBasics(action.On.Nr, isRequired: false);

                // Missing
                if (action.On.Nr == null)
                {
                    // Not required attribute
                    continue;
                }

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttibute(this, action, action.On.Nr, action.Id.RawValue));
                    continue;
                }

                // Invalid
                if (status.HasFlag(GenericStatus.Invalid))
                {
                    results.Add(Error.InvalidValue(this, action, action.On.Nr, action.On.Nr.RawValue, action.Id.RawValue));
                    continue;
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedAttribute(this, action, action.On.Nr, action.On.Nr.RawValue, action.Id.RawValue));
                    continue;
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (!(context.Result.ReferenceNode is IActionsAction readAction))
            {
                result.Message = "ReferenceNode not of type IActionsAction.";
                return result;
            }

            var editAction = context.Protocol?.Actions?.Get(readAction);
            if (editAction == null)
            {
                result.Message = "editAction is null.";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:

                    editAction.On.Nr.Value = readAction.On.Nr.Value;
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