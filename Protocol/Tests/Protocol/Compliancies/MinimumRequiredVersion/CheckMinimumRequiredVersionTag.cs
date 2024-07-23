namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Compliancies.MinimumRequiredVersion.CheckMinimumRequiredVersionTag
{
    using System.Collections.Generic;
    using System.Threading;

    using Skyline.DataMiner.CICD.Common;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckMinimumRequiredVersionTag, Category.Protocol)]
    internal class CheckMinimumRequiredVersionTag : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (context?.ProtocolModel?.Protocol == null)
            {
                return results;
            }

            ValidateHelper helper = new ValidateHelper(this, context, results);
            helper.CheckBasics(out DataMinerVersion parsedVersion);
            helper.CheckUsedFeatures(parsedVersion);
            helper.CheckSupportedDmVersion(parsedVersion);

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            var editNode = context.Protocol.GetOrCreateCompliancies().GetOrCreateMinimumRequiredVersion();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedTag:
                    {
                        editNode.Value = editNode.Value.Trim();
                        result.Success = true;
                        break;
                    }

                case ErrorIds.MinVersionTooLow:
                    {
                        DataMinerVersion expected = (DataMinerVersion)context.Result.ExtraData[ExtraData.TooLow];

                        editNode.Value = expected.ToString();
                        result.Success = true;
                        break;
                    }

                case ErrorIds.MissingTag:
                case ErrorIds.EmptyTag:
                case ErrorIds.BelowMinimumSupportedVersion:
                    {
                        editNode.Value = context.ValidatorSettings.MinimumSupportedDataMinerVersion.ToString();
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

            var oldMinSupportedVersionTag = context?.PreviousProtocolModel?.Protocol?.Compliancies?.MinimumRequiredVersion;
            var newMinSupportedVersionTag = context?.NewProtocolModel?.Protocol?.Compliancies?.MinimumRequiredVersion;

            // Removing will never be an issue
            if (newMinSupportedVersionTag == null ||
                !DataMinerVersion.TryParse(newMinSupportedVersionTag.Value, out DataMinerVersion newMinSupportedVersion))
            {
                return results;
            }

            // Updating to minimum supported DM version or lower is no issue
            if (newMinSupportedVersion <= context.ValidatorSettings.MinimumSupportedDataMinerVersion)
            {
                return results;
            }

            DataMinerVersion oldMinSupportedVersion;
            if (oldMinSupportedVersionTag == null)
            {
                oldMinSupportedVersion = context.ValidatorSettings.MinimumSupportedDataMinerVersion;
            }
            else if (!DataMinerVersion.TryParse(oldMinSupportedVersionTag.Value, out oldMinSupportedVersion))
            {
                return results;
            }

            if (newMinSupportedVersion > oldMinSupportedVersion)
            {
                results.Add(ErrorCompare.MinVersionIncreased(newMinSupportedVersionTag, newMinSupportedVersionTag, oldMinSupportedVersionTag?.RawValue, newMinSupportedVersionTag.RawValue));
            }

            return results;
        }
    }

    internal enum ExtraData
    {
        TooLow
    }

    internal class ValidateHelper : ValidateHelperBase
    {
        private readonly IProtocol protocol;
        private readonly ICompliancies compliancies;
        private readonly IValueTag<string> tag;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results) : base(test, context, results)
        {
            protocol = context.ProtocolModel.Protocol;
            compliancies = protocol.Compliancies;
            tag = compliancies?.MinimumRequiredVersion;
        }

        public void CheckBasics(out DataMinerVersion parsedVersion)
        {
            parsedVersion = null;

            (GenericStatus status, _, _) = GenericTests.CheckBasics(tag, isRequired: true);

            // Missing
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingTag(test, null, GetPositionNode()));
                return;
            }

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyTag(test, tag, tag));
                return;
            }

            if (!DataMinerVersion.TryParse(tag.RawValue, out parsedVersion))
            {
                results.Add(Error.InvalidValue(test, tag, tag, tag.RawValue));
                return;
            }

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedTag(test, tag, tag, tag.RawValue));
            }
        }

        public void CheckUsedFeatures(DataMinerVersion parsedVersion)
        {
            // Default to minimum supported version
            DataMinerVersion minRequiredVersion = parsedVersion ?? context.ValidatorSettings.MinimumSupportedDataMinerVersion;

            IDmVersionCheckResults versionCheckResults = VersionChecker.GetUsedFeatures(context.InputData,
                    context.CompiledQActions, context.InputData.QActionCompilationModel?.IsSolutionBased ?? false, CancellationToken.None);

            DataMinerVersion expectedVersion = minRequiredVersion;
            List<IValidationResult> subResults = new List<IValidationResult>();

            foreach (Feature feature in versionCheckResults.Features)
            {
                if (feature.MinMainRelease <= minRequiredVersion || feature.MinFeatureRelease <= minRequiredVersion)
                {
                    continue;
                }

                DataMinerVersion version = GetDataMinerVersion(feature);

                if (expectedVersion < version)
                {
                    expectedVersion = version;
                }

                CreateSubResult(feature, version);
            }

            if (subResults.Count > 0)
            {
                IValidationResult minVersionTooLow = Error.MinVersionTooLow(test, null, GetPositionNode(), tag?.RawValue, expectedVersion.ToString());
                minVersionTooLow.WithSubResults(subResults.ToArray())
                                .WithExtraData(ExtraData.TooLow, expectedVersion);
                results.Add(minVersionTooLow);
            }

            /* Local functions */

            DataMinerVersion GetDataMinerVersion(Feature feature)
            {
                // Feature release is always lower than the main release.
                return feature.MinFeatureRelease ?? feature.MinMainRelease;
            }

            void CreateSubResult(Feature feature, DataMinerVersion version)
            {
                IValidationResult minVersionTooLowSub = Error.MinVersionTooLow_Sub(test, null, GetPositionNode(), version.ToString(), feature.Title);

                foreach (var featureItem in feature.FeatureItems)
                {
                    var node = featureItem.Node;
                    (string identifierType, string itemId) = DetectIdentifier(node);

                    IValidationResult result;

                    if (itemId == null)
                    {
                        result = Error.MinVersionFeatureUsedInItem_Sub(test, node, node, node.TagName);
                    }
                    else
                    {
                        result = Error.MinVersionFeatureUsedInItemWithId_Sub(test, node, node, node.TagName, identifierType, itemId);
                    }

                    if (featureItem is CSharpFeatureCheckResultItem csharpFeature)
                    {
                        result = result.WithCSharp(csharpFeature.CSharp);
                    }

                    minVersionTooLowSub.WithSubResults(result);
                }

                subResults.Add(minVersionTooLowSub);
            }
        }

        public void CheckSupportedDmVersion(DataMinerVersion parsedVersion)
        {
            if (parsedVersion == null)
            {
                // Covered by the basic checks.
                return;
            }
            
            if (parsedVersion < context.ValidatorSettings.MinimumSupportedDataMinerVersion)
            {
                results.Add(Error.BelowMinimumSupportedVersion(test, null, GetPositionNode(), parsedVersion.ToString(), context.ValidatorSettings.MinimumSupportedDataMinerVersion.ToString()));
            }
        }

        private static (string identifierType, string itemId) DetectIdentifier(IReadable featureItem)
        {
            string identifier = null;
            string id = null;
            switch (featureItem)
            {
                case IParamsParam param:
                    identifier = "ID";
                    id = param.Id?.RawValue;
                    break;
                case IGroupsGroup group:
                    identifier = "ID";
                    id = group.Id?.RawValue;
                    break;
                case IQActionsQAction qAction:
                    identifier = "ID";
                    id = qAction.Id?.RawValue;
                    break;
                case ITriggersTrigger trigger:
                    identifier = "ID";
                    id = trigger.Id?.RawValue;
                    break;
                case IActionsAction action:
                    identifier = "ID";
                    id = action.Id?.RawValue;
                    break;
                case ITimersTimer timer:
                    identifier = "ID";
                    id = timer.Id?.RawValue;
                    break;
                case ICommandsCommand command:
                    identifier = "ID";
                    id = command.Id?.RawValue;
                    break;
                case IResponsesResponse response:
                    identifier = "ID";
                    id = response.Id?.RawValue;
                    break;
                case IPairsPair pair:
                    identifier = "ID";
                    id = pair.Id?.RawValue;
                    break;
                case IHTTPSession session:
                    identifier = "ID";
                    id = session.Id?.RawValue;
                    break;
                case IHTTPSessionConnection connection:
                    identifier = "ID";
                    id = connection.Id?.RawValue;
                    break;
                case IAlarmLevelLinksAlarmLevelLink alarmLevelLink:
                    identifier = "ID";
                    id = alarmLevelLink.Id?.RawValue;
                    break;

                case IPortSettings portSettings:
                    identifier = "Name";
                    id = portSettings.Name?.Value;
                    break;
                case IChainsChain chain:
                    identifier = "Name";
                    id = chain.Name?.Value;
                    break;
                case ITypeChainsChainField field:
                    identifier = "Name";
                    id = field.Name?.Value;
                    break;
                case IDVEsDVEProtocolsDVEProtocol dveProtocol:
                    identifier = "Name";
                    id = dveProtocol.Name?.Value;
                    break;
                case IExportRulesExportRule exportRule:
                    identifier = "Name";
                    id = exportRule.Name?.Value;
                    break;
                case IRelationsRelation relation:
                    identifier = "Name";
                    id = relation.Name?.Value;
                    break;
                case ITopologiesTopology topology:
                    identifier = "Name";
                    id = topology.Name?.Value;
                    break;
                case ITopology topology2:
                    identifier = "Name";
                    id = topology2.Name?.Value;
                    break;

                case IParameterGroupsGroup paramGroup:
                    identifier = "ID";
                    id = paramGroup.Id?.RawValue;

                    if (id == null)
                    {
                        identifier = "Name";
                        id = paramGroup.Name?.Value;
                    }

                    break;
            }

            return (identifier, id);
        }

        private IReadable GetPositionNode()
        {
            if (compliancies == null)
            {
                return protocol;
            }

            if (tag == null)
            {
                return compliancies;
            }

            return tag;
        }
    }
}