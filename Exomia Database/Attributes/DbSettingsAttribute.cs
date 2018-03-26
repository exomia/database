using System;

namespace Exomia.Database.Attributes
{
    /// <inheritdoc />
    /// <summary>
    ///     DbSettingsAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DbSettingsAttribute : Attribute
    {
        private string _connectionString;

        /// <inheritdoc />
        /// <summary>
        ///     DbSettingsAttribute constructor
        /// </summary>
        public DbSettingsAttribute(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        ///     ConnectionString
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
    }
}