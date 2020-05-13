using System;

namespace NRig
{
    public class NRigException : Exception
    {
        public NRigException()
        {
        }

        public NRigException(string message) : base(message)
        {
        }

        public NRigException(Exception innerException) : base(innerException.Message, innerException)
        {
        }

        public NRigException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}