namespace SLDisValidator2.Common.Attributes
{
    using System;
    using System.Reflection;

    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common.Enums;
    using SLDisValidator2.Interfaces;

    [AttributeUsage(AttributeTargets.Class)]
    public class TestAttribute : Attribute
    {
        public uint CheckId { get; }

        public Category Category { get; }

        public TestOrder TestOrder { get; }

        public TestAttribute(uint checkId, Category category, TestOrder testOrder = TestOrder.Mid)
        {
            Category = category;
            TestOrder = testOrder;
            CheckId = checkId;
        }

        public static TestAttribute GetAttribute(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetCustomAttribute(typeof(TestAttribute)) as TestAttribute;
        }

        public static TestAttribute GetAttribute(IValidate test)
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }

            return GetAttribute(test.GetType());
        }
    }
}