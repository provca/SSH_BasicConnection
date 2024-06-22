using LibSSH.AppSettings;
using Renci.SshNet;
using System.Diagnostics;
using System.Security;
using System.Security.Cryptography;

namespace LibSSH.Manager
{
    /// <summary>
    /// Manages SSH connections and port forwarding.
    /// </summary>
    public class SshManager : IDisposable
    {
        private readonly string _host;              // Host address for the SSH connection
        private readonly string _username;          // Username for the SSH connection
        private readonly string _password;          // Password for the SSH connection
        private readonly int _port;                 // Port for the SSH connection (22)
        private SshClient? _sshClient;              // SSH client instance
        private bool _disposed = false;             // Flag to indicate if the object has been disposed
        private byte[]? _hostKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="SshManager"/> class.
        /// </summary>
        /// <param name="host">The SSH server host address.</param>
        /// <param name="username">The username for the SSH connection.</param>
        /// <param name="password">The password for the SSH connection.</param>
        /// <param name="port">The port 22 by default for the SSH connection.</param>
        public SshManager(string host, string username, string password, int port)
        {
            _host = host;
            _username = username;
            _password = password;
            _port = port;
        }

        /// <summary>
        /// Connects to the SSH server.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the SSH client is already connected.</exception>
        public void Connect()
        {
            if (_sshClient == null)
            {
                _sshClient = new SshClient(_host, _port, _username, _password);
                _sshClient.HostKeyReceived += (sender, e) =>
                {
                    _hostKey = e.HostKey;
                };

                try
                {
                    _sshClient.Connect();
                    Trace.WriteLine("SSH connection established.");

                    VerifyServerFingerprint();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"Error connecting to SSH: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the SSH client is connected.
        /// </summary>
        public bool IsConnected
        {
            get { return _sshClient != null && _sshClient.IsConnected; }
        }

        /// <summary>
        /// Disconnects from the SSH server.
        /// </summary>
        public void Disconnect()
        {
            _sshClient?.Disconnect();
        }

        /// <summary>
        /// Adds a local port forwarding configuration.
        /// </summary>
        /// <param name="localPort">The local port to be forwarded.</param>
        /// <param name="remoteHost">The remote host address.</param>
        /// <param name="remotePort">The remote port to be forwarded to.</param>
        /// <returns>The <see cref="ForwardedPortLocal"/> instance representing the forwarded port.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the SSH client is not connected.</exception>
        public ForwardedPortLocal AddPortForwarded(int localPort, string remoteHost, int remotePort)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("SSH client is not connected.");
            }

            Trace.WriteLine("Adding port forwarding...");

            var forwardedPort = new ForwardedPortLocal("127.0.0.1", (uint)localPort, remoteHost, (uint)remotePort);
            _sshClient.AddForwardedPort(forwardedPort);
            try
            {
                forwardedPort.Start();
                //_logger.LogInformation($"Port forwarding started: localPort={localPort}, remoteHost={remoteHost}, remotePort={remotePort}");
                Trace.WriteLine($"Port forwarding starting...");
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error starting port forwarding: {ex.Message}");
                throw;
            }
            return forwardedPort;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="SshManager"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">A boolean value indicating whether to release both managed and unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _sshClient?.Dispose();         // Dispose managed resources
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="SshManager"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);              // Suppress finalization
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SshManager"/> class.
        /// </summary>
        ~SshManager()
        {
            Dispose(false);
        }

        /// <summary>
        /// Verifies the server SSH fingerprint against an expected fingerprint.
        /// </summary>
        private void VerifyServerFingerprint()
        {
            // Expected Finger print (example).
            var expectedFingerPrint = AppSettingsReader.GetConfigurationValue("SSH:ExpectedFingerprintRaspberry");

            // Check recived hostkey.
            if (_hostKey == null)
            {
                throw new InvalidOperationException("Host key not received.");
            }

            // Calculate Finger Print from the SSH server using hostKey.
            string serverFingerprint = CalculateFingerprint(_hostKey);

            // Compare results.
            if (serverFingerprint != expectedFingerPrint)
            {
                throw new SecurityException("SSH server fingerprint mismatch. Possible MITM attack.");
            }

            Trace.WriteLine("SSH server fingerprint verified.");
        }

        /// <summary>
        /// Calculates the fingerprint of an SSH public key.
        /// </summary>
        /// <param name="publicKey">The SSH public key as a byte array.</param>
        /// <returns>The fingerprint of the SSH public key as a Base64-encoded string.</returns>
        private static string CalculateFingerprint(byte[] publicKey)
        {
            // Create an instance of SHA-256 for hashing.
            using (var sha256 = SHA256.Create())
            {
                // Compute the hash of the public key.
                var hash = sha256.ComputeHash(publicKey);

                // Convert the hashed bytes to a Base64-encoded string (fingerprint).
                return Convert.ToBase64String(hash);
            }
        }
    }
}
