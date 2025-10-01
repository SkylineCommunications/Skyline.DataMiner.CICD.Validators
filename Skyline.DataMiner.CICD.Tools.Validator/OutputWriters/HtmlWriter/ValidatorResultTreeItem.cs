namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.HtmlWriter
{
    using System.Text;

    using Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.Results;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    internal abstract class ValidatorResultTreeItem(ValidatorResult validatorResult)
    {
        /// <summary>
        /// Gets or sets the certainty of the validator result.
        /// </summary>
        /// <value>The certainty of the validator result.</value>
        public Certainty Certainty { get; } = validatorResult.Certainty;

        /// <summary>
        /// Gets or sets the impact of fixing this issue.
        /// </summary>
        /// <value>The impact of fixing this issue.</value>
        public FixImpact FixImpact { get; } = validatorResult.FixImpact;

        /// <summary>
        /// Gets or sets the category of this result.
        /// </summary>
        /// <value>The category of this result.</value>
        public Category Category { get; } = validatorResult.Category;

        /// <summary>
        /// Gets or sets the ID of the result.
        /// </summary>
        /// <value>The ID.</value>
        public string Id { get; } = validatorResult.Id;

        /// <summary>
        /// Gets or sets the severity of the result.
        /// </summary>
        /// <value>The severity.</value>
        public Severity Severity { get; } = validatorResult.Severity;

        /// <summary>
        /// Gets or sets the description of the result.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; } = validatorResult.Description;

        /// <summary>
        /// Gets or sets the line where this issue occurs.
        /// </summary>
        /// <value>The line where this issue occurs.</value>
        public int Line { get; } = validatorResult.Line;

        /// <summary>
        /// Gets or sets the column where this issue occurs.
        /// </summary>
        /// <value>The column where this issue occurs.</value>
        public int Column { get; } = validatorResult.Column;

        /// <summary>
        /// Gets or sets the DVE for which this issue was detected.
        /// </summary>
        /// <value>The DVE for which this issue was detected.</value>
        public string? Dve { get; } = validatorResult.Dve;

        /// <summary>
        /// Gets or sets a value indicating whether this issue is suppressed (Default: false).
        /// </summary>
        /// <value><c>true</c> if the value is suppressed; otherwise, <c>false</c>.</value>
        public bool Suppressed { get; } = validatorResult.Suppressed;

        /// <summary>
        /// Gets the number of suppressed items this item represents.
        /// </summary>
        /// <value>The number of suppressed items this item represents.</value>
        public abstract int SuppressedCount { get; }

        /// <summary>
        /// Gets the number of non-suppressed items this item represents.
        /// </summary>
        /// <value>The number of non-suppressed items this item represents.</value>
        public abstract int NonSuppressedCount { get; }

        /// <summary>
        /// Produces the HTML for this item.
        /// </summary>
        /// <param name="stringBuilder">String builder.</param>
        /// <param name="depth">The current node depth.</param>
        /// <param name="includeSuppressed">Indicates whether suppressed results are included.</param>
        public abstract void WriteHtml(StringBuilder stringBuilder, bool includeSuppressed, int depth = 2);

        protected string GetState()
        {
            return Suppressed ? "Suppressed" : "Active";
        }
    }
}
