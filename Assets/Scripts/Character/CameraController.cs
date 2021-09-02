
using System.Collections;
using System.Collections.Generic;
using Mirror;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropHunt.Character
{
    public class CameraController : NetworkBehaviour
    {
        /// <summary>
        /// Network service for managing network calls
        /// </summary>
        public INetworkService networkService;

        /// <summary>
        /// Mocked unity service for accessing inputs, delta time, and
        /// various other static unity inputs in a testable manner.
        /// </summary>
        public IUnityService unityService = new UnityService();

        /// <summary>
        /// Maximum pitch for rotating character camera in degrees
        /// </summary>
        public float maxPitch = 90;

        /// <summary>
        /// Minimum pitch for rotating character camera in degrees
        /// </summary>
        public float minPitch = -90;

        /// <summary>
        /// Rotation rate of camera in degrees per second per one unit of axis movement
        /// </summary>
        public float rotationRate = 180;

        /// <summary>
        /// How much the character rotated about the vertical axis this frame
        /// </summary>
        public float frameRotation;

        /// <summary>
        /// Transform holding camera position and rotation data
        /// </summary>
        public Transform cameraTransform;

        /// <summary>
        /// Camera offset from character center
        /// </summary>        
        public Vector3 baseCameraOffset;

        /// <summary>
        /// Minimum distance (closest zoom) of player camera
        /// </summary>
        public float minCameraDistance = 0.0f;

        /// <summary>
        /// Maximum distance (farthest zoom) of player camera
        /// </summary>
        public float maxCameraDistance = 4.0f;

        /// <summary>
        /// Current distance of the camera from the player position
        /// </summary>
        public float currentDistance;

        /// <summary>
        /// Zoom distance change in units per second
        /// </summary>
        public float zoomSpeed = 1.0f;

        /// <summary>
        /// What can the camera collide with
        /// </summary>
        public LayerMask cameraRaycastMask = ~0;

        /// <summary>
        /// Distance in which the third person character will be completely transparent and only cast shadows
        /// </summary>
        public float shadowOnlyDistance = 0.5f;

        /// <summary>
        /// Distance where the player object will dither but still be visible
        /// </summary>
        public float ditherDistance = 1.0f;

        /// <summary>
        /// Base object where all the third person character is stored.
        /// </summary>
        public GameObject thirdPersonCharacterBase;

        /// <summary>
        /// Time in seconds it takes to transition between opacity states
        /// </summary>
        public float transitionTime = 0.1f;

        /// <summary>
        /// Previous player opacity for dithering
        /// </summary>
        private float previousOpacity = 0.0f;

        /// <summary>
        /// Change in yaw from mouse movement
        /// </summary>
        private float yawChange;

        /// <summary>
        /// Change in pitch from mouse movement
        /// </summary>
        private float pitchChange;

        /// <summary>
        /// Current target position of the camera.
        /// </summary>
        private Vector3 cameraPosition;

        /// <summary>
        /// Objects to ignore when drawing raycast for camera
        /// </summary>
        private List<GameObject> ignoreObjects = new List<GameObject>();

        /// <summary>
        /// Get the current distance of the camera from the player camera location
        /// </summary>
        public float CameraDistance { get; private set; }

        /// <summary>
        /// Source camera position in real world space, this is where the head of 
        /// the player would be, where the camera zooms out from
        /// </summary>
        public Vector3 CameraSource => transform.TransformDirection(this.baseCameraOffset) + transform.position;

        /// <summary>
        /// Add an object to the ignore list when raycasting camera position
        /// </summary>
        /// <param name="go"></param>
        public void AddIgnoreObject(GameObject go) => ignoreObjects.Add(go);

        /// <summary>
        /// Remove an object to the ignore list when raycasting camera position
        /// </summary>
        /// <param name="go"></param>
        public bool RemoveIgnoreObject(GameObject go) => ignoreObjects.Remove(go);

        /// <summary>
        /// Get the current yaw of the player camera.
        /// </summary>
        public float Yaw => cameraTransform.rotation.eulerAngles.y;

        /// <summary>
        /// Get the current pitch of the player camera.
        /// </summary>
        public float Pitch => cameraTransform.rotation.eulerAngles.x;

        public void Start()
        {
            this.networkService = new NetworkService(this);
            this.baseCameraOffset = this.cameraTransform.localPosition;
            this.currentDistance = minCameraDistance;
            this.ignoreObjects.Add(gameObject);
        }

        public bool RaycastFromCameraBase(float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction, out RaycastHit hit)
        {
            return PhysicsUtils.RaycastFirstHitIgnore(ignoreObjects, CameraSource, cameraTransform.forward, maxDistance,
                layerMask, queryTriggerInteraction, out hit);
        }

        public bool SpherecastFromCameraBase(float maxDistance, LayerMask layerMask, float sphereRadius, QueryTriggerInteraction queryTriggerInteraction, out RaycastHit hit)
        {
            return PhysicsUtils.SphereCastFirstHitIgnore(ignoreObjects, CameraSource, sphereRadius, cameraTransform.forward, maxDistance,
                layerMask, queryTriggerInteraction, out hit);
        }

        /// <summary>
        /// Look action changes for camera movement
        /// </summary>
        public void OnLook(InputAction.CallbackContext context)
        {
            Vector2 look = context.ReadValue<Vector2>();
            look *= PlayerInputManager.mouseSensitivity;
            yawChange = look.x;
            pitchChange = look.y;
        }

        public void Update()
        {
            if (!networkService.isLocalPlayer)
            {
                // exit from update if this is not the local player
                return;
            }
            float deltaTime = unityService.deltaTime;

            // bound pitch between -180 and 180
            float yaw = cameraTransform.rotation.eulerAngles.y;
            float pitch = (cameraTransform.rotation.eulerAngles.x % 360 + 180) % 360 - 180;

            float zoomChange = 0;
            // Only allow rotation if player is allowed to move
            if (PlayerInputManager.playerMovementState == PlayerInputState.Allow)
            {
                yawChange = rotationRate * deltaTime * yawChange;
                yaw += yawChange;
                pitch += rotationRate * deltaTime * -1 * pitchChange;
                // zoomChange = zoomSpeed * deltaTime * -1 * unityService.GetAxis("Mouse ScrollWheel");
            }
            // Clamp rotation of camera between minimum and maximum specified pitch
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            frameRotation = yawChange;
            // Change camera zoom by desired level
            // Bound the current distance between minimum and maximum
            this.currentDistance = Mathf.Clamp(this.currentDistance + zoomChange, this.minCameraDistance, this.maxCameraDistance);

            // Set the local position of the camera to be the current rotation projected
            //   backwards by the current distance of the camera from the player
            Vector3 cameraDirection = -cameraTransform.forward * this.currentDistance;
            Vector3 cameraSource = CameraSource;

            // Draw a line from our camera source in the camera direction. If the line hits anything that isn't us
            // Limit the distance by how far away that object is
            // If we hit something
            if (PhysicsUtils.SphereCastFirstHitIgnore(ignoreObjects, cameraSource, 0.01f, cameraDirection, cameraDirection.magnitude,
                this.cameraRaycastMask, QueryTriggerInteraction.Ignore, out RaycastHit hit))
            {
                // limit the movement by that hit
                cameraDirection = cameraDirection.normalized * hit.distance;
            }

            this.CameraDistance = cameraDirection.magnitude;

            bool hittingSelf = PhysicsUtils.SphereCastAllow(gameObject, cameraSource + cameraDirection, 0.01f, -cameraDirection.normalized,
                cameraDirection.magnitude, ~0, QueryTriggerInteraction.Ignore, out RaycastHit selfHit);

            // float actualDistance = Mathf.Cos(Mathf.Deg2Rad * pitch) * cameraDirection.magnitude;
            float actualDistance = hittingSelf ? selfHit.distance : cameraDirection.magnitude;

            if (thirdPersonCharacterBase != null)
            {
                thirdPersonCharacterBase.transform.rotation = Quaternion.Euler(0, yaw, 0);

                if (actualDistance < shadowOnlyDistance)
                {
                    MaterialUtils.RecursiveSetShadowCasingMode(thirdPersonCharacterBase, UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly);
                }
                else
                {
                    MaterialUtils.RecursiveSetShadowCasingMode(thirdPersonCharacterBase, UnityEngine.Rendering.ShadowCastingMode.On);
                }

                if (actualDistance > shadowOnlyDistance && actualDistance < ditherDistance)
                {
                    float newOpacity = (actualDistance - shadowOnlyDistance) / (ditherDistance - minCameraDistance);
                    float lerpPosition = transitionTime > 0 ? deltaTime * 1 / transitionTime : 1;
                    previousOpacity = Mathf.Lerp(previousOpacity, newOpacity, lerpPosition);
                    // Set opacity of character based on how close the camera is
                    MaterialUtils.RecursiveSetFloatProperty(thirdPersonCharacterBase, "_Opacity", previousOpacity);
                }
                else
                {
                    // Set opacity of character based on how close the camera is
                    MaterialUtils.RecursiveSetFloatProperty(thirdPersonCharacterBase, "_Opacity", 1);
                    previousOpacity = actualDistance > shadowOnlyDistance ? 1 : 0;
                }
            }
            cameraPosition = cameraSource + cameraDirection;
            cameraTransform.position = cameraPosition;

            // Set pitch to be camera's rotation
            cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0);
        }
    }
}