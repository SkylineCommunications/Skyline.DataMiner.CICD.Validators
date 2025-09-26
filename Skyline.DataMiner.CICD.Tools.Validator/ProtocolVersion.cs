using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Skyline.DataMiner.CICD.Tools.Validator;

namespace Skyline.DataMiner.CICD.Tools.Validator
{
    public class ProtocolVersion
    {
        public static readonly Regex protocolVersionRegex = new Regex("(?<branchVersion>[1-9][0-9]*)\\.(?<systemVersion>[0-9]+)\\.(?<majorVersion>[0-9]+)\\.(?<minorVersion>[0-9]+)(?<suffix>.*)");

        public ProtocolVersion(string version)
        {
            Match match = protocolVersionRegex.Match(version);

            if (match.Success)
            {
                string sBranch = match.Groups["branchVersion"].Value;
                string sSystemVersion = match.Groups["systemVersion"].Value;
                string sMajorVersion = match.Groups["majorVersion"].Value;
                string sMinorVersion = match.Groups["minorVersion"].Value;
                string suffix = match.Groups["suffix"].Value;

                int branch;
                int system;
                int major;
                int minor;

                if (Int32.TryParse(sBranch, out branch) && Int32.TryParse(sSystemVersion, out system) && Int32.TryParse(sMajorVersion, out major) && Int32.TryParse(sMinorVersion, out minor))
                {
                    BranchVersion = branch;
                    SystemVersion = system;
                    MajorVersion = major;
                    MinorVersion = minor;
                    Suffix = suffix;
                }
                else
                {
                    throw new ArgumentException("Unexpected format: " + version);
                }
            }
            else
            {
                throw new ArgumentException("Unexpected format: " + version);
            }
        }

        public ProtocolVersion(ProtocolVersion protocolVersion)
        {
            if (protocolVersion == null)
            {
                throw new ArgumentNullException(nameof(protocolVersion));
            }

            SystemVersion = protocolVersion.SystemVersion;
            BranchVersion = protocolVersion.BranchVersion;
            MajorVersion = protocolVersion.MajorVersion;
            MinorVersion = protocolVersion.MinorVersion;
            Suffix = protocolVersion.Suffix;
        }

        public ProtocolVersion(int branchVersion, int systemVersion, int majorVersion, int minorVersion)
        {
            BranchVersion = branchVersion;
            SystemVersion = systemVersion;
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
        }

        public int BranchVersion { get; private set; }
        public int SystemVersion { get; private set; }
        public int MajorVersion { get; private set; }
        public int MinorVersion { get; private set; }
        public string Suffix { get; private set; }

        /// <summary>
        /// Returns if this version is a newer major range than the other version.
        /// </summary>
        /// <param name="other">The other version.</param>
        /// <returns><c>true</c> if this version is a newer major range; otherwise, <c>false</c>.</returns>
        public bool IsNewerMajorRangeThan(ProtocolVersion other)
        {
            return BranchVersion == other.BranchVersion &&
                SystemVersion == other.SystemVersion &&
                MajorVersion > other.MajorVersion;
        }

        public bool IsNewerVersionInRange(ProtocolVersion other)
        {
            return BranchVersion == other.BranchVersion &&
                SystemVersion == other.SystemVersion &&
                MajorVersion == other.MajorVersion &&
                MinorVersion > other.MinorVersion;
        }

        public override bool Equals(object obj)
        {
            var version = obj as ProtocolVersion;
            return version != null &&
                   BranchVersion == version.BranchVersion &&
                   SystemVersion == version.SystemVersion &&
                   MajorVersion == version.MajorVersion &&
                   MinorVersion == version.MinorVersion &&
                   Suffix == version.Suffix;
        }

        public override int GetHashCode()
        {
            var hashCode = -1502655453;
            hashCode = hashCode * -1521134295 + BranchVersion.GetHashCode();
            hashCode = hashCode * -1521134295 + SystemVersion.GetHashCode();
            hashCode = hashCode * -1521134295 + MajorVersion.GetHashCode();
            hashCode = hashCode * -1521134295 + MinorVersion.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Suffix);
            return hashCode;
        }

        public string ToRangeString()
        {
            return $"{BranchVersion}.{SystemVersion}.{MajorVersion}.X";
        }

        public override string ToString()
        {
            return $"{BranchVersion}.{SystemVersion}.{MajorVersion}.{MinorVersion}{Suffix}";
        }
    }
}

