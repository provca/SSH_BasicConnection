using LibSSH.Manager;
using Renci.SshNet;

namespace LibServices.Settings
{
    /// <summary>
    /// Configuration class for managing ssh connections.
    /// </summary>
    public class SshSettings
    {
        private static SshManager? _sshManager;
        private static ForwardedPortLocal? _forwardedPort;

        public static string SshHost { get; set; } = string.Empty;
        public static string SshUserName { get; set; } = string.Empty;
        public static string SshPassword { get; set; } = string.Empty;
        public static int SshPort { get; set; } = 22;

        /// <summary>
        /// Establishes an SSH connection.
        /// </summary>
        public static void SshConnect()
        {
            _sshManager = new SshManager(SshHost, SshUserName, SshPassword, SshPort);
            _sshManager.Connect();
        }

        /// <summary>
        /// Sets up port forwarding from a local port to a remote database server.
        /// </summary>
        /// <param name="localPort">The local port to forward.</param>
        /// <param name="remoteHost">The remote database server host address.</param>
        /// <param name="remotePort">The remote database server port.</param>
        /// <returns>The local port that is forwarded.</returns>
        public static uint SshBridge(int localPort, string remoteHost, int remotePort)
        {
            // Ensure the SSH connection is established before port forwarding
            if (_sshManager == null || !_sshManager.IsConnected)
            {
                throw new InvalidOperationException("SSH Manager is not connected. Call SshConnect first.");
            }

            _forwardedPort = _sshManager.AddPortForwarded(localPort, remoteHost, remotePort);
            return _forwardedPort.BoundPort;
        }

        /// <summary>
        /// Disconnects the SSH connection and stops port forwarding.
        /// </summary>
        public static void SshDisconnect()
        {
            // Dispose of the forwarded port and disconnect the SSH connection
            _forwardedPort?.Dispose();
            _sshManager?.Disconnect();
        }

        /// <summary>
        /// Updates SSH settings with the provided values.
        /// </summary>
        /// <param name="sshHost">The SSH host address.</param>
        /// <param name="sshUserName">The SSH user name.</param>
        /// <param name="sshPassword">The SSH password.</param>
        /// <param name="sshPort">The SSH port number.</param>
        public static void UpdateSshSettings(string sshHost, string sshUserName, string sshPassword, int sshPort)
        {
            // Update SSH settings with new values
            SshHost = sshHost;
            SshUserName = sshUserName;
            SshPassword = sshPassword;
            SshPort = sshPort;
        }
    }
}
