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
    ///     Container for spin wait database i/o pools. This class cannot be inherited.
    /// </summary>
    /// <typeparam name="TDatabase"> Type of the database. </typeparam>
    public sealed class SpinWaitDatabaseIOPoolContainer<TDatabase> : IDatabasePoolContainer<TDatabase>
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

        /// <inheritdoc />
        public SpinWaitDatabaseIOPoolContainer()
            : this(CONSTANTS.DEFAULT_DATABASE_IO_POOL_SIZE) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpinWaitDatabaseIOPoolContainer{TDatabase}" /> class.
        /// </summary>
        /// <param name="capacity"> The capacity. </param>
        public SpinWaitDatabaseIOPoolContainer(int capacity)
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

            lock (this)
            {
                SpinWait.SpinUntil(() => { return _queue.Count > 0; });

                lock (_queue)
                {
                    database = _queue.Dequeue();
                }
            }

            action.Invoke(database);

            lock (_queue)
            {
                _queue.Enqueue(database);
            }
        }

        /// <inheritdoc />
        public TResult Lock<TResult>(DatabaseFunction<TDatabase, TResult> func)
        {
            TDatabase database;

            lock (this)
            {
                SpinWait.SpinUntil(() => { return _queue.Count > 0; });

                lock (_queue)
                {
                    database = _queue.Dequeue();
                }
            }

            TResult result = func.Invoke(database);

            lock (_queue)
            {
                _queue.Enqueue(database);
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