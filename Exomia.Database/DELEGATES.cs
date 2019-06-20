#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Data.Common;

namespace Exomia.Database
{
    /// <summary>
    ///     called if a database prepares a new command.
    /// </summary>
    /// <typeparam name="TCommand"> Type of the command. </typeparam>
    /// <param name="command"> The command. </param>
    public delegate void PrepareDbCommand<in TCommand>(TCommand command)
        where TCommand : DbCommand;

    /// <summary>
    ///     called if an action on a IDatabase is performed.
    /// </summary>
    /// <typeparam name="TDatabase"> Type of the database. </typeparam>
    /// <param name="database"> The database. </param>
    public delegate void DatabaseAction<in TDatabase>(TDatabase database)
        where TDatabase : IDatabase;

    /// <summary>
    ///     called if a function on a IDatabase is performed.
    /// </summary>
    /// <typeparam name="TDatabase"> Type of the database. </typeparam>
    /// <typeparam name="TResult">   Type of the result. </typeparam>
    /// <param name="database"> The database. </param>
    /// <returns>
    ///     A TResult.
    /// </returns>
    public delegate TResult DatabaseFunction<in TDatabase, out TResult>(TDatabase database)
        where TDatabase : IDatabase;
}