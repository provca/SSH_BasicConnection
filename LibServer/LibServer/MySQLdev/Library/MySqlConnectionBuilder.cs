using LibServer.Interfaces;
using MySqlConnector;

namespace LibServer.MySQLdev.Library
{
    /// <summary>
    /// Implementation of the <see cref="IConnectionBuilder"/> interface for building MySqlConnection instances.
    /// </summary>
    public class MySqlConnectionBuilder : IConnectionBuilder
    {
        // Use automatic properties for simplicity
        private MySqlConnectionStringBuilder _connectionString = new();

        // Properties for connection information
        public string Server { get; set; } = string.Empty;
        public string UserID { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public uint Port { get; set; } = 0;

        /// <inheritdoc/>
        public string ConnectionString => _connectionString.ConnectionString;

        /// <summary>
        /// Constructor for initializing connection details.
        /// </summary>
        /// <param name="server">The server address.</param>
        /// <param name="userID">The user ID for authentication.</param>
        /// <param name="password">The password for authentication.</param>
        /// <param name="database">The database name.</param>
        /// <param name="port">Port number.</param>
        public MySqlConnectionBuilder(string server, string userID, string password, string database, uint port)
        {
            Server = server;
            UserID = userID;
            Password = password;
            Database = database;
            Port = port;

            Initialize();
        }

        /// <inheritdoc/>
        public void Initialize()
        {
            // Validate and set connection details
            if (string.IsNullOrEmpty(Server) || string.IsNullOrEmpty(UserID) || string.IsNullOrEmpty(Database))
            {
                throw new ArgumentNullException($"All values ​​(server, userID, password and database) must be provided.");
            }
            try
            {
                // Building the connection string using MySqlConnectionStringBuilder
                _connectionString.Server = Server;
                _connectionString.UserID = UserID;
                _connectionString.Password = Password;
                _connectionString.Database = Database;
                _connectionString.Port = Port;
                _connectionString.ConnectionTimeout = 30;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error building connection string. Exception Type: {ex.GetType().Name}, Message: {ex.Message}, StackTrace: {ex.StackTrace}");
            }
        }
    }
}
