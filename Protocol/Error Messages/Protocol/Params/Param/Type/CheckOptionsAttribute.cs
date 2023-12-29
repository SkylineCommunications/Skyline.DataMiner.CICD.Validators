namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Type.CheckOptionsAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    internal static class Error
    {
        public static IValidationResult MissingHeaderTrailerLinkOptions(IValidate test, IReadable referenceNode, IReadable positionNode, string headerOrTrailer, string paramId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.MissingHeaderTrailerLinkOptions,
                FullId = "2.21.4",
                Category = Category.Param,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("HeaderTrailerLink option should be defined on {0} with PID '{1}'.", headerOrTrailer, paramId),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidHeaderTrailerLinkOptions(IValidate test, IReadable referenceNode, IReadable positionNode, string headerOrTrailer, string paramId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.InvalidHeaderTrailerLinkOptions,
                FullId = "2.21.5",
                Category = Category.Param,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("HeaderTrailerLink option is wrongly defined on {0} with PID '{1}'.", headerOrTrailer, paramId),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult ExcessiveHeaderTrailerLinkOptions(IValidate test, IReadable referenceNode, IReadable positionNode, string paramId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.ExcessiveHeaderTrailerLinkOptions,
                FullId = "2.21.6",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("HeaderTrailerLink option should not be defined on Param '{0}' as it is nor a header nor a trailer.", paramId),
                HowToFix = "Remove the option.",
                ExampleCode = "",
                Details = "",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult DuplicateHeaderTrailerLinkOptions(IValidate test, IReadable referenceNode, IReadable positionNode, string linkId, string headerOrTrailer, string paramPids)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.DuplicateHeaderTrailerLinkOptions,
                FullId = "2.21.7",
                Category = Category.Param,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("HeaderTrailerLink with ID '{0}' defined on more than 1 {1}. PIDs {2}.", linkId, headerOrTrailer, paramPids),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InconsistentColumnTypeDimensions(IValidate test, IReadable referenceNode, IReadable positionNode, string columnTypes, string dimensions, string matrixPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.InconsistentColumnTypeDimensions,
                FullId = "2.21.8",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Matrix option '{0}' not inline with option '{1}'. Matrix PID '{2}'.", columnTypes, dimensions, matrixPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidColumnTypeParamRawType(IValidate test, IReadable referenceNode, IReadable positionNode, string rawType, string columnTypePid, string matrixPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.InvalidColumnTypeParamRawType,
                FullId = "2.21.9",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid Interprete/RawType '{0}' for 'Matrix ColumnType Param'. ColumnType PID '{1}'. Matrix PID '{2}'. Possible values 'numeric text, unsigned number'.", rawType, columnTypePid, matrixPid),
                HowToFix = "Either by changing the reference to the columntype parameter withing the matrix parameter." + Environment.NewLine + "Either by changing the RawType of the parameter currently referenced as bying the columntype parameter.",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult MissingMatrixOptions(IValidate test, IReadable referenceNode, IReadable positionNode, string optionName, string matrixPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.MissingMatrixOptions,
                FullId = "2.21.11",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing '{0}' option for matrix. Param ID '{1}'.", optionName, matrixPid),
                HowToFix = "Make sure both dimensions and columntypes options are correctly defined via the Param/Type@options attribute.",
                ExampleCode = "",
                Details = "Following options in Param/Type@options attribute are required for matrixes." + Environment.NewLine + " - dimensions=rowCount,columnCount" + Environment.NewLine + " - columnTypes=pid:minDiscreetValue-maxDiscreetValue",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult MissingAttributeForMatrix(IValidate test, IReadable referenceNode, IReadable positionNode, string matrixPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.MissingAttributeForMatrix,
                FullId = "2.21.12",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing attribute '{0}' in {1} '{2}'.", "Param/Type@options", "Matrix", matrixPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "Following options in Param/Type@options attribute are required for matrixes." + Environment.NewLine + " - dimensions=rowCount,columnCount" + Environment.NewLine + " - columnTypes=pid:minDiscreetValue-maxDiscreetValue",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidMatrixParamType(IValidate test, IReadable referenceNode, IReadable positionNode, string paramType, string matrixPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.InvalidMatrixParamType,
                FullId = "2.21.13",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid Param Type '{0}' on matrix. Matrix PID '{1}'.", paramType, matrixPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidMatrixOption(IValidate test, IReadable referenceNode, IReadable positionNode, string optionName, string matrixPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.InvalidMatrixOption,
                FullId = "2.21.14",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid syntax for the '{0}' option. Matrix PID '{1}'.", optionName, matrixPid),
                HowToFix = "Make sure both dimensions and columntypes options are correctly defined.",
                ExampleCode = "",
                Details = "Following options in Param/Type@options attribute are required for matrixes." + Environment.NewLine + " - dimensions=rowCount,columnCount" + Environment.NewLine + " - columnTypes=pid:minDiscreetValue-maxDiscreetValue",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidColumnTypeParamLengthType(IValidate test, IReadable referenceNode, IReadable positionNode, string lengthType, string columnTypePid, string matrixPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.InvalidColumnTypeParamLengthType,
                FullId = "2.21.15",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid Interprete/LengthType '{0}' for 'Matrix ColumnType Param'. ColumnType PID '{1}'. Matrix PID '{2}'. Possible values 'next param, fixed'.", lengthType, columnTypePid, matrixPid),
                HowToFix = "Either by changing the reference to the columntype parameter withing the matrix parameter." + Environment.NewLine + "Either by changing the LengthType of the parameter currently referenced as bying the columntype parameter.",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidColumnTypeParamType(IValidate test, IReadable referenceNode, IReadable positionNode, string type, string columnTypePid, string matrixPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.InvalidColumnTypeParamType,
                FullId = "2.21.16",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid Interprete/Type '{0}' for 'Matrix ColumnType Param'. ColumnType PID '{1}'. Matrix PID '{2}'. Possible values 'double'.", type, columnTypePid, matrixPid),
                HowToFix = "Either by changing the reference to the columntype parameter withing the matrix parameter." + Environment.NewLine + "Either by changing the Inteprete/Type of the parameter currently referenced as bying the columntype parameter.",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult MissingColumnTypeParam(IValidate test, IReadable referenceNode, IReadable positionNode, string missingColumnTypePid, string matrixPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.MissingColumnTypeParam,
                FullId = "2.21.17",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing 'columntypes' Param with ID '{0}' for matrix Param with ID '{1}'.", missingColumnTypePid, matrixPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult MissingColumnTypeParamInterprete(IValidate test, IReadable referenceNode, IReadable positionNode, string missingColumnTypePid, string matrixPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.MissingColumnTypeParamInterprete,
                FullId = "2.21.18",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing 'Interprete' Tag on matrix ColumnType Param with ID '{0}' for matrix Param with ID '{1}'.", missingColumnTypePid, matrixPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidColumnTypeParamInterprete(IValidate test, IReadable referenceNode, IReadable positionNode, string columnTypePid, string matrixPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.InvalidColumnTypeParamInterprete,
                FullId = "2.21.19",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid Interprete for 'Matrix ColumnType Param'. ColumnType PID '{0}'. Matrix PID '{1}'.", columnTypePid, matrixPid),
                HowToFix = "Either by changing the reference to the columntype parameter withing the matrix parameter." + Environment.NewLine + "Either by changing the Inteprete of the parameter currently referenced as bying the columntype parameter.",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UnrecommendedSshOptions(IValidate test, IReadable referenceNode, IReadable positionNode, string option, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.UnrecommendedSshOptions,
                FullId = "2.21.20",
                Category = Category.Param,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Unrecommended option '{0}' in Param '{1}'", option, pid),
                HowToFix = "Remove the option from the parameter." + Environment.NewLine + "Reference the parameter in the PortSettings.",
                ExampleCode = " <PortSettings name=\"SSH Connection\">" + Environment.NewLine + "  <IPport>" + Environment.NewLine + "   <DefaultValue>22</DefaultValue>" + Environment.NewLine + "  </IPport>" + Environment.NewLine + "  <BusAddress>" + Environment.NewLine + "   <Disabled>true</Disabled>" + Environment.NewLine + "  </BusAddress>" + Environment.NewLine + "  <PortTypeSerial>" + Environment.NewLine + "   <Disabled>true</Disabled>" + Environment.NewLine + "  </PortTypeSerial>" + Environment.NewLine + "  <PortTypeUDP>" + Environment.NewLine + "   <Disabled>true</Disabled>" + Environment.NewLine + "  </PortTypeUDP>" + Environment.NewLine + "  <SSH>" + Environment.NewLine + "   <Credentials>" + Environment.NewLine + "    <Username pid=\"1\" />" + Environment.NewLine + "    <Password pid=\"2\" />" + Environment.NewLine + "   </Credentials>" + Environment.NewLine + "  </SSH>" + Environment.NewLine + " </PortSettings>",
                Details = "Using SSH Username, SSH Password, or SSH Options will restrict the protocol to a single SSH connection over port 22. Attempting to use these with additional SSH connections or a different port may result in unpredictable behavior." + Environment.NewLine + "" + Environment.NewLine + "It is recommended to reference the parameter in the PortSettings instead. This allows you to define multiple SSH connections and configure any port number.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidMixOfSshOptionsAndPortSettings(IValidate test, IReadable referenceNode, IReadable positionNode, string option, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.InvalidMixOfSshOptionsAndPortSettings,
                FullId = "2.21.21",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Mixing option {0} and PortSettings SSH is invalid. Param ID '{1}'.", option, pid),
                HowToFix = "Remove the option from the parameter." + Environment.NewLine + "Reference the parameter in the PortSettings.",
                ExampleCode = "<PortSettings name=\"SSH Connection\">" + Environment.NewLine + "    <IPport>" + Environment.NewLine + "        <DefaultValue>22</DefaultValue>" + Environment.NewLine + "    </IPport>" + Environment.NewLine + "    <BusAddress>" + Environment.NewLine + "        <Disabled>true</Disabled>" + Environment.NewLine + "    </BusAddress>" + Environment.NewLine + "    <PortTypeSerial>" + Environment.NewLine + "        <Disabled>true</Disabled>" + Environment.NewLine + "    </PortTypeSerial>" + Environment.NewLine + "    <PortTypeUDP>" + Environment.NewLine + "        <Disabled>true</Disabled>" + Environment.NewLine + "    </PortTypeUDP>" + Environment.NewLine + "    <SSH>" + Environment.NewLine + "        <Credentials>" + Environment.NewLine + "            <Username pid=\"1100\"/>" + Environment.NewLine + "            <Password pid=\"1101\"/>" + Environment.NewLine + "        </Credentials>" + Environment.NewLine + "    </SSH>" + Environment.NewLine + "</PortSettings>",
                Details = "Conflicting SSH configurations detected." + Environment.NewLine + "You're using both SSH Username, SSH Password, SSH Options and 'PortSettings/SSH'." + Environment.NewLine + "SSH Username, SSH Password, SSH Options are restricted to one SSH connection on port 22 only and shouldn't be mixed with 'PortSettings/SSH'." + Environment.NewLine + "Use only one of these configurations." + Environment.NewLine + "'PortSettings/SSH' is generally better as it supports multiple SSH connections and use any port number." + Environment.NewLine + "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorCompare
    {
        public static IValidationResult MatrixDimensionsChanged(IReadable referenceNode, IReadable positionNode, string matrixPid, string oldDimensions, string newDimensions)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.MatrixDimensionsChanged,
                FullId = "2.21.2",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Matrix Dimensions on Param '{0}' was changed from '{1}' to '{2}'.", matrixPid, oldDimensions, newDimensions),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult MatrixDimensionsRemoved(IReadable referenceNode, IReadable positionNode, string matrixDimensions, string matrixPid)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.MatrixDimensionsRemoved,
                FullId = "2.21.3",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Matrix Dimensions '{0}' on Param '{1}' were removed.", matrixDimensions, matrixPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint MatrixDimensionsChanged = 2;
        public const uint MatrixDimensionsRemoved = 3;
        public const uint MissingHeaderTrailerLinkOptions = 4;
        public const uint InvalidHeaderTrailerLinkOptions = 5;
        public const uint ExcessiveHeaderTrailerLinkOptions = 6;
        public const uint DuplicateHeaderTrailerLinkOptions = 7;
        public const uint InconsistentColumnTypeDimensions = 8;
        public const uint InvalidColumnTypeParamRawType = 9;
        public const uint MissingMatrixOptions = 11;
        public const uint MissingAttributeForMatrix = 12;
        public const uint InvalidMatrixParamType = 13;
        public const uint InvalidMatrixOption = 14;
        public const uint InvalidColumnTypeParamLengthType = 15;
        public const uint InvalidColumnTypeParamType = 16;
        public const uint MissingColumnTypeParam = 17;
        public const uint MissingColumnTypeParamInterprete = 18;
        public const uint InvalidColumnTypeParamInterprete = 19;
        public const uint UnrecommendedSshOptions = 20;
        public const uint InvalidMixOfSshOptionsAndPortSettings = 21;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckOptionsAttribute = 21;
    }
}