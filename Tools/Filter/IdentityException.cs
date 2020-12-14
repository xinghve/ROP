using System;

namespace Tools.Filter
{
    /// <summary>
    /// Exception type for app exceptions
    /// </summary>
    public class IdentityException : Exception
    {
        public IdentityException()
        { }

        public IdentityException(string message)
            : base(message)
        { }

        public IdentityException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
