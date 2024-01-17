namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Type.CheckTypeTag
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Edit;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckTypeTag, Category.Param)]
    internal class CheckTypeTag : IValidate, ICodeFix
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.Measurement?.Type == null)
                {
                    continue;
                }

                var measurementType = param.Measurement.Type;

                (GenericStatus status, string rawValue, EnumParamMeasurementType? _) = Generic.GenericTests.CheckBasics(measurementType, false);

                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyTag(this, param, measurementType, param.Id.RawValue));
                    continue;
                }

                if (status.HasFlag(GenericStatus.Invalid))
                {
                    results.Add(Error.InvalidValue(this, param, measurementType, rawValue, param.Id.RawValue));
                    continue;
                }

                ValidateHelper helper = new ValidateHelper(this, context, results, param);
                helper.CheckParamType();
                helper.CheckObviousToggleButtons();

                if (param.IsMatrix())
                {
                    ValidateMatrix matrixValidator = new ValidateMatrix(this, param, param.Id.RawValue);
                    results.AddIfNotNull(matrixValidator.CheckMatrixInterprete());
                    results.AddIfNotNull(matrixValidator.CheckMatrixTrending());
                    results.AddIfNotNull(matrixValidator.CheckMatrixAlarming());
                    results.AddIfNotNull(matrixValidator.CheckMatrixSetterOnWrite());
                }

                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedTag(this, param, measurementType, param.Id.RawValue, rawValue));
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.TogglebuttonRecommended:
                    {
                        var readParam = (IParamsParam)context.Result.ReferenceNode;
                        var editParam = context.Protocol.Params.Get(readParam);

                        if (context.Result.ExtraData.TryGetValue(ExtraData.ExpectedMeasurementType, out object expectedMeasurementType) &&
                            editParam?.Measurement?.Type != null)
                        {
                            editParam.Measurement.Type.Value = (EnumParamMeasurementType)expectedMeasurementType;
                            result.Success = true;
                        }

                        break;
                    }

                case ErrorIds.MatrixInvalidInterprete:
                case ErrorIds.MatrixInvalidInterpreteRawType:
                case ErrorIds.MatrixInvalidInterpreteType:
                case ErrorIds.MatrixInvalidInterpreteLengthType:
                    ValidateMatrix.FixMatrixInterprete(context, result);
                    break;

                case ErrorIds.MatrixTrendingEnabled:
                    ValidateMatrix.FixMatrixTrendingEnabled(context, result);
                    break;

                case ErrorIds.MatrixAlarmingDisabled:
                    ValidateMatrix.FixMatrixAlarmingDisabled(context, result);
                    break;

                case ErrorIds.MatrixSetterOnWrite:
                    ValidateMatrix.FixMatrixSetterOnWrite(context, result);
                    break;

                case ErrorIds.UntrimmedTag:
                    {
                        var readParam = (IParamsParam)context.Result.ReferenceNode;
                        var editParam = context.Protocol.Params.Get(readParam);

                        // String value, trimmed
                        string type = editParam.Measurement.Type.ReadNode.InnerText.Trim();
                        EnumParamMeasurementType? newType = EnumParamMeasurementTypeConverter.Convert(type);

                        if (newType != null)
                        {
                            editParam.Measurement.Type.Value = newType;
                            result.Success = true;
                        }

                        break;
                    }

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }
    }

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly ValidatorContext context;
        private readonly List<IValidationResult> results;
        private readonly IParamsParam param;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam param)
        {
            this.test = test;
            this.context = context;
            this.results = results;
            this.param = param;
        }

        internal void CheckParamType()
        {
            switch (param.Measurement?.Type?.Value)
            {
                case EnumParamMeasurementType.Button:
                case EnumParamMeasurementType.Pagebutton:
                case EnumParamMeasurementType.Togglebutton:
                    if (!param.IsWrite())
                    {
                        results.Add(Error.InvalidParamType(test, param, param.Measurement.Type, param.Type?.RawValue, param.Measurement?.Type?.RawValue, param.Id?.RawValue));
                    }
                    break;
                // Possible to have other combinations (currently not implemented)
                default:
                    break;
            }
        }

        internal void CheckObviousToggleButtons()
        {
            // A toggle button is used for write parameters with only two possible values, where the second value is obvious when you read the first
            // - Param.Type is write or write bit AND there is a corresponding read param
            // - Param.Measurement.Type is 'discreet'
            // - Exactly two discreets

            if (!param.IsWrite() ||
                param.Measurement?.Type?.Value != EnumParamMeasurementType.Discreet ||
                param.Measurement?.Discreets?.Count != 2 ||
                !param.TryGetRead(context.ProtocolModel.RelationManager, out _))
            {
                return;
            }

            string discreet1 = param.Measurement.Discreets[0].Display?.Value;
            string discreet2 = param.Measurement.Discreets[1].Display?.Value;

            if (ObviousDiscreets.Contains(discreet1, discreet2))
            {
                IValidationResult togglebuttonRecommended = Error.TogglebuttonRecommended(test, param, param.Measurement.Type, param.Id?.RawValue);
                togglebuttonRecommended.WithExtraData(ExtraData.ExpectedMeasurementType, EnumParamMeasurementType.Togglebutton);
                results.Add(togglebuttonRecommended);
            }
        }

    }

    internal class ValidateMatrix
    {
        public ValidateMatrix(CheckTypeTag linkToTest, IParamsParam param, string pid)
        {
            LinkToTest = linkToTest;
            Pid = pid;
            Param = param;
            ParamType = param?.Type;
        }

        public CheckTypeTag LinkToTest { get; }

        public IParamsParam Param { get; }

        public IParamsParamType ParamType { get; }

        public string Pid { get; }

        public static void FixMatrixAlarmingDisabled(CodeFixContext context, CodeFixResult result)
        {
            var readParam = (IParamsParam)context.Result.ReferenceNode;
            var editParam = context.Protocol.Params.Get(readParam);
            if (editParam == null)
            {
                return;
            }

            if (editParam.Alarm == null)
            {
                editParam.Alarm = new ParamsParamAlarm();
            }

            editParam.Alarm.Monitored = new ParamsParamAlarmMonitored(true);
            result.Success = true;
        }

        public static void FixMatrixInterprete(CodeFixContext context, CodeFixResult result)
        {
            var readParam = (IParamsParam)context.Result.ReferenceNode;
            var editParam = context.Protocol.Params.Get(readParam);

            var type = editParam?.Type?.Value;
            if (type == null)
            {
                return;
            }

            if (type == EnumParamType.Array)
            {
                editParam.Interprete = new ParamsParamInterprete
                {
                    RawType = new ParamsParamInterpreteRawType(EnumParamInterpretRawType.Other),
                    LengthType = new ParamsParamInterpreteLengthType(EnumParamInterpretLengthType.NextParam),
                    Type = new ParamsParamInterpreteType(EnumParamInterpretType.Double)
                };
                result.Success = true;
            }
            else if (type == EnumParamType.Write)
            {
                editParam.Interprete = new ParamsParamInterprete
                {
                    RawType = new ParamsParamInterpreteRawType(EnumParamInterpretRawType.Other),
                    LengthType = new ParamsParamInterpreteLengthType(EnumParamInterpretLengthType.NextParam),
                    Type = new ParamsParamInterpreteType(EnumParamInterpretType.String)
                };
                result.Success = true;
            }
        }

        public static void FixMatrixSetterOnWrite(CodeFixContext context, CodeFixResult result)
        {
            var readParam = (IParamsParam)context.Result.ReferenceNode;
            var editParam = context.Protocol.Params.Get(readParam);
            if (editParam == null)
            {
                return;
            }

            editParam.Setter = null;
            result.Success = true;
        }

        public static void FixMatrixTrendingEnabled(CodeFixContext context, CodeFixResult result)
        {
            var readParam = (IParamsParam)context.Result.ReferenceNode;
            var editParam = context.Protocol.Params.Get(readParam);
            if (editParam == null)
            {
                return;
            }

            editParam.Trending = null;
            result.Success = true;
        }

        public IValidationResult CheckMatrixAlarming()
        {
            var monitored = Param?.Alarm?.Monitored?.Value;
            if (monitored != true && ParamType?.Value != EnumParamType.Write)
            {
                return Error.MatrixAlarmingDisabled(LinkToTest, Param, Param, Pid);
            }

            return null;
        }

        public IValidationResult CheckMatrixInterprete()
        {
            uint errorCounter = 0;

            (IReadable tag, string currentValue)? rawTypeResult = CheckMatrixInterpreteRawType(ref errorCounter);
            (IReadable tag, string currentValue, string expectedValue)? interpreteTypeResult = CheckMatrixInterpreteType(ref errorCounter);
            (IReadable tag, string currentValue)? lengthTypeResult = CheckMatrixInterpreteLengthType(ref errorCounter);

            if (errorCounter <= 0)
            {
                return null;
            }

            if (errorCounter > 1)
            {
                IValidationResult error = Error.MatrixInvalidInterprete(LinkToTest, Param, Param, Pid);
                if (rawTypeResult.HasValue)
                {
                    IValidationResult subResult = Error.MatrixInvalidInterpreteRawType(LinkToTest, Param, rawTypeResult.Value.tag, rawTypeResult.Value.currentValue, Pid, false);
                    error.WithSubResults(subResult);
                }

                if (interpreteTypeResult.HasValue)
                {
                    IValidationResult subResult = Error.MatrixInvalidInterpreteType(LinkToTest, Param, interpreteTypeResult.Value.tag, interpreteTypeResult.Value.currentValue, Pid, interpreteTypeResult.Value.expectedValue, false);
                    error.WithSubResults(subResult);
                }

                if (lengthTypeResult.HasValue)
                {
                    IValidationResult subResult = Error.MatrixInvalidInterpreteLengthType(LinkToTest, Param, lengthTypeResult.Value.tag, lengthTypeResult.Value.currentValue, Pid, false);
                    error.WithSubResults(subResult);
                }

                return error;
            }

            if (rawTypeResult.HasValue)
            {
                return Error.MatrixInvalidInterpreteRawType(LinkToTest, Param, rawTypeResult.Value.tag, rawTypeResult.Value.currentValue, Pid, true);
            }

            if (interpreteTypeResult.HasValue)
            {
                return Error.MatrixInvalidInterpreteType(LinkToTest, Param, interpreteTypeResult.Value.tag, interpreteTypeResult.Value.currentValue, Pid, interpreteTypeResult.Value.expectedValue, true);
            }

            if (lengthTypeResult.HasValue)
            {
                return Error.MatrixInvalidInterpreteLengthType(LinkToTest, Param, lengthTypeResult.Value.tag, lengthTypeResult.Value.currentValue, Pid, true);
            }

            return null;
        }

        public (IReadable tag, string currentValue)? CheckMatrixInterpreteRawType(ref uint errorCounter)
        {
            var rawTypeTag = Param?.Interprete?.RawType;
            var rawType = rawTypeTag?.Value;
            if (rawType != EnumParamInterpretRawType.Other)
            {
                string niceRawType = String.Empty;
                if (rawType.HasValue)
                {
                    niceRawType = EnumParamInterpretRawTypeConverter.ConvertBack(rawType.Value);
                }

                errorCounter++;
                return (rawTypeTag, niceRawType);
            }

            return null;
        }

        public (IReadable tag, string currentValue, string expectedValue)? CheckMatrixInterpreteType(ref uint errorCounter)
        {
            var typeTag = Param?.Interprete?.Type;
            var type = typeTag?.Value;

            string niceType = String.Empty;
            if (type.HasValue)
            {
                niceType = EnumParamInterpretTypeConverter.ConvertBack(type.Value);
            }

            if (Param?.Type?.Value == EnumParamType.Array && type != EnumParamInterpretType.Double)
            {
                errorCounter++;
                return (typeTag, niceType, "double");
            }

            if (Param?.Type?.Value == EnumParamType.Write && type != EnumParamInterpretType.String)
            {
                errorCounter++;
                return (typeTag, niceType, "string");
            }

            return null;
        }

        public (IReadable tag, string currentValue)? CheckMatrixInterpreteLengthType(ref uint errorCounter)
        {
            var lengthTypeTag = Param?.Interprete?.LengthType;
            var lengthType = lengthTypeTag?.Value;
            if (lengthType != EnumParamInterpretLengthType.NextParam)
            {
                string niceLengthType = String.Empty;
                if (lengthType.HasValue)
                {
                    niceLengthType = EnumParamInterpretLengthTypeConverter.ConvertBack(lengthType.Value);
                }

                errorCounter++;
                return (lengthTypeTag, niceLengthType);
            }

            return null;
        }

        public IValidationResult CheckMatrixSetterOnWrite()
        {
            bool setter = Param?.Setter?.Value ?? false;
            if (setter && ParamType?.Value == EnumParamType.Write)
            {
                return Error.MatrixSetterOnWrite(LinkToTest, Param, Param, Pid);
            }

            return null;
        }

        public IValidationResult CheckMatrixTrending()
        {
            bool trended = Param?.Trending?.Value ?? false;
            if (trended)
            {
                return Error.MatrixTrendingEnabled(LinkToTest, Param, Param, Pid);
            }

            return null;
        }
    }

    internal static class ObviousDiscreets
    {
        private static readonly ISet<ToggleButtonDiscreets> _list = new HashSet<ToggleButtonDiscreets>(new ToggleButtonOptionsComparer())
        {
            new ToggleButtonDiscreets("Disabled", "Enabled"),
            new ToggleButtonDiscreets("On", "Off"),
            new ToggleButtonDiscreets("Yes", "No"),
            new ToggleButtonDiscreets("Enable", "Disable"),
            new ToggleButtonDiscreets("False", "True"),
            new ToggleButtonDiscreets("0 deg", "180 deg"),
            new ToggleButtonDiscreets("Masked", "Unmasked"),
            new ToggleButtonDiscreets("Up", "Down"),
            new ToggleButtonDiscreets("Manual", "Auto"),
            new ToggleButtonDiscreets("Active", "Not Active"),
            new ToggleButtonDiscreets("Processing Ch 1", "Processing Ch 2"),
            new ToggleButtonDiscreets("Manual", "Automatic"),
            new ToggleButtonDiscreets("Not Monitored", "Monitored"),
            new ToggleButtonDiscreets("Open", "Closed"),
            new ToggleButtonDiscreets("Locked", "Unlocked"),
            new ToggleButtonDiscreets("Inactive", "Active"),
            new ToggleButtonDiscreets("Rail 1", "Rail 2"),
            new ToggleButtonDiscreets("Low", "High"),
            new ToggleButtonDiscreets("Drop", "Pass"),
            new ToggleButtonDiscreets("Output", "Input"),
            new ToggleButtonDiscreets("Mono", "Stereo"),
            new ToggleButtonDiscreets("High (-)", "Low (+)"),
            new ToggleButtonDiscreets("Blocked", "Not Blocked"),
            new ToggleButtonDiscreets("Trap Disabled", "Trap Enabled"),
            new ToggleButtonDiscreets("Email Disabled", "Email Enabled"),
            new ToggleButtonDiscreets("Manual", "DHCP"),
            new ToggleButtonDiscreets("Internal", "External"),
            new ToggleButtonDiscreets("Accepted", "Not Accepted"),
            new ToggleButtonDiscreets("Link A", "Link B"),
            new ToggleButtonDiscreets("Input 1", "Input 2"),
            new ToggleButtonDiscreets("Local", "Remote"),
            new ToggleButtonDiscreets("Alarm Disabled", "Alarm Enabled"),
            new ToggleButtonDiscreets("Operational", "Suspended"),
            new ToggleButtonDiscreets("Mute", "Unmute"),
            new ToggleButtonDiscreets("Unbalanced", "Balanced"),
            new ToggleButtonDiscreets("A", "B"),
            new ToggleButtonDiscreets("Position A", "Position B"),
            new ToggleButtonDiscreets("Processing Channel 1", "Processing Channel 2"),
            new ToggleButtonDiscreets("Stop", "Start"),
            new ToggleButtonDiscreets("Top", "Bottom"),
            new ToggleButtonDiscreets("Slow", "Fast"),
            new ToggleButtonDiscreets("Lock", "UnLock"),
            new ToggleButtonDiscreets("Horizontal", "Vertical"),
            new ToggleButtonDiscreets("Position 1", "Position 2"),
            new ToggleButtonDiscreets("Include", "Exclude"),
            new ToggleButtonDiscreets("Overwrite", "Append"),
            new ToggleButtonDiscreets("Close", "Open"),
            new ToggleButtonDiscreets("DVB-S", "DVB-S2"),
            new ToggleButtonDiscreets("Present", "Absent"),
            new ToggleButtonDiscreets("Celsius", "Fahrenheit"),
            new ToggleButtonDiscreets("Master", "Slave"),
            new ToggleButtonDiscreets("Input A", "Input B"),
            new ToggleButtonDiscreets("Activate", "Deactivate"),
            new ToggleButtonDiscreets("Started", "Stopped"),
            new ToggleButtonDiscreets("Stop", "Play"),
            new ToggleButtonDiscreets("Primary", "Secondary"),
            new ToggleButtonDiscreets("Input2", "Input1"),
            new ToggleButtonDiscreets("No Latch", "Latch"),
            new ToggleButtonDiscreets("No Polling", "Polling"),
            new ToggleButtonDiscreets("MAIN", "BACKUP"),
            new ToggleButtonDiscreets("Tracking", "No Tracking"),
            new ToggleButtonDiscreets("Active", "Standby"),
            new ToggleButtonDiscreets("Not Present", "Present"),
            new ToggleButtonDiscreets("Show", "Hide"),
            new ToggleButtonDiscreets("Analog", "Digital"),
            new ToggleButtonDiscreets("OFFLINE", "ONLINE"),
            new ToggleButtonDiscreets("IPv4", "IPv6"),
            new ToggleButtonDiscreets("Alarm", "OK"),
            new ToggleButtonDiscreets("Opened", "Closed")
        };

        public static bool Contains(string discreet1, string discreet2)
        {
            return _list.Contains(new ToggleButtonDiscreets(discreet1, discreet2));
        }
    }

    internal class ToggleButtonDiscreets
    {
        public ToggleButtonDiscreets(string discreet1, string discreet2)
        {
            Discreet1 = discreet1;
            Discreet2 = discreet2;
        }

        public string Discreet1 { get; }

        public string Discreet2 { get; }

        public override string ToString()
        {
            return String.Join("/", Discreet1, Discreet2);
        }
    }

    internal class ToggleButtonOptionsComparer : IEqualityComparer<ToggleButtonDiscreets>
    {
        public bool Equals(ToggleButtonDiscreets x, ToggleButtonDiscreets y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (String.Equals(x.Discreet1, y.Discreet1, StringComparison.OrdinalIgnoreCase) && String.Equals(x.Discreet2, y.Discreet2, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (String.Equals(x.Discreet1, y.Discreet2, StringComparison.OrdinalIgnoreCase) && String.Equals(x.Discreet2, y.Discreet1, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(ToggleButtonDiscreets obj)
        {
            int result = 0;

            unchecked
            {
                if (obj.Discreet1 != null)
                {
                    result ^= obj.Discreet1.ToLower().GetHashCode();
                }

                if (obj.Discreet2 != null)
                {
                    result ^= obj.Discreet2.ToLower().GetHashCode();
                }
            }

            return result;
        }
    }

    internal enum ExtraData
    {
        ExpectedMeasurementType
    }
}