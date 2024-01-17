namespace Skyline.DataMiner.CICD.Validators.Protocol.Helpers.Conditions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The exception that is thrown when a condition is invalid.
    /// </summary>
    [Serializable]
    public class InvalidConditionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConditionException"/> class.
        /// </summary>
        public InvalidConditionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConditionException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InvalidConditionException(string message)
        : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConditionException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public InvalidConditionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConditionException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is <see langword="null" />.</exception>
        /// <exception cref="SerializationException">The class name is <see langword="null" /> or HResult is zero (0).</exception>
        /// <remarks>This constructor is called during deserialization to reconstitute the exception object transmitted over a stream.</remarks>
        protected InvalidConditionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}