namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckVolatileTableConsistency
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckVolatileTableConsistency, Category.Param)]
	internal class CheckVolatileTableConsistency : IValidate/*, ICodeFix, ICompare*/
	{
		public List<IValidationResult> Validate(ValidatorContext context)
		{
			List<IValidationResult> results = new List<IValidationResult>();

			var tableParams = context.EachParamWithValidId().Where(tableParam => IsVolatileArray(tableParam));

			foreach (IParamsParam tableParam in tableParams)
			{
				ValidateColumns(this, context, tableParam, results);
				ValidateExportRule(this, context, tableParam, results);
				ValidateDcfUsage(this, context, tableParam, results);
				ValidateColumnOptions(this, tableParam, results);
			}

			return results;
		}

		private static bool IsVolatileArray(IParamsParam param) =>
			param.Type?.Value == EnumParamType.Array &&
			param.ArrayOptions?.GetOptions()?.HasVolatile == true;

		private static void ValidateColumns(IValidate test,ValidatorContext context, IParamsParam tableParam, List<IValidationResult> results)
		{
			var relationManager = context.ProtocolModel?.RelationManager;
			var tableColumns = tableParam.GetColumns(relationManager, returnBaseColumnsIfDuplicateAs: true)
				.Select(c => c.pid)
				.Where(pid => !string.IsNullOrEmpty(pid));

			foreach (string columnPid in tableColumns)
			{
				if (!context.ProtocolModel.TryGetObjectByKey(Mappings.ParamsById, columnPid, out IParamsParam columnParam))
					continue;

				if (columnParam.Trending?.Value == true)
				{
					results.Add(Error.InvalidVolatileTableUsage(test, tableParam, columnParam, tableParam.Id?.RawValue, "trended column"));
				}

				if (columnParam.Alarm?.Monitored?.Value == true)
				{
					results.Add(Error.InvalidVolatileTableUsage(test, tableParam, columnParam, tableParam.Id?.RawValue, "alarmed column"));
				}
			}
		}

		private static void ValidateExportRule(IValidate test, ValidatorContext context, IParamsParam tableParam, List<IValidationResult> results)
		{
			if (context.ProtocolModel?.Protocol?.ExportRules?.Any(er => er.Table?.Value == tableParam.Id?.RawValue) == true)
			{
				results.Add(Error.InvalidVolatileTableUsage(test, tableParam, tableParam, tableParam.Id?.RawValue, "DVE customization"));
			}
		}

		private static void ValidateDcfUsage(IValidate test,ValidatorContext context, IParamsParam tableParam, List<IValidationResult> results)
		{
			if (context.ProtocolModel?.Protocol?.ParameterGroups?.Any(pg => pg.DynamicId?.Value == tableParam.Id?.Value) == true)
			{
				results.Add(Error.InvalidVolatileTableUsage(test, tableParam, tableParam, tableParam.Id?.RawValue, "DCF usage"));
			}
		}

		private static void ValidateColumnOptions(IValidate test,IParamsParam tableParam, List<IValidationResult> results)
		{
			foreach (var column in tableParam.ArrayOptions)
			{
				var options = column.GetOptions();
				if (options == null)
					continue;

				if (options.ForeignKey?.Pid != null)
				{
					results.Add(Error.InvalidVolatileTableUsage(test, tableParam, column, tableParam.Id?.RawValue, "foreign key"));
				}

				if (options.IsSaved)
				{
					results.Add(Error.InvalidVolatileTableUsage(test, tableParam, column, tableParam.Id?.RawValue, "saved column"));
				}
			}
		}

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {

        ////        default:
        ////            result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
        ////            break;
        ////    }

        ////    return result;
        ////}

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }
}