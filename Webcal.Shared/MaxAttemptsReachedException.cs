namespace Webcal.Shared
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class MaxAttemptsReachedException : Exception
    {
        public MaxAttemptsReachedException()
        {
        }

        public MaxAttemptsReachedException(string message) : base(message)
        {
        }

        public MaxAttemptsReachedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected MaxAttemptsReachedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}