namespace SLDisValidator2.Tests.Protocol.Pairs.Pair.Content.Command.CheckCommandTag
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Generic;
    using SLDisValidator2.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    [Test(CheckId.CheckCommandTag, Category.Pair)]
    public class CheckCommandTag : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IPairsPair pair in context.EachPairWithValidId())
            {
                if (pair.Content == null)
                {
                    continue;
                }

                var command = pair.Content.Command;

                (GenericStatus status, string rawId, uint? id) = GenericTests.CheckBasics(command, isRequired: true);

                // Missing
                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingTag(this, command, command, pair.Id.RawValue));
                    continue;
                }

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyTag(this, command, command, pair.Id.RawValue));
                    continue;
                }

                // Invalid
                if (status.HasFlag(GenericStatus.Invalid) || !GenericTests.IsPlainNumbers(rawId))
                {
                    results.Add(Error.InvalidValue(this, command, command, rawId, pair.Id.RawValue));
                    continue;
                }

                // Non-Existing Command
                if (!context.ProtocolModel.TryGetObjectByKey<ICommandsCommand>(Mappings.CommandsById, Convert.ToString(id), out _))
                {
                    results.Add(Error.NonExistingId(this, command, command, rawId, pair.Id.RawValue));
                    continue;
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedTag(this, command, command, pair.Id.RawValue, rawId));
                    continue;
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedTag:
                    if (!(context.Result.ReferenceNode is IPairsPairContentCommand readCommand))
                    {
                        result.Message = "context.Result.Node is not of type IPairsPairContentCommand";
                        break;
                    }

                    if (context.Protocol?.Pairs == null)
                    {
                        result.Message = "context.Protocol.Pairs == null";
                        break;
                    }

                    foreach (var pair in context.Protocol.Pairs)
                    {
                        var editCommand = pair.Content.Get(readCommand) as Skyline.DataMiner.CICD.Models.Protocol.Edit.PairsPairContentCommand;
                        if (editCommand == null)
                        {
                            continue;
                        }

                        editCommand.RawValue = editCommand.RawValue.Trim();
                        result.Success = true;
                    }

                    if (!result.Success)
                    {
                        result.Message = "Matching 'Content/Command' tag not found.";
                    }

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