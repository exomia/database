﻿#region MIT License

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
using System.Data;
using System.Data.Common;
using Exomia.Database.Attributes;
using Exomia.Database.Exceptions;

namespace Exomia.Database
{
    /// <inheritdoc />
    public abstract class ADatabase<TCommand> : IDatabase
        where TCommand : DbCommand, new()
    {
        #region Variables

        /// <summary>
        ///     DATABASE_TIMEOUT
        /// </summary>
        protected const int DATABASE_TIMEOUT = 10000;

        private Dictionary<int, TCommand> _commands;

        /// <summary>
        ///     connection
        /// </summary>
        protected DbConnection _connection;

        /// <summary>
        ///     connectionString
        /// </summary>
        protected string _connectionString = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        ///     ADatabase constructor
        /// </summary>
        protected ADatabase()
        {
            _commands = new Dictionary<int, TCommand>();

            foreach (DbSettingsAttribute settings in GetType().GetCustomAttributes(typeof(DbSettingsAttribute), true))
            {
                _connectionString = settings.ConnectionString;
            }
        }

        /// <summary>
        ///     ADatabase destructor
        /// </summary>
        ~ADatabase()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public void Connect()
        {
            if (!CreateConnection(out _connection) || _connection == null)
            {
                throw new NullDbConnectionException("the connection is null pls initialize the connection first.");
            }

            if (!string.IsNullOrEmpty(_connection.ConnectionString))
            {
                _connectionString = _connection.ConnectionString;
            }

            else if (!string.IsNullOrEmpty(_connectionString))
            {
                _connection.ConnectionString = _connectionString;
            }
            else
            {
                throw new NullDbConnectionStringException("the connection string is null.");
            }

            if (!_connection.OpenAsync().Wait(DATABASE_TIMEOUT))
            {
                throw new DbConnectionException("can't establish a connection to the database server. TIMEOUT.");
            }

            if (_connection.State != ConnectionState.Open)
            {
                throw new DbConnectionException("can't establish a connection to the database server.");
            }

            OnConnected();

            PrepareCommands();
        }

        /// <inheritdoc />
        public void Connect(string connectionString)
        {
            _connectionString = connectionString;
            Connect();
        }

        /// <inheritdoc />
        public void Close()
        {
            _connection?.Close();
            OnClosed();
        }

        /// <summary>
        ///     create a new database connection
        /// </summary>
        /// <param name="connection">out DbConnection</param>
        /// <returns><c>true</c> if succesfully created; <c>false</c> otherwise</returns>
        protected abstract bool CreateConnection(out DbConnection connection);

        /// <summary>
        ///     called than a new connection is established
        /// </summary>
        protected virtual void OnConnected() { }

        /// <summary>
        ///     called than a connection is closed
        /// </summary>
        protected virtual void OnClosed() { }

        /// <summary>
        ///     called after a valid connection is established
        /// </summary>
        protected abstract void PrepareCommands();

        /// <summary>
        ///     Add a new Command to the command list
        ///     the command will be prepared after action
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="query">query</param>
        /// <param name="action">action</param>
        /// <param name="prepare">true if prepare command; false oherwise</param>
        protected void Add(int index, string query, PrepareDbCommand<TCommand> action = null, bool prepare = true)
        {
            if (_connection == null)
            {
                throw new NullReferenceException("the connection is null pls initialize the connection first!");
            }

            TCommand cmd = new TCommand
            {
                CommandText = query,
                Connection = _connection
            };

            action?.Invoke(cmd);

            if (prepare)
            {
                cmd.Prepare();
            }

            _commands.Add(index, cmd);
        }

        /// <summary>
        ///     Add a new database command to the command list
        ///     the command will be prepared after action
        /// </summary>
        /// <typeparam name="TPrim">struct, IConvertible</typeparam>
        /// <param name="index">index</param>
        /// <param name="query">query</param>
        /// <param name="action">action</param>
        /// <param name="prepare">true if prepare command; false oherwise</param>
        protected void Add<TPrim>(TPrim index, string query, PrepareDbCommand<TCommand> action = null,
            bool prepare = true)
            where TPrim : struct, IConvertible
        {
            Add((int)Convert.ChangeType(index, typeof(int)), query, action, prepare);
        }

        /// <summary>
        ///     get a database command from the command list
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="args">arguments</param>
        /// <returns></returns>
        protected TCommand Get(int index, params object[] args)
        {
            if (!_commands.TryGetValue(index, out TCommand cmd))
            {
                throw new KeyNotFoundException($"key '{index}' not found.");
            }

            for (int i = 0; i < cmd.Parameters.Count; i++)
            {
                cmd.Parameters[i].Value = args[i];
            }

            return cmd;
        }

        /// <summary>
        ///     get a database command from the command list
        /// </summary>
        /// <typeparam name="TPrim">struct, IConvertible</typeparam>
        /// <param name="index">index</param>
        /// <param name="args">arguments</param>
        /// <returns></returns>
        protected TCommand Get<TPrim>(TPrim index, params object[] args)
            where TPrim : struct, IConvertible
        {
            if (!_commands.TryGetValue((int)Convert.ChangeType(index, typeof(int)), out TCommand cmd))
            {
                throw new KeyNotFoundException($"key '{index}' not found.");
            }

            for (int i = 0; i < cmd.Parameters.Count; i++)
            {
                cmd.Parameters[i].Value = args[i];
            }

            return cmd;
        }

        #endregion

        #region IDisposable Support

        /// <summary>
        ///     true if the instance is allready disposed; false otherwise
        /// </summary>
        protected bool _disposed;

        /// <summary>
        ///     call to dispose the instance
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                OnDispose(disposing);
                if (disposing)
                {
                    /* USER CODE */
                    Close();

                    _connection?.Dispose();
                    _connection = null;

                    _commands.Clear();
                    _commands = null;
                }
                _disposed = true;
            }
        }

        /// <summary>
        ///     called then the instance is disposing
        /// </summary>
        /// <param name="disposing">true if user code; false called by finalizer</param>
        protected virtual void OnDispose(bool disposing) { }

        #endregion
    }
}