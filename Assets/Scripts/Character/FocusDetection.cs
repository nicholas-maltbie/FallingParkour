﻿using PropHunt.Environment;
using UnityEngine;
using UnityEngine.InputSystem;
using MLAPI;
using MLAPI.Messaging;

namespace PropHunt.Character
{
    /// <summary>
    /// Detect what the player is currently looking at
    /// </summary>
    [RequireComponent(typeof(CameraController))]
    public class FocusDetection : NetworkBehaviour
    {
        /// <summary>
        /// Create sphere radius variable -J
        /// </summary>
        public float sphereRadius = 0.1f;

        /// <summary>
        /// Determines how far the player can look
        /// </summary>
        public float viewDistance = 5.0f;

        /// <summary>
        /// Controller that operates the player camera
        /// </summary>
        private CameraController cameraController;

        /// <summary>
        /// What the player is looking at
        /// </summary>
        public GameObject focus;

        /// <summary>
        /// How far away the hit from the spherecast is
        /// </summary>
        public float currentHitDistance;

        /// <summary>
        /// What does view interaction able to hit
        /// </summary>
        public LayerMask viewLayermask = ~0;

        /// <summary>
        /// How does view intearaction interact with triggers
        /// </summary>
        public QueryTriggerInteraction focusTriggerInteraction;

        /// <summary>
        /// Is the player attempting to interact with anything
        /// </summary>
        public bool interacting;

        /// <summary>
        /// Interact with object the player is looking at
        /// </summary>
        public void Interact(InputAction.CallbackContext context)
        {
            interacting = context.ReadValueAsButton();
        }

        /// <summary> Start is called before the first frame update</summary>
        public void Start()
        {
            this.cameraController = GetComponent<CameraController>();
        }

        public void InteractWithObject(GameObject target, GameObject source)
        {
            if (IsHost)
            {
                target.GetComponent<Interactable>().Interact(source);
            }
            else
            {
                // InteractWithObjectServerRpc(target, source);
            }
        }

        // [ServerRpc]
        // public void InteractWithObjectServerRpc(GameObject target, GameObject source)
        // {
        //     target.GetComponent<Interactable>().Interact(source);
        // }

        /// <summary>Update is called once per frame</summary>
        public void Update()
        {
            // Increase view distance by the current camera zoom distance
            CameraController controller = GetComponent<CameraController>();
            float cameraDistance = controller != null ? controller.CameraDistance : 0;
            float focusRange = viewDistance + cameraDistance;
            // Only update if IsLocalPlayer is true
            if (!this.IsLocalPlayer)
            {
                // exit from update if this is not the local player
                return;
            }
            // if spherecast hits something, update the player's focus and distance variables -J
            if (controller.SpherecastFromCameraBase(focusRange,
                viewLayermask, sphereRadius, focusTriggerInteraction, out RaycastHit hit))
            {
                focus = hit.transform.gameObject;
                // If the object has a focus component, tell the object it is 'focused'
                Focusable focusable = focus.GetComponent<Focusable>();
                if (focusable != null)
                {
                    // Invoke focus every frame locally
                    focusable.Focus(gameObject);
                }
                currentHitDistance = hit.distance - cameraDistance;
                // If player interacts with what they're looking at
                if (interacting && focus.GetComponent<Interactable>() != null)
                {
                    InteractWithObject(focus, gameObject);
                }
            }
            // otherwise change the variables to the non-hit state -J
            else
            {
                focus = null;
                currentHitDistance = viewDistance;
            }
        }
    }
}