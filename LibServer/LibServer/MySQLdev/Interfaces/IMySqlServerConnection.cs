using LibServer.Interfaces;
using MySqlConnector;

namespace LibServer.MySQLdev.Interfaces
{
    /// <summary>
    /// Represents a server connection interface for database operations.
    /// </summary>
    public interface IMySqlServerConnection
    {
        /// <summary>
        /// Gets the connection string for the database.
        /// </summary>
        string ConnectionString { get; }

        /// Initializes a new instance of the <see cref="MySqlServerConnection"/> class.
        /// </summary>
        /// <param name="connectionBuilder">The IConnectionBuilder interface.</param>
        void Initialize(IConnectionBuilder connectionBuilder);

        /// <summary>
        /// Checks the connection state by attempting to open and close it.
        /// </summary>
        /// <returns>True if the connection is successful, false otherwise.</returns>
        bool CheckConnectionStatus();

        /// <summary>
        /// Opens a new MySqlConnection based on the current connection string.
        /// </summary>
        /// <returns>The opened MySqlConnection instance.</returns>
        public MySqlConnection OpenNewConnection();
    }
}
