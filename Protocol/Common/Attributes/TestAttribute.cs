namespace Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes
{
    using System;
    using System.Reflection;

    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Enums;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    /// <summary>
    /// Indicates that the class is a validator check.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class)]
    public class TestAttribute : Attribute
    {
        /// <summary>
        /// Gets the check identifier.
        /// </summary>
        public uint CheckId { get; }

        /// <summary>
        /// Gets the category.
        /// </summary>
        public Category Category { get; }

        /// <summary>
        /// Gets the test order.
        /// </summary>
        public TestOrder TestOrder { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAttribute"/> class.
        /// </summary>
        /// <param name="checkId">The check identifier.</param>
        /// <param name="category">The category.</param>
        /// <param name="testOrder">The test order.</param>
        public TestAttribute(uint checkId, Category category, TestOrder testOrder = TestOrder.Mid)
        {
            Category = category;
            TestOrder = testOrder;
            CheckId = checkId;
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The attribute.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
        internal static TestAttribute GetAttribute(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetCustomAttribute(typeof(TestAttribute)) as TestAttribute;
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <returns>The attribute.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="test"/> is <see langword="null"/>.</exception>
        internal static TestAttribute GetAttribute(IValidate test)
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }

            return GetAttribute(test.GetType());
        }
    }
}