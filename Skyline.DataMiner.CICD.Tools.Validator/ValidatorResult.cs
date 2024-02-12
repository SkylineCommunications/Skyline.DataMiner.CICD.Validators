namespace Skyline.DataMiner.CICD.Tools.Validator
{
	using System.Collections.Generic;

	using Skyline.DataMiner.CICD.Validators.Common.Model;

	/// <summary>
	/// Represents a validator result.
	/// </summary>
	public class ValidatorResult
	{
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

		/// <summary>
		/// Gets or sets the sub results.
		/// </summary>
		/// <value>The sub results.</value>
		public List<ValidatorResult> SubResults { get; set; }
	}
}
