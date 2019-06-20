#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Database.Exceptions;

namespace Exomia.Database
{
    /// <summary>
    ///     Interface for database.
    /// </summary>
    public interface IDatabase : IDisposable
    {
        /// <summary>
        ///     Opens a connection to a database with the specified connection string.
        /// </summary>
        /// <exception cref="DbConnectionException">           if an error occured. </exception>
        /// <exception cref="NullDbConnectionException">       if no connection is initialized. </exception>
        /// <exception cref="NullDbConnectionStringException"> if connection string is null or empty. </exception>
        void Connect();

        /// <summary>
        ///     sets the connection string and call Connect() <see cref="Connect()" /> for details.
        /// </summary>
        /// <param name="connectionString"> The connection string to connect. </param>
        /// <exception cref="DbConnectionException">           if an error occured. </exception>
        /// <exception cref="NullDbConnectionException">       if no connection is initialized. </exception>
        /// <exception cref="NullDbConnectionStringException"> if connection string is null or empty. </exception>
        void Connect(string connectionString);

        /// <summary>
        ///     close a opened connection.
        /// </summary>
        void Close();
    }
}