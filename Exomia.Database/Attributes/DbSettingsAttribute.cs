#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Database.Attributes
{
    /// <summary>
    ///     Attribute for database settings. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DbSettingsAttribute : Attribute
    {
        /// <summary>
        ///     The connection string.
        /// </summary>
        private string _connectionString;

        /// <summary>
        ///     ConnectionString.
        /// </summary>
        /// <value>
        ///     The connection string.
        /// </value>
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DbSettingsAttribute" /> class.
        /// </summary>
        /// <param name="connectionString"> The connection string. </param>
        public DbSettingsAttribute(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}