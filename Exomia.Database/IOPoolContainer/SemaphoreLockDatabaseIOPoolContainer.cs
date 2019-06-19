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

        /// <inheritdoc/>
        public SemaphoreLockDatabaseIOPoolContainer()
            :
            this(CONSTANTS.DEFAULT_DATABASE_IO_POOL_SIZE) { }


        /// <summary>
        ///     Initializes a new instance of the <see cref="SemaphoreLockDatabaseIOPoolContainer{TDatabase}"/> class.
        /// </summary>
        /// <param name="capacity"> The capacity. </param>
        public SemaphoreLockDatabaseIOPoolContainer(int capacity)
        {
            _database = new List<TDatabase>(capacity);
            _queue = new Queue<TDatabase>(capacity);
            _semaphore = new SemaphoreSlim(capacity, capacity);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _semaphore.Dispose();
            _semaphore = null;

            _database.Clear();
            _database = null;

            _queue.Clear();
            _queue = null;
        }

        /// <inheritdoc/>
        public void Add(TDatabase database)
        {
            lock (_queue)
            {
                _database.Add(database);
                _queue.Enqueue(database);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<TDatabase> Foreach()
        {
            foreach (TDatabase database in _database)
            {
                yield return database;
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
    }
}