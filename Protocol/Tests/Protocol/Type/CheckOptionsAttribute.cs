namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Type.CheckOptionsAttribute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckOptionsAttribute, Category.Protocol)]
    internal class CheckOptionsAttribute : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var typeTag = context.ProtocolModel?.Protocol?.Type;
            var optionsAttribute = typeTag?.Options;
            if (optionsAttribute == null)
            {
                return results;
            }

            (GenericStatus status, string optionsRawValue, string _) = GenericTests.CheckBasics(optionsAttribute, isRequired: false);

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(this, typeTag, optionsAttribute));
                return results;
            }

            // Further Validation
            ValidateHelper helper = new ValidateHelper(this, context, results, typeTag);
            helper.ValidateExportProtocol();

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(this, typeTag, optionsAttribute, optionsRawValue));
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.EmptyAttribute:
                    {
                        var editNode = context.Protocol.Type;

                        editNode.Options = null;
                        result.Success = true;
                        break;
                    }

                case ErrorIds.UntrimmedAttribute:
                    {
                        var readNode = (IProtocolType)context.Result.ReferenceNode;
                        var editNode = context.Protocol.Type;

                        editNode.Options.Value = readNode.Options.Value;
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
            var oldProtocol = context.PreviousProtocolModel.Protocol;
            var newProtocol = context.NewProtocolModel.Protocol;
            if (oldProtocol == null || newProtocol == null)
            {
                return results;
            }

            var oldType = oldProtocol.Type;
            var newType = newProtocol.Type;

            ProtocolTypeOptions oldOptions = oldType?.GetOptions();
            ProtocolTypeOptions newOptions = newType?.GetOptions();

            CompareHelper helper = new CompareHelper(results, oldType, oldOptions, newType, newOptions);
            helper.CheckExportProtocol();
            helper.CheckUnicode();

            return results;
        }
    }

    internal class ValidateHelper : ValidateHelperBase
    {
        private readonly IProtocolModel model;

        private readonly IProtocolType protocolTypeTag;
        private readonly IValueTag<string> optionsAttribute;
        private readonly ProtocolTypeOptions options;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IProtocolType protocolTypeTag)
            : base(test, context, results)
        {
            model = context.ProtocolModel;

            this.protocolTypeTag = protocolTypeTag;
            optionsAttribute = protocolTypeTag.Options;
            options = protocolTypeTag.GetOptions();
        }

        public void ValidateExportProtocol()
        {
            foreach (ExportProtocol exportProtocol in options.ExportProtocols)
            {
                if (String.IsNullOrWhiteSpace(exportProtocol.Name) || exportProtocol.TablePid == null)
                {
                    // TODO: Invalid exportProtocol option
                    continue;
                }

                ValidateDveProtocolName(exportProtocol.Name);
                ValidateDveTable(exportProtocol.TablePid.Value);
            }
        }

        private void ValidateDveProtocolName(string dveProtocolName)
        {
            // TODO: implement
        }

        private void ValidateDveTable(uint dveTablePid)
        {
            string dveTablePidString = dveTablePid.ToString();

            // Non Existing Param
            if (!model.TryGetObjectByKey(Mappings.ParamsById, dveTablePidString, out IParamsParam dveParam))
            {
                results.Add(Error.NonExistingId(test, protocolTypeTag, optionsAttribute, dveTablePidString));
                return;
            }

            // Param Wrong Type
            if (!dveParam.IsTable())
            {
                results.Add(Error.ReferencedParamWrongType(test, protocolTypeTag, optionsAttribute, dveParam.Type?.RawValue, dveTablePidString));
            }

            // Param Requiring RTDisplay
            IValidationResult rtDisplayError = Error.ReferencedParamExpectingRTDisplay(test, protocolTypeTag, optionsAttribute, dveTablePidString);
            context.CrossData.RtDisplay.AddParam(dveParam, rtDisplayError);
        }
    }

    internal class CompareHelper
    {
        private readonly List<IValidationResult> results;
        private readonly IProtocolType oldType;
        private readonly ProtocolTypeOptions oldOptions;
        private readonly IProtocolType newType;
        private readonly ProtocolTypeOptions newOptions;

        public CompareHelper(List<IValidationResult> results, IProtocolType oldType, ProtocolTypeOptions oldOptions, IProtocolType newType, ProtocolTypeOptions newOptions)
        {
            this.results = results;
            this.oldType = oldType;
            this.oldOptions = oldOptions;
            this.newType = newType;
            this.newOptions = newOptions;
        }

        public void CheckExportProtocol()
        {
            if (oldOptions?.ExportProtocols == null || oldOptions.ExportProtocols.Count == 0)
            {
                return;
            }

            foreach (var oldExportProtocol in oldOptions.ExportProtocols)
            {
                // Get Matching from newOptions
                ExportProtocol newExportProtocol = newOptions?.ExportProtocols?.FirstOrDefault(x => oldExportProtocol.TablePid == x.TablePid);

                if (newExportProtocol == null)
                {
                    results.Add(
                        ErrorCompare.RemovedDveExportProtocolName(oldType, oldType, oldExportProtocol.Name, Convert.ToString(oldExportProtocol.TablePid)));
                    continue;
                }

                if (!oldExportProtocol.Name.Equals(newExportProtocol.Name))
                {
                    results.Add(
                        ErrorCompare.UpdatedDveExportProtocolName(
                            newType,
                            newType,
                            oldExportProtocol.Name,
                            Convert.ToString(oldExportProtocol.TablePid),
                            newExportProtocol.Name));
                }

                if (!oldExportProtocol.NoElementPrefix && newExportProtocol.NoElementPrefix)
                {
                    results.Add(ErrorCompare.AddedNoElementPrefix(newType, newType, newExportProtocol.Name, Convert.ToString(newExportProtocol.TablePid)));
                }

                if (oldExportProtocol.NoElementPrefix && !newExportProtocol.NoElementPrefix)
                {
                    results.Add(
                        ErrorCompare.RemovedNoElementPrefix(newType, newType, newExportProtocol.Name, Convert.ToString(newExportProtocol.TablePid)));
                }
            }
        }

        public void CheckUnicode()
        {
            bool oldUnicode = (oldOptions?.Unicode).GetValueOrDefault();
            bool newUnicode = (newOptions?.Unicode).GetValueOrDefault();

            if (oldUnicode && !newUnicode)
            {
                results.Add(ErrorCompare.RemovedUnicode(newType, newType));
            }
            else if (!oldUnicode && newUnicode)
            {
                results.Add(ErrorCompare.AddedUnicode(newType, newType));
            }
        }
    }
}