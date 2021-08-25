
using Mirror;
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
        /// How many seconds after delete time will it actually be deleted
        /// </summary>
        public float extraTime = 0.1f;

        /// <summary>
        /// Percentage instant color change when this starts deleting 
        /// </summary>
        [Range(0, 1)]
        public float extraStep = 0.1f;

        /// <summary>
        /// is this cube being deleted
        /// </summary>
        [SyncVar]
        public bool deleting = false;

        /// <summary>
        /// How long has this cube been fading
        /// </summary>
        [SyncVar]
        private float deleteElapsed = 0.0f;
        
        /// <summary>
        /// Normal color 1 of hex when it's not being deleted
        /// </summary>
        [SyncVar]
        public Color normalColor1;

        /// <summary>
        /// Normal color 2 of hex when it's not being deleted
        /// </summary>
        [SyncVar]
        public Color normalColor2;
        
        /// <summary>
        /// COlor to fade color 1 towards when being deleted
        /// </summary>
        [SyncVar]
        public Color fadeColor1;

        /// <summary>
        /// COlor to fade color 2 towards when being deleted
        /// </summary>
        [SyncVar]
        public Color fadeColor2;

        /// <summary>
        /// Update current colors of the hex
        /// </summary>
        public void UpdateColor()
        {
            MaterialUtils.RecursiveSetColorProperty(
                gameObject,
                "_Background1",
                Color.Lerp(normalColor1, fadeColor1, deleteElapsed / deleteTime + extraStep));
            MaterialUtils.RecursiveSetColorProperty(
                gameObject,
                "_Background2",
                Color.Lerp(normalColor2, fadeColor2, deleteElapsed / deleteTime + extraStep));
        }

        public void Start()
        {
            UpdateColor();
        }

        public void Update()
        {
            if (deleting)
            {
                deleteElapsed += Time.deltaTime;
                UpdateColor();
            }
            if (isServer && deleteElapsed >= (deleteTime + extraTime))
            {
                NetworkServer.Destroy(gameObject);
            }
        }

        /// <summary>
        /// When a player steps onto this tile
        /// </summary>
        /// <param name="sender">Who stepped on this object</param>
        [Command(requiresAuthority = false)]
        public override void CmdStepOn(NetworkConnectionToClient sender = null)
        {
            deleting = true;
        }

        /// <summary>
        /// When a player steps off of this tile
        /// </summary>
        /// <param name="sender">Who stepped on this object</param>
        [Command(requiresAuthority = false)]
        public override void CmdStepOff(NetworkConnectionToClient sender = null)
        {

        }
    }
}