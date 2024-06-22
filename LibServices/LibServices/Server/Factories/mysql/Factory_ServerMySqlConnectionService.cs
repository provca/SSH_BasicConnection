using LibServer.MySQLdev.Library;
using LibServices.Server.Interfaces;
using LibServices.Server.mysql;
using LibServices.Settings;

namespace LibServices.Server.Factories.mysql
{
    /// <summary>
    /// Factory class for creating instances related to server connections.
    /// </summary>
    public class Factory_ServerMySqlConnectionService
    {
        private static MySqlConnectionBuilder? _connectionBuilder;

        /// <summary>
        /// Initializes the MySQL connection builder with the provided server details.
        /// </summary>
        /// <param name="server">The server address.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="password">The password.</param>
        /// <param name="database">The database name.</param>
        /// <param name="port">The server port.</param>
        public static void InitializeConnectionBuilder(string server, string userID, string password, string database, int port)
        {
            _connectionBuilder = new MySqlConnectionBuilder(server, userID, password, database, (uint)port);
        }

        /// <summary>
        /// Creates and initializes a MySQL server connection.
        /// </summary>
        /// <returns>An instance of <see cref="IService_ServerConnection"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the server connection is not open.</exception>
        public static IService_ServerConnection Connection()
        {
            var connection = new MySqlServerConnection();
            connection.Initialize(GetNonNullConnectionBuilder());

            // Check the connection status, and throw an exception if the connection is not open.
            if (!connection.CheckConnectionStatus())
            {
                throw new InvalidOperationException("Server connection is not open.");
            }

            // Return an instance of Service_ServerConnection using the initialized connection.
            return new Service_MySqlServerConnection(connection);
        }

        /// <summary>
        /// Creates and initializes a MySQL server connection via SSH.
        /// </summary>
        /// <returns>An instance of <see cref="IService_ServerConnection"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the server connection is not open or when SSH connection fails.</exception>
        public static IService_ServerConnection SshConnection()
        {
            try
            {
                // Initialize SSH and MySQL connection.
                var connection = InitializeSshAndMySqlConnection();

                // Check the connection status, and throw an exception if the connection is not open.
                if (!connection.CheckConnectionStatus())
                {
                    throw new InvalidOperationException("Server connection is not open.");
                }

                // Return an instance of Service_ServerConnection using the initialized connection.
                return new Service_MySqlServerConnection(connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ssh connection: {ex.Message}");
                SshSettings.SshDisconnect();
                throw;
            }
            finally
            {
                Console.WriteLine("SSH connection closed.");
                SshSettings.SshDisconnect();
            }
        }

        /// <summary>
        /// Retrieves the non-null MySQL connection builder.
        /// </summary>
        /// <returns>The initialized MySQL connection builder.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the connection builder is not initialized.</exception>
        private static MySqlConnectionBuilder GetNonNullConnectionBuilder()
        {
            return _connectionBuilder ?? throw new InvalidOperationException("Connection builder not initialized. Call InitializeConnectionBuilder before using Connection.");
        }

        /// <summary>
        /// Ensures the provided connection builder is not null.
        /// </summary>
        /// <param name="connectionBuilder">The MySQL connection builder.</param>
        /// <returns>The provided connection builder if not null.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the connection builder is null.</exception>
        private static MySqlConnectionBuilder GetNonNullConnectionBuilder(MySqlConnectionBuilder connectionBuilder)
        {
            return connectionBuilder ?? throw new InvalidOperationException("Connection builder not initialized. Call InitializeConnectionBuilder before using Connection.");
        }

        /// <summary>
        /// Initializes an SSH connection and a MySQL connection using port forwarding.
        /// </summary>
        /// <returns>An initialized instance of <see cref="MySqlServerConnection"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the SSH or MySQL connection cannot be established.</exception>
        private static MySqlServerConnection InitializeSshAndMySqlConnection()
        {
            // Initialize the ssh connection builder.
            var connectionBuilder = MySqlConnectionBuilderSsh();

            // Create and initialize the MySQL connection.
            var connection = new MySqlServerConnection();
            connection.Initialize(GetNonNullConnectionBuilder(connectionBuilder));
            Console.WriteLine("MySQL connection initialized.");

            return connection;
        }

        /// <summary>
        /// Initializes the SSH connection and MySQL connection builder.
        /// </summary>
        /// <returns>Returns an initialized MySqlConnectionBuilder configured with SSH port forwarding.</returns>
        /// <exception cref="InvalidOperationException">Thrown when SSH settings are not configured correctly or if the SSH connection fails.</exception>
        private static MySqlConnectionBuilder MySqlConnectionBuilderSsh()
        {
            // Update SSH settings based on connection builder settings.
            SshSettings.UpdateSshSettings(ConnectionBuildersSettings.Server, SshSettings.SshUserName, SshSettings.SshPassword, SshSettings.SshPort);

            // Establish SSH connection.
            SshSettings.SshConnect();
            Console.WriteLine("SSH connection established.");

            // Set up port forwarding and get the local port.
            uint localPort = SshSettings.SshBridge(ConnectionBuildersSettings.Port, ConnectionBuildersSettings.Server, ConnectionBuildersSettings.Port);

            // Initialize the connection builder with the local port.
            var connectionBuilder = new MySqlConnectionBuilder(SshSettings.SshHost, ConnectionBuildersSettings.UserID, ConnectionBuildersSettings.Password, ConnectionBuildersSettings.Database, localPort);

            return new MySqlConnectionBuilder(SshSettings.SshHost, ConnectionBuildersSettings.UserID, ConnectionBuildersSettings.Password, ConnectionBuildersSettings.Database, localPort);
        }
    }
}
