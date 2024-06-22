using LibServer.Interfaces;
using LibServer.MySQLdev.Interfaces;
using LibServices.Server.Interfaces;

namespace LibServices.Server.mysql
{
    /// <summary>
    /// Implementation of the service handling server connection.
    /// </summary>
    public class Service_MySqlServerConnection : IService_ServerConnection
    {
        private readonly IMySqlServerConnection _connection;

        /// <summary>
        /// Constructor to initialize the Service_ServerConnection instance.
        /// </summary>
        /// <param name="connection">The server connection instance.</param>
        public Service_MySqlServerConnection(IMySqlServerConnection connection)
        {
            _connection = connection;
        }

        /// <inheritdoc/>
        public void Initialize(IConnectionBuilder connectionBuilder)
        {
            // Create a connection builder and initialize the server connection.
            _connection.Initialize(connectionBuilder);
        }

        /// <inheritdoc/>
        public bool State()
        {
            // Check and return the connection status
            return _connection.CheckConnectionStatus();
        }
    }
}
