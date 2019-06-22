#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Collections.Generic;
using System.Threading;

namespace Exomia.Database.IOPoolContainer
{
    /// <summary>
    ///     Container for semaphore lock database i/o pools. This class cannot be inherited.
    /// </summary>
    /// <typeparam name="TDatabase"> Type of the database. </typeparam>
    public sealed class SemaphoreLockDatabaseIOPoolContainer<TDatabase> : IDatabasePoolContainer<TDatabase>
        where TDatabase : IDatabase
    {
        /// <summary>
        ///     The database.
        /// </summary>
        private List<TDatabase> _database;

        /// <summary>
        ///     The queue.
        /// </summary>
        private Queue<TDatabase> _queue;

        /// <summary>
        ///     The semaphore.
        /// </summary>
        private SemaphoreSlim _semaphore;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SemaphoreLockDatabaseIOPoolContainer{TDatabase}" /> class.
        /// </summary>
        public SemaphoreLockDatabaseIOPoolContainer()
            :
            this(CONSTANTS.DEFAULT_DATABASE_IO_POOL_SIZE) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SemaphoreLockDatabaseIOPoolContainer{TDatabase}" /> class.
        /// </summary>
        /// <param name="capacity"> The capacity. </param>
        public SemaphoreLockDatabaseIOPoolContainer(int capacity)
        {
            _database  = new List<TDatabase>(capacity);
            _queue     = new Queue<TDatabase>(capacity);
            _semaphore = new SemaphoreSlim(capacity, capacity);
        }

        /// <inheritdoc />
        public void Add(TDatabase database)
        {
            lock (_queue)
            {
                _database.Add(database);
                _queue.Enqueue(database);
            }
        }

        /// <inheritdoc />
        public IEnumerable<TDatabase> Foreach()
        {
            foreach (TDatabase database in _database)
            {
                yield return database;
            }
        }

        /// <inheritdoc />
        public void Lock(DatabaseAction<TDatabase> action)
        {
            TDatabase database;

            _semaphore.Wait();

            lock (_queue)
            {
                database = _queue.Dequeue();
            }

            action.Invoke(database);

            lock (_queue)
            {
                _queue.Enqueue(database);
            }

            _semaphore.Release();
        }

        /// <inheritdoc />
        public TResult Lock<TResult>(DatabaseFunction<TDatabase, TResult> func)
        {
            TDatabase database;

            _semaphore.Wait();

            lock (_queue)
            {
                database = _queue.Dequeue();
            }
            TResult result = func.Invoke(database);

            lock (_queue)
            {
                _queue.Enqueue(database);
            }

            _semaphore.Release();

            return result;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _semaphore.Dispose();
            _semaphore = null;

            _database.Clear();
            _database = null;

            _queue.Clear();
            _queue = null;
        }
    }
}