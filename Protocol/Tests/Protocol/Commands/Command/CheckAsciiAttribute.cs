namespace SLDisValidator2.Tests.Protocol.Commands.Command.CheckAsciiAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Generic;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckAsciiAttribute, Category.Command)]
    public class CheckAsciiAttribute : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (ICommandsCommand command in context.EachCommandWithValidId())
            {
                if (command.Ascii == null)
                {
                    continue;
                }

                (GenericStatus status, string rawValue, string _) = GenericTests.CheckBasics(command.Ascii, isRequired: false);

                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, command, command, command.Id.RawValue));
                    continue;
                }

                if (Boolean.TryParse(command.Ascii.Value, out _))
                {
                    continue;
                }

                List<IValidationResult> asciiParameterResults = new List<IValidationResult>();

                string[] asciiParameters = rawValue.Split(';');
                foreach (string asciiParameter in asciiParameters)
                {
                    (GenericStatus valueStatus, uint _) = GenericTests.CheckBasics<uint>(asciiParameter);

                    if (valueStatus.HasFlag(GenericStatus.Invalid))
                    {
                        asciiParameterResults.Add(Error.InvalidAttribute(this, command, command, asciiParameter, command.Id.RawValue));
                        continue;
                    }

                    if (!context.ProtocolModel.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, asciiParameter, out _))
                    {
                        asciiParameterResults.Add(Error.NonExistingId(this, command, command, asciiParameter, command.Id.RawValue));
                        continue;
                    }
                }

                if (asciiParameterResults.Count > 0)
                {
                    if (asciiParameterResults.Count > 1 || asciiParameters.Length > 1)
                    {
                        IValidationResult invalidAttribute = Error.InvalidAttribute(this, command, command, command.Ascii.Value, command.Id.RawValue);
                        invalidAttribute.WithSubResults(asciiParameterResults.ToArray());
                        results.Add(invalidAttribute);
                    }
                    else
                    {
                        results.Add(asciiParameterResults[0]);
                    }
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