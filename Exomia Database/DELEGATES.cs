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