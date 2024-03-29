﻿// <auto-generated>
// Do not change. Treat this as auto-generated code as this is converted from code in DataMiner.
// If there are software changes this allows easier comparison.
// </auto-generated>
namespace Skyline.DataMiner.CICD.Validators.Protocol.Helpers.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests;

    class ConditionMemberBlock
    {
        private readonly Variant varValue = new Variant();

        public ConditionMemberBlock(string strData, List<string> saStrings)
        {
            if (strData.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
            {
                string idValue = strData.Substring(3);

                try
                {
                    ParameterID = Convert.ToInt32(idValue);
                }
                catch (FormatException)
                {
                    throw new InvalidConditionException("Invalid id: operand: '" + strData + "'.");
                }
            }
            else if (strData.StartsWith("$", StringComparison.OrdinalIgnoreCase))
            {
                // This is a string placeholder "$idx$", where the idx value refers to the position in the saStrings parameter.
                int iPosition = Convert.ToInt32(strData.Substring(1, strData.Length - 2));
                if (saStrings.Count > iPosition)
                {
                    varValue.Type = VariantType.VT_BSTR;
                    varValue.StringValue = saStrings[iPosition];
                }
            }
            else if (strData.Equals("empty", StringComparison.OrdinalIgnoreCase))
            {
                varValue.Type = VariantType.VT_NULL;
            }
            else if (strData.Equals("emptystring", StringComparison.OrdinalIgnoreCase))
            {
                varValue.Type = VariantType.VT_BSTR;
                varValue.StringValue = "";
            }
            else if (!strData.StartsWith("#"))
            {
                try
                {
                    varValue.DoubleValue = Convert.ToDouble(strData, CultureInfo.InvariantCulture);
                    varValue.Type = VariantType.VT_R8;
                }
                catch (FormatException)
                {
                    throw new InvalidConditionException("Unexpected condition member block: '" + strData + "'.");
                }
                catch (OverflowException)
                {
                    throw new InvalidConditionException("Value too big to fit in a double: '" + strData + "'.");
                }
            }
        }

        public int? ParameterID { get; }

        public Variant GetValue(IProtocolModel protocolModel)
        {
            Variant value = new Variant();

            if (ParameterID == null || ParamHelper.IsGeneralParam((uint)ParameterID.Value))
            {
                value.Type = varValue.Type;
                value.BooleanValue = varValue.BooleanValue;
                value.StringValue = varValue.StringValue;
                value.DoubleValue = varValue.DoubleValue;
            }
            else
            {
                if (protocolModel.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, Convert.ToString(ParameterID.Value), out IParamsParam parameter))
                {
                    if (parameter.IsTable())
                    {
                        value.Type = VariantType.VT_ARRAY;
                        return value;
                    }

                    var interpretType = parameter?.Interprete?.Type;

                    if (interpretType == null)
                    {
                        throw new InvalidConditionException("The condition references a parameter that does not have a value defined for Interprete/Type");
                    }

                    if (interpretType.Value == Skyline.DataMiner.CICD.Models.Protocol.Enums.EnumParamInterpretType.Double || interpretType.Value == Skyline.DataMiner.CICD.Models.Protocol.Enums.EnumParamInterpretType.HighNibble)
                    {
                        value.Type = VariantType.VT_R8;
                        value.DoubleValue = 10;
                    }
                    else
                    {
                        value.Type = VariantType.VT_BSTR;
                        // A condition could assume that the string value store in a parameter represents a double.
                        value.StringValue = "10";
                    }
                }
                else
                {
                    throw new InvalidConditionException("Referenced parameter does not exist. Parameter ID: " + ParameterID.Value);
                }
            }

            return value;
        }

        public bool IsEmpty()
        {
            return false;
        }
    }
}