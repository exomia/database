using System.Collections.Generic;
using System.Threading;

namespace Exomia.Database.IOPoolContainer
{
    /// <inheritdoc />
    public sealed class MonitorWaitDatabaseIOPoolContainer<TDatabase> : IDatabasePoolContainer<TDatabase>
        where TDatabase : IDatabase
    {
        private List<TDatabase> _database;
        private Queue<TDatabase> _queue;

        /// <inheritdoc />
        public MonitorWaitDatabaseIOPoolContainer()
            :
            this(CONSTANTS.DEFAULT_DATABASE_IO_POOL_SIZE) { }

        /// <summary>
        ///     MonitorWaitDatabaseIOPoolContainer constructor
        /// </summary>
        /// <param name="capacity">capacity</param>
        public MonitorWaitDatabaseIOPoolContainer(int capacity)
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
    }
}