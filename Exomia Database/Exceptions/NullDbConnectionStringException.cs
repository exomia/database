﻿#region MIT License

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
using System.Data.Common;

namespace Exomia.Database.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     NullDbConnectionStringException class
    /// </summary>
    public class NullDbConnectionStringException : DbException
    {
        #region Constructors

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

        #endregion
    }
}