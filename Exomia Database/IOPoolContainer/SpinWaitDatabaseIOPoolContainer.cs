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
    /// <inheritdoc />
    public sealed class SpinWaitDatabaseIOPoolContainer<TDatabase> : IDatabasePoolContainer<TDatabase>
        where TDatabase : IDatabase
    {
        #region Variables

        private List<TDatabase> _database;
        private Queue<TDatabase> _queue;

        #endregion

        #region Constructors

        /// <inheritdoc />
        public SpinWaitDatabaseIOPoolContainer()
            :
            this(CONSTANTS.DEFAULT_DATABASE_IO_POOL_SIZE) { }

        /// <summary>
        ///     SpinWaitDatabaseIOPoolContainer constructor
        /// </summary>
        /// <param name="capacity">capacity</param>
        public SpinWaitDatabaseIOPoolContainer(int capacity)
        {
            _database = new List<TDatabase>(capacity);
            _queue = new Queue<TDatabase>(capacity);
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public void Dispose()
        {
            _database.Clear();
            _database = null;

            _queue.Clear();
            _queue = null;
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

        #endregion
    }
}