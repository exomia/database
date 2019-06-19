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

namespace Exomia.Database
{

    /// <summary>
    ///     Interface for database manager.
    /// </summary>
    public interface IDatabaseManager
    {
        /// <summary>
        ///     Register a new database IO pool.
        /// </summary>
        /// <typeparam name="TDatabase"> IDatabase. </typeparam>
        /// <param name="count">                 count to register. </param>
        /// <param name="createIOPoolContainer"> (Optional) function to create a new
        ///                                      IDatabasePoolContainer{TDatabase} </param>
        /// <param name="action">                (Optional) called then a new database is initialized. </param>
        void Register<TDatabase>(int count, Func<IDatabasePoolContainer<TDatabase>> createIOPoolContainer = null,
            DatabaseAction<TDatabase> action = null)
            where TDatabase : IDatabase, new();

        /// <summary>
        ///     Unregister a database IO pool.
        /// </summary>
        /// <typeparam name="TDatabase"> IDatabase. </typeparam>
        /// <param name="action"> (Optional) called for every initialized database. </param>
        void Unregister<TDatabase>(DatabaseAction<TDatabase> action = null)
            where TDatabase : IDatabase;

        /// <summary>
        ///     Lock a database from the IO pool and perform an action on it.
        /// </summary>
        /// <typeparam name="TDatabase"> Type of the database. </typeparam>
        /// <param name="action"> Called then a new database is initialized. </param>
        void Lock<TDatabase>(DatabaseAction<TDatabase> action)
            where TDatabase : IDatabase;

        /// <summary>
        ///     Lock a database from the IO pool and perform a function on it.
        /// </summary>
        /// <typeparam name="TDatabase"> Type of the database. </typeparam>
        /// <typeparam name="TResult">   Type of the result. </typeparam>
        /// <param name="func"> The function. </param>
        /// <returns>
        ///     T2.
        /// </returns>
        TResult Lock<TDatabase, TResult>(DatabaseFunction<TDatabase, TResult> func)
            where TDatabase : IDatabase;
    }
}