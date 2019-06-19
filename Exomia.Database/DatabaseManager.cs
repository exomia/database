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

using System;
using System.Collections.Generic;

namespace Exomia.Database
{
    /// <summary>
    ///     Manager for databases. This class cannot be inherited.
    /// </summary>
    public sealed class DatabaseManager : IDatabaseManager
    {
        /// <summary>
        ///     The database i/o pool.
        /// </summary>
        private readonly Dictionary<Type, IDatabasePoolContainer> _databaseIOPool;

        /// <summary>
        ///     DatabaseManager constructor.
        /// </summary>
        public DatabaseManager()
        {
            _databaseIOPool = new Dictionary<Type, IDatabasePoolContainer>();
        }

        /// <inheritdoc/>
        public void Register<TDatabase>(int count, Func<IDatabasePoolContainer<TDatabase>> createIOPoolContainer = null,
            DatabaseAction<TDatabase> action = null)
            where TDatabase : IDatabase, new()
        {
            IDatabasePoolContainer container = null;

            lock (_databaseIOPool)
            {
                if (!_databaseIOPool.TryGetValue(typeof(TDatabase), out container))
                {
                    container = createIOPoolContainer?.Invoke() ?? throw new ArgumentNullException(nameof(container));
                    _databaseIOPool.Add(typeof(TDatabase), container);
                }
            }

            if (container is IDatabasePoolContainer<TDatabase> tContainer)
            {
                for (int i = 0; i < count; i++)
                {
                    TDatabase database = new TDatabase();
                    action?.Invoke(database);
                    tContainer.Add(database);
                }
            }
        }

        /// <inheritdoc/>
        public void Unregister<TDatabase>(DatabaseAction<TDatabase> action = null)
            where TDatabase : IDatabase
        {
            IDatabasePoolContainer container = null;

            lock (_databaseIOPool)
            {
                if (_databaseIOPool.TryGetValue(typeof(TDatabase), out container))
                {
                    _databaseIOPool.Remove(typeof(TDatabase));
                }
            }

            if (container is IDatabasePoolContainer<TDatabase> tContainer)
            {
                foreach (TDatabase database in tContainer.Foreach())
                {
                    action?.Invoke(database);
                }
            }

            container?.Dispose();
        }

        /// <inheritdoc/>
        public void Lock<TDatabase>(DatabaseAction<TDatabase> action)
            where TDatabase : IDatabase
        {
            if (action == null) { throw new ArgumentNullException(nameof(action), "the action can't be null"); }

            IDatabasePoolContainer container = null;
            lock (_databaseIOPool)
            {
                if (!_databaseIOPool.TryGetValue(typeof(TDatabase), out container))
                {
                    throw new KeyNotFoundException($"no database of type: '{typeof(TDatabase)}' registered.");
                }
            }
            ((IDatabasePoolContainer<TDatabase>)container).Lock(action);
        }

        /// <inheritdoc/>
        public TResult Lock<TDatabase, TResult>(DatabaseFunction<TDatabase, TResult> func)
            where TDatabase : IDatabase
        {
            if (func == null) { throw new ArgumentNullException(nameof(func), "the function can't be null"); }

            IDatabasePoolContainer container = null;
            lock (_databaseIOPool)
            {
                if (!_databaseIOPool.TryGetValue(typeof(TDatabase), out container))
                {
                    throw new KeyNotFoundException($"no database of type: '{typeof(TDatabase)}' registered.");
                }
            }

            return ((IDatabasePoolContainer<TDatabase>)container).Lock(func);
        }
    }


    /// <summary>
    ///     Manager for databases.
    /// </summary>
    /// <typeparam name="TDatabase"> Type of the database. </typeparam>
    public static class DatabaseManager<TDatabase>
        where TDatabase : IDatabase, new()
    {
        /// <summary>
        ///     The container.
        /// </summary>
        private static IDatabasePoolContainer<TDatabase> s_container;

        /// <summary>
        ///     <see
        ///         cref="IDatabaseManager.Register{TDatabase}(int,
        ///         Func{IDatabasePoolContainer{TDatabase}}, DatabaseAction{TDatabase})" />
        /// </summary>
        /// <param name="count">                 Number of. </param>
        /// <param name="createIOPoolContainer"> (Optional) The create i/o pool container. </param>
        /// <param name="action">                (Optional) The action. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public static void Register(int count, Func<IDatabasePoolContainer<TDatabase>> createIOPoolContainer = null,
            DatabaseAction<TDatabase> action = null)
        {
            s_container = createIOPoolContainer?.Invoke() ?? throw new ArgumentNullException(nameof(s_container));

            for (int i = 0; i < count; i++)
            {
                TDatabase database = new TDatabase();
                action?.Invoke(database);
                s_container.Add(database);
            }
        }

        /// <summary>
        ///     <see cref="IDatabaseManager.Unregister{TDatabase}(DatabaseAction{TDatabase})" />
        /// </summary>
        /// <param name="action"> (Optional) The action. </param>
        public static void Unregister(DatabaseAction<TDatabase> action = null)
        {
            foreach (TDatabase database in s_container.Foreach())
            {
                action?.Invoke(database);
            }

            s_container.Dispose();
            s_container = null;
        }

        /// <summary>
        ///     <see cref="IDatabaseManager.Lock{TDatabase}(DatabaseAction{TDatabase})" />
        /// </summary>
        /// <param name="action"> The action. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public static void Lock(DatabaseAction<TDatabase> action)
        {
            if (s_container == null)
            {
                throw new ArgumentNullException(
                    nameof(s_container),
                    "call DatabaseManager<TDatabase>.Register(int,  Func<IDatabasePoolContainer<TDatabase>>, DatabaseAction<TDatabase>) first.");
            }
            if (action == null) { throw new ArgumentNullException(nameof(action), "the action can't be null"); }

            s_container.Lock(action);
        }

        /// <summary>
        ///     <see cref="IDatabaseManager.Lock{TDatabase, TResult}(DatabaseFunction{TDatabase, TResult})" />
        /// </summary>
        /// <typeparam name="TResult"> Type of the result. </typeparam>
        /// <param name="func"> The function. </param>
        /// <returns>
        ///     A TResult.
        /// </returns>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public static TResult Lock<TResult>(DatabaseFunction<TDatabase, TResult> func)
        {
            if (s_container == null)
            {
                throw new ArgumentNullException(
                    nameof(s_container),
                    "call DatabaseManager<TDatabase>.Register(int,  Func<IDatabasePoolContainer<TDatabase>>, DatabaseAction<TDatabase>) first.");
            }
            if (func == null) { throw new ArgumentNullException(nameof(func), "the function can't be null"); }

            return s_container.Lock(func);
        }
    }
}