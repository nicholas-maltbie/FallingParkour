using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Animation
{
    /// <summary>
    /// Example IK Control tests for how to use the NetworkIKController
    /// </summary>
    [RequireComponent(typeof(NetworkIKControl))]
    public class NetworkIKControlTest : NetworkBehaviour
    {
        /// <summary>
        /// Network IK control for synchronizing IK information
        /// </summary>
        private NetworkIKControl networkIKControl;

        /// <summary>
        /// Right foot state that we want
        /// </summary>
        public bool rightFootState;

        /// <summary>
        /// Right foot state of previous frame
        /// </summary>
        private bool previousRightFootState;

        /// <summary>
        /// Network service for abstracting network commands
        /// </summary>
        public INetworkService networkService;

        public void Awake()
        {
            // Find and load network ik controller
            this.networkIKControl = GetComponent<NetworkIKControl>();
            this.networkService = new NetworkService(this);
        }

        public void Update()
        {
            // If our desired state has changed, update the current state
            if (rightFootState != previousRightFootState && networkService.isLocalPlayer)
            {
                // These set commands abstract whether the task should be done on the client or server
                this.networkIKControl.SetIKGoalState(AvatarIKGoal.RightFoot, rightFootState);
                this.networkIKControl.SetIKGoalWeight(AvatarIKGoal.RightFoot, rightFootState ? 1 : 0);
            }
            previousRightFootState = rightFootState;
        }
    }
}