using PropHunt.Game.Flow;
using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Enable an object when a given in game state is reached.
    /// </summary>
    public class EnableScriptForInGameState : MonoBehaviour
    {
        /// <summary>
        /// Desired state to enable game object for
        /// </summary>
        [Tooltip("Desired state in which to enable GameObject")]
        [SerializeField]
        private InGameState desiredState = InGameState.Running;

        /// <summary>
        /// Script controleld by this object
        /// </summary>
        [Tooltip("Script controleld by this object")]
        [SerializeField]
        private MonoBehaviour controlledScript;

        public void Start()
        {
            VerifyState();
        }

        public void Update()
        {
            VerifyState();
        }

        public void VerifyState()
        {
            bool enabledState = !(InGameStateManager.Singleton == null || InGameStateManager.Singleton.CurrentState != desiredState);
            if (controlledScript.enabled != enabledState)
            {
                controlledScript.enabled = enabledState;
            }
        }
    }
}