using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using PropHunt.Utils;

namespace PropHunt.Animation
{
    /// <summary>
    /// Allow for player food grounding synchronization over network
    /// </summary>
    public class NetworkFootGrounding : NetworkBehaviour
    {
        /// <summary>
        /// Current state of player foot grounding
        /// </summary>
        public NetworkVariable<bool> groundFeet = new NetworkVariable<bool>(
            new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly },
            false);

        /// <summary>
        /// component to control player foot grounding
        /// </summary>
        public PlayerFootGrounded playerFootGrounded;

        public void OnGroundFeetChange(bool _, bool newState)
        {
            if (playerFootGrounded != null)
            {
                playerFootGrounded.enableFootGrounded = newState;
            }
        }

        public void SetFootGroundedState(bool newState)
        {
            if (!this.IsHost)
            {
                SetFootGroundedServerRpc(newState);
            }
            else if (IsLocalPlayer)
            {
                groundFeet.Value = newState;
                if (playerFootGrounded != null)
                {
                    playerFootGrounded.enableFootGrounded = newState;
                }
            }
        }

        public void OnEnable()
        {
            groundFeet.OnValueChanged += OnGroundFeetChange;
        }

        public void OnDisable()
        {
            groundFeet.OnValueChanged -= OnGroundFeetChange;
        }

        public void Start()
        {
            if (this.IsLocalPlayer)
            {
                SetFootGroundedState(true);
            }
        }

        [ServerRpc]
        public void SetFootGroundedServerRpc(bool newState)
        {
            SetFootGroundedState(newState);
        }
    }
}