namespace Skyline.DataMiner.CICD.Validators.Protocol.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.CSharpAnalysis.Classes;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;

    using Skyline.DataMiner.CICD.Validators.Protocol.Tests;

    internal static class QActionHelper
    {
        public static bool ParameterCanBeSet(IParamsParam parameter)
        {
            return ParameterCanBeSetBasedOnTypeValue(parameter, out _);

            // TODO: RelationManager => In case of a column? Or include the column stuff in one big ParameterCanBeSet (Not sure if we'll have an usecase where we don't want to differentiate between columns and standalones)
            // TODO: parameter.IsTreeControl => When used as a treecontrol it is still read, but shouldn't be set.
            // TODO: Name => ContextMenu
            // TODO: Measurement\Type => Pagebutton

            // TODO: Check also on other ways to check if a parameter can be set or not. (Only check the attributes that can match with the allowedTypes)
            // Example: Virtual attribute => When set to destination it is meant to be set by element connections
        }

        public static bool ParameterCanBeSetBasedOnTypeValue(IParamsParam parameter, out EnumParamType? type)
        {
            type = parameter?.Type?.Value;
            if (type == null)
            {
                return false;
            }

            List<EnumParamType> allowedTypes = new List<EnumParamType>
            {
                // Technically the dummy can also be set, but then it doesn't make sense to keep it as dummy.
                EnumParamType.Read,
                EnumParamType.ReadBit,
                EnumParamType.Write,
                EnumParamType.WriteBit
                // Not sure about the group parameter?
            };

            if (allowedTypes.Contains(type.Value))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the provided table can be set from a QAction.
        /// </summary>
        public static bool TableCanBeSet(IParamsParam table)
        {
            var arrayOptions = table?.ArrayOptions;
            if (arrayOptions == null || arrayOptions.Count == 0)
            {
                return false;
            }

            if (arrayOptions.Any(typeColumnOption => typeColumnOption.Type?.Value == EnumColumnOptionType.ViewTableKey))
            {
                // Entire table shouldn't be touched
                return false;
            }

            // TODO: Check if there are any other case to be covered if necessary.

            return true;
        }

        /// <summary>
        /// Check if the provided column can be set from a QAction.
        /// In case there is no table specified, it will be retrieved.
        /// </summary>
        public static bool ColumnCanBeSet(IParamsParam column, IProtocolModel protocolModel, IParamsParam table = null)
        {
            return ColumnCanBeSetBasedOnType(column, protocolModel.RelationManager, out _, table) &&
                   ColumnCanBeSetBasedOnOptions(column, protocolModel.RelationManager, out _, table) &&
                   ColumnCanBeSetBasedOnTimerOptions(column, protocolModel, out _, table);
        }

        /// <summary>
        /// Check if the provided column can be set from a QAction.
        /// In case there is no table specified, it will be retrieved.
        /// </summary>
        public static bool ColumnCanBeSetBasedOnType(IParamsParam column, RelationManager relationManager, out EnumColumnOptionType? unsettableType, IParamsParam table = null)
        {
            unsettableType = null;

            if (column == null)
            {
                return false;
            }

            if (table == null && !column.TryGetTable(relationManager, out table))
            {
                // Couldn't find the correct table.
                return false;
            }

            if (table.ArrayOptions == null || table.ArrayOptions.Count == 0)
            {
                // Table has no columns defined.
                return false;
            }

            var columnOption = table.ArrayOptions.FirstOrDefault(typeColumnOption => typeColumnOption.Pid?.Value == column.Id?.Value);
            if (columnOption == null)
            {
                return false;
            }

            unsettableType = columnOption.Type?.Value;
            return unsettableType == EnumColumnOptionType.Custom || unsettableType == EnumColumnOptionType.Retrieved || unsettableType == EnumColumnOptionType.Snmp;
        }

        /// <summary>
        /// Check if the provided column can be set from a QAction.
        /// In case there is no table specified, it will be retrieved.
        /// </summary>
        public static bool ColumnCanBeSetBasedOnOptions(IParamsParam column, RelationManager relationManager, out IReadOnlyList<string> failingOptions, IParamsParam table = null)
        {
            failingOptions = new List<string>(0);

            if (column == null)
            {
                return false;
            }

            if (table == null && !column.TryGetTable(relationManager, out table))
            {
                // Couldn't find the correct table.
                return false;
            }

            if (table.ArrayOptions == null || table.ArrayOptions.Count == 0)
            {
                // Table has no columns defined.
                return false;
            }

            var columnOption = table.ArrayOptions.FirstOrDefault(typeColumnOption => typeColumnOption.Pid?.Value == column.Id?.Value);
            if (columnOption == null)
            {
                return false;
            }

            var options = columnOption.GetOptions();

            if (options == null)
            {
                // No options defined, so is safe to set.
                return true;
            }

            List<string> optionsThatFail = new List<string>();
            if (options.DVE.IsElement)
            {
                optionsThatFail.Add("element");
            }

            if (options.DVE.IsSeverity)
            {
                optionsThatFail.Add("severity");
            }

            // TODO: Delta is special... Not sure how to cover this
            // TODO: Groupby is very unclear in the DLL.
            // TODO: Separator is normally covered by the Type as it's only usable on concatenation & autoincrement columns.

            // TODO: Extend with other options if necessary

            failingOptions = optionsThatFail;

            return failingOptions.Count == 0;
        }

        /// <summary>
        /// Check if the provided column can be set from a QAction.
        /// In case there is no table specified, it will be retrieved.
        /// </summary>
        public static bool ColumnCanBeSetBasedOnTimerOptions(IParamsParam column, IProtocolModel protocolModel, out IReadOnlyDictionary<string, IReadOnlyList<string>> failingOptionsByTimerId, IParamsParam table = null)
        {
            failingOptionsByTimerId = new Dictionary<string, IReadOnlyList<string>>(0);

            if (column == null)
            {
                return false;
            }

            if (table == null && !column.TryGetTable(protocolModel.RelationManager, out table))
            {
                // Couldn't find the correct table.
                return false;
            }

            if (table.ArrayOptions == null || table.ArrayOptions.Count == 0)
            {
                // Table has no columns defined.
                return false;
            }

            uint? columnPid = column.Id?.Value;

            Dictionary<string, IReadOnlyList<string>> optionsByTimerId = new Dictionary<string, IReadOnlyList<string>>();
            foreach (ITimersTimer timer in protocolModel.EachTimerWithValidId())
            {
                ColumnCanBeSetBasedOnTimerOptionsForTimer(table, timer, columnPid, optionsByTimerId);
            }

            failingOptionsByTimerId = optionsByTimerId;
            return failingOptionsByTimerId.Count == 0;
        }

        private static void ColumnCanBeSetBasedOnTimerOptionsForTimer(IParamsParam table, ITimersTimer timer, uint? columnPid, Dictionary<string, IReadOnlyList<string>> optionsByTimerId)
        {
            TimerOptions timerOptions = timer.GetOptions();

            if (timerOptions == null || table.ArrayOptions == null)
            {
                return;
            }

            if (timerOptions.IPAddress?.TableParameterId?.Value != table.Id?.Value)
            {
                return;
            }

            List<string> failingOptions = new List<string>();

            var ping = timerOptions.Ping;
            (int index, string optionName)[] optionsToCheck =
            {
                (GetZeroBasedIndex(ping?.RttColumnPosition?.Value), "ping/rttColumn"),
                (GetZeroBasedIndex(ping?.TimeStampColumnPosition?.Value), "ping/timestampColumn"),
                (GetZeroBasedIndex(ping?.JitterColumnPosition?.Value), "ping/jitterColumn"),
                (GetZeroBasedIndex(ping?.LatencyColumnPosition?.Value), "ping/latencyColumn"),
                (GetZeroBasedIndex(ping?.PacketLossRateColumnPosition?.Value), "ping/packetLossRateColumn")
            };

            foreach ((int index, string optionName) in optionsToCheck)
            {
                if (index == -1)
                {
                    continue;
                }

                if (table.ArrayOptions.TryGetColumnOption(index, out ITypeColumnOption columnOption) && columnOption.Pid?.Value == columnPid)
                {
                    failingOptions.Add(optionName);
                }
            }

            if (failingOptions.Count > 0)
            {
                optionsByTimerId.Add(timer.Id.RawValue, failingOptions);
            }

            int GetZeroBasedIndex(uint? value)
            {
                return value == null ? -1 : (int)value.Value - 1;
            }
        }

        /// <summary>
        /// Will try to retrieve the parameter from the id specified in the Value.
        /// In case the value is not guaranteed or numeric, then it won't retrieve the parameter.
        /// In case it's an internal parameter, then it won't be retrieved as it's not in the protocol.
        /// </summary>
        public static bool TryToGetParamFromValue(Value value, IProtocolModel model, out IParamsParam param)
        {
            param = null;

            if (value == null || !value.HasStaticValue || !value.IsNumeric() || ParamHelper.IsInternalPid(value.AsInt32))
            {
                return false;
            }

            return model.TryGetObjectByKey(Mappings.ParamsById, value.AsInt32.ToString(), out param);
        }

        /// <summary>
        /// Will try to retrieve the parameter from the id specified in the Value.
        /// Lite version will not check if the Value is guaranteed, numeric or internal parameter. This method should be used if those conditions are already covered.
        /// </summary>
        public static bool TryToGetParamFromValueLite(Value value, IProtocolModel model, out IParamsParam param)
        {
            param = null;

            if (value == null)
            {
                return false;
            }

            return model.TryGetObjectByKey(Mappings.ParamsById, value.AsInt32.ToString(), out param);
        }
    }
}