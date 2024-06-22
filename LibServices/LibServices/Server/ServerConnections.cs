using LibServer.Interfaces;
using LibServer.MySQLdev.Library;
using LibServices.Server.Interfaces;
using LibServices.Server.mysql;

namespace LibServices.Server
{
    /// <summary>
    /// Implementation of the <see cref="IServerConnections"/> interface for managing server connections.
    /// </summary>
    public class ServerConnections : IServerConnections
    {
        /// <inheritdoc />
        public IService_ServerConnection CreateServerConnection(IConnectionBuilder connectionBuilder)
        {
            // Create a MySQL server connection and initialize it with the provided connection builder.
            var mySqlServerConnection = CreateMySqlServerConnection();
            mySqlServerConnection.Initialize(connectionBuilder);

            // Check if the MySQL server connection is open; throw an exception if not.
            if (!mySqlServerConnection.CheckConnectionStatus())
            {
                throw new InvalidOperationException("MySQL server connection is not open.");
            }

            // Return a service wrapper for the MySQL server connection.
            return new Service_MySqlServerConnection(mySqlServerConnection);
        }

        /// <inheritdoc />
        public IConnectionBuilder CreateConnectionBuilder(string server, string userID, string password, string database, int port)
        {
            // Return a connection builder based on the specified connection type.
            return CreateMySqlConnectionBuilder(server, userID, password, database, port);
        }

        /// <summary>
        /// Creates a MySQL connection builder with the specified server, user ID, password, and database.
        /// </summary>
        /// <param name="server">Server address</param>
        /// <param name="userID">User ID</param>
        /// <param name="password">Password</param>
        /// <param name="database">Database name</param>
        /// <returns>Initialized MySQL connection builder</returns>
        private static IConnectionBuilder CreateMySqlConnectionBuilder(string server, string userID, string password, string database, int port)
        {
            // Create a new MySQL connection builder instance and ensure it is initialized.
            return new MySqlConnectionBuilder(server, userID, password, database, (uint)port) ?? throw new InvalidOperationException("Connection builder not initialized.");
        }

        /// <summary>
        /// Creates a new instance of the MySQL server connection.
        /// </summary>
        /// <returns>Initialized MySQL server connection</returns>
        private static MySqlServerConnection CreateMySqlServerConnection() => new();
    }
}
