using System;
using Exomia.Database.Exceptions;

namespace Exomia.Database
{
    /// <inheritdoc />
    /// <summary>
    ///     IDatabase interface
    /// </summary>
    public interface IDatabase : IDisposable
    {
        /// <summary>
        ///     Opens a connection to a database with the speciefied connection string
        /// </summary>
        /// <returns>true if the connection is successfull</returns>
        /// <exception cref="DbConnectionException">if an error occured</exception>
        /// <exception cref="NullDbConnectionException">if no connction is initialized</exception>
        /// <exception cref="NullDbConnectionStringException">if connection string is null or empty</exception>
        void Connect();

        /// <summary>
        ///     sets the connection string and call Connect() <see cref="Connect()" /> for details
        /// </summary>
        /// <returns>true if the connection is successfull</returns>
        /// <exception cref="DbConnectionException">if an error occured</exception>
        /// <exception cref="NullDbConnectionException">if no connction is initialized</exception>
        /// <exception cref="NullDbConnectionStringException">if connection string is null or empty</exception>
        void Connect(string connectionString);

        /// <summary>
        ///     close a opened connection
        /// </summary>
        void Close();
    }
}