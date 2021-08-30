
using Mirror;
using PropHunt.Character.Avatar;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Animation
{
    /// <summary>
    /// Allow for player food grounding synchronization over network
    /// </summary>
    public class NetworkFootGrounding : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnGroundFeetChange))]
        public bool groundFeet;

        /// <summary>
        /// component to control player foot grounding
        /// </summary>
        public PlayerFootGrounded playerFootGrounded;

        /// <summary>
        /// Network service for operating character control
        /// </summary>
        public INetworkService networkService;

        public void OnGroundFeetChange(bool _, bool newState)
        {
            if (playerFootGrounded != null)
            {
                playerFootGrounded.enableFootGrounded = newState;
            }
        }

        public void SetFootGroundedState(bool newState)
        {
            if (!networkService.isServer)
            {
                CmdSetFootGroundedState(newState);
            }
            else
            {
                groundFeet = newState;
                if (playerFootGrounded != null)
                {
                    playerFootGrounded.enableFootGrounded = newState;
                }
            }
        }

        public void Awake()
        {
            this.networkService = new NetworkService(this);
        }

        public void Start()
        {
            if (networkService.isServer)
            {
                SetFootGroundedState(true);
            }
        }

        [Command]
        public void CmdSetFootGroundedState(bool newState)
        {
            SetFootGroundedState(newState);
        }
    }
}