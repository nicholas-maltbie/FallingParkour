using Mirror;

namespace PropHunt.Utils
{
    /// <summary>
    /// Network service to wrap functionality of NetworkBehaviour in an
    /// interface so it is testable
    /// </summary>
    public interface INetworkService
    {
        /// <summary>
        /// Returns true if this object is active on an active server.
        /// <para>This is only true if the object has been spawned. This is different from NetworkServer.active, which is true if the server itself is active rather than this object being active.</para>
        /// </summary>
        bool isServer { get; }

        /// <summary>
        /// Returns true if running as a client and this object was spawned by a server.
        /// </summary>
        bool isClient { get; }

        /// <summary>
        /// This returns true if this object is the one that represents the player on the local machine.
        /// <para>In multiplayer games, there are multiple instances of the Player object. The client needs to know which one is for "themselves" so that only that player processes input and potentially has a camera attached. The IsLocalPlayer function will return true only for the player instance that belongs to the player on the local machine, so it can be used to filter out input for non-local players.</para>
        /// </summary>
        bool isLocalPlayer { get; }

        /// <summary>
        /// True if this object only exists on the server
        /// </summary>
        bool isServerOnly { get; }

        /// <summary>
        /// True if this object exists on a client that is not also acting as a server
        /// </summary>
        bool isClientOnly { get; }

        /// <summary>
        /// This returns true if this object is the authoritative version of the object in the distributed network application.
        /// <para>The <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see> value on the NetworkIdentity determines how authority is determined. For most objects, authority is held by the server. For objects with <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see> set, authority is held by the client of that player.</para>
        /// </summary>
        bool hasAuthority { get; }

        /// <summary>
        /// The unique network Id of this object.
        /// <para>This is assigned at runtime by the network server and will be unique for all objects for that network session.</para>
        /// </summary>
        uint netId { get; }

        /// <summary>
        /// The <see cref="NetworkConnection">NetworkConnection</see> associated with this <see cref="NetworkIdentity">NetworkIdentity.</see> This is only valid for player objects on the client.
        /// </summary>
        NetworkConnection connectionToServer { get; }

        /// <summary>
        /// The <see cref="NetworkConnection">NetworkConnection</see> associated with this <see cref="NetworkIdentity">NetworkIdentity.</see> This is only valid for player objects on the server.
        /// </summary>
        NetworkConnection connectionToClient { get; }
    }

    /// <summary>
    /// Basic implementation of an INetwork service to take information from a NetworkBehaviour
    /// </summary>
    public class NetworkService : INetworkService
    {
        /// <summary>
        /// Network behaviour controlling this object
        /// </summary>
        public NetworkBehaviour behaviour;

        /// <summary>
        /// Creates a basic NetworkService for a given network behaviour
        /// </summary>
        /// <param name="behaviour">Network behaviour attached to game object</param>
        public NetworkService(NetworkBehaviour behaviour)
        {
            this.behaviour = behaviour;
        }

        /// <inheritdoc/>
        public bool isServer => behaviour.isServer;

        /// <inheritdoc/>
        public bool isClient => behaviour.isClient;

        /// <inheritdoc/>
        public bool isLocalPlayer => behaviour.isLocalPlayer;

        /// <inheritdoc/>
        public bool isServerOnly => behaviour.isServerOnly;

        /// <inheritdoc/>
        public bool isClientOnly => behaviour.isClientOnly;

        /// <inheritdoc/>
        public bool hasAuthority => behaviour.hasAuthority;

        /// <inheritdoc/>
        public uint netId => behaviour.netId;

        /// <inheritdoc/>
        public NetworkConnection connectionToServer => behaviour.connectionToServer;

        /// <inheritdoc/>
        public NetworkConnection connectionToClient => behaviour.connectionToClient;
    }
}
