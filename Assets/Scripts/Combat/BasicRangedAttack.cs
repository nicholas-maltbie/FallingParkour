using Mirror;
using PropHunt.Character;
using PropHunt.Environment.Sound;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropHunt.Combat
{
    /// <summary>
    /// Basic ranged attack for a character
    /// </summary>
    [RequireComponent(typeof(CameraController))]
    public class BasicRangedAttack : NetworkBehaviour
    {
        /// <summary>
        /// Cooldown between character attacks
        /// </summary>
        [Tooltip("Time in seconds between character attacks")]
        [SerializeField]
        private float attackCooldown = 0.1f;

        /// <summary>
        /// Layer mask for player aim and what the player can hit. By default this will
        /// collide with all layers.
        /// </summary>
        [Tooltip("What layers should the player aim intersect with when detecting aim")]
        [SerializeField]
        private LayerMask raycastMask = ~0;

        /// <summary>
        /// Radius of attack made by player
        /// </summary>
        [Tooltip("Radius of attack when casting sphere in front of player")]
        [SerializeField]
        private float attackRadius = 0.05f;

        /// <summary>
        /// The team this player can make ranged attacks upon
        /// </summary>
        [Tooltip("Which team this player can shoot at")]
        [SerializeField]
        private Team targetTeam = Team.Prop;

        /// <summary>
        /// Time of previous character attack
        /// </summary>
        private float previousAttack = Mathf.NegativeInfinity;

        /// <summary>
        /// Unity service for getting time and input commands
        /// </summary>
        public IUnityService unityService = new UnityService();

        /// <summary>
        /// Network service for checking network state
        /// </summary>
        public INetworkService networkService;

        /// <summary>
        /// Camera controller for getting player focus and aim
        /// </summary>
        private CameraController cameraController;

        /// <summary>
        /// Can the player attack right now? This is based off the previous attack
        /// cooldown and the current time
        /// </summary>
        /// <returns>True if the player can attack, false otherwise</returns>
        public bool CanAttack => (unityService.time - previousAttack) >= attackCooldown;

        /// <summary>
        /// Is the player attacking this frame
        /// </summary>
        public bool attacking;

        /// <summary>
        /// Tell the player to attempt to attack this frame
        /// </summary>
        public void Attack(InputAction.CallbackContext context)
        {
            attacking = context.ReadValueAsButton();
        }

        public void Start()
        {
            networkService = new NetworkService(this);
            this.cameraController = GetComponent<CameraController>();
        }

        /// <summary>
        /// Gets target of what the player is currently looking at. Will use the raycast 
        /// </summary>
        /// <returns></returns>
        public bool GetTarget(out RaycastHit hit) =>
            cameraController.SpherecastFromCameraBase(Mathf.Infinity, raycastMask, attackRadius, QueryTriggerInteraction.Ignore, out hit);

        /// <summary>
        /// Command to attack a given game object. Hit object should be computed client side
        /// as the lag between client and server could lead to misunderstood errors.
        /// A future check could be added to ensure that the player is not shooting through
        /// walls or around corners to discourage errors or cheating.
        /// </summary>
        /// <param name="hitObject">Object that the player is attacking</param>
        [Server]
        private void Attack(GameObject hitObject)
        {
            previousAttack = unityService.time;
            // Play an attack sound
            SoundEffectManager.CreateNetworkedSoundEffectAtPoint(transform.position, SoundMaterial.Misc, SoundType.Attack);
            // null object means the player fired and missed
            if (hitObject != null)
            {
                CombatManager.Attack(gameObject, hitObject);
            }
        }

        public void Update()
        {
            // Assert that the player has a target
            if (Character.PlayerInputManager.playerMovementState == PlayerInputState.Allow &&
                networkService.isLocalPlayer && attacking && CanAttack)
            {
                GetTarget(out RaycastHit hit);
                GameObject target = null;
                // Check if the target has a player team component
                // If the player team is the target team, then attack the player
                PlayerTeam team = null;
                if (hit.collider != null &&
                    (team = hit.collider.gameObject.GetComponent<PlayerTeam>()) != null &&
                    team.playerTeam == targetTeam)
                {
                    target = hit.collider.gameObject;
                }

                // If the player can attack
                if (networkService.isServer)
                {
                    Attack(target);
                }
                else
                {
                    previousAttack = unityService.time;
                    CmdAttackAction(target);
                }

                attacking = false;
            }
        }

        [Command]
        public void CmdAttackAction(GameObject hitObject)
        {
            Attack(hitObject);
        }
    }
}
