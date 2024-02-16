using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Skyline.DataMiner.CICD.Validators.Common.Model;

namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.HtmlWriter
{
    internal abstract class ValidatorResultTreeItem
    {
        public ValidatorResultTreeItem(ValidatorResult validatorResult)
        {
            Certainty = validatorResult.Certainty;
            FixImpact = validatorResult.FixImpact;
            Category = validatorResult.Category;
            Id = validatorResult.Id;
            Severity = validatorResult.Severity;
            Description = validatorResult.Description;
            Line = validatorResult.Line;
            Column = validatorResult.Column;
            Dve = validatorResult.Dve;
            Suppressed = validatorResult.Suppressed;
        }

        /// <summary>
        /// Gets or sets the certainty of the validator result.
        /// </summary>
        /// <value>The certainty of the validator result.</value>
        public Certainty Certainty { get; set; }

        /// <summary>
        /// Gets or sets the impact of fixing this issue.
        /// </summary>
        /// <value>The impact of fixing this issue.</value>
        public FixImpact FixImpact { get; set; }

        /// <summary>
        /// Gets or sets the category of this result.
        /// </summary>
        /// <value>The category of this result.</value>
        public Category Category { get; set; }

        /// <summary>
        /// Gets or sets the ID of the result.
        /// </summary>
        /// <value>The ID.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the severity of the result.
        /// </summary>
        /// <value>The severity.</value>
        public Severity Severity { get; set; }

        /// <summary>
        /// Gets or sets the description of the result.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the line where this issue occurs.
        /// </summary>
        /// <value>The line where this issue occurs.</value>
        public int Line { get; set; }

        /// <summary>
        /// Gets or sets the column where this issue occurs.
        /// </summary>
        /// <value>The column where this issue occurs.</value>
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets the DVE for which this issue was detected.
        /// </summary>
        /// <value>The DVE for which this issue was detected.</value>
        public string Dve { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this issue is suppressed (Default: false).
        /// </summary>
        /// <value><c>true</c> if the value is suppressed; otherwise, <c>false</c>.</value>
        public bool Suppressed { get; set; }

        public abstract int SuppressedCount { get; }

        public abstract int NonSuppressedCount { get; }

        /// <summary>
        /// Produces the HTML for this item.
        /// </summary>
        /// <param name="stringBuilder">String builder.</param>
        /// <param name="depth">The current node depth.</param>
        public abstract void WriteHtml(StringBuilder stringBuilder, int depth = 2);

        protected string GetState()
        {
            return Suppressed ? "Suppressed" : "Active";
        }
    }
}
