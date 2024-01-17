namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Type.CheckOptionsAttribute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Edit;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckOptionsAttribute, Category.Param)]
    internal class CheckOptionsAttribute : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            var results = new List<IValidationResult>();

            var model = context.ProtocolModel;

            if (model.Protocol?.Params == null)
            {
                return results;
            }

            ValidateHeaderTrailerLink validateHeaderTrailerLink = new ValidateHeaderTrailerLink(this, context, model, results);
            ValidateSshOptionsHelper validateSshOptionsHelper = new ValidateSshOptionsHelper(this, context, model, results);

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.Type == null)
                {
                    continue;
                }

                validateHeaderTrailerLink.ValidateHeaderTrailerLinkOption(param);

                if (param.IsMatrix())
                {
                    ValidateMatrix validateMatrix = new ValidateMatrix(this, param, param.Id.RawValue);

                    if (results.AddIfNotNull(validateMatrix.CheckMissingAttribute()))
                    {
                        continue;
                    }

                    if (results.AddIfNotNull(validateMatrix.CheckInvalidMatrixType()))
                    {
                        continue;
                    }

                    if (results.AddIfNotNull(validateMatrix.CheckMissingOption()))
                    {
                        continue;
                    }

                    if (results.AddIfNotNull(validateMatrix.CheckInvalidOptionSyntax()))
                    {
                        continue;
                    }

                    if (results.AddIfNotNull(validateMatrix.CheckMissingTypeForMatrixColumns()))
                    {
                        continue;
                    }

                    foreach (var columnTypeError in validateMatrix.CheckColumnType(model.Protocol?.Params))
                    {
                        results.Add(columnTypeError);
                    }
                }

                validateSshOptionsHelper.CheckUnrecommendedSshOptions(param);
                validateSshOptionsHelper.CheckInvalidMixOfSshOptionsAndPortSettings(param);
            }

            validateHeaderTrailerLink.ValidateHeaderTrailerLinksAreUnique();

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            var result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.ExcessiveHeaderTrailerLinkOptions:
                    {
                        var param = (IParamsParam)context.Result.ReferenceNode;
                        ValidateHeaderTrailerLink.RemoveHeaderTrailerLinkOption(context.Protocol.Params.Get(param));

                        result.Success = true;
                        break;
                    }

                case ErrorIds.MissingAttributeForMatrix:
                    ValidateMatrix.FixMissingAttribute(context);

                    result.Success = true;
                    break;

                case ErrorIds.InvalidColumnTypeParamRawType:
                    ValidateMatrix.FixColumnTypeInterpreteRawType(context);

                    result.Success = true;
                    break;

                case ErrorIds.InvalidColumnTypeParamLengthType:
                case ErrorIds.MissingColumnTypeParamInterprete:
                case ErrorIds.InvalidColumnTypeParamType:
                    ValidateMatrix.FixColumnTypeInterprete(context);

                    result.Success = true;
                    break;

                case ErrorIds.MissingColumnTypeParam:
                    ValidateMatrix.FixMissingColumnTypeParameter(context);

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

            foreach ((IParamsParam oldParam, IParamsParam newParam) in context.EachMatchingParam())
            {
                var oldType = oldParam.Type;
                var newType = newParam.Type;
                if (oldType == null || newType == null)
                {
                    continue;
                }

                var oldOptions = oldType.GetOptions();
                var newOptions = newType.GetOptions();

                bool hasOldDimensions = oldOptions?.Dimensions != null;
                bool hasNewDimensions = newOptions?.Dimensions != null;

                if (hasOldDimensions && !hasNewDimensions)
                {
                    string oldDimensionValue = $"{oldOptions.Dimensions.Rows},{oldOptions.Dimensions.Columns}";
                    results.Add(ErrorCompare.MatrixDimensionsRemoved(newType, newType, oldDimensionValue, newParam.Id?.RawValue));
                    continue;
                }

                if (hasOldDimensions)
                {
                    string oldDimensionValue = $"{oldOptions.Dimensions.Rows},{oldOptions.Dimensions.Columns}";
                    string newDimensionValue = $"{newOptions.Dimensions.Rows},{newOptions.Dimensions.Columns}";
                    if (oldDimensionValue != newDimensionValue)
                    {
                        results.Add(ErrorCompare.MatrixDimensionsChanged(newType, newType, newParam.Id?.RawValue, oldDimensionValue, newDimensionValue));
                    }
                }
            }

            return results;
        }
    }

    internal class ValidateSshOptionsHelper : ValidateHelperBase
    {
        private readonly bool hasSshPortSettings;

        public ValidateSshOptionsHelper(IValidate test, ValidatorContext context, IProtocolModel model, List<IValidationResult> results)
            : base(test, context, results)
        {
            hasSshPortSettings = HasSshPortSettings(model.Protocol);
        }

        public void CheckUnrecommendedSshOptions(IParamsParam param)
        {
            ParamTypeOptions paramTypeOptions = param.Type?.GetOptions();
            if (paramTypeOptions?.HasSshUsername == true)
            {
                results.Add(Error.UnrecommendedSshOptions(test, param, param.Type.Options, "SSH Username", param.Id?.RawValue));
            }

            if (paramTypeOptions?.HasSshPwd == true)
            {
                results.Add(Error.UnrecommendedSshOptions(test, param, param.Type.Options, "SSH PWD", param.Id?.RawValue));
            }

            if (paramTypeOptions?.HasSshOptions == true)
            {
                results.Add(Error.UnrecommendedSshOptions(test, param, param.Type.Options, "SSH Options", param.Id?.RawValue));
            }
        }

        public void CheckInvalidMixOfSshOptionsAndPortSettings(IParamsParam param)
        {
            if (!hasSshPortSettings)
            {
                return;
            }

            ParamTypeOptions paramTypeOptions = param?.Type?.GetOptions();

            if (paramTypeOptions?.HasSshUsername == true)
            {
                results.Add(Error.InvalidMixOfSshOptionsAndPortSettings(test, param, param.Type.Options, "SSH Username", param.Id?.RawValue));
            }

            if (paramTypeOptions?.HasSshPwd == true)
            {
                results.Add(Error.InvalidMixOfSshOptionsAndPortSettings(test, param, param.Type.Options, "SSH PWD", param.Id?.RawValue));
            }

            if (paramTypeOptions?.HasSshOptions == true)
            {
                results.Add(Error.InvalidMixOfSshOptionsAndPortSettings(test, param, param.Type.Options, "SSH Options", param.Id?.RawValue));
            }
        }

        private static bool HasSshPortSettings(IProtocol protocol)
        {
            bool mainConnectionHasSsh = protocol.PortSettings?.SSH != null;
            bool advancedConnectionHasSsh = protocol.Ports?.Any(settings => settings.SSH != null) == true;
            return mainConnectionHasSsh || advancedConnectionHasSsh;
        }
    }

    internal class ValidateHeaderTrailerLink : ValidateHelperBase
    {
        private readonly bool protocolTypeRequiresHeaderTrailerLinkOption;
        private readonly bool hasMultipleSerialConnections;
        private readonly Dictionary<uint, HashSet<IParamsParam>> headerAndTrailerParamsPerHeaderTrailerLinkOptionId;
        private readonly HashSet<uint> invalidConnectionIds;

        public ValidateHeaderTrailerLink(IValidate test, ValidatorContext context, IProtocolModel model, List<IValidationResult> results)
            : base(test, context, results)
        {
            headerAndTrailerParamsPerHeaderTrailerLinkOptionId = new Dictionary<uint, HashSet<IParamsParam>>();
            invalidConnectionIds = new HashSet<uint>();

            int serialConnectionCount = 0;
            protocolTypeRequiresHeaderTrailerLinkOption = false;

            foreach (var connection in model.Protocol.GetConnections())
            {
                switch (connection.Type)
                {
                    case EnumProtocolType.SmartSerial:
                    case EnumProtocolType.SmartSerialSingle:
                        serialConnectionCount++;
                        protocolTypeRequiresHeaderTrailerLinkOption = true;
                        break;
                    case EnumProtocolType.Serial:
                    case EnumProtocolType.SerialSingle:
                        serialConnectionCount++;
                        break;
                    case EnumProtocolType.Snmp:
                    case EnumProtocolType.Snmpv2:
                    case EnumProtocolType.Snmpv3:
                        invalidConnectionIds.Add(connection.Number);
                        break;
                    default:
                        // do nothing.
                        break;
                }
            }

            hasMultipleSerialConnections = serialConnectionCount > 1;
        }

        public static void RemoveHeaderTrailerLinkOption(ParamsParam param)
        {
            if (param.Type.Options == null)
            {
                return;
            }

            List<string> options = param.Type.Options.Value.Split(';').ToList();
            options.RemoveAll(option => option.StartsWith("headerTrailerLink", StringComparison.OrdinalIgnoreCase));

            if (options.Any())
            {
                param.Type.Options = String.Join(";", options);
            }
            else
            {
                param.Type.Options = null;
            }
        }

        public void ValidateHeaderTrailerLinkOption(IParamsParam param)
        {
            if (protocolTypeRequiresHeaderTrailerLinkOption &&
                (param.Type.Value == EnumParamType.Header || param.Type.Value == EnumParamType.Trailer))
            {
                ValidateIfHeaderTrailerLinkOptionIsDefinedAndValid(param);
            }
            else
            {
                ValidateIfHeaderTrailerLinkOptionIsNotDefined(param);
            }
        }

        public void ValidateHeaderTrailerLinksAreUnique()
        {
            foreach (var headerTrailerLink in headerAndTrailerParamsPerHeaderTrailerLinkOptionId)
            {
                ValidateHeaderTrailerLinkIdIsNotUsedMoreThanOnce(headerTrailerLink.Key, headerTrailerLink.Value);
            }
        }

        private void ValidateHeaderTrailerLinkIdIsNotUsedMoreThanOnce(uint headerTrailerLinkId, HashSet<IParamsParam> paramsUsingHeaderTrailerLinkId)
        {
            var linkId = Convert.ToString(headerTrailerLinkId);

            IParamsParam[] headerParams = paramsUsingHeaderTrailerLinkId.Where(param => param.Type.Value == EnumParamType.Header).ToArray();
            if (headerParams.Length > 1)
            {
                string paramPids = String.Join(", ", headerParams.Select(p => p.Id?.RawValue));
                IValidationResult error = Error.DuplicateHeaderTrailerLinkOptions(test, null, null, linkId, "header", paramPids);
                foreach (IParamsParam headerParam in headerParams)
                {
                    IValidationResult subResult = Error.DuplicateHeaderTrailerLinkOptions(test, headerParam, headerParam, linkId, "header", headerParam.Id?.RawValue);
                    error.WithSubResults(subResult);
                }

                results.Add(error);
            }

            IParamsParam[] trailerParams = paramsUsingHeaderTrailerLinkId.Where(p => p.Type.Value == EnumParamType.Trailer).ToArray();
            if (trailerParams.Length > 1)
            {
                string paramPids = String.Join(", ", trailerParams.Select(p => p.Id?.RawValue));
                IValidationResult error = Error.DuplicateHeaderTrailerLinkOptions(test, null, null, linkId, "trailer", paramPids);
                foreach (IParamsParam trailerParam in trailerParams)
                {
                    IValidationResult subResult = Error.DuplicateHeaderTrailerLinkOptions(test, trailerParam, trailerParam, linkId, "trailer", trailerParam.Id?.RawValue);
                    error.WithSubResults(subResult);
                }

                results.Add(error);
            }
        }

        private void ValidateIfHeaderTrailerLinkOptionIsDefinedAndValid(IParamsParam param)
        {
            string paramId = param.Id?.RawValue;
            string headerOrTrailer = EnumParamTypeConverter.ConvertBack(param.Type.Value.Value);

            if (param.Type.Options == null)
            {
                results.Add(Error.MissingHeaderTrailerLinkOptions(test, param, param, headerOrTrailer, paramId));
                return;
            }

            var paramTypeOptions = param.Type.GetOptions();

            if (paramTypeOptions.HeaderTrailerLink == null)
            {
                results.Add(Error.MissingHeaderTrailerLinkOptions(test, param, param, headerOrTrailer, paramId));
                return;
            }

            if (!paramTypeOptions.HeaderTrailerLink.Id.HasValue)
            {
                results.Add(Error.InvalidHeaderTrailerLinkOptions(test, param, param, headerOrTrailer, paramId));
                return;
            }

            if (!headerAndTrailerParamsPerHeaderTrailerLinkOptionId.TryGetValue(paramTypeOptions.HeaderTrailerLink.Id.Value, out var headerOrTrailerParams))
            {
                headerOrTrailerParams = new HashSet<IParamsParam>();
                headerAndTrailerParamsPerHeaderTrailerLinkOptionId.Add(paramTypeOptions.HeaderTrailerLink.Id.Value, headerOrTrailerParams);
            }

            headerOrTrailerParams.Add(param);
            ValidateIfHeaderTrailerLinkOptionHasValidConnection(paramTypeOptions, param, paramId, headerOrTrailer);
        }

        private void ValidateIfHeaderTrailerLinkOptionIsNotDefined(IParamsParam param)
        {
            if (param.Type.Options == null)
            {
                return;
            }

            if (param.Type.Options.Value.Split(';').Any(o => o.StartsWith("headerTrailerLink", StringComparison.OrdinalIgnoreCase)))
            {
                results.Add(Error.ExcessiveHeaderTrailerLinkOptions(test, param, param, param.Id?.RawValue));
            }
        }

        private void ValidateIfHeaderTrailerLinkOptionHasValidConnection(ParamTypeOptions paramTypeOptions, IParamsParam param, string paramId, string headerOrTrailer)
        {
            if (hasMultipleSerialConnections && paramTypeOptions.Connection == null)
            {
                results.Add(Error.HeaderTrailerLinkShouldHaveConnection(test, param, param, headerOrTrailer, paramId));
            }

            if (paramTypeOptions.Connection != null && invalidConnectionIds.Contains(paramTypeOptions.Connection.Value))
            {
                results.Add(Error.HeaderTrailerConnectionShouldBeValid(test, param, param, Convert.ToString(paramTypeOptions.Connection.Value), headerOrTrailer, paramId));
            }
        }
    }

    internal class ValidateMatrix
    {
        private readonly IValidate test;
        private readonly ParamTypeOptions options;
        private readonly IParamsParam param;
        private readonly IParamsParamType paramType;
        private readonly string matrixPid;

        public ValidateMatrix(IValidate test, IParamsParam param, string matrixPid)
        {
            this.test = test;
            this.matrixPid = matrixPid;
            this.param = param;
            options = param?.Type?.GetOptions();
            paramType = param?.Type;
        }

        public static void FixColumnTypeInterprete(CodeFixContext context)
        {
            IParamsParam readParam = (IParamsParam)context.Result.ReferenceNode;
            ParamsParam writeParam = context.Protocol.Params.Get(readParam);
            writeParam.Interprete = new ParamsParamInterprete
            {
                RawType = new ParamsParamInterpreteRawType(EnumParamInterpretRawType.NumericText),
                LengthType = new ParamsParamInterpreteLengthType(EnumParamInterpretLengthType.NextParam),
                Type = new ParamsParamInterpreteType(EnumParamInterpretType.Double)
            };
        }

        public static void FixColumnTypeInterpreteRawType(CodeFixContext context)
        {
            IParamsParam readParam = (IParamsParam)context.Result.ReferenceNode;
            ParamsParam writeParam = context.Protocol.Params.Get(readParam);
            writeParam.Interprete.RawType = new ParamsParamInterpreteRawType(EnumParamInterpretRawType.NumericText);
        }

        public static void FixMissingAttribute(CodeFixContext context)
        {
            IParamsParam readParam = (IParamsParam)context.Result.ReferenceNode;
            ParamsParam writeParam = context.Protocol.Params.Get(readParam);
            if (writeParam.Type == null)
            {
                writeParam.Type = new ParamsParamType(EnumParamType.Array);
            }

            writeParam.Type.Options = new AttributeValue<string>("dimensions={NoOfRows},{NoOfColumns};columntypes={PID}:0-{MaxColumn(0-based)}");
        }

        public static void FixMissingColumnTypeParameter(CodeFixContext context)
        {
            IParamsParam readParam = (IParamsParam)context.Result.ReferenceNode;
            uint? pid = readParam?.Type?.GetOptions()?.ColumnTypes?.ColumnTypes?[0].pid;
            if (pid == null)
            {
                return;
            }

            string matrixName = readParam.Name?.Value ?? "Matrix";

            ParamsParam newParam = new ParamsParam
            {
                Id = new AttributeValue<uint?>(pid),
                Name = new ElementValue<string>(matrixName + "_ColumnType"),
                Description = new ElementValue<string>(matrixName + "_ColumnType"),
                Trending = new AttributeValue<bool?>(false),
                Type = new ParamsParamType(EnumParamType.Read),
                Interprete = new ParamsParamInterprete
                {
                    RawType = new ParamsParamInterpreteRawType(EnumParamInterpretRawType.NumericText),
                    LengthType = new ParamsParamInterpreteLengthType(EnumParamInterpretLengthType.NextParam),
                    Type = new ParamsParamInterpreteType(EnumParamInterpretType.Double)
                }
            };

            context.Protocol.Params.Add(newParam);
        }

        public IValidationResult CheckInvalidMatrixType()
        {
            if (paramType == null)
            {
                return null;
            }

            if (paramType.Value != EnumParamType.Array && paramType.Value != EnumParamType.Write)
            {
                string typeValue;
                if (paramType.Value == null)
                {
                    typeValue = String.Empty;
                }
                else
                {
                    typeValue = EnumParamTypeConverter.ConvertBack(paramType.Value.Value);
                }

                return Error.InvalidMatrixParamType(test, param, paramType, typeValue, matrixPid);
            }

            return null;
        }

        public IValidationResult CheckMissingAttribute()
        {
            if (paramType == null)
            {
                return null;
            }

            if (options == null)
            {
                return Error.MissingAttributeForMatrix(test, param, paramType, matrixPid);
            }

            return null;
        }

        public IValidationResult CheckMissingOption()
        {
            if (paramType == null || options == null)
            {
                return null;
            }

            bool hasColumnTypes = options.ColumnTypes != null;
            bool hasDimensions = options.Dimensions != null;
            if (!hasColumnTypes && !hasDimensions)
            {
                return Error.MissingMatrixOptions(test, param, paramType, "dimensions and columntypes", matrixPid);
            }

            if (!hasDimensions)
            {
                return Error.MissingMatrixOptions(test, param, paramType, "dimensions", matrixPid);
            }

            if (!hasColumnTypes)
            {
                return Error.MissingMatrixOptions(test, param, paramType, "columntypes", matrixPid);
            }

            return null;
        }

        public IValidationResult CheckInvalidOptionSyntax()
        {
            if (paramType == null || options == null)
            {
                return null;
            }

            var dimensions = options.Dimensions;
            var columnTypes = options.ColumnTypes;

            if (columnTypes == null || dimensions == null)
            {
                return null;
            }

            if (dimensions.Columns == null || dimensions.Rows == null)
            {
                return Error.InvalidMatrixOption(test, param, paramType, dimensions.ToString(), matrixPid);
            }

            if (columnTypes.ColumnTypes == null || columnTypes.ColumnTypes.Count != 1 || !columnTypes.ColumnTypes[0].isValid)
            {
                return Error.InvalidMatrixOption(test, param, paramType, columnTypes.ToString(), matrixPid);
            }

            return null;
        }

        public IValidationResult CheckMissingTypeForMatrixColumns()
        {
            if (options == null)
            {
                return null;
            }

            var dimensions = options.Dimensions;
            var columnTypes = options.ColumnTypes;
            if (dimensions == null || columnTypes == null || dimensions.Columns == null)
            {
                return null;
            }

            if (columnTypes.ColumnTypes?.Count == 1 && columnTypes.ColumnTypes[0].isValid)
            {
                (uint? _, uint? fromIdx, uint? toIndex, bool isValid) = columnTypes.ColumnTypes[0];
                if (!isValid)
                {
                    return null;
                }

                if (fromIdx != 0 || toIndex != (dimensions.Columns - 1))
                {
                    return Error.InconsistentColumnTypeDimensions(test, param, paramType, columnTypes.ToString(), dimensions.ToString(), matrixPid);
                }

                return null;
            }

            return null;
        }

        public List<IValidationResult> CheckColumnType(IParams parameters)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            uint? columnTypePid = options?.ColumnTypes?.ColumnTypes?[0].pid;
            if (columnTypePid == null)
            {
                return new List<IValidationResult>(0);
            }

            var columnTypeParam = parameters.FirstOrDefault(p => p.Id.Value == columnTypePid);
            if (columnTypeParam == null)
            {
                results.Add(Error.MissingColumnTypeParam(test, param, param, Convert.ToString(columnTypePid), matrixPid));
                return results;
            }

            if (results.AddIfNotNull(CheckColumnTypeMissingInterprete(columnTypePid, columnTypeParam)))
            {
                return results;
            }

            results.AddIfNotNull(CheckColumnTypeInvalidInterprete(columnTypePid, columnTypeParam));

            return results;
        }

        private IValidationResult CheckColumnTypeMissingInterprete(uint? columnTypePid, IParamsParam columnTypeParam)
        {
            var interprete = columnTypeParam?.Interprete;
            if (interprete == null)
            {
                return Error.MissingColumnTypeParamInterprete(test, columnTypeParam, columnTypeParam, Convert.ToString(columnTypePid), matrixPid);
            }

            return null;
        }

        private IValidationResult CheckColumnTypeInvalidInterprete(uint? columnTypePid, IParamsParam columnTypeParam)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            results.AddIfNotNull(CheckColumnTypeRawType(columnTypePid, columnTypeParam));
            results.AddIfNotNull(CheckColumnTypeType(columnTypePid, columnTypeParam));
            results.AddIfNotNull(CheckColumnTypeLengthType(columnTypePid, columnTypeParam));

            if (results.Count > 0)
            {
                if (results.Count > 1)
                {
                    IValidationResult invalidColumnTypeParamInterprete = Error.InvalidColumnTypeParamInterprete(test, columnTypeParam, columnTypeParam, Convert.ToString(columnTypePid), matrixPid);
                    invalidColumnTypeParamInterprete.WithSubResults(results.ToArray());
                    return invalidColumnTypeParamInterprete;
                }

                return results[0];
            }

            return null;
        }

        private IValidationResult CheckColumnTypeRawType(uint? columnTypePid, IParamsParam columnTypeParam)
        {
            EnumParamInterpretRawType? rawType = columnTypeParam?.Interprete?.RawType?.Value;
            if (rawType != EnumParamInterpretRawType.NumericText && rawType != EnumParamInterpretRawType.UnsignedNumber)
            {
                string type = String.Empty;
                if (rawType.HasValue)
                {
                    type = EnumParamInterpretRawTypeConverter.ConvertBack(rawType.Value);
                }

                return Error.InvalidColumnTypeParamRawType(test, columnTypeParam, columnTypeParam, type, Convert.ToString(columnTypePid), matrixPid);
            }

            return null;
        }

        private IValidationResult CheckColumnTypeType(uint? columnTypePid, IParamsParam columnTypeParam)
        {
            EnumParamInterpretType? type = columnTypeParam?.Interprete?.Type?.Value;
            if (type != EnumParamInterpretType.Double)
            {
                string typeString = String.Empty;
                if (type.HasValue)
                {
                    typeString = EnumParamInterpretTypeConverter.ConvertBack(type.Value);
                }

                return Error.InvalidColumnTypeParamType(test, columnTypeParam, columnTypeParam, typeString, Convert.ToString(columnTypePid), matrixPid);
            }

            return null;
        }

        private IValidationResult CheckColumnTypeLengthType(uint? columnTypePid, IParamsParam columnTypeParam)
        {
            EnumParamInterpretLengthType? lengthType = columnTypeParam?.Interprete?.LengthType?.Value;
            if (lengthType != EnumParamInterpretLengthType.NextParam && lengthType != EnumParamInterpretLengthType.Fixed)
            {
                string type = String.Empty;
                if (lengthType.HasValue)
                {
                    type = EnumParamInterpretLengthTypeConverter.ConvertBack(lengthType.Value);
                }

                return Error.InvalidColumnTypeParamLengthType(test, columnTypeParam, columnTypeParam, type, Convert.ToString(columnTypePid), matrixPid);
            }

            return null;
        }
    }
}