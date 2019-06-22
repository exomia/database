#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Exomia.Database.Attributes;
using Exomia.Database.Exceptions;

namespace Exomia.Database
{
    /// <summary>
    ///     A database.
    /// </summary>
    /// <typeparam name="TCommand"> Type of the command. </typeparam>
    public abstract class Database<TCommand> : IDatabase
        where TCommand : DbCommand, new()
    {
        /// <summary>
        ///     DATABASE_TIMEOUT.
        /// </summary>
        protected const int DATABASE_TIMEOUT = 10000;

        /// <summary>
        ///     connection.
        /// </summary>
        protected DbConnection _connection;

        /// <summary>
        ///     connectionString.
        /// </summary>
        protected string _connectionString = string.Empty;

        /// <summary>
        ///     The commands.
        /// </summary>
        private Dictionary<int, TCommand> _commands;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Database{TCommand}" /> class.
        /// </summary>
        protected Database()
        {
            _commands = new Dictionary<int, TCommand>();

            foreach (DbSettingsAttribute settings in GetType().GetCustomAttributes(typeof(DbSettingsAttribute), true))
            {
                _connectionString = settings.ConnectionString;
            }
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="Database{TCommand}" /> class.
        /// </summary>
        ~Database()
        {
            Dispose(false);
        }

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
        ///     create a new database connection.
        /// </summary>
        /// <param name="connection"> [out] out DbConnection. </param>
        /// <returns>
        ///     <c>true</c> if successfully created; <c>false</c> otherwise.
        /// </returns>
        protected abstract bool CreateConnection(out DbConnection connection);

        /// <summary>
        ///     called than a new connection is established.
        /// </summary>
        protected virtual void OnConnected() { }

        /// <summary>
        ///     called than a connection is closed.
        /// </summary>
        protected virtual void OnClosed() { }

        /// <summary>
        ///     called after a valid connection is established.
        /// </summary>
        protected abstract void PrepareCommands();

        /// <summary>
        ///     Add a new Command to the command list the command will be prepared after action.
        /// </summary>
        /// <param name="index">   index. </param>
        /// <param name="query">   query. </param>
        /// <param name="action">  (Optional) action. </param>
        /// <param name="prepare"> (Optional) true if prepare command; false otherwise. </param>
        /// <exception cref="NullReferenceException"> Thrown when a value was unexpectedly null. </exception>
        protected void Add(int index, string query, PrepareDbCommand<TCommand> action = null, bool prepare = true)
        {
            if (_connection == null)
            {
                throw new NullReferenceException("the connection is null pls initialize the connection first!");
            }

            TCommand cmd = new TCommand { CommandText = query, Connection = _connection };

            action?.Invoke(cmd);

            if (prepare)
            {
                cmd.Prepare();
            }

            _commands.Add(index, cmd);
        }

        /// <summary>
        ///     Add a new database command to the command list the command will be prepared after action.
        /// </summary>
        /// <typeparam name="TPrim"> struct, IConvertible. </typeparam>
        /// <param name="index">   index. </param>
        /// <param name="query">   query. </param>
        /// <param name="action">  (Optional) action. </param>
        /// <param name="prepare"> (Optional) true if prepare command; false otherwise. </param>
        protected void Add<TPrim>(TPrim index, string query, PrepareDbCommand<TCommand> action = null,
                                  bool  prepare = true)
            where TPrim : struct, IConvertible
        {
            Add((int)Convert.ChangeType(index, typeof(int)), query, action, prepare);
        }

        /// <summary>
        ///     get a database command from the command list.
        /// </summary>
        /// <param name="index"> index. </param>
        /// <param name="args">  arguments. </param>
        /// <returns>
        ///     A TCommand.
        /// </returns>
        /// <exception cref="KeyNotFoundException"> Thrown when a Key Not Found error condition occurs. </exception>
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
        ///     get a database command from the command list.
        /// </summary>
        /// <typeparam name="TPrim"> struct, IConvertible. </typeparam>
        /// <param name="index"> index. </param>
        /// <param name="args">  arguments. </param>
        /// <returns>
        ///     A TCommand.
        /// </returns>
        /// <exception cref="KeyNotFoundException"> Thrown when a Key Not Found error condition occurs. </exception>
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

        #region IDisposable Support

        /// <summary>
        ///     true if the instance is allready disposed; false otherwise.
        /// </summary>
        protected bool _disposed;

        /// <summary>
        ///     call to dispose the instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     call to dispose the instance.
        /// </summary>
        /// <param name="disposing"> true if user code; false called by finalizer. </param>
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
        ///     called then the instance is disposing.
        /// </summary>
        /// <param name="disposing"> true if user code; false called by finalizer. </param>
        protected virtual void OnDispose(bool disposing) { }

        #endregion
    }
}