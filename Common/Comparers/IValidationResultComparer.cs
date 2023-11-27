namespace Skyline.DataMiner.CICD.Validators.Common.Comparers
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    /// <summary>
    /// Validation result comparer.
    /// </summary>
    public class IValidationResultComparer : IEqualityComparer<IValidationResult>
    {
        /// <summary>
        /// Gets the instance of the <see cref="IValidationResultComparer"/>.
        /// </summary>
        public static IValidationResultComparer Instance { get; } = new IValidationResultComparer();

        public bool Equals(IValidationResult x, IValidationResult y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Category == y.Category &&
                   x.Severity == y.Severity &&
                   String.Equals(x.FullId ?? Convert.ToString(x.ErrorId), y.FullId ?? Convert.ToString(y.ErrorId)) &&
                   String.Equals(x.Description, y.Description) &&
                   String.Equals(x.PositionNode.GetIdentifier(), y.PositionNode.GetIdentifier()) &&
                   String.Equals(x.ReferenceNode.GetIdentifier(), y.ReferenceNode.GetIdentifier()) &&
                   String.Equals(x.DveExport?.TablePid, y.DveExport?.TablePid);
        }

        public int GetHashCode(IValidationResult obj)
        {
            int hash = 17;

            unchecked
            {
                if (obj != null)
                {
                    hash = hash * 23 + obj.Category.GetHashCode();
                    hash = hash * 23 + obj.Severity.GetHashCode();
                    hash = hash * 23 + (obj.FullId ?? Convert.ToString(obj.ErrorId)).GetHashCode();
                    hash = hash * 23 + obj.Description.GetHashCode();
                    hash = hash * 23 + (obj.PositionNode != null ? obj.PositionNode.GetIdentifier().GetHashCode() : 0);
                    hash = hash * 23 + (obj.ReferenceNode != null ? obj.ReferenceNode.GetIdentifier().GetHashCode() : 0);
                    hash = hash * 23 + (obj.DveExport?.TablePid != null ? obj.DveExport.Value.GetHashCode() : 0);
                }
            }

            return hash;
        }
    }
}
