namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Type.CheckWidthAttribute
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckWidthAttribute, Category.Param)]
    internal class CheckWidthAttribute : IValidate, ICodeFix
    {
        private const uint DefaultWidth = 110;

        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            Dictionary<string, List<IParamsParam>> buttonsPerPages = new Dictionary<string, List<IParamsParam>>();

            var model = context.ProtocolModel;
            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                // Check if width attribute is there
                var width = param.Measurement?.Type?.Width;

                // Check if type is Button or PageButton
                if (param.IsPageButton() || param.IsButton())
                {
                    // Check if attribute is there
                    if (width == null)
                    {
                        results.Add(Error.MissingAttribute(this, param, param.Measurement.Type, param.Id.RawValue));
                        continue;
                    }

                    string widthRawValue = param.Measurement.Type.ReadNode.GetAttributeValue("width");

                    // Check if attribute has a parse-able value
                    if (!width.Value.HasValue)
                    {
                        if (String.IsNullOrWhiteSpace(widthRawValue))
                        {
                            // Check if empty
                            results.Add(Error.EmptyWidth(this, param, width, param.Id.RawValue));
                            continue;
                        }

                        // Couldn't parse it - Format Exception
                        results.Add(Error.InvalidWidth(this, param, width, widthRawValue, param.Id.RawValue));
                        continue;
                    }

                    // Check for Undersized width (against the minimum recommended width)
                    if (width.Value.Value < DefaultWidth)
                    {
                        results.Add(Error.UnrecommendedWidth(this, param, width, widthRawValue, param.Id.RawValue));
                        continue;
                    }

                    // Check if Tag starts/ends with whitespace
                    if (Helper.IsUntrimmed(widthRawValue))
                    {
                        results.Add(Error.UntrimmedWidth(this, param, width, widthRawValue, param.Id.RawValue));
                        continue;
                    }

                    // TODO : Check for Undersized width (against the width required to display the button caption)

                    // Check for consistency on page level
                    if (!param.IsPositioned(model.RelationManager) || param.Display?.Positions == null)
                    {
                        continue;
                    }

                    // Check for inconsistent width
                    foreach (var position in param.Display.Positions)
                    {
                        string page = position?.Page?.Value;

                        if (page == null)
                        {
                            continue;
                        }

                        if (buttonsPerPages.ContainsKey(page))
                        {
                            buttonsPerPages[page].Add(param);
                        }
                        else
                        {
                            buttonsPerPages.Add(page, new List<IParamsParam> { param });
                        }
                    }
                }
                else
                {
                    if (width != null)
                    {
                        // If not Button nor PageButton
                        results.Add(Error.UnsupportedAttribute(this, param, width, Convert.ToString(param?.Measurement?.Type?.Value), param.Id.RawValue));
                        continue;
                    }
                }
            }

            // Iterate through all Button/PageButton parameters in order to check for inconsistent width.
            foreach (KeyValuePair<string, List<IParamsParam>> kvp in buttonsPerPages)
            {
                string pageName = kvp.Key;

                List<uint> widths = new List<uint>();
                List<string> paramIds = new List<string>();
                List<IValidationResult> subResults = new List<IValidationResult>();
                foreach (IParamsParam param in kvp.Value)
                {
                    uint widthValue = param.Measurement.Type.Width.Value.Value;
                    string paramId = param.Id.RawValue;

                    if (!widths.Contains(widthValue))
                    {
                        widths.Add(widthValue);
                    }

                    if (!paramIds.Contains(paramId))
                    {
                        paramIds.Add(paramId);
                    }

                    subResults.Add(Error.InconsistentWidth(this, param, param.Measurement.Type.Width, pageName, paramId, Convert.ToString(widthValue), hasCodeFix: false));
                }

                if (widths.Count > 1)
                {
                    IValidationResult inconsistentWidth = Error.InconsistentWidth(this, null, null, pageName, String.Join(", ", paramIds), String.Join(", ", widths), hasCodeFix: true);
                    inconsistentWidth.WithExtraData(ExtraData.MaxWidth, widths.Max()).WithSubResults(subResults.ToArray());
                    results.Add(inconsistentWidth);
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.MissingAttribute:
                    {
                        var readParam = (IParamsParam)context.Result.ReferenceNode;
                        var editParam = context.Protocol.Params.Get(readParam);

                        editParam.Measurement.Type.Width = new Skyline.DataMiner.CICD.Models.Protocol.Edit.AttributeValue<uint?>(DefaultWidth);
                        result.Success = true;

                        break;
                    }

                case ErrorIds.InvalidWidth:
                case ErrorIds.EmptyWidth:
                case ErrorIds.UnrecommendedWidth:
                    {
                        var readParam = (IParamsParam)context.Result.ReferenceNode;
                        var editParam = context.Protocol.Params.Get(readParam);

                        editParam.Measurement.Type.Width = DefaultWidth;
                        result.Success = true;

                        break;
                    }

                case ErrorIds.UntrimmedWidth:
                    {
                        var readParam = (IParamsParam)context.Result.ReferenceNode;
                        var editParam = context.Protocol.Params.Get(readParam);

                        string sValue = Convert.ToString(readParam.Measurement.Type.Width.Value).Trim();

                        editParam.Measurement.Type.Width = Convert.ToUInt32(sValue);
                        result.Success = true;

                        break;
                    }

                case ErrorIds.InconsistentWidth:
                    {
                        uint maxWidth = Convert.ToUInt32(context.Result.ExtraData[ExtraData.MaxWidth]);

                        var readParams = context.Result.SubResults.Select(subResult => (IParamsParam)subResult.ReferenceNode);
                        foreach (var readParam in readParams)
                        {
                            var editParam = context.Protocol.Params.Get(readParam);

                            editParam.Measurement.Type.Width = maxWidth;
                        }

                        result.Success = true;

                        break;
                    }

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }
    }

    internal enum ExtraData
    {
        MaxWidth
    }
}