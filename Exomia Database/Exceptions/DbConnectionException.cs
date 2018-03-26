using System;
using System.Data.Common;

namespace Exomia.Database.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     DbConnectionException class
    /// </summary>
    public class DbConnectionException : DbException
    {
        /// <inheritdoc />
        /// <summary>
        ///     DbConnectionException constructor
        /// </summary>
        public DbConnectionException() { }

        /// <inheritdoc />
        /// <summary>
        ///     DbConnectionException constructor
        /// </summary>
        /// <param name="message">mesaage</param>
        public DbConnectionException(string message)
            : base(message) { }

        /// <inheritdoc />
        /// <summary>
        ///     DbConnectionException constructor
        /// </summary>
        /// <param name="message">mesaage</param>
        /// <param name="errorCode">error code</param>
        public DbConnectionException(string message, int errorCode)
            : base(message, errorCode) { }

        /// <inheritdoc />
        /// <summary>
        ///     DbConnectionException constructor
        /// </summary>
        /// <param name="message">mesaage</param>
        /// <param name="innerException">inner exception</param>
        public DbConnectionException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}