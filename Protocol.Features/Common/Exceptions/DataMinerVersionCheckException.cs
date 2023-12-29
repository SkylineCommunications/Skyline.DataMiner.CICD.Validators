namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents errors that occur during application execution.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class DataMinerVersionCheckException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataMinerVersionCheckException"/> class.
        /// </summary>
        public DataMinerVersionCheckException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataMinerVersionCheckException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DataMinerVersionCheckException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataMinerVersionCheckException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public DataMinerVersionCheckException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataMinerVersionCheckException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected DataMinerVersionCheckException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}