using LibServer.Interfaces;
using LibServer.MySQLdev.Library;

namespace LibServices.Settings
{
    /// <summary>
    /// Configuration class for managing application connections.
    /// </summary>
    public static class ConnectionBuildersSettings
    {
        /// <summary>
        /// Connection builder instance for creating database connections.
        /// </summary>
        private static IConnectionBuilder _connectionBuilder;

        // Static fields for storing connection details
        private static string _server = string.Empty;
        private static string _userID = string.Empty;
        private static string _password = string.Empty;
        private static string _database = string.Empty;
        private static int _port = 0;

        /// <summary>
        /// Gets or sets the server address for the database connection.
        /// </summary>
        public static string Server { get => _server; set => _server = value; }
        /// <summary>
        /// Gets or sets the user ID for authentication.
        /// </summary>
        public static string UserID { get => _userID; set => _userID = value; }

        /// <summary>
        /// Gets or sets the password for authentication.
        /// </summary>
        public static string Password { get => _password; set => _password = value; }

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        public static string Database { get => _database; set => _database = value; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        public static int Port { get => _port; set => _port = value; }

        /// <summary>
        /// Gets the connection string built by the connection builder.
        /// </summary>
        public static IConnectionBuilder ConnectionBuilder { get => _connectionBuilder; set => _connectionBuilder = value ?? throw new InvalidOperationException("ConnectionString is null."); }

        /// <summary>
        /// Static constructor to initialize connection details based on the selected connection type.
        /// </summary>
        static ConnectionBuildersSettings()
        {
            try
            {
                _server = "localhost";
                _userID = "root";
                _password = string.Empty;
                _database = "mysql";
                _port = 3306;
                _connectionBuilder = new MySqlConnectionBuilder(_server, _userID, _password, _database, (uint)_port);
            }
            catch (Exception ex)
            {
                // Handle the specific exception (can be more specific depending on connection classes' exceptions)
                throw new InvalidOperationException($"Error initializing connection: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Update connection configuration.
        /// </summary>
        public static void UpdateConfiguration(string server, string userID, string password, string database, int port)
        {
            // Actualiza los valores de configuración
            Server = server;
            UserID = userID;
            Password = password;
            Database = database;
            Port = port;

            _connectionBuilder = new MySqlConnectionBuilder(server, userID, password, database, (uint)port);
        }
    }
}
