namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Commands.Command.CheckAsciiAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckAsciiAttribute, Category.Command)]
    internal class CheckAsciiAttribute : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            ValidateHelper helper = new ValidateHelper(this, context, results);
            foreach (ICommandsCommand command in context.EachCommandWithValidId())
            {
                if (command.Ascii == null)
                {
                    continue;
                }

                helper.ValidateAttribute(command);
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

    internal class ValidateHelper : ValidateHelperBase
    {
        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results) : base(test, context, results)
        {
        }

        public void ValidateAttribute(ICommandsCommand command)
        {
            (GenericStatus status, string rawValue, string _) = GenericTests.CheckBasics(command.Ascii, isRequired: false);

            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(test, command, command, command.Id.RawValue));
                return;
            }

            if (Boolean.TryParse(command.Ascii.Value, out _))
            {
                return;
            }

            ValidatePartsOfAttribute(command, rawValue);
        }

        private void ValidatePartsOfAttribute(ICommandsCommand command, string rawValue)
        {
            List<IValidationResult> asciiParameterResults = new List<IValidationResult>();

            string[] asciiParameters = rawValue.Split(';');
            foreach (string asciiParameter in asciiParameters)
            {
                (GenericStatus valueStatus, uint _) = GenericTests.CheckBasics<uint>(asciiParameter);

                if (valueStatus.HasFlag(GenericStatus.Invalid))
                {
                    asciiParameterResults.Add(Error.InvalidAttribute(test, command, command, asciiParameter, command.Id.RawValue));
                    continue;
                }

                if (!context.ProtocolModel.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, asciiParameter, out _))
                {
                    asciiParameterResults.Add(Error.NonExistingId(test, command, command, asciiParameter, command.Id.RawValue));
                    continue;
                }
            }

            if (asciiParameterResults.Count <= 0)
            {
                return;
            }

            if (asciiParameterResults.Count > 1 || asciiParameters.Length > 1)
            {
                IValidationResult invalidAttribute = Error.InvalidAttribute(test, command, command, command.Ascii.Value, command.Id.RawValue);
                invalidAttribute.WithSubResults(asciiParameterResults.ToArray());
                results.Add(invalidAttribute);
            }
            else
            {
                results.Add(asciiParameterResults[0]);
            }
        }
    }
}