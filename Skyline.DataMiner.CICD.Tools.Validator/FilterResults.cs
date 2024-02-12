using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

namespace Skyline.DataMiner.CICD.Tools.Validator
{
	using System.Collections.Generic;

	public class FilterResults
	{
		public IList<IValidationResult> Errors { get; set; }
		public IList<IValidationResult> SuppressedErrors { get; set; }
		//public IList<IValidationResult> InfoMessages { get; set; }

		public FilterResults()
		{
			Errors = new List<IValidationResult>();
			SuppressedErrors = new List<IValidationResult>();
			//InfoMessages = new List<IValidationResult>();
		}
	}
}
