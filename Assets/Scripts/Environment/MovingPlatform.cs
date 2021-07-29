using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace PropHunt.Environment
{
    public class MovingPlatform : NetworkBehaviour
    {
        /// <summary>
        /// Velocity at which this platform should move
        /// </summary>
        public float linearSpeed = 3;

        private int currentTargetIndex = 0;

        /// <summary>
        /// Gets the current target we're moving towards
        /// </summary>
        public Transform CurrentTarget => targetsList[currentTargetIndex];

        [SerializeField]
        public List<Transform> targetsList;

        public Vector3 moved;

        // Start is called before the first frame update
        void Start()
        {

        }

        void FixedUpdate()
        {
            moved = Vector3.zero;
            if (!isServer)
            {
                return;
            }

            if (targetsList == null || targetsList.Count == 0)
            {
                return;
            }
            float fixedDeltaTime = Time.fixedDeltaTime;
            var direction = (CurrentTarget.position - transform.position).normalized;
            var displacement = direction * fixedDeltaTime * this.linearSpeed;
            var distanceToTarget = Vector3.Distance(transform.position, CurrentTarget.position);

            if (direction == Vector3.zero || distanceToTarget < displacement.magnitude)
            {
                displacement = CurrentTarget.position - transform.position;
                currentTargetIndex = (currentTargetIndex + 1) % targetsList.Count;
            }

            transform.position += displacement;
            moved += displacement;

        }
    }
}