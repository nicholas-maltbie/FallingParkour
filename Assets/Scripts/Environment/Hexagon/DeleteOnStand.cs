using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment.Hexagon
{
    public class DeleteOnStand : DetectPlayerStand
    {
        /// <summary>
        /// How many seconds until this object deletes
        /// </summary>
        public float deleteTime = 1.0f;

        /// <summary>
        /// is this cube being deleted
        /// </summary>
        public NetworkVariable<bool> deleting = new NetworkVariable<bool>(false);

        /// <summary>
        /// How long has this cube been fading
        /// </summary>
        private float deleteElapsed;

        /// <summary>
        /// Normal color 1 of hex when it's not being deleted
        /// </summary>
        public NetworkVariable<Color> normalColor1 = new NetworkVariable<Color>();

        /// <summary>
        /// Normal color 2 of hex when it's not being deleted
        /// </summary>
        public NetworkVariable<Color> normalColor2 = new NetworkVariable<Color>();

        /// <summary>
        /// COlor to fade color 1 towards when being deleted
        /// </summary>
        public NetworkVariable<Color> fadeColor1 = new NetworkVariable<Color>();

        /// <summary>
        /// COlor to fade color 2 towards when being deleted
        /// </summary>
        public NetworkVariable<Color> fadeColor2 = new NetworkVariable<Color>();

        /// <summary>
        /// Update current colors of the hex
        /// </summary>
        public void UpdateColor()
        {
            MaterialUtils.RecursiveSetColorProperty(
                gameObject,
                "_Background1",
                Color.Lerp(normalColor1.Value, fadeColor1.Value, deleting.Value ?
                    Mathf.Pow(deleteElapsed / deleteTime, 2) : 0));
            MaterialUtils.RecursiveSetColorProperty(
                gameObject,
                "_Background2",
                Color.Lerp(normalColor2.Value, fadeColor2.Value, deleting.Value ?
                    Mathf.Pow(deleteElapsed / deleteTime, 2) : 0));
        }

        public void Start()
        {
            UpdateColor();
        }

        public void Update()
        {
            if (deleting.Value)
            {
                deleteElapsed += Time.deltaTime;
                UpdateColor();
            }
            if (NetworkManager.Singleton.IsServer && deleteElapsed >= deleteTime)
            {
                GameObject.Destroy(gameObject);
            }
        }

        /// <summary>
        /// When a player steps onto this tile
        /// </summary>
        /// <param name="sender">Who stepped on this object</param>
        [ServerRpc(RequireOwnership = false)]
        public override void StepOnServerRpc()
        {
            deleting.Value = true;
        }

        /// <summary>
        /// When a player steps off of this tile
        /// </summary>
        /// <param name="sender">Who stepped on this object</param>
        [ServerRpc(RequireOwnership = false)]
        public override void StepOffServerRpc()
        {

        }
    }
}