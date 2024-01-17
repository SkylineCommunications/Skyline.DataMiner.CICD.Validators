namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.CheckIdAttribute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckIdAttribute, Category.Param)]
    internal class CheckIdAttribute : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (context?.ProtocolModel?.Protocol?.Params == null)
            {
                return results;
            }

            var protocol = context.ProtocolModel.Protocol;

            bool isSpectrumDriver = protocol.IsSpectrum();
            bool isMediationDriver = protocol.IsMediation();
            bool isEnhancedServiceDriver = protocol.IsEnhancedService();
            bool isSlaDriver = protocol.IsSLA();

            foreach (IParamsParam param in protocol.Params)
            {
                (GenericStatus status, string rawPid, uint? pid) = GenericTests.CheckBasics(param.Id, isRequired: true);

                // Missing
                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingAttribute(this, param, param));
                    continue;
                }

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, param, param));
                    continue;
                }

                // Invalid
                if (status.HasFlag(GenericStatus.Invalid) || !GenericTests.IsPlainNumbers(rawPid))
                {
                    results.Add(Error.InvalidValue(this, param, param, rawPid, param.Name?.RawValue));
                    continue;
                }

                // Check Normal Range
                if (pid == 0 ||
                    (pid >= 64300 && pid < 70000) ||      // DataMiner Params
                    (pid >= 100000 && pid < 1000000) ||   // DataMiner Params
                    pid >= 10000000)                               // Out of range
                {
                    results.Add(Error.OutOfRangeId(this, param, param.Id, rawPid));
                    continue;
                }

                // Check Spectrum Range
                if (pid >= 64000 && pid < 64300)
                {
                    if (isSpectrumDriver)
                    {
                        // Check that the param has the expected name/description based on the ID
                        ////if (!ParamHelper.IsCorrectSpectrumParam(param))
                        ////{
                        ////    // TODO: Param name and descriptions should be reviewed by Lightning before applying this check
                        ////    // When the list is reviewed, an extra error could be made to improve the error description so that it contains the expected name and description?
                        ////    Results.Add(Error.InvalidUseOfSpectrumIdRange(this, param, param.Id, rawId));
                        ////    continue;
                        ////}

                        // Param Requiring RTDisplay
                        IValidationResult rtDisplayError = Error.RTDisplayExpectedOnSpectrumParam(this, param, param.Id, rawPid);
                        context.CrossData.RtDisplay.AddParam(param, rtDisplayError);
                    }
                    else
                    {
                        results.Add(Error.InvalidUseOfSpectrumIdRange(this, param, param.Id, rawPid));
                        continue;
                    }
                }

                // Check Mediation Range
                if (pid >= 70000 && pid < 80000)
                {
                    if (!isMediationDriver)
                    {
                        results.Add(Error.InvalidUseOfMediationIdRange(this, param, param.Id, rawPid));
                        continue;
                    }
                }

                // Check DataMiner Modules Range
                if (pid >= 80000 && pid < 100000)
                {
                    // Check that the param has the expected name/description based on the PID
                    bool isOk = true;  //TODO: Check if the expected DataMiner param is added
                    if (!isOk)
                    {
                        results.Add(Error.InvalidUseOfDataMinerModulesIdRange(this, param, param.Id, rawPid));
                        continue;
                    }
                }

                // Enhanced Service Driver Exceptions
                if (isEnhancedServiceDriver && pid >= 1 && pid < 1000)
                {
                    // Check that the param has the expected name/description based on the ID
                    if (!ParamHelper.IsCorrectEnhancedServiceParam(param))
                    {
                        results.Add(Error.InvalidUseOfEnhancedServiceIdRange(this, param, param.Id, rawPid));
                        continue;
                    }
                }

                // Spectrum Driver Exceptions
                // For spectrum elements parameter 60000 might not be handled correctly when defined in the driver
                if (isSpectrumDriver && pid >= 50000 && pid <= 60000)
                {
                    results.Add(Error.InvalidUseOfSpectrumIdRange(this, param, param.Id, rawPid));
                    continue;
                }

                // SLA Driver Exceptions
                if (isSlaDriver && pid >= 1 && pid < 3000)
                {
                    // Check that the param has the expected name/description based on the ID
                    if (!ParamHelper.IsCorrectSlaParam(param))
                    {
                        results.Add(Error.InvalidUseOfSlaIdRange(this, param, param.Id, rawPid));
                        continue;
                    }
                }

                // Aggregation Drivers can be ignored.
                // We are not allowed to work on such drivers, this driver is provided together with the software package and is a hidden driver used by the software to perform aggregation actions.
                // The driver can be found in "C:\Skyline DataMiner\Protocols\Skyline Generic Aggregator"
                // Aggregation Driver Exceptions
                ////if (isAggregationDriver)
                ////{
                ////    if (id >= 1 && id < 5000)
                ////    {
                ////        // Check that the param has the expected name/description based on the ID
                ////        bool isOk = true;
                ////        if (!isOk)
                ////        {
                ////            Results.Add(Error.InvalidUseOfAggregationIdRange(this, param, param.Id, rawId));
                ////            continue;
                ////        }
                ////    }
                ////}

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedAttribute(this, param, param, rawPid));
                }
            }

            // Duplicate
            var duplicateResults = GenericTests.CheckDuplicateIds(
                items: context.EachParamWithValidId(),
                getDuplicationIdentifier: param => param.Id?.Value,
                getName: param => param.Name?.RawValue,
                generateSubResult: x => Error.DuplicatedId(this, x.item, x.item, x.id, x.name),
                generateSummaryResult: x => Error.DuplicatedId(this, null, null, x.id, String.Join(", ", x.names)).WithSubResults(x.subResults)
                );

            results.AddRange(duplicateResults);

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    if (!(context.Result.ReferenceNode is IParamsParam readParam))
                    {
                        break;
                    }

                    var editParam = context.Protocol?.Params?.Get(readParam);

                    var idAttribute = editParam?.EditNode.Attribute["id"];
                    if (idAttribute == null)
                    {
                        break;
                    }

                    idAttribute.Value = idAttribute.Value.Trim();
                    result.Success = true;

                    break;
                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            IProtocol newProtocol = context?.NewProtocolModel?.Protocol;
            IProtocol previousProtocol = context?.PreviousProtocolModel?.Protocol;

            if (newProtocol == null)
            {
                return results;
            }

            if (previousProtocol == null)
            {
                return results;
            }

            foreach (IParamsParam previousParam in context.PreviousProtocolModel.EachParamWithValidId())
            {
                // Find corresponding parameter with the same name id.
                IParamsParam newParam = newProtocol.Params?.FirstOrDefault(param => param?.Id?.Value == previousParam.Id.Value);

                if (newParam == null && previousParam.GetRTDisplay())
                {
                    results.Add(ErrorCompare.MissingParam(null, newProtocol.Params, previousParam.Name?.Value, previousParam.Type.ReadNode.InnerText, previousParam.Id.RawValue));
                }
            }

            return results;
        }
    }
}