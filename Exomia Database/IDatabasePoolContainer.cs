﻿using System;
using System.Collections.Generic;

namespace Exomia.Database
{
    /// <inheritdoc />
    /// <summary>
    ///     IDatabasePoolContainer wrapper interface
    /// </summary>
    public interface IDatabasePoolContainer : IDisposable { }

    /// <inheritdoc />
    /// <summary>
    ///     IDatabasePoolContainer interface
    /// </summary>
    /// <typeparam name="TDatabase">IDatabase</typeparam>
    public interface IDatabasePoolContainer<TDatabase> : IDatabasePoolContainer
        where TDatabase : IDatabase
    {
        /// <summary>
        ///     adds a new database
        /// </summary>
        /// <param name="database">database to add</param>
        void Add(TDatabase database);

        /// <summary>
        ///     iterate over all added databases
        /// </summary>
        /// <returns>IEnumerable{TDatabase}</returns>
        IEnumerable<TDatabase> Foreach();

        /// <summary>
        ///     Lock a database from the IO pool and perform an action on it
        /// </summary>
        /// <param name="action">the action to perform</param>
        void Lock(DatabaseAction<TDatabase> action);

        /// <summary>
        ///     Lock a database from the IO pool and perform a function on it
        /// </summary>
        /// <typeparam name="TResult">function return value</typeparam>
        /// <param name="func">the function to call</param>
        /// <returns>T2</returns>
        TResult Lock<TResult>(DatabaseFunction<TDatabase, TResult> func);
    }
}