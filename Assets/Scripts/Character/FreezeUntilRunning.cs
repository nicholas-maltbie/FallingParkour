using PropHunt.Game.Flow;
using UnityEngine;

namespace PropHunt.Character
{
    /// <summary>
    /// Freeze a player until the InGameState is Running
    /// </summary>
    [RequireComponent(typeof(KinematicCharacterController))]
    public class FreezeUntilRunning : MonoBehaviour
    {
        private KinematicCharacterController kcc;

        public void Start()
        {
            this.kcc = GetComponent<KinematicCharacterController>();
        }

        public void Update()
        {
            // Freeze the player when not in InGameState
            InGameStateManager stateManager = InGameStateManager.Singleton;
            if (stateManager != null)
            {
                kcc.Frozen = stateManager.CurrentState != InGameState.Running;
            }
            else
            {
                kcc.Frozen = true;
            }
        }
    }
}
