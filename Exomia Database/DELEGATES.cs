using System.Data.Common;

namespace Exomia.Database
{
    /// <summary>
    ///     called if a database prepares a new command
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <param name="command"></param>
    public delegate void PrepareDbCommand<in TCommand>(TCommand command)
        where TCommand : DbCommand;

    /// <summary>
    ///     called if an action on a IDatabase is performed
    /// </summary>
    /// <typeparam name="TDatabase">IDatabase</typeparam>
    /// <param name="database">IDatabase</param>
    public delegate void DatabaseAction<in TDatabase>(TDatabase database)
        where TDatabase : IDatabase;

    /// <summary>
    ///     called if a function on a IDatabase is performed
    /// </summary>
    /// <typeparam name="TDatabase">IDatabase</typeparam>
    /// <typeparam name="TResult">result</typeparam>
    /// <param name="database">IDatabase</param>
    /// <returns></returns>
    public delegate TResult DatabaseFunction<in TDatabase, out TResult>(TDatabase database)
        where TDatabase : IDatabase;
}