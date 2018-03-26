using System.Collections.Generic;
using System.Threading;

namespace Exomia.Database.IOPoolContainer
{
    /// <inheritdoc />
    public sealed class SemaphoreLockDatabaseIOPoolContainer<TDatabase> : IDatabasePoolContainer<TDatabase>
        where TDatabase : IDatabase
    {
        private List<TDatabase> _database;
        private Queue<TDatabase> _queue;
        private SemaphoreSlim _semaphore;

        /// <inheritdoc />
        public SemaphoreLockDatabaseIOPoolContainer()
            :
            this(CONSTANTS.DEFAULT_DATABASE_IO_POOL_SIZE) { }

        /// <summary>
        ///     SemaphoreLockDatabaseIOPoolContainer constructor
        /// </summary>
        /// <param name="capacity">capacity</param>
        public SemaphoreLockDatabaseIOPoolContainer(int capacity)
        {
            _database = new List<TDatabase>(capacity);
            _queue = new Queue<TDatabase>(capacity);
            _semaphore = new SemaphoreSlim(capacity, capacity);
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
    }
}