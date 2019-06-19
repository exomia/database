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
using System.Collections.Generic;

namespace Exomia.Database
{
    /// <summary>
    ///     IDatabasePoolContainer wrapper interface.
    /// </summary>
    public interface IDatabasePoolContainer : IDisposable { }

    /// <summary>
    ///     IDatabasePoolContainer interface.
    /// </summary>
    /// <typeparam name="TDatabase"> IDatabase. </typeparam>
    public interface IDatabasePoolContainer<TDatabase> : IDatabasePoolContainer
        where TDatabase : IDatabase
    {
        /// <summary>
        ///     adds a new database.
        /// </summary>
        /// <param name="database"> database to add. </param>
        void Add(TDatabase database);

        /// <summary>
        ///     iterate over all added databases.
        /// </summary>
        /// <returns>
        ///     IEnumerable{TDatabase}
        /// </returns>
        IEnumerable<TDatabase> Foreach();

        /// <summary>
        ///     Lock a database from the IO pool and perform an action on it.
        /// </summary>
        /// <param name="action"> the action to perform. </param>
        void Lock(DatabaseAction<TDatabase> action);

        /// <summary>
        ///     Lock a database from the IO pool and perform a function on it.
        /// </summary>
        /// <typeparam name="TResult"> function return value. </typeparam>
        /// <param name="func"> the function to call. </param>
        /// <returns>
        ///     T2.
        /// </returns>
        TResult Lock<TResult>(DatabaseFunction<TDatabase, TResult> func);
    }
}