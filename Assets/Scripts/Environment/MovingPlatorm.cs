using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace PropHunt.Environment
{    public class MovingPlatorm : NetworkBehaviour
    {
        /// <summary>
        /// Velocity at which this platform should move
        /// </summary>
        [SyncVar]
        public float linearSpeed = 3;

        [SyncVar]
        private int currentTargetIndex = 0;

        /// <summary>
        /// Gets the current target we're moving towards
        /// </summary>
        public Transform CurrentTarget => targetsList[currentTargetIndex];
        
        [SerializeField]
        public List<Transform> targetsList;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        void FixedUpdate()
        {
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
            
            Debug.Log("Direction: " + direction + "\n" + 
                      "Displacement: " + displacement + "\n" + 
                      "DistanceToTarget: " + distanceToTarget + "\n" +
                      "CurrentTarget: " + CurrentTarget);

            if (distanceToTarget > displacement.magnitude)
            {
                transform.position += displacement;
                
                if (Vector3.Distance(transform.position, CurrentTarget.position) < 0.001)
                {
                    currentTargetIndex = (currentTargetIndex + 1) % targetsList.Count;
                }
            }
            else
            {
                var remainingDisplacement = displacement.magnitude - distanceToTarget;
                transform.position = CurrentTarget.transform.position;
                currentTargetIndex = (currentTargetIndex + 1) % targetsList.Count;

                distanceToTarget = Vector3.Distance(transform.position, CurrentTarget.position);
                var movement = remainingDisplacement < distanceToTarget ? remainingDisplacement : distanceToTarget;
                direction = (CurrentTarget.transform.position - this.transform.position).normalized;
                transform.position += direction * movement;
            }
        }
    }
}