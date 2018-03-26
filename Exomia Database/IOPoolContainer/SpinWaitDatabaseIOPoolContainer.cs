using System.Collections.Generic;
using System.Threading;

namespace Exomia.Database.IOPoolContainer
{
    /// <inheritdoc />
    public sealed class SpinWaitDatabaseIOPoolContainer<TDatabase> : IDatabasePoolContainer<TDatabase>
        where TDatabase : IDatabase
    {
        private List<TDatabase> _database;
        private Queue<TDatabase> _queue;

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
    }
}