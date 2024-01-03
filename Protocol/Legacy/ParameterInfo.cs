namespace Skyline.DataMiner.CICD.Validators.Protocol.Legacy
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    /// <summary>
    /// Holds info on the parameter and its <see cref="Position"/>s.
    /// </summary>
    /// <seealso cref="System.IEquatable{Skyline.DataMiner.ProtocolValidator.ParameterInfo}" />
    internal class ParameterInfo : IEquatable<ParameterInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterInfo" /> class.
        /// </summary>
        /// <param name="pid">The parameter id.</param>
        /// <param name="name">The parameter name.</param>
        /// <param name="description">The description.</param>
        /// <param name="type">The parameter type.</param>
        /// <param name="intType">Type of the interprete.</param>
        /// <param name="intRawType">RawType of the interprete.</param>
        /// <param name="meastype">The type of the measurement.</param>
        /// <param name="rtDisplay">If set to <c>true</c> [rt display].</param>
        /// <param name="linenum">The line number.</param>
        /// <param name="lengthType">Type of the length.</param>
        /// <param name="lengthTypeId">The length type identifier.</param>
        /// <param name="trended">If set to <c>true</c> [trended].</param>
        /// <param name="alarmed">If set to <c>true</c> [alarmed].</param>
        /// <param name="virtualsource">If set to <c>true</c> [virtualsource].</param>
        /// <param name="element">The element.</param>
        /// <param name="position">The position.</param>
        public ParameterInfo(int pid, string name, string description, string type, string intType, string intRawType, string meastype, bool rtDisplay, string linenum, string lengthType, string lengthTypeId, bool trended, bool alarmed, bool virtualsource, XmlNode element, params Position[] position)
        {
            Pid = pid;
            Name = name;
            Description = description;
            Type = type;
            IntType = intType;
            IntRawType = intRawType;
            MeasType = meastype;
            Positions = position;
            RTDisplay = rtDisplay;
            LineNum = linenum;
            LengthType = lengthType;
            LengthTypeId = lengthTypeId;
            Trended = trended;
            Alarmed = alarmed;
            VirtualSource = virtualsource;
            Element = element;
        }

        public int? DuplicateAs { get; set; }

        /// <summary>
        /// Gets a value indicating whether a Parameter is alarmed.
        /// </summary>
        public bool Alarmed { get; }

        /// <summary>
        /// Gets the Parameter description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the element.
        /// </summary>
        public XmlNode Element { get; }

        /// <summary>
        /// Gets the Parameter interprete type : string, double, ...
        /// </summary>
        public string IntType { get; }

        /// <summary>
        /// Gets the Parameter interprete raw type : other, numeric, ...
        /// </summary>
        public string IntRawType { get; }

        /// <summary>
        /// Gets the length type.
        /// </summary>
        public string LengthType { get; }

        /// <summary>
        /// Gets the length type id.
        /// </summary>
        public string LengthTypeId { get; }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        public string LineNum { get; }

        /// <summary>
        /// Gets the Parameter measurement type : string, number, discreet, ...
        /// </summary>
        public string MeasType { get; }

        /// <summary>
        /// Gets the Parameter name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the Parameter id.
        /// </summary>
        public int Pid { get; }

        /// <summary>
        /// Gets the positions.
        /// </summary>
        public Position[] Positions { get; }

        /// <summary>
        /// Gets a value indicating whether a Parameter is needed in SLElement.
        /// </summary>
        public bool RTDisplay { get; }

        /// <summary>
        /// Gets a value indicating whether a Parameter is trended.
        /// </summary>
        public bool Trended { get; }

        /// <summary>
        /// Gets the Parameter type: read, write, ...
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Gets a value indicating whether a Parameter type has virtual=source attribute.
        /// </summary>
        public bool VirtualSource { get; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// True if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(ParameterInfo other)
        {
            if (other == null)
            {
                return false;
            }

            bool positioncomparer = true;
            if (Positions.Length == other.Positions.Length)
            {
                for (int i = 0; i < Positions.Length; i++)
                {
                    positioncomparer &= Positions[i].Equals(other.Positions[i]);
                }
            }
            else
            {
                positioncomparer = false;
            }

            if (Pid == other.Pid && Name == other.Name && Description == other.Description &&
                Type == other.Type && IntType == other.IntType && MeasType == other.MeasType &&
                positioncomparer && RTDisplay == other.RTDisplay && LineNum == other.LineNum &&
                LengthType == other.LengthType && LengthTypeId == other.LengthTypeId &&
                Trended == other.Trended && Alarmed == other.Alarmed && IntRawType == other.IntRawType &&
                VirtualSource == other.VirtualSource && Element == other.Element)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as ParameterInfo);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            // Overflow is fine, just wrap
            unchecked
            {
                int hash = 17;

                // Suitable nullity checks etc, of course :)
                hash = (hash * 23) + Pid.GetHashCode();
                hash = (hash * 23) + Name.GetHashCode();
                hash = (hash * 23) + Description.GetHashCode();
                hash = (hash * 23) + Type.GetHashCode();
                hash = (hash * 23) + IntType.GetHashCode();
                hash = (hash * 23) + IntRawType.GetHashCode();
                hash = (hash * 23) + MeasType.GetHashCode();
                hash = (hash * 23) + Positions.GetHashCode();
                hash = (hash * 23) + RTDisplay.GetHashCode();
                hash = (hash * 23) + LineNum.GetHashCode();
                hash = (hash * 23) + LengthType.GetHashCode();
                hash = (hash * 23) + LengthTypeId.GetHashCode();
                hash = (hash * 23) + Trended.GetHashCode();
                hash = (hash * 23) + Alarmed.GetHashCode();
                hash = (hash * 23) + VirtualSource.GetHashCode();
                hash = (hash * 23) + Element.GetHashCode();
                return hash;
            }
        }
    }

    /// <summary>
    /// Compare two <see cref="ParameterInfo"/> objects.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IEqualityComparer{Skyline.DataMiner.ProtocolValidator.ParameterInfo}" />
    internal class ParameterInfoComparer : IEqualityComparer<ParameterInfo>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>
        /// True if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(ParameterInfo x, ParameterInfo y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(ParameterInfo obj)
        {
            return obj.GetHashCode();
        }
    }
}