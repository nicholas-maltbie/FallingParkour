using PropHunt.Environment;
using PropHunt.Environment.Sound;
using UnityEngine;

namespace PropHunt.Prop
{
    /// <summary>
    /// Prop for saving transformation data when players want to 
    /// hide and transform into a given item.
    /// </summary>
    public class Prop : Interactable
    {
        /// <summary>
        /// Name of the prop, one prop will be saved for each unique name
        /// so multiple props can have the same name if they are the same object
        /// </summary>
        [Tooltip("Prop name for saving and loading the disguise information")]
        public string propName;

        /// <summary>
        /// Visual prefab to load when player transforms into a prop
        /// </summary>
        [Tooltip("Visual prefab to load when player transforms into a prop")]
        public GameObject disguiseVisual;

        /// <summary>
        /// Collider associated with this prop
        /// </summary>
        [Tooltip("Collider associated with this prop")]
        public Collider disguiseCollider;

        /// <summary>
        /// Offset of camera from base player model
        /// </summary>
        [Tooltip("Camera offset from player center")]
        public Vector3 cameraOffset;

        /// <summary>
        /// Grab a transformation sound with this material when
        /// a player transforms into a prop
        /// </summary>
        [Tooltip("SoundMaterial when playing prop transformation sound")]
        [SerializeField]
        private SoundMaterial transformationMaterialSound = SoundMaterial.Misc;

        /// <summary>
        /// Get the sound material for when this player transforms into 
        /// a prop
        /// </summary>
        public SoundMaterial GetTransformationSoundMaterial => transformationMaterialSound;

        public void Start()
        {
            PropDatabase.AddDisguiseIfNonExists(propName,
                new Disguise
                {
                    disguiseVisual = disguiseVisual,
                    disguiseCollider = disguiseCollider,
                    cameraOffset = cameraOffset
                }
            );
        }

        public override void Interact(GameObject source)
        {
            PropDisguise disguise = source.GetComponent<PropDisguise>();
            if (disguise != null)
            {
                disguise.SetSelectedDisguise(gameObject);
            }
        }
    }
}