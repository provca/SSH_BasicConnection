using LibServices.Server.Factories;
using LibServices.Settings;

namespace SSH_ConnectionConsole.Menu
{
    internal class MainMenu
    {
        /// <summary>
        /// Starts the main menu loop.
        /// </summary>
        public static void StartMenu()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("This is a simple application to connect a server by SSH.");
                Console.WriteLine("You need a MySQL server running to test this app.\n");

                if (!CollectServerData()) break;

                Console.WriteLine("\n-------------------------------------------");
                Console.WriteLine("----------------SSH------------------------");

                if (!CollectSshData()) break;

                Console.ReadKey();
            }
        }

        /// <summary>
        /// Collects server data from the user.
        /// </summary>
        /// <returns>Returns true if data collection is successful, otherwise false.</returns>
        private static bool CollectServerData()
        {
            // Get server details from the user or use default values
            string server = GetInput("Server", ConnectionBuildersSettings.Server);
            if (ExitProgram(server)) return false;

            string userID = GetInput("User", ConnectionBuildersSettings.UserID);
            if (ExitProgram(userID)) return false;

            string password = GetInput("Password", ConnectionBuildersSettings.Password);
            if (ExitProgram(password)) return false;

            string database = GetInput("Database", ConnectionBuildersSettings.Database);
            if (ExitProgram(database)) return false;

            string port = GetInput("Port", Convert.ToString(ConnectionBuildersSettings.Port));
            if (ExitProgram(port)) return false;

            return Connect(server, userID, password, database, Convert.ToInt32(port));
        }

        /// <summary>
        /// Collects SSH data from the user.
        /// </summary>
        /// <returns>Returns true if data collection is successful, otherwise false.</returns>
        private static bool CollectSshData()
        {
            // Get SSH details from the user or use default values
            string sshServer = GetInput("SSH Server", SshSettings.SshHost);
            if (ExitProgram(sshServer)) return false;

            string sshUser = GetInput("SSH User", SshSettings.SshUserName);
            if (ExitProgram(sshUser)) return false;

            string sshPassword = GetInput("SSH Password", SshSettings.SshPassword);
            if (ExitProgram(sshPassword)) return false;

            string server = ConnectionBuildersSettings.Server;
            string userID = ConnectionBuildersSettings.UserID;
            string password = ConnectionBuildersSettings.Password;
            string database = ConnectionBuildersSettings.Database;
            int port = Convert.ToInt32(ConnectionBuildersSettings.Port);

            return SshConnect(server, userID, password, database, port, sshServer, sshUser, sshPassword);
        }

        /// <summary>
        /// Gets user input with a default value.
        /// </summary>
        /// <param name="prompt">The prompt message to display.</param>
        /// <param name="defaultValue">The default value to use if input is empty.</param>
        /// <returns>The user input or the default value if input is empty.</returns>
        private static string GetInput(string prompt, string defaultValue)
        {
            Console.Write($"{prompt} ({defaultValue}): ");
            string? input = Console.ReadLine();
            return string.IsNullOrEmpty(input) ? defaultValue : input;
        }

        /// <summary>
        /// Checks if the input is a command to exit the program.
        /// </summary>
        /// <param name="input">The user input.</param>
        /// <returns>Returns true if the input is "Q", otherwise false.</returns>
        private static bool ExitProgram(string input)
        {
            return input?.Trim().ToUpper() == "Q";
        }

        /// <summary>
        /// Connects to the server with the provided details.
        /// </summary>
        /// <param name="server">The server address.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="password">The password.</param>
        /// <param name="database">The database name.</param>
        /// <param name="port">The port number.</param>
        /// <returns>Returns true if connection is successful, otherwise false.</returns>
        private static bool Connect(string server, string userID, string password, string database, int port)
        {
            UpdateConfiguration(server, userID, password, database, port);
            try
            {
                Factory_Servers.InitializeConnectionBuilder(server, userID, password, database, port);
                bool success = Factory_Servers.ConnectionState();

                Console.WriteLine(success ? "Connected successfully." : "Unable to connect");
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return false;
            }
        }

        /// <summary>
        /// Connects to the server via SSH with the provided details.
        /// </summary>
        /// <param name="server">The server address.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="password">The password.</param>
        /// <param name="database">The database name.</param>
        /// <param name="port">The port number.</param>
        /// <param name="sshHost">The SSH host address.</param>
        /// <param name="sshUserName">The SSH user name.</param>
        /// <param name="sshPassword">The SSH password.</param>
        /// <param name="sshPort">The SSH port number.</param>
        /// <returns>Returns true if connection is successful, otherwise false.</returns>
        private static bool SshConnect(string server, string userID, string password, string database, int port, string sshHost, string sshUserName, string sshPassword, int sshPort = 22)
        {
            UpdateConfiguration(server, userID, password, database, port);
            UpdateSSH_Values(sshHost, sshUserName, sshPassword, sshPort);
            try
            {
                Factory_Servers.InitializeConnectionBuilder(server, userID, password, database, port);
                bool success = Factory_Servers.SshConnectionState();

                Console.WriteLine(success ? "Connected successfully." : "Unable to connect");
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return false;
            }
        }

        /// <summary>
        /// Updates the configuration settings.
        /// </summary>
        /// <param name="server">The server address.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="password">The password.</param>
        /// <param name="database">The database name.</param>
        /// <param name="port">The port number.</param>
        private static void UpdateConfiguration(string server, string userID, string password, string database, int port)
        {
            Console.WriteLine($"\nUpdating Connection Builder values: Server={server}, UserID={userID}, Password={password}, Database={database}, Port={port}");
            ConnectionBuildersSettings.UpdateConfiguration(server, userID, password, database, port);
            Console.WriteLine("Connection Builder has been updated.");
        }

        /// <summary>
        /// Updates the SSH configuration settings.
        /// </summary>
        /// <param name="sshHost">The SSH host address.</param>
        /// <param name="sshUserName">The SSH user name.</param>
        /// <param name="sshPassword">The SSH password.</param>
        /// <param name="sshPort">The SSH port number.</param>
        private static void UpdateSSH_Values(string sshHost, string sshUserName, string sshPassword, int sshPort)
        {
            Console.WriteLine($"\nUpdating SSH values: Host={sshHost}, UserName={sshUserName}, UserPassword={sshPassword}, Port={sshPort}");
            SshSettings.UpdateSshSettings(sshHost, sshUserName, sshPassword, sshPort);
            Console.WriteLine("SSH values updated.\n");
        }
    }

}
