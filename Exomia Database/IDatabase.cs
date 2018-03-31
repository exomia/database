#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

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
        #region Methods

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

        #endregion
    }
}