namespace SLDisValidator2.Common
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common.Attributes;

    internal class TestCollection<T>
    {
        internal TestCollection(IReadOnlyList<(T, TestAttribute)> tests)
        {
            Tests = tests;
        }

        public IReadOnlyList<(T Test, TestAttribute Attribute)> Tests { get; }

        public TestCollection<T> Filter(IEnumerable<(Category Category, uint CheckId)> checks)
        {
            checks = checks.ToList();

            return new TestCollection<T>(Tests.Where(FilterOut).ToList());

            bool FilterOut<U>((T test, U attribute) tuple) where U : TestAttribute
            {
                return checks.Any(x => x.Category == tuple.attribute.Category && x.CheckId == tuple.attribute.CheckId);
            }
        }
    }
}