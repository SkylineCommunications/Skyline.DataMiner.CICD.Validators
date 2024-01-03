namespace Skyline.DataMiner.CICD.Validators.Protocol.Legacy
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Info on parameter positions.
    /// </summary>
    /// <seealso cref="System.IEquatable{Skyline.DataMiner.ProtocolValidator.Position}" />
    internal class Position : IEquatable<Position>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class. This <see cref="Position"/> will be empty.
        /// </summary>
        public Position()
        {
            Page = String.Empty;
            Row = "-1";
            Column = "-1";
            Export = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class without export (defaults to 0).
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        public Position(string page, string row, string column)
        {
            Page = page;
            Row = row;
            Column = column;
            Export = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class with export.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="export">Pid of the exported table.</param>
        public Position(string page, string row, string column, int export)
        {
            Page = page;
            Row = row;
            Column = column;
            Export = export;
        }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// Gets or sets the export.
        /// Pid of the exported table if present, 0 if not exported, -1 if export = true.
        /// </summary>
        public int Export { get; set; }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        public string Page { get; set; }

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        public string Row { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Position other)
        {
            if (other == null)
            {
                return false;
            }

            if (Page == other.Page && Row == other.Row && Column == other.Column && Export == other.Export)
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
            return Equals(obj as Position);
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
                hash = (hash * 23) + Page.GetHashCode();
                hash = (hash * 23) + Row.GetHashCode();
                hash = (hash * 23) + Column.GetHashCode();
                hash = (hash * 23) + Export.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Returns true if the <see cref="Position"/> is valid.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid()
        {
            return Page != String.Empty && Row != String.Empty && Column != String.Empty;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("Page: {0} Column: {1} Row: {2} Export: {3}", Page, Column, Row, Export);
        }
    }

    /// <summary>
    /// Compare two <see cref="Position"/> objects.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IEqualityComparer{Skyline.DataMiner.ProtocolValidator.Position}" />
    internal class PositionComparer : IEqualityComparer<Position>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(Position x, Position y)
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
        public int GetHashCode(Position obj)
        {
            return obj.GetHashCode();
        }
    }
}