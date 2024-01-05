namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Relations.Relation.CheckPathAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    using static Skyline.DataMiner.CICD.Models.Protocol.Read.ColumnOptionOptions;

    [Test(CheckId.CheckPathAttribute, Category.Relation)]
    internal class CheckPathAttribute : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var relations = context?.ProtocolModel?.Protocol?.Relations;
            if (relations == null)
            {
                return results;
            }

            foreach (var relation in relations)
            {
                (GenericStatus status, string _, string value) = GenericTests.CheckBasics(relation.Path, isRequired: true);

                // Missing
                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingAttribute(this, relation, relation));
                    continue;
                }

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, relation, relation));
                    continue;
                }

                ValidateHelper helper = new ValidateHelper(this, context, results, relation, value);
                helper.CheckTables();
                helper.CheckForeignKeys();
            }

            // Check for duplicate paths.
            var resultsForDuplicatePaths = GenericTests.CheckDuplicates(
                items: relations,
                getDuplicationIdentifier: relation => relation.Path?.Value,
                generateSubResult: x => Error.DuplicateValue(this, x.item, x.item, x.duplicateValue),
                generateSummaryResult: x => Error.DuplicateValue(this, relations, null, x.duplicateValue).WithSubResults(x.subResults)
            );

            results.AddRange(resultsForDuplicatePaths);

            return results;
        }

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {

        ////        default:
        ////            result.Message = String.Format("This error ({0}) isn't implemented.", context.Result.ErrorId.ToString());
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

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly ValidatorContext context;
        private readonly IProtocolModel model;
        private readonly List<IValidationResult> results;

        private readonly IRelationsRelation relation;
        private readonly string pathValue;
        private readonly string[] pathValues;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IRelationsRelation relation, string pathValue)
        {
            this.test = test;
            this.context = context;
            this.model = context.ProtocolModel;
            this.results = results;

            this.relation = relation;
            this.pathValue = pathValue;
            this.pathValues = pathValue.Split(';');
        }

        public void CheckTables()
        {
            List<IValidationResult> subResults = new List<IValidationResult>();

            foreach (string pathTableId in pathValues)
            {
                (GenericStatus valueStatus, uint _) = GenericTests.CheckBasics<uint>(pathTableId);

                // Invalid Value
                if (valueStatus.HasFlag(GenericStatus.Invalid))
                {
                    subResults.Add(Error.InvalidValue(test, relation, relation, pathTableId));
                    continue;
                }

                // Non Existing PID
                if (!model.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, pathTableId, out var tableParam))
                {
                    subResults.Add(Error.NonExistingId(test, relation, relation, pathTableId));
                    continue;
                }

                // Not a table
                if (!tableParam.IsTable())
                {
                    subResults.Add(Error.ReferencedParamWrongType(test, relation, tableParam, tableParam.Type?.RawValue, pathTableId));
                    continue;
                }

                // Param Requiring RTDisplay
                IValidationResult rtDisplayError = Error.ReferencedParamExpectingRTDisplay(test, relation, relation, pathTableId);
                context.CrossData.RtDisplay.AddParam(tableParam, rtDisplayError);
            }

            if (subResults.Count <= 0)
            {
                return;
            }

            if (subResults.Count > 1 || pathValues.Length > 1)
            {
                IValidationResult invalidValue = Error.InvalidValue(test, relation, relation, pathValue);
                invalidValue.WithSubResults(subResults.ToArray());
                results.Add(invalidValue);
            }
            else
            {
                results.Add(subResults[0]);
            }
        }

        public void CheckForeignKeys()
        {
            List<IValidationResult> subFkResults = new List<IValidationResult>();

            // Compare 2 by 2
            // path="1000;2000;3000" length = 3 but we want index to get to the second element only (1) so length - 1 = 2
            for (int index = 0; index < pathValues.Length - 1; index++)
            {
                string table1Pid = pathValues[index];
                string table2Pid = pathValues[index + 1];

                // First check if both are tables
                if (!model.TryGetObjectByKey(Mappings.ParamsById, table1Pid, out IParamsParam tableParamLeft) || !tableParamLeft.IsTable())
                {
                    continue;
                }

                if (!model.TryGetObjectByKey(Mappings.ParamsById, table2Pid, out IParamsParam tableParamRight) || !tableParamRight.IsTable())
                {
                    continue;
                }

                // Missing FK
                bool foreignKeyFound = CheckForForeignKeys(tableParamLeft, table2Pid) || CheckForForeignKeys(tableParamRight, table1Pid);
                if (!foreignKeyFound)
                {
                    subFkResults.Add(Error.MissingForeignKeyInTable_Sub(test, relation, relation, table1Pid, table2Pid));
                }
            }

            if (subFkResults.Count > 0)
            {
                IValidationResult missingForeignKeyForRelation = Error.MissingForeignKeyForRelation(test, relation, relation, pathValue);
                missingForeignKeyForRelation.WithSubResults(subFkResults.ToArray());
                results.Add(missingForeignKeyForRelation);
            }
        }

        private static bool CheckForForeignKeys(IParamsParam tableParam, string pathTableId)
        {
            if (tableParam?.ArrayOptions == null)
            {
                return false;
            }

            foreach (ITypeColumnOption column in tableParam.ArrayOptions)
            {
                ColumnOptionOptions options = column.GetOptions();

                ForeignKeyClass foreignKey = options?.ForeignKey;
                if (foreignKey?.Pid == null)
                {
                    continue;
                }

                if (Convert.ToString(foreignKey.Pid) == pathTableId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}