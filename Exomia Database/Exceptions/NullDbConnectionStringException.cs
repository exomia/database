using System;
using System.Data.Common;

namespace Exomia.Database.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     NullDbConnectionStringException class
    /// </summary>
    public class NullDbConnectionStringException : DbException
    {
        /// <inheritdoc />
        /// <summary>
        ///     NullDbConnectionStringException constructor
        /// </summary>
        public NullDbConnectionStringException() { }

        /// <inheritdoc />
        /// <summary>
        ///     NullDbConnectionStringException constructor
        /// </summary>
        /// <param name="message">mesaage</param>
        public NullDbConnectionStringException(string message)
            : base(message) { }

        /// <inheritdoc />
        /// <summary>
        ///     NullDbConnectionStringException constructor
        /// </summary>
        /// <param name="message">mesaage</param>
        /// <param name="errorCode">error code</param>
        public NullDbConnectionStringException(string message, int errorCode)
            : base(message, errorCode) { }

        /// <inheritdoc />
        /// <summary>
        ///     NullDbConnectionStringException constructor
        /// </summary>
        /// <param name="message">mesaage</param>
        /// <param name="innerException">inner exception</param>
        public NullDbConnectionStringException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}