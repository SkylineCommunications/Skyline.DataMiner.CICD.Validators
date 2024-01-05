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
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckMinimumRequiredVersionTag, Category.Protocol)]
    internal class CheckMinimumRequiredVersionTag : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            var model = context.ProtocolModel;
            var protocol = model?.Protocol;
            if (protocol == null)
            {
                return new List<IValidationResult>(0);
            }

            ValidateHelper helper = new ValidateHelper(this, context);
            helper.CheckUntrimmed();
            helper.CheckUsedFeatures();

            return helper.GetResults();
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

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly ValidatorContext context;
        private readonly IValueTag<string> tag;
        private readonly List<IValidationResult> results = new List<IValidationResult>();

        public ValidateHelper(IValidate test, ValidatorContext context)
        {
            this.test = test;
            this.context = context;

            tag = context.ProtocolModel.Protocol.Compliancies?.MinimumRequiredVersion;
        }

        public List<IValidationResult> GetResults()
        {
            return results;
        }

        public void CheckUntrimmed()
        {
            if (tag?.Value == null)
            {
                return;
            }

            if (Helper.IsUntrimmed(tag.RawValue))
            {
                results.Add(Error.UntrimmedTag(test, tag, tag, tag.RawValue));
            }
        }

        public void CheckUsedFeatures()
        {
            string versionValue = tag?.Value;
            var model = context.ProtocolModel;
            var protocol = model.Protocol;

            if (!DataMinerVersion.TryParse(versionValue, out DataMinerVersion minRequiredVersion))
            {
                minRequiredVersion = context.ValidatorSettings.MinimumSupportedDataMinerVersion;
            }

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
                IValidationResult minVersionTooLow = Error.MinVersionTooLow(test, null, GetPositionNode(), versionValue, expectedVersion.ToString());
                minVersionTooLow.WithSubResults(subResults.ToArray())
                                .WithExtraData(ExtraData.TooLow, expectedVersion);
                results.Add(minVersionTooLow);
            }

            /* Local functions */

            IReadable GetPositionNode()
            {
                if (protocol.Compliancies == null)
                {
                    return protocol;
                }

                if (protocol.Compliancies.MinimumRequiredVersion == null)
                {
                    return protocol.Compliancies;
                }

                return protocol.Compliancies.MinimumRequiredVersion;
            }

            DataMinerVersion GetDataMinerVersion(Feature feature)
            {
                // Feature release is always lower then the main release.
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
    }
}