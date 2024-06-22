using LibServices.Server.Factories.mysql;
using LibServices.Server.Interfaces;

namespace LibServices.Server.Factories
{
    /// <summary>
    /// Factory class for creating server connections.
    /// </summary>
    public class Factory_Servers
    {
        /// <summary>
        /// Initializes the connection builder with the provided server information.
        /// </summary>
        /// <param name="server">Server address</param>
        /// <param name="userID">User ID</param>
        /// <param name="password">Password</param>
        /// <param name="database">Database name</param>
        /// <param name="port">Port number</param>
        public static void InitializeConnectionBuilder(string server, string userID, string password, string database, int port)
        {
            Factory_ServerMySqlConnectionService.InitializeConnectionBuilder(server, userID, password, database, port);
        }

        /// <summary>
        /// Checks the connection state using the configured server connections.
        /// </summary>
        /// <returns>True if the connection is successful, otherwise false</returns>
        public static bool ConnectionState()
        {
            IService_ServerConnection? serverConnection;
            serverConnection = Factory_ServerMySqlConnectionService.Connection();
            return serverConnection.State();
        }

        /// <summary>
        /// Checks the SSH connection state.
        /// </summary>
        /// <returns>True if the connection is successful, otherwise false</returns>
        public static bool SshConnectionState()
        {
            IService_ServerConnection? sshServerConnection;
            sshServerConnection = Factory_ServerMySqlConnectionService.SshConnection();
            return sshServerConnection.State();
        }
    }
}
