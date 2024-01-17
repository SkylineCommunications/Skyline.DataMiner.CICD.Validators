namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.DVEs.DVEProtocols.DVEProtocol.ElementPrefix.CheckElementPrefixTag
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckElementPrefixTag, Category.Protocol)]
    internal class CheckElementPrefixTag : ICompare
    {
        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var oldDves = context?.PreviousProtocolModel?.Protocol?.DVEs?.DVEProtocols;
            var newDves = context?.NewProtocolModel?.Protocol?.DVEs?.DVEProtocols;

            if (oldDves == null)
            {
                return results;
            }

            foreach (var oldDve in oldDves)
            {
                if (!CompareHelper.ValidateTagData(oldDve))
                {
                    continue;
                }

                if (newDves != null)
                {
                    CompareHelper.CompareWithNewDve(results, newDves, oldDve);
                }
            }

            return results;
        }
    }

    internal static class CompareHelper
    {
        public static void CompareWithNewDve(List<IValidationResult> results, IDVEsDVEProtocols newDves, IDVEsDVEProtocolsDVEProtocol oldDve)
        {
            IDVEsDVEProtocolsDVEProtocol newDve = FindMatchingDve(newDves, oldDve);

            if (newDve == null || oldDve == null)
            {
                return;
            }

            if (!ValidateTagData(newDve))
            {
                return;
            }

            var oldElementPrefix = oldDve.ElementPrefix?.Value;
            var newElementPrefix = newDve.ElementPrefix?.Value;

            bool oldElementHasPrefix = oldElementPrefix.GetValueOrDefault(true);
            bool newElementHasPrefix = newElementPrefix.GetValueOrDefault(true);

            if (oldElementHasPrefix && !newElementHasPrefix)
            {
                results.Add(ErrorCompare.RemovedElementPrefix(newDve, newDve, oldDve.Name.Value, Convert.ToString(oldDve.TablePID.Value)));
            }

            if (!oldElementHasPrefix && newElementHasPrefix)
            {
                results.Add(ErrorCompare.AddedElementPrefix(newDve, newDve, oldDve.Name.Value, Convert.ToString(oldDve.TablePID.Value)));
            }
        }

        public static bool ValidateTagData(IDVEsDVEProtocolsDVEProtocol dve)
        {
            if (dve?.Name?.Value == null)
            {
                return false;
            }

            if (dve.TablePID?.Value == null)
            {
                return false;
            }

            return true;
        }

        private static IDVEsDVEProtocolsDVEProtocol FindMatchingDve(IDVEsDVEProtocols newDves, IDVEsDVEProtocolsDVEProtocol oldDve)
        {
            IDVEsDVEProtocolsDVEProtocol matchingDve = null;
            foreach (var newDve in newDves)
            {
                var oldTableId = oldDve?.TablePID?.Value;
                var newTableId = newDve?.TablePID?.Value;

                if (oldTableId != null && newTableId != null && oldTableId == newTableId)
                {
                    matchingDve = newDve;
                    break;
                }
            }

            return matchingDve;
        }
    }
}