using LibServer.Interfaces;

namespace LibServices.Server.Interfaces
{
    /// <summary>
    /// Interface for a service handling server connection.
    /// </summary>
    public interface IService_ServerConnection
    {
        /// <summary>
        /// Initializes the server connection.
        /// </summary>
        /// <param name="connectionBuilder">The IConnectionBuilder interface for configuring the connection.</param>
        void Initialize(IConnectionBuilder connectionBuilder);

        /// <summary>
        /// Checks the connection state.
        /// </summary>
        /// <returns>True if the connection is successful; otherwise, false.</returns>
        bool State();
    }
}
