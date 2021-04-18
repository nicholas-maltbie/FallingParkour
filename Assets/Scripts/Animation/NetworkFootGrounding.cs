
using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Animation
{
    /// <summary>
    /// Allow for player food grounding synchronization over network
    /// </summary>
    [RequireComponent(typeof(NetworkIKControl))]
    public class NetworkFootGrounding : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnGroundFeetChange))]
        public bool groundFeet;

        /// <summary>
        /// component to control player foot grounding
        /// </summary>
        public PlayerFootGrounded playerFootGrounded;

        /// <summary>
        /// Network IK control for synchronizing IK information
        /// </summary>
        private NetworkIKControl networkIKControl;

        /// <summary>
        /// Network service for operating character control
        /// </summary>
        public INetworkService networkService;

        public void OnGroundFeetChange(bool _, bool newState)
        {
            playerFootGrounded.enableFootGrounded = newState;
        }

        private void SetFootGroundedStateInternal(bool newState)
        {
            // Change from newtorked control to local control
            // Grounding feet is done locally so invert network state
            networkIKControl.SetIKGoalState(AvatarIKGoal.LeftFoot, !newState);
            networkIKControl.SetIKGoalState(AvatarIKGoal.RightFoot, !newState);
        }

        public void SetFootGroundedState(bool newState)
        {
            UnityEngine.Debug.Log(networkService.isServer);
            if (!networkService.isServer)
            {
                CmdSetFootGroundedState(newState);
            }
            else
            {
                groundFeet = newState;
                playerFootGrounded.enableFootGrounded = newState;
            }

            if (networkService.isServer || networkService.isLocalPlayer)
            {
                SetFootGroundedStateInternal(newState);
            }
        }

        public void Awake()
        {
            this.networkService = new NetworkService(this);

            this.networkIKControl = GetComponent<NetworkIKControl>();
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
            SetFootGroundedStateInternal(newState);
        }
    }
}