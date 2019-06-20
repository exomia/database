#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

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
        /// <param name="createIOPoolContainer">
        ///     (Optional) function to create a new
        ///     IDatabasePoolContainer{TDatabase}
        /// </param>
        /// <param name="action">                (Optional) called then a new database is initialized. </param>
        void Register<TDatabase>(int                                     count,
                                 Func<IDatabasePoolContainer<TDatabase>> createIOPoolContainer = null,
                                 DatabaseAction<TDatabase>               action                = null)
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