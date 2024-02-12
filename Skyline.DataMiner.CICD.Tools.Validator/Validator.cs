namespace Skyline.DataMiner.CICD.Tools.Validator
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Xml.Linq;

	using Skyline.DataMiner.CICD.Common;
	using Skyline.DataMiner.CICD.Models.Protocol;
	using Skyline.DataMiner.CICD.Models.Protocol.Read;
	using Skyline.DataMiner.CICD.Parsers.Common.Xml;
	using Skyline.DataMiner.CICD.Validators.Common.Data;
	using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
	using Skyline.DataMiner.CICD.Validators.Common.Model;
	using Skyline.DataMiner.CICD.Validators.Common.Suppressions;
	using Skyline.DataMiner.CICD.Validators.Common.Tools;
	using Skyline.DataMiner.XmlSchemas.Protocol;

	internal class Validator
	{
		/// <summary>
		/// Gets the minimum DataMiner version with its build number for which support is still given.
		/// </summary>
		private static string MinimumSupportedDataMinerVersionWithBuildNumber { get; } = "10.1.0.0 - 9966";

		/// <summary>
		/// Gets the minimum DataMiner version with its build number for which support is still given.
		/// </summary>
		private static DataMinerVersion MinSupportedDataMinerVersionWithBuildNumber { get; } = DataMinerVersion.Parse(MinimumSupportedDataMinerVersionWithBuildNumber);

		public async Task<ValidatorResults> ValidateProtocolSolution(string solutionFilePath, bool includeSuppressed)
		{
			string protocolCode = GetProtocolCode(solutionFilePath);

			return await ValidateSolution(solutionFilePath, protocolCode, includeSuppressed);
		}

		private static string GetProtocolCode(string protocolFolderPath)
		{
			var solutionDirectoryPath = new FileInfo(protocolFolderPath).Directory.FullName;

			string protocolFilePath = Path.GetFullPath(Path.Combine(solutionDirectoryPath, "protocol.xml"));

			if (!File.Exists(protocolFilePath))
			{
				throw new InvalidOperationException($"protocol.xml not found. Expected location '${protocolFilePath}'.");
			}

			string protocolCode = File.ReadAllText(protocolFilePath);
			return protocolCode;
		}

		private async Task<ValidatorResults> ValidateSolution(string solutionFilePath, string protocolCode, bool includeSuppressed)
		{
			DateTime timestamp = DateTime.Now;
			XDocument uomDoc;

			var uom = Resources.uom;
			uomDoc = XDocument.Parse(uom);

			using CancellationTokenSource cts = new CancellationTokenSource();

			var workspace = Microsoft.CodeAnalysis.MSBuild.MSBuildWorkspace.Create();
			var solution = await workspace.OpenSolutionAsync(solutionFilePath, cancellationToken: cts.Token);

			var res = workspace.Diagnostics;

			var parser = new Parser(protocolCode);
			var document = parser.Document;
			var model = new ProtocolModel(document);
			var inputData = new ProtocolInputData(model, document, new QActionCompilationModel(model, solution));

			ValidatorSettings settings = new ValidatorSettings(MinSupportedDataMinerVersionWithBuildNumber, new UnitList(uomDoc));

			Task<IList<IValidationResult>>[] tasks =
			{
				Task.Factory.StartNew(() =>
				{
                    // Legacy validator.
                    IValidator validator = new Validators.Protocol.Legacy.Validator();

					return validator.RunValidate(inputData, settings, cts.Token);
				}),
				Task.Factory.StartNew(() =>
				{
                    // New validator.
					IValidator validator = new Validators.Protocol.Validator();

					return validator.RunValidate(inputData, settings, cts.Token);
				})
			};

			// Run validator tasks and combine results.
			IList<IValidationResult> validatorResults = Task.WhenAll(tasks).Result.SelectMany(x => x).ToList();

			var lineInfoProvider = new SimpleLineInfoProvider(protocolCode);

			CommentSuppressionManager suppressionManager = new CommentSuppressionManager(document, lineInfoProvider);

			var results = ConvertResults(validatorResults, suppressionManager, lineInfoProvider);

			ProcessSubresults(results, results.Issues, includeSuppressed);

			results.Protocol = model?.Protocol?.Name?.Value;
			results.Version = model?.Protocol?.Version?.Value;

			results.ValidatorVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			results.ValidationTimestamp = timestamp;

			return results;
		}

		private void ProcessSubresults(ValidatorResults validatorResults, List<ValidatorResult> subResults, bool includeSuppressed)
		{
			foreach (var validatorResult in subResults)
			{
				if (validatorResult.Suppressed == false)
				{
					if (validatorResult.SubResults?.Count > 0)
					{
						ProcessSubresults(validatorResults, validatorResult.SubResults, includeSuppressed);
					}
					else
					{
						AddIssueToCounter(validatorResults, validatorResult.Severity, false);
					}
				}
				else
				{
					AddIssueToCounter(validatorResults, validatorResult.Severity, true);
				}
			}

			if (subResults.Count > 0 && !includeSuppressed)
			{
				var removedItems = subResults.RemoveAll(v => (v.Suppressed == true));
			}
		}

		private IList<IValidationResult> FilterResults(IList<IValidationResult> validatorResults, CommentSuppressionManager suppressionManager, XmlDocument document, string protocolCode)
		{
			List<IValidationResult> filteredResults = new List<IValidationResult>();

			foreach (var validatorResult in validatorResults)
			{
				if (!suppressionManager.IsSuppressed(validatorResult))
				{
					filteredResults.Add(validatorResult);

					if (validatorResult.SubResults?.Count > 0)
					{
						validatorResult.SubResults?.RemoveAll(v => suppressionManager.IsSuppressed(v));

						if (validatorResult.SubResults?.Count > 0)
						{
							FilterResults(validatorResult.SubResults, suppressionManager, document, protocolCode);
						}
					}
				}
			}

			return filteredResults;
		}

		private static string BuildIssueMessage(IValidationResult result)
		{
			var aoParameters = result.DescriptionParameters;

			if (aoParameters != null)
			{
				string[] parameters = aoParameters.Select(p => p?.ToString()).ToArray();
				return String.Format(result.DescriptionFormat, parameters);
			}
			else
			{
				if (result.DescriptionFormat != null)
				{
					// case when <d2p1:DescriptionParameters i:nil="true" />
					return result.DescriptionFormat;
				}
				else
				{
					return result.Description;
				}
			}
		}

		private static ValidatorResults ConvertResults(IList<IValidationResult> results, CommentSuppressionManager suppressionManager, ILineInfoProvider lineInfoProvider)
		{
			ValidatorResults validatorResults = new ValidatorResults();
			validatorResults.Issues = new List<ValidatorResult>();

			foreach (var result in results)
			{
				bool isSuppressed = suppressionManager.IsSuppressed(result);

				var validatorResult = CreateValidatorResult(result, isSuppressed, lineInfoProvider);

				if (result?.SubResults?.Count > 0)
				{
					AddSubResults(validatorResult, result, isSuppressed, suppressionManager, lineInfoProvider);
				}

				validatorResults.Issues.Add(validatorResult);
			}

			return validatorResults;
		}

		private static void AddSubResults(ValidatorResult validatorResult, IValidationResult result, bool isSuppressed, CommentSuppressionManager suppressionManager, ILineInfoProvider lineInfoProvider)
		{
			validatorResult.SubResults = new List<ValidatorResult>();

			foreach (var subresult in result.SubResults)
			{
				bool isSubResultSuppressed;
				if (!isSuppressed)
				{
					isSubResultSuppressed = suppressionManager.IsSuppressed(subresult);
				}
				else
				{
					isSubResultSuppressed = true;
				}

				var r = CreateValidatorResult(subresult, isSubResultSuppressed, lineInfoProvider);

				validatorResult.SubResults.Add(r);

				if (subresult.SubResults != null && subresult.SubResults.Count > 0)
				{
					AddSubResults(r, subresult, isSubResultSuppressed, suppressionManager, lineInfoProvider);
				}
			}
		}

		private void AddIssueToCounter(ValidatorResults validatorResults, Severity severity, bool isSuppressed)
		{
			if (isSuppressed)
			{
				switch (severity)
				{
					case Severity.Critical:
						validatorResults.SuppressedCriticalIssueCount++;
						break;
					case Severity.Major:
						validatorResults.SuppressedMajorIssueCount++;
						break;
					case Severity.Minor:
						validatorResults.SuppressedMinorIssueCount++;
						break;
					case Severity.Warning:
						validatorResults.SuppressedWarningIssueCount++;
						break;
				}
			}
			else
			{
				switch (severity)
				{
					case Severity.Critical:
						validatorResults.CriticalIssueCount++;
						break;
					case Severity.Major:
						validatorResults.MajorIssueCount++;
						break;
					case Severity.Minor:
						validatorResults.MinorIssueCount++;
						break;
					case Severity.Warning:
						validatorResults.WarningIssueCount++;
						break;
				}
			}
		}

		private static ValidatorResult CreateValidatorResult(IValidationResult result, bool isSuppressed, ILineInfoProvider lineInfoProvider)
		{
			string message = BuildIssueMessage(result);

			var validatorResult = new ValidatorResult
			{
				Certainty = result.Certainty,
				FixImpact = result.FixImpact,
				Category = result.Category,
				Description = message,
				Severity = result.Severity,
				Id = result.FullId != null ? result.FullId : Convert.ToString(result.ErrorId),
				Dve = result.DveExport != null ? $"{result.DveExport.Value.Name} [{result.DveExport.Value.TablePid}]" : null,
				Suppressed = isSuppressed
			};

			GetPosition(result, validatorResult, lineInfoProvider);

			return validatorResult;
		}

		private static void GetPosition(IValidationResult result, ValidatorResult validatorResult, ILineInfoProvider lineInfoProvider)
		{
			if (result.Position > 0 && result.Line <= 0)
			{
				try
				{
					lineInfoProvider.GetLineAndColumn(result.Position, out int line, out int col);
					validatorResult.Line = line + 1;
					validatorResult.Column = col + 1;
				}
				catch (Exception)
				{
					// Do nothing.
				}
			}
			else if (result.Position <= 0 && result.Line > 0)
			{
				try
				{
					int pos = lineInfoProvider.GetOffset(result.Line - 1, 0);
					pos += lineInfoProvider.GetFirstNonWhitespaceOffset(result.Line - 1) ?? 0;

					lineInfoProvider.GetLineAndColumn(pos, out int line, out int col);
					validatorResult.Line = line + 1;
					validatorResult.Column = col + 1;
				}
				catch (Exception)
				{
					// Do nothing.
				}
			}
		}
	}
}
