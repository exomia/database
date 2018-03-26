using System;
using System.Data.Common;

namespace Exomia.Database.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     NullDbConnectionException class
    /// </summary>
    public class NullDbConnectionException : DbException
    {
        /// <inheritdoc />
        /// <summary>
        ///     NullDbConnectionException constructor
        /// </summary>
        public NullDbConnectionException() { }

        /// <inheritdoc />
        /// <summary>
        ///     NullDbConnectionException constructor
        /// </summary>
        /// <param name="message">mesaage</param>
        public NullDbConnectionException(string message)
            : base(message) { }

        /// <inheritdoc />
        /// <summary>
        ///     NullDbConnectionException constructor
        /// </summary>
        /// <param name="message">mesaage</param>
        /// <param name="errorCode">error code</param>
        public NullDbConnectionException(string message, int errorCode)
            : base(message, errorCode) { }

        /// <inheritdoc />
        /// <summary>
        ///     NullDbConnectionException constructor
        /// </summary>
        /// <param name="message">mesaage</param>
        /// <param name="innerException">inner exception</param>
        public NullDbConnectionException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}