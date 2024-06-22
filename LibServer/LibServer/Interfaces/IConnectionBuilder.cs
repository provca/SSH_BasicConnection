namespace LibServer.Interfaces
{
    public interface IConnectionBuilder
    {
        // Properties for connection information
        string Server { get; set; }
        string UserID { get; set; }
        string Password { get; set; }
        string Database { get; set; }
        uint Port { get; set; }

        /// <summary>
        /// Gets the connection string for the database.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Initializes the connection details.
        /// </summary>
        void Initialize();
    }
}
