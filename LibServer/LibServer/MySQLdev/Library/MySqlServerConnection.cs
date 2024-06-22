using LibServer.Interfaces;
using LibServer.MySQLdev.Interfaces;
using MySqlConnector;
using System.Data;

namespace LibServer.MySQLdev.Library
{
    /// <summary>
    /// Implementation of the <see cref="IMySqlServerConnection"/> interface for managing server connections.
    /// </summary>
    public class MySqlServerConnection : IMySqlServerConnection
    {
        private IConnectionBuilder? _connectionBuilder;

        /// <inheritdoc/>
        public string ConnectionString => _connectionBuilder?.ConnectionString ?? throw new InvalidOperationException("ConnectionString is null.");

        /// <inheritdoc/>
        public void Initialize(IConnectionBuilder connectionBuilder)
        {
            try
            {
                // Set the connection builder and initialize connection details
                _connectionBuilder = connectionBuilder ?? throw new ArgumentNullException(nameof(connectionBuilder));
                _connectionBuilder.Initialize();
            }
            catch (Exception ex)
            {
                throw new Exception("Error during initialization.", ex);
            }
        }

        /// <inheritdoc/>
        public bool CheckConnectionStatus()
        {
            using MySqlConnection mySqlConn = new(ConnectionString.ToString());
            try
            {
                // Attempt to open and close the connection to check its status
                OpenConnection(mySqlConn);
                return mySqlConn.State == ConnectionState.Open;
            }
            catch (InvalidOperationException ex)
            {
                // Log InvalidOperationException
                throw new InvalidOperationException($"Exception Type: {ex.GetType().Name}, Message: {ex.Message}, StackTrace: {ex.StackTrace}");
            }
            catch (MySqlException ex)
            {
                // Log connection error
                throw new Exception($"Error connecting to the database. Exception Type: {ex.GetType().Name}, Message: {ex.Message}, StackTrace: {ex.StackTrace}");
            }
            finally
            {
                CloseConnection(mySqlConn);
            }
        }

        /// <inheritdoc/>
        public MySqlConnection OpenNewConnection()
        {
            // Create a new MySqlConnection using the current connection string.
            MySqlConnection mySqlConn = new(ConnectionString.ToString());

            // Open the connection.
            OpenConnection(mySqlConn);

            // Return the opened MySqlConnection instance.
            return mySqlConn;
        }

        /// <summary>
        /// Opens the database connection if it's not already open.
        /// </summary>
        /// <param name="connection">The MySqlConnection to open.</param>
        private void OpenConnection(MySqlConnection connection)
        {
            try
            {
                // Open the connection if it's not already open
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine($"InvalidCastException: Cannot cast from DBNull to another type. Message: {ex.Message}, StackTrace: {ex.StackTrace}");

                // Handle specific InvalidCastException
                throw new Exception($"InvalidCastException: Cannot cast from DBNull to another type. Message: {ex.Message}, StackTrace: {ex.StackTrace}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when trying to open the connection. Exception Type: {ex.GetType().Name}, Message: {ex.Message}, StackTrace: {ex.StackTrace}");

                throw new Exception($"Error when trying to open the connection. Exception Type: {ex.GetType().Name}, Message: {ex.Message}, StackTrace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Closes the database connection if it's not already closed.
        /// </summary>
        /// <param name="connection">The MySqlConnection to close.</param>
        private static void CloseConnection(MySqlConnection connection)
        {
            try
            {
                // Close the connection if it's not already closed
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when trying to close the connection. Exception Type: {ex.GetType().Name}, Message: {ex.Message}, StackTrace: {ex.StackTrace}");
            }
        }
    }
}
