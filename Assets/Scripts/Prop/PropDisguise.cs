using System.Collections;
using Mirror;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Prop
{
    /// <summary>
    /// Disguise information for hiding a player
    /// </summary>
    public struct Disguise
    {
        /// <summary>
        /// Game object prefab for player disguise
        /// </summary>
        public GameObject disguiseVisual;

        /// <summary>
        /// Collider associated with the disguise
        /// </summary>
        public Collider disguiseCollider;

        /// <summary>
        /// Offset of camera from the props center
        /// </summary>
        public Vector3 cameraOffset;
    }

    /// <summary>
    /// Disguise associated with a player
    /// </summary>
    public class PropDisguise : NetworkBehaviour
    {
        [SyncVar(hook = nameof(SetDisguise))]
        public string selectedDisguise;

        public Transform disguiseBase;

        public void SetDisguise(string oldDisguise, string newDisguise)
        {
            if (PropDatabase.HasDisguise(newDisguise))
            {
                Disguise disguiseInformation = PropDatabase.GetDisguise(newDisguise);
                StartCoroutine(ChangeDisguise(disguiseInformation.disguiseVisual));
                StartCoroutine(ChangeCollider(disguiseInformation.disguiseCollider));
                // Change camera position
                CameraController cameraController = gameObject.GetComponent<CameraController>();
                if (cameraController != null)
                {
                    cameraController.baseCameraOffset = disguiseInformation.cameraOffset;
                }
            }
        }

        public void SetSelectedDisguise(GameObject targetProp)
        {
            Prop prop = targetProp.GetComponent<Prop>();
            if (prop != null)
            {
                selectedDisguise = prop.propName;
            }
        }

        public void Start()
        {
            if (selectedDisguise != "")
            {
                SetDisguise("", selectedDisguise);
            }
        }

        public IEnumerator ChangeCollider(Collider collider)
        {
            // Remove collider if it exists
            Collider currentCollider = gameObject.GetComponent<Collider>();
            if (currentCollider != null)
            {
                GameObject.Destroy(currentCollider);
                yield return null;
            }
            // Setup new collider data
            if (collider is BoxCollider)
            {
                BoxCollider baseCollider = (BoxCollider)collider;
                BoxCollider newCollider = ComponentUtils.CopyComponent<BoxCollider>(baseCollider, gameObject);
                newCollider.center = baseCollider.center;
                newCollider.size = baseCollider.size;
            }
            else if (collider is SphereCollider)
            {
                SphereCollider baseCollider = (SphereCollider)collider;
                SphereCollider newCollider = ComponentUtils.CopyComponent<SphereCollider>(baseCollider, gameObject);
                newCollider.radius = baseCollider.radius;
                newCollider.center = baseCollider.center;
            }
            else if (collider is CapsuleCollider)
            {
                CapsuleCollider baseCollider = (CapsuleCollider)collider;
                CapsuleCollider newCollider = ComponentUtils.CopyComponent<CapsuleCollider>(baseCollider, gameObject);
                newCollider.height = baseCollider.height;
                newCollider.radius = baseCollider.radius;
                newCollider.center = baseCollider.center;
            }
            else if (collider is MeshCollider)
            {
                MeshCollider baseCollider = (MeshCollider)collider;
                MeshCollider newCollider = ComponentUtils.CopyComponent<MeshCollider>(baseCollider, gameObject);
                newCollider.sharedMesh = baseCollider.sharedMesh;
            }
        }

        public IEnumerator ChangeDisguise(GameObject disguise)
        {
            // Clear out old disguise
            while (disguiseBase.childCount > 0)
            {
                GameObject.Destroy(disguiseBase.GetChild(0).gameObject);
                yield return null;
            }

            // Setup player new disguise
            GameObject spawnedDisguise = GameObject.Instantiate(disguise, this.disguiseBase);
        }
    }
}