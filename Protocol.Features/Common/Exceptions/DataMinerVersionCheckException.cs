namespace SLDisDmFeatureCheck.Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class DataMinerVersionCheckException : Exception
    {
        public DataMinerVersionCheckException() { }

        public DataMinerVersionCheckException(string message) : base(message) { }

        public DataMinerVersionCheckException(string message, Exception inner) : base(message, inner) { }

        protected DataMinerVersionCheckException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}