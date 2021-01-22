using Mirror;
using PropHunt.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropHunt.Character
{
    public class FocusDetection : NetworkBehaviour
    {
        /// <summary>Network service for managing network calls</summary>
        public INetworkService networkService;
        ///<summary>Create sphere radius variable -J</summary>
        public float sphereRadius = 0.1f;
        /// <summary>Determines how far the player can look</summary>
        public float viewDistance = 5.0f;
        ///<summary>Create layermask I guess -J</summary>
        public LayerMask layerMask;
        /// <summary>What direction is the player looking</summary>
        public Transform cameraTransform;
        /// <summary>What the player is looking at</summary>
        public GameObject focus;
        /// <summary>How far away the hit from the spherecast is</summary>
        public float currentHitDistance;

        /// <summary> Start is called before the first frame update</summary>
        public void Start()
        {
            this.networkService = new NetworkService(this);
        }

        /// <summary>Update is called once per frame</summary>
        public void Update()
        {
            // Only upadte if IsLocalPlayer is true
            if (!this.networkService.isLocalPlayer)
            {
                /// exit from update if this is not the local player
                return;
            }
            //Update origin -J
            Vector3 origin = cameraTransform.position;
            Vector3 direction = cameraTransform.forward;
            RaycastHit hit;
            //if spherecast hits something, update the player's focus and distance variables -J
            if (Physics.SphereCast(origin, sphereRadius, direction, out hit, viewDistance))
            {
                focus = hit.transform.gameObject;
                currentHitDistance = hit.distance;
            }
            //otherwise change the variables to the non-hit state -J
            else
            {
                focus = null;
                currentHitDistance = viewDistance;
            }
        }
    }
}