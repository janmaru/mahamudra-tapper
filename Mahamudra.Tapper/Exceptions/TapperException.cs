using System;

namespace Mahamudra.Tapper.Exceptions
{
    public class TapperException : Exception
    {
        public TapperException()
        {
        }

        public TapperException(string message) : base(message)
        {
        }

        public TapperException(string message, Exception ex) : base(message, ex)
        {
        }
    }
} 