namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Name.CheckNameTag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Edit;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckNameTag, Category.Protocol)]
    internal class CheckNameTag : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var model = context.ProtocolModel;

            if (model.Protocol == null)
            {
                return results;
            }

            var name = model.Protocol.Name;

            (GenericStatus status, _, _) = GenericTests.CheckBasics(name, true);

            // Check if Tag is there
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingTag(this, model.Protocol, model.Protocol));
                return results;
            }

            // Check if Tag is empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyTag(this, name, name));
                return results;
            }

            // Check if Tag starts/ends with whitespace
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedTag(this, name, name, name.RawValue));
                return results;
            }

            // Check if Tag doesn't contain invalid characters
            IList<char> invalidCharacters = Helper.CheckInvalidChars(name.Value, ProtocolHelper.RestrictedProtocolNameChars).ToList();
            if (invalidCharacters.Any())
            {
                results.Add(Error.InvalidChars(this, name, name, name.RawValue, String.Join(" ", invalidCharacters)));
                return results;
            }

            // Check if Tag does not contain forbidden prefixes
            string invalidPrefix = ProtocolHelper.GetProtocolNameInvalidPrefix(name.Value);
            if (invalidPrefix != null)
            {
                results.Add(Error.InvalidPrefix(this, name, name, name.RawValue, invalidPrefix));
                return results;
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            ElementValue<string> editNode = context.Protocol.GetOrCreateName();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedTag:
                    {
                        editNode.Value = editNode.Value.Trim();
                        result.Success = true;
                        break;
                    }

                case ErrorIds.InvalidChars:
                    {
                        editNode.Value = ProtocolHelper.ReplaceProtocolNameInvalidChars(editNode.Value);
                        result.Success = true;
                        break;
                    }

                case ErrorIds.InvalidPrefix:
                    {
                        editNode.Value = ProtocolHelper.RemoveProtocolNameInvalidPrefix(editNode.Value);
                        result.Success = true;
                        break;
                    }

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var oldProtocolNameTag = context.PreviousProtocolModel.Protocol?.Name;
            var newProtocolNameTag = context.NewProtocolModel.Protocol?.Name;

            if (oldProtocolNameTag == null || newProtocolNameTag == null)
            {
                return results;
            }

            string oldName = oldProtocolNameTag.Value;
            string newName = newProtocolNameTag.Value;

            if (!String.Equals(oldName, newName, StringComparison.Ordinal))
            {
                results.Add(ErrorCompare.UpdatedValue(newProtocolNameTag, newProtocolNameTag, oldName, newName));
            }

            return results;
        }
    }
}