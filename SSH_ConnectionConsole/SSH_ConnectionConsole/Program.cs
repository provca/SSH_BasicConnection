using LibServices.Settings;
using SSH_ConnectionConsole.Menu;

//**************************************************************
// Initializated settings here. Only for educational propouses.

// MySql connection.
ConnectionBuildersSettings.Server = "localhost";    // server ip, for example 192.168.1.xx
ConnectionBuildersSettings.UserID = "root";         // mysql user
ConnectionBuildersSettings.Password = "";           // mysql password
ConnectionBuildersSettings.Database = "mysql";      // one of mysql's databases
ConnectionBuildersSettings.Port = 3306;             // default port

// Server conection by ssh
SshSettings.SshHost = "127.0.0.1";                  // don't change
SshSettings.SshUserName = "piserver";               // server's user name where mysql is allocated
SshSettings.SshPassword = "raspberry";              // server's password where mysql is allocated
SshSettings.SshPort = 22;                           // default port for ssh
//**************************************************************

// Start the main menu to test ssh connection.
MainMenu.StartMenu();

// Wait for a key press before exiting.
Console.WriteLine("Press any key to exit.");
Console.ReadKey();