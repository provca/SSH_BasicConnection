using LibServer.Interfaces;

namespace LibServices.Server.Interfaces
{
    /// <summary>
    /// Interface for managing server connections.
    /// </summary>
    public interface IServerConnections
    {
        /// <summary>
        /// Creates a connection builder based on the specified parameters.
        /// </summary>
        /// <param name="server">Server address</param>
        /// <param name="userID">User ID</param>
        /// <param name="password">Password</param>
        /// <param name="database">Database name</param>
        /// <param name="port">Port number</param>
        /// <returns>Instance of the connection builder</returns>
        IConnectionBuilder CreateConnectionBuilder(string server, string userID, string password, string database, int port);

        /// <summary>
        /// Creates a server connection using the specified connection type and builder.
        /// </summary>
        /// <param name="connectionBuilder">Connection builder instance</param>
        /// <returns>Instance of the server connection</returns>
        IService_ServerConnection CreateServerConnection(IConnectionBuilder connectionBuilder);
    }
}
