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
    ///     Container for monitor wait database i/o pools. This class cannot be inherited.
    /// </summary>
    /// <typeparam name="TDatabase"> Type of the database. </typeparam>
    public sealed class MonitorWaitDatabaseIOPoolContainer<TDatabase> : IDatabasePoolContainer<TDatabase>
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
        ///     Initializes a new instance of the <see cref="MonitorWaitDatabaseIOPoolContainer{TDatabase}" /> class.
        /// </summary>
        public MonitorWaitDatabaseIOPoolContainer()
            : this(CONSTANTS.DEFAULT_DATABASE_IO_POOL_SIZE) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MonitorWaitDatabaseIOPoolContainer{TDatabase}" /> class.
        /// </summary>
        /// <param name="capacity"> The capacity. </param>
        public MonitorWaitDatabaseIOPoolContainer(int capacity)
        {
            _database = new List<TDatabase>(capacity);
            _queue    = new Queue<TDatabase>(capacity);
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

            lock (_queue)
            {
                while (_queue.Count == 0)
                {
                    Monitor.Wait(_queue);
                }
                database = _queue.Dequeue();
            }

            action.Invoke(database);

            lock (_queue)
            {
                _queue.Enqueue(database);
                Monitor.Pulse(_queue);
            }
        }

        /// <inheritdoc />
        public TResult Lock<TResult>(DatabaseFunction<TDatabase, TResult> func)
        {
            TDatabase database;
            lock (_queue)
            {
                while (_queue.Count == 0)
                {
                    Monitor.Wait(_queue);
                }
                database = _queue.Dequeue();
            }

            TResult result = func.Invoke(database);

            lock (_queue)
            {
                _queue.Enqueue(database);
                Monitor.Pulse(_queue);
            }

            return result;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _database.Clear();
            _database = null;

            _queue.Clear();
            _queue = null;
        }
    }
}