#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Data.Common;

namespace Exomia.Database.Exceptions
{
    /// <summary>
    ///     Exception for signalling null database connection errors.
    /// </summary>
    public class NullDbConnectionException : DbException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NullDbConnectionException" /> class.
        /// </summary>
        public NullDbConnectionException() { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NullDbConnectionException" /> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        public NullDbConnectionException(string message)
            : base(message) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NullDbConnectionException" /> class.
        /// </summary>
        /// <param name="message">   The message. </param>
        /// <param name="errorCode"> The error code. </param>
        public NullDbConnectionException(string message, int errorCode)
            : base(message, errorCode) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NullDbConnectionException" /> class.
        /// </summary>
        /// <param name="message">        The message. </param>
        /// <param name="innerException"> The inner exception. </param>
        public NullDbConnectionException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}