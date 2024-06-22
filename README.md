# LibSSH Connection Manager
A .NET project for secure SSH connections using `SSH.NET`, offering streamlined setup and management. Ensures robust security through fingerprint verification, facilitating seamless integration of SSH functionality into .NET applications.

## LibSSH
This project provides a simple way to integrate `SSH.NET` in your project. In this repository you will find a solution with three main components:
1. **SSH_ConnectionConsole (Main Solution):** Sample console application that interacts with users and initiates SSH connections using LibServices.
2. **LibServices (Middle Layer):** Middle layer that abstracts the SSH connection logic and manages connection settings and forwards requests to LibSSH.
3. **LibSSH (Backend):** Backend library responsible to use `SSH.NET`, using cryptographic methods to ensure secure communications.

## Key Features:
+ Establish SSH connections with remote servers.
+ Securely manage server fingerprints to detect MITM (Man-in-the-Middle) attacks.

## Technologies Used:
+ C#/.NET Core for cross-platform compatibility.
+ Libraries: `SSH.NET` for SSH connectivity[^1].
+ Configuration management using appsettings.json for settings[^2].
+ MySQLConnector to test connection state[^3].

# Uses and Applications
LibSSH is a basic sample to use `SSH.NET` package into C# projects, enabling secure management of remote servers via SSH.

## Compatibility
The class library has been developed in .NET 8.0. It worked on Windows, Linux, and MacOS. You will need to install .NET on your OS if you do not already have it. Remember to publish your application with the target architecture: Linux-64, osx-64, win-x64...

# Files System Structure
This is the file system structure of the LibSSH library:
```
LibSSH/
│
├── AppSettings/
│   └── AppSettingsReader.cs
│
├── Manager/
│   └── SshManager.cs
│
└── appsettings.json
```

`SshManager.cs` in LibSSH orchestrates SSH connections within C# applications. It encapsulates logic for establishing, managing, and securing SSH sessions with remote servers. This class ensures robust server fingerprint verification, leveraging cryptography to authenticate hosts and prevent security breaches.

`AppSettingsReader.cs` provides a streamlined approach to retrieve application settings from appsettings.json in C# projects. It utilizes the `Microsoft.Extensions.Configuration.Json` library to manage configurations such as server fingerprints securely. This class enhances maintainability by centralizing configuration management, ensuring robust application settings handling.

`appsettings.json` serves as a configuration file in C# projects utilizing the *Microsoft.Extensions.Configuration* library. It centralizes key application settings, including server fingerprints, facilitating easy access and management. This file enhances application scalability and maintainability by separating configuration from code, enabling swift adjustments without recompilation.

This is the file system structure of the LibServices library. Server folder and ConnectionBuildersSettings.cs file  is a sample to show how integrate SSH in a project:

```
LibServices/
│
├── Server/
│   ├── Factories/
│   │   └── mysql/
│   │       └── Factory_ServerMySqlConnectionService.cs
│   │
│   ├── Factory_Servers.cs
│   │
│   ├── Interfaces/
│   │
│   ├── mysql/
│   │
│   └── ServerConnections.cs
│
└── Settings/
    ├── ConnectionBuildersSettings.cs
    │
    └── SshSettings.cs
```

`SshSettings.cs` facilitates SSH connections in C# projects, managing authentication with remote servers via username, password, and port parameters. The class also supports local port forwarding for secure communication across systems.

`LibServices` project as an intermediary layer and, in this example, includes files for MySQL connections. The file organization adheres to SOLID principles and lays the groundwork for integrating additional connections. See how LibSSH is integrated and use it as a reference for your own projects.

To understand the example program flow, you need to be familiar with the following files:
* **Server/Factories/Factory_Servers.cs:** Manages connection decoupling and provides project-wide scalability. While not part of LibSSH, it includes the method public static bool SshConnectionState().
* **Server/Factories/mysql/Factory_ServerMySqlConnectionService.cs:** Contains methods that incorporate SSH:
	* private static MySqlServerConnection InitializeSshAndMySqlConnection()
	* public static IService_ServerConnection SshConnection()

`LibServer` is an independent project used as an example within this application. Additional connections or other projects can be integrated into this structure.

# Solution Flow
**Step 1: Initialization**

    Program.cs -> MainMenu -> StartMenu(); -> CollectSshData(); -> SshConnect()

**Step 2: Creating the ConnectionBuilder (MySQL connection)**

    Factory_Servers.InitializeConnectionBuilder -> Factory_ServerMySqlConnectionService.InitializeConnectionBuilder()

**Step 3: Establishing Connection**

    Factory_Servers.ConnectionState(); -> Factory_Servers.SshConnectionState(); -> Factory_ServerMySqlConnectionService.SshConnection(); -> Factory_ServerMySqlConnectionService.InitializeSshAndMySqlConnection();

## Wrapping MySQL Connection with SSH
Below is a comparison between a raw connection and one using SSH. Both connections can be seen and compared within the project.

### Raw Connection:

    public static IService_ServerConnection Connection()
    {
        var connection = new MySqlServerConnection();
        connection.Initialize(GetNonNullConnectionBuilder());
    
        // Code...
    }

### SSH Connection:

    public static IService_ServerConnection SshConnection()
    {
        try
        {
            // Initialize SSH and MySQL connection.
            var connection = InitializeSshAndMySqlConnection();
    
            // Code...
        }
        catch (Exception ex)
        {
            SshSettings.SshDisconnect();
            throw;
        }
        finally
        {
            SshSettings.SshDisconnect();
        }
    }
    
    private static MySqlServerConnection InitializeSshAndMySqlConnection()
    {
        // Initialize the ssh connection builder.
        var connectionBuilder = MySqlConnectionBuilderSsh();
    
        // Code...
    }
    
    private static MySqlConnectionBuilder MySqlConnectionBuilderSsh()
    {
        // Update SSH settings based on connection builder settings.
        SshSettings.UpdateSshSettings(ConnectionBuildersSettings.Server, SshSettings.SshUserName, SshSettings.SshPassword, SshSettings.SshPort);
    
        // Establish SSH connection.
        SshSettings.SshConnect();
    
        // Set up port forwarding and get the local port.
        uint localPort = SshSettings.SshBridge(ConnectionBuildersSettings.Port, ConnectionBuildersSettings.Server, ConnectionBuildersSettings.Port);
    
        // Initialize the connection builder with the local port.
        var connectionBuilder = new MySqlConnectionBuilder(SshSettings.SshHost, ConnectionBuildersSettings.UserID, ConnectionBuildersSettings.Password, ConnectionBuildersSettings.Database, localPort);
    
        return new MySqlConnectionBuilder(SshSettings.SshHost, ConnectionBuildersSettings.UserID, ConnectionBuildersSettings.Password, ConnectionBuildersSettings.Database, localPort);
    }


## NuGet Packages used
Project LibSSH: [SSH.NET](https://github.com/sshnet/SSH.NET)


## Implementation
It's important to understand that this repository can be interpreted in two ways:

**Option 1:** You can download the entire repository, which contains the SSH_ConnectionConsole solution, to test the app flow:

```
SSH_ConnectionConsole
│
└── LibServices
    │
    ├── LibServer
    │
    └── LibSSH
```

If you use the entire solution, you must also install the `MySqlConnection` package in the LibServer project. Ensure you have a server with MySQL installed and configured. Additionally, you will need to know your server's fingerprint.

**Option 2:** If you choose this option, it is assumed that your project already has the necessary architecture, the appsettings.json file (or another variable storage system), the fingerprint, and your Host data. You may need to change the namespaces of the files from this repository. To implement the basic SSH configurations into your project, you only need:
+ The content of LibSSH or the entire project.
+ The `SshSettings.cs` file located in LibServices.
+ Install the [SSH.NET](https://github.com/sshnet/SSH.NET) NuGet package.

## Basic Parameters
Originally, the naming conventions are set up for connecting to a Raspberry Pi. You can modify or add new variables in `appsettings.json` according to your specific requirements.

If your host is a different type, ensure you customize it appropriately for accurate identification and best practices. You can find the `ExpectedFingerprintRaspberry` variable in:
+ appsettings.json
+ SshManager.cs

Update the values for your host in SshSettings.cs:

    SshHost
    SshUserName
    SshPassword
    SshPort

Obtain the fingerprint of your host and update it in the appsettings.json file. Make sure to include the `'='` character at the end. `SSH.NET` may fail to recognize the fingerprint if this detail is omitted:
```
{
  "SSH": {
    "ExpectedFingerprintRaspberry": "Server_FingerPrint="
  }
}
```

If you're practicing with the **SSH_ConnectionConsole** solution and its linked projects, also update the connection data in `Program.cs` under `ConnectionBuildersSettings.cs`:

```
Server
UserID
Password
Database
Port
```

> [!WARNING]
>Remember that to read `appsettings.json` file (AppSettingsReader.cs), the LibSSH needs `Microsoft.Extensions.Configuration.Json` in LibSSH project. Install other package or use your own classes If you are using other method to read it.

Adjust these settings to match your specific environment and configuration needs.


[^1]: [SSH.NET](https://github.com/sshnet/SSH.NET)
[^2]: [Microsoft.Extensions.Configuration.Json](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json)
[^3]: [MySQLConnector](https://mysqlconnector.net/)