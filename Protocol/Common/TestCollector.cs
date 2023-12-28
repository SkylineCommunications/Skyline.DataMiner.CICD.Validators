namespace SLDisValidator2.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Interfaces;

    public static class TestCollector
    {
        private static readonly Lazy<TestCollection<IValidate>> _cacheValidateTestsCollection;
        private static readonly Lazy<TestCollection<ICompare>> _cacheCompareTestsCollection;

        static TestCollector()
        {
            _cacheValidateTestsCollection = new Lazy<TestCollection<IValidate>>(GetAllTests<IValidate>, true);
            _cacheCompareTestsCollection = new Lazy<TestCollection<ICompare>>(GetAllTests<ICompare>, true);
        }

        public static TestCollection<IValidate> GetAllValidateTests()
        {
            return _cacheValidateTestsCollection.Value;
        }

        public static TestCollection<ICompare> GetAllCompareTests()
        {
            return _cacheCompareTestsCollection.Value;
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