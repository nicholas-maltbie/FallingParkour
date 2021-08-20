using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropHunt.Spectator
{
    /// <summary>
    /// Script to track a spectatable object
    /// </summary>
    public class SpectatorFollow : NetworkBehaviour
    {
        /// <summary>
        /// Action to move to next spectator object.
        /// </summary>
        [SerializeField]
        [Tooltip("Minimum time between player follow target swaps")]
        private UnityEngine.InputSystem.InputActionReference forwardAction;

        /// <summary>
        /// Action to move to previous spectator object.
        /// </summary>
        [SerializeField]
        [Tooltip("Minimum time between player follow target swaps")]
        private UnityEngine.InputSystem.InputActionReference backwardAction;

        /// <summary>
        /// What is the player currently following?
        /// </summary>
        [SerializeField]
        private Followable target;

        /// <summary>
        /// Update function to have the player follow the current target object.
        /// </summary>
        public void LateUpdate()
        {
            if (target != null)
            {
                transform.position = target.transform.position;
            }
        }

        /// <summary>
        /// Initialize player position and target
        /// </summary>
        public void Start()
        {
            NextTarget(0);
            forwardAction.action.started += action => NextTarget(1);
            backwardAction.action.started += action => NextTarget(-1);
        }

        /// <summary>
        /// Get the list of all followable objects currently in the scene.
        /// Will return followable objects in a consistent order
        /// </summary>
        /// <returns>List of all followable objects.</returns>
        public static List<Followable> GetFollowables()
        {
            var followables = new List<Followable>(GameObject.FindObjectsOfType<Followable>());
            followables.Sort((Followable a, Followable b) => a.CompareTo(b));
            return followables;
        }

        /// <summary>
        /// Change target to next target out of all targets currently in the scene.
        /// </summary>
        /// <param name="step">Direction and count to move forward in followable list.</param>
        public void NextTarget(int step = 1)
        {
            if (isLocalPlayer)
            {
                List<Followable> followables = GetFollowables();
                if (followables.Count > 0)
                {
                    int index = followables.FindIndex(0, other => other == target);
                    target = followables[((index + step) + followables.Count) % followables.Count];
                }
            }
        }
    }
}
