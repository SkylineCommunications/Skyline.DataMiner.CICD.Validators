namespace SLDisValidator2.Tests.Protocol.Params.Param.Information.Subtext.CheckSubtextTag
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckSubtextTag, Category.Param)]
    internal class CheckSubtextTag : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var model = context.ProtocolModel;
            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                /* Displayed parameters that don't need Subtext:
                 * - Title
                 * - Write parameters with the exception of buttons
                 * - TreeControl
                 */

                if (param.IsTitle() || (param.IsWrite() && !param.IsButton()) || param.IsTreeControl(model.RelationManager))
                {
                    continue;
                }

                if (!param.IsPositioned(model.RelationManager))
                {
                    continue;
                }

                var information = param.Information;

                IReadable positionNode;
                if (information == null)
                {
                    positionNode = param;
                }
                else
                {
                    positionNode = information;
                }

                var subtext = information?.Subtext;

                // Check if tag is there
                if (subtext == null)
                {
                    results.Add(Error.MissingTag(this, param, positionNode, param.Id.RawValue));
                    continue;
                }

                // Check if tag is filled in
                if (String.IsNullOrWhiteSpace(subtext.Value))
                {
                    results.Add(Error.EmptyTag(this, param, subtext, param.Id.RawValue));
                    continue;
                }

                // Check if the tag contains a multi-line value
                if (subtext.Value.Contains(Environment.NewLine) && !subtext.HasCData())
                {
                    //Results.Add(Error.NoCData(this, param, subtext, ));
                }
            }

            return results;
        }
    }
}