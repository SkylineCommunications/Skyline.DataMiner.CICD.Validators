namespace Skyline.DataMiner.CICD.Validators.Protocol.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    internal static class GenericTests
    {
        /// <summary>
        /// Allows to perform some basic and generic checks on Tags and attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueTag">Tag or attribute to be checked.</param>
        /// <param name="isRequired">Allows to define if the tag/attribute is mandatory or not. In other word, if this method will potentially return a Missing status or not.</param>
        /// <returns>A bit-flag status, the rawValue as found in the XML, the parsed and trimmed value.</returns>
        public static (GenericStatus status, string rawValue, T value) CheckBasics<T>(IValueTag<T> valueTag, bool isRequired)
        {
            //TODO: we probably don't need to return a tuple anymore. Since we now have RawValue & Value, only returning the status should be enough.

            GenericStatus status = GenericStatus.None;

            if (valueTag == null)
            {
                // Attribute isn't there
                return (isRequired ? GenericStatus.Missing : status, null, default);
            }

            if (String.IsNullOrWhiteSpace(valueTag.RawValue))
            {
                // Attribute is empty (regardless of type)
                status |= GenericStatus.Empty;
            }

            if (valueTag.Value == null)
            {
                // Attribute couldn't be parsed to related model type (ex:Enum/UInt32/...)
                status |= GenericStatus.Invalid;
            }

            string trimmedValue = valueTag.RawValue?.Trim();
            if (trimmedValue != valueTag.RawValue)
            {
                // Attribute is untrimmed (regardless of type)
                status |= GenericStatus.Untrimmed;
            }

            dynamic returnValue = typeof(T) == typeof(string) ? (dynamic)trimmedValue : valueTag.Value;
            return (status, valueTag.RawValue, returnValue);
        }

        /// <summary>
        /// Allows to perform some basic and generic checks on splitted values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Value to be checked.</param>
        /// <returns>A bit-flag status, the parsed and trimmed value.</returns>
        public static (GenericStatus valueStatus, T convertedValue) CheckBasics<T>(string value)
        {
            GenericStatus status = GenericStatus.None;

            if (String.IsNullOrWhiteSpace(value))
            {
                // Value is empty
                status |= GenericStatus.Empty;
            }

            T newValue;
            try
            {
                newValue = (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception)
            {
                // Value couldn't be parsed to related type (ex:Int32/UInt32/...)
                status |= GenericStatus.Invalid;
                newValue = default;
            }

            string trimmedValue = value?.Trim();
            if (trimmedValue != value)
            {
                // Value is untrimmed
                status |= GenericStatus.Untrimmed;
            }

            dynamic returnValue = typeof(T) == typeof(string) ? (dynamic)trimmedValue : newValue;
            return (status, returnValue);
        }

        /// <summary>
        /// Checking for duplicate items in a provided collection. (Items with an Id)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The collection of items potentially containing duplicates.</param>
        /// <param name="getDuplicationIdentifier">The way of getting the identifier that needs to be compared in order to consider two items duplicated.</param>
        /// <param name="getName">The way of getting the name.</param>
        /// <param name="generateSubResult">The way of generating the subResults.</param>
        /// <param name="generateSummaryResult">The way of generating the summary result containing all subResults.</param>
        /// <returns>The summary result.</returns>
        /// <exception cref="System.ArgumentNullException">items OR getItemIdentifier OR generateResult</exception>
        public static IEnumerable<IValidationResult> CheckDuplicateIds<T>(IEnumerable<T> items,
            Func<T, uint?> getDuplicationIdentifier,
            Func<T, string> getName,
            Func<(T item, string name, string id), IValidationResult> generateSubResult,
            Func<(ICollection<string> names, string id, IValidationResult[] subResults), IValidationResult> generateSummaryResult)
            where T : IReadable
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (getDuplicationIdentifier == null)
            {
                throw new ArgumentNullException(nameof(getDuplicationIdentifier));
            }

            if (generateSubResult == null)
            {
                throw new ArgumentNullException(nameof(generateSubResult));
            }

            return CheckDuplicateIdsIterator(items, getDuplicationIdentifier, getName, generateSubResult, generateSummaryResult);
        }

        private static IEnumerable<IValidationResult> CheckDuplicateIdsIterator<T>(IEnumerable<T> items, Func<T, uint?> getDuplicationIdentifier, Func<T, string> getName, Func<(T item, string name, string id), IValidationResult> generateSubResult,
            Func<(ICollection<string> names, string id, IValidationResult[] subResults), IValidationResult> generateSummaryResult) where T : IReadable
        {
            var groups = items.GroupBy(getDuplicationIdentifier);

            foreach (var group in groups.Where(g => g.Count() > 1))
            {
                if (!group.Key.HasValue)
                {
                    // Ignore all tags without or with an invalid id
                    continue;
                }

                string id = Convert.ToString(group.Key.Value);

                var names = new List<string>();
                var subResults = new List<IValidationResult>();

                foreach (T item in group)
                {
                    string name = getName(item);
                    var result = generateSubResult((item, name, id));

                    names.Add(name);
                    subResults.Add(result);
                }

                yield return generateSummaryResult((names, id, subResults.ToArray()));
            }
        }

        /// <summary>
        /// Checking for duplicate items in a provided collection. (Items with an Id)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The collection of items potentially containing duplicates.</param>
        /// <param name="getDuplicationIdentifier">The way of getting the identifier that needs to be compared in order to consider two items duplicated.</param>
        /// <param name="getId">The way of getting the identifier.</param>
        /// <param name="generateSubResult">The way of generating the subResults.</param>
        /// <param name="generateSummaryResult">The way of generating the summary result containing all subResults.</param>
        /// <param name="isValidDuplicate">Can be used if duplicate values are allowed in some cases (ex: read/write param names and descriptions).</param>
        /// <returns>The summary result.</returns>
        /// <exception cref="System.ArgumentNullException">items OR getItemIdentifier OR generateResult</exception>
        public static IEnumerable<IValidationResult> CheckDuplicates<T>(
            IEnumerable<T> items,
            Func<T, string> getDuplicationIdentifier,
            Func<T, string> getId,
            Func<(T item, string id, string duplicateValue), IValidationResult> generateSubResult,
            Func<(ICollection<string> ids, string duplicateValue, IValidationResult[] subResults), IValidationResult> generateSummaryResult,
            Func<IList<T>, bool> isValidDuplicate = null)
            where T : IReadable
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (getDuplicationIdentifier == null)
            {
                throw new ArgumentNullException(nameof(getDuplicationIdentifier));
            }

            if (generateSubResult == null)
            {
                throw new ArgumentNullException(nameof(generateSubResult));
            }

            return CheckDuplicatesIterator(items, getDuplicationIdentifier, getId, generateSubResult, generateSummaryResult, isValidDuplicate);
        }

        /// <summary>
        /// Checking for duplicate items in a provided collection. (Items without an Id)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The collection of items potentially containing duplicates.</param>
        /// <param name="getDuplicationIdentifier">The way of getting the identifier that needs to be compared in order to consider two items duplicated.</param>
        /// <param name="generateSubResult">The way of generating the subResults.</param>
        /// <param name="generateSummaryResult">The way of generating the summary result containing all subResults.</param>
        /// <returns>The summary result.</returns>
        /// <exception cref="System.ArgumentNullException">items OR getItemIdentifier OR generateResult</exception>
        public static IEnumerable<IValidationResult> CheckDuplicates<T>(IEnumerable<T> items,
            Func<T, string> getDuplicationIdentifier,
            Func<(T item, string duplicateValue), IValidationResult> generateSubResult,
            Func<(string duplicateValue, IValidationResult[] subResults), IValidationResult> generateSummaryResult)
            where T : IReadable
        {
            return CheckDuplicatesNonModel(items, getDuplicationIdentifier, generateSubResult, generateSummaryResult);
        }

        private static IEnumerable<IValidationResult> CheckDuplicatesIterator<T>(IEnumerable<T> items, Func<T, string> getDuplicationIdentifier, Func<T, string> getId, Func<(T item, string id, string duplicateValue), IValidationResult> generateSubResult,
            Func<(ICollection<string> ids, string duplicateValue, IValidationResult[] subResults), IValidationResult> generateSummaryResult, Func<IList<T>, bool> isValidDuplicate) where T : IReadable
        {
            var groups = items.GroupBy(getDuplicationIdentifier, StringComparer.OrdinalIgnoreCase);

            foreach (var group in groups.Where(g => g.Count() > 1))
            {
                string duplicateValue = group.Key;

                if (String.IsNullOrWhiteSpace(duplicateValue))
                {
                    // Ignore all tags without or with an empty value
                    continue;
                }

                if (isValidDuplicate != null && isValidDuplicate(group.Select(x => x).ToList()))
                {
                    // Used if duplicate values are allowed in some cases (ex: read/write param names and descriptions)
                    continue;
                }

                var ids = new List<string>();
                var subResults = new List<IValidationResult>();

                foreach (T item in group)
                {
                    string id = getId(item);
                    var result = generateSubResult((item, id, getDuplicationIdentifier(item)));

                    ids.Add(id);
                    subResults.Add(result);
                }

                yield return generateSummaryResult((ids, duplicateValue, subResults.ToArray()));
            }
        }

        /// <summary>
        /// Checking for duplicate items in a provided collection. (Items without an Id)
        /// DUPLICATE as sometimes it's a collection of multiple different names (Example: Connections)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The collection of items potentially containing duplicates.</param>
        /// <param name="getDuplicationIdentifier">The way of getting the identifier that needs to be compared in order to consider two items duplicated.</param>
        /// <param name="generateSubResult">The way of generating the subResults.</param>
        /// <param name="generateSummaryResult">The way of generating the summary result containing all subResults.</param>
        /// <returns>The summary result.</returns>
        /// <exception cref="System.ArgumentNullException">items OR getItemIdentifier OR generateResult</exception>
        public static IEnumerable<IValidationResult> CheckDuplicatesNonModel<T>(IEnumerable<T> items,
            Func<T, string> getDuplicationIdentifier,
            Func<(T item, string duplicateValue), IValidationResult> generateSubResult,
            Func<(string duplicateValue, IValidationResult[] subResults), IValidationResult> generateSummaryResult)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (getDuplicationIdentifier == null)
            {
                throw new ArgumentNullException(nameof(getDuplicationIdentifier));
            }

            if (generateSubResult == null)
            {
                throw new ArgumentNullException(nameof(generateSubResult));
            }

            return CheckDuplicatesNonModelIterator(items, getDuplicationIdentifier, generateSubResult, generateSummaryResult);
        }

        private static IEnumerable<IValidationResult> CheckDuplicatesNonModelIterator<T>(IEnumerable<T> items, Func<T, string> getDuplicationIdentifier, Func<(T item, string duplicateValue), IValidationResult> generateSubResult,
            Func<(string duplicateValue, IValidationResult[] subResults), IValidationResult> generateSummaryResult)
        {
            var groups = items.GroupBy(getDuplicationIdentifier, StringComparer.InvariantCultureIgnoreCase);

            foreach (var group in groups.Where(g => g.Count() > 1))
            {
                string duplicateValue = group.Key;

                if (String.IsNullOrWhiteSpace(duplicateValue))
                {
                    // Ignore all tags without or with an empty value
                    continue;
                }

                var subResults = new List<IValidationResult>();

                foreach (T item in group)
                {
                    var result = generateSubResult((item, duplicateValue));
                    subResults.Add(result);
                }

                yield return generateSummaryResult((duplicateValue, subResults.ToArray()));
            }
        }

        public static bool IsPlainNumbers(string value)
        {
            string trimmedValue = value.Trim();
            return !trimmedValue.StartsWith("+") && (trimmedValue == "0" || !trimmedValue.StartsWith("0"));
        }
    }
}