using Mirror;
using PropHunt.Utils;
using PropHunt.Environment;
using UnityEngine;

namespace PropHunt.Character
{
    public class FocusDetection : NetworkBehaviour
    {
        /// <summary>Unity service for getting player inputs</summary>
        public IUnityService unityService = new UnityService();
        /// <summary>Network service for managing network calls</summary>
        public INetworkService networkService;
        ///<summary>Create sphere radius variable -J</summary>
        public float sphereRadius = 0.1f;
        /// <summary>Determines how far the player can look</summary>
        public float viewDistance = 5.0f;
        /// <summary>What direction is the player looking</summary>
        public Transform cameraTransform;
        /// <summary>What the player is looking at</summary>
        public GameObject focus;
        /// <summary>How far away the hit from the spherecast is</summary>
        public float currentHitDistance;
        /// <summary>What does view interaction able to hit</summary>
        public LayerMask viewLayermask = ~0;
        /// <summary>How does view intearaction interact with triggers</summary>
        public QueryTriggerInteraction focusTriggerInteraction;

        /// <summary> Start is called before the first frame update</summary>
        public void Start()
        {
            this.networkService = new NetworkService(this);
        }

        public void InteractWithObject(GameObject target, GameObject source)
        {
            if (networkService.isServer)
            {
                target.GetComponent<Interactable>().Interact(source);
            }
            else
            {
                CmdInteractWithObject(target, source);
            }
        }

        [Command]
        public void CmdInteractWithObject(GameObject target, GameObject source)
        {
            target.GetComponent<Interactable>().Interact(source);
        }

        /// <summary>Update is called once per frame</summary>
        public void Update()
        {
            // Increase view distance by the current camera zoom distance
            CameraController controller = GetComponent<CameraController>();
            float cameraDistance = controller != null ? controller.CameraDistance : 0;
            float focusRange = viewDistance + cameraDistance;
            // Only update if IsLocalPlayer is true
            if (!this.networkService.isLocalPlayer)
            {
                // exit from update if this is not the local player
                return;
            }
            //Update origin -J
            Vector3 origin = cameraTransform.position;
            Vector3 direction = cameraTransform.forward;
            RaycastHit hit;
            // if spherecast hits something, update the player's focus and distance variables -J
            if (PhysicsUtils.SphereCastFirstHitIgnore(gameObject, origin, sphereRadius, direction, focusRange,
                viewLayermask, focusTriggerInteraction, out hit))
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
                if (unityService.GetButtonDown("Interact") && focus.GetComponent<Interactable>() != null)
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