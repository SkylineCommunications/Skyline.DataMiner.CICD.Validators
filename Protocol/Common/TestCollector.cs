namespace Skyline.DataMiner.CICD.Validators.Protocol.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    internal static class TestCollector
    {
        private static readonly Lazy<TestCollection<IValidate>> CacheValidateTestsCollection = new Lazy<TestCollection<IValidate>>(GetAllTests<IValidate>, true);
        private static readonly Lazy<TestCollection<ICompare>> CacheCompareTestsCollection = new Lazy<TestCollection<ICompare>>(GetAllTests<ICompare>, true);

        public static TestCollection<IValidate> GetAllValidateTests()
        {
            return CacheValidateTestsCollection.Value;
        }

        public static TestCollection<ICompare> GetAllCompareTests()
        {
            return CacheCompareTestsCollection.Value;
        }

        private static TestCollection<T> GetAllTests<T>() where T : IRoot
        {
            List<(T, TestAttribute attr)> tests = new List<(T, TestAttribute)>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!typeof(T).IsAssignableFrom(type) || !type.IsClass)
                {
                    continue;
                }

                TestAttribute testAttribute = TestAttribute.GetAttribute(type);
                if (testAttribute == null)
                {
                    continue;
                }

                T test = (T)Activator.CreateInstance(type);
                tests.Add((test, testAttribute));
            }

            tests = tests.OrderBy(x => x.attr.TestOrder).ToList();

            return new TestCollection<T>(tests);
        }
    }
}