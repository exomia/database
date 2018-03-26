using System;

namespace Exomia.Database
{
    /// <summary>
    ///     IDatabaseManager interface
    /// </summary>
    public interface IDatabaseManager
    {
        /// <summary>
        ///     Register a new database IO pool
        /// </summary>
        /// <typeparam name="TDatabase">IDatabase</typeparam>
        /// <param name="count">count to register</param>
        /// <param name="createIOPoolContainer">function to create a new IDatabasePoolContainer{TDatabase}</param>
        /// <param name="action">called then a new database is initialized</param>
        void Register<TDatabase>(int count, Func<IDatabasePoolContainer<TDatabase>> createIOPoolContainer = null,
            DatabaseAction<TDatabase> action = null)
            where TDatabase : IDatabase, new();

        /// <summary>
        ///     Unregister a database IO pool
        /// </summary>
        /// <typeparam name="TDatabase">IDatabase</typeparam>
        /// <param name="action">called for ebvery initialized database</param>
        void Unregister<TDatabase>(DatabaseAction<TDatabase> action = null)
            where TDatabase : IDatabase;

        /// <summary>
        ///     Lock a database from the IO pool and perform an action on it
        /// </summary>
        /// <typeparam name="TDatabase">IDatabase</typeparam>
        /// <param name="action">the action to perform</param>
        void Lock<TDatabase>(DatabaseAction<TDatabase> action)
            where TDatabase : IDatabase;

        /// <summary>
        ///     Lock a database from the IO pool and perform a function on it
        /// </summary>
        /// <typeparam name="TDatabase">IDatabase</typeparam>
        /// <typeparam name="TResult">function return value</typeparam>
        /// <param name="func">the function to call</param>
        /// <returns>T2</returns>
        TResult Lock<TDatabase, TResult>(DatabaseFunction<TDatabase, TResult> func)
            where TDatabase : IDatabase;
    }
}