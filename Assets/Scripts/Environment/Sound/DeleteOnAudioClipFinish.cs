using System.Collections;
using UnityEngine;

namespace PropHunt.Environment.Sound
{
    /// <summary>
    /// Delete an audio source when the given audio clip finishes
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class DeleteOnAudioClipFinish : MonoBehaviour
    {
        /// <summary>
        /// Source related to this audio clip
        /// </summary>
        protected AudioSource source;

        /// <summary>
        /// Amount of time to wait after the sound effect has finished before deleting this element
        /// </summary>
        public float clearTime = 1.0f;

        /// <summary>
        /// Has the delete co-routine been started?
        /// </summary>
        private bool cleared = false;

        public void Awake()
        {
            source = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Co-routine intended to delete this object after a given delay
        /// </summary>
        public IEnumerator DestorySelf()
        {
            yield return new WaitForSeconds(clearTime);
            GameObject.Destroy(gameObject);
            yield return null;
        }

        public void Update()
        {
            if (!source.isPlaying && !cleared)
            {
                cleared = true;
                StartCoroutine(DestorySelf());
            }
        }
    }
}