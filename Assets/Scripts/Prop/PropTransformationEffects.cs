using PropHunt.Environment.Sound;
using UnityEngine;

namespace PropHunt.Prop
{
    /// <summary>
    /// Play sound effects whenever a prop transforms into a given object
    /// </summary>
    public class PropTransformationEffects : MonoBehaviour
    {
        /// <summary>
        /// Sound effect volume
        /// </summary>
        [Tooltip("Volume modifier for created sound effects")]
        [SerializeField]
        [Range(0, 1)]
        private float sfxVolume = 1.0f;

        /// <summary>
        /// Minimum pitch range for created prop transformation sound effects
        /// </summary>
        [Tooltip("Minimum pitch value for created sound effects")]
        [SerializeField]
        private float minPitch = 0.75f;

        /// <summary>
        /// Maximum pitch range for created prop transformation sound effects
        /// </summary>
        [Tooltip("Maximum pitch value for created sound effects")]
        [SerializeField]
        private float maxPitch = 1.25f;

        public void Start()
        {
            PropDisguise.OnChangeDisguise += HandlePropDisguiseChange;
        }

        public void OnDestroy()
        {
            PropDisguise.OnChangeDisguise -= HandlePropDisguiseChange;
        }

        public void HandlePropDisguiseChange(object sender, ChangeDisguiseEvent changeDisguise)
        {
            string clipId = SoundEffectManager.Instance.soundEffectLibrary.HasSoundEffect(changeDisguise.transformationSoundMaterial, SoundType.PropTransformation) ?
                SoundEffectManager.Instance.soundEffectLibrary.GetSFXClipBySoundMaterialAndType(
                    changeDisguise.transformationSoundMaterial, SoundType.PropTransformation).soundId :
                SoundEffectManager.Instance.soundEffectLibrary.GetSFXClipBySoundMaterialAndType(SoundMaterial.Misc, SoundType.PropTransformation).soundId;
            GameObject player = changeDisguise.player;
            SoundEffectManager.CreateNetworkedSoundEffectAtPoint(new SoundEffectEvent
            {
                sfxId = clipId,
                volume = sfxVolume,
                pitchValue = Random.Range(minPitch, maxPitch),
                point = player.transform.position
            });
        }
    }
}