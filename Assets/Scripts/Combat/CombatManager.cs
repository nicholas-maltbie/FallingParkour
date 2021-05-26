
using System;
using Mirror;
using PropHunt.Character;
using PropHunt.Game.Communication;
using UnityEngine;

namespace PropHunt.Combat
{
    /// <summary>
    /// Events that describe a player attack
    /// </summary>
    public class PlayerAttackEvent : EventArgs
    {
        /// <summary>
        /// Player attacking (source is attacking target)
        /// </summary>
        public GameObject source;
        /// <summary>
        /// Target of the attack
        /// </summary>
        public GameObject target;
    }

    /// <summary>
    /// Static combat manager class for managing various combat events and actions that may occur during a game
    /// </summary>
    public static class CombatManager
    {
        /// <summary>
        /// Event that is invoked whenever a player is attacked by another player.
        /// Normally this should only be invoked on the server as attack actions, health, and various
        /// attrbitues should be handled by the server.
        /// </summary>
        public static event EventHandler<PlayerAttackEvent> OnPlayerAttack;

        /// <summary>
        /// Invoke when one player successfully attacks another player
        /// </summary>
        /// <param name="source">PLayer starting the attack</param>
        /// <param name="target">Player being attacked</param>
        public static void Attack(GameObject source, GameObject target)
        {
            PlayerAttackEvent attackEvent = new PlayerAttackEvent();
            attackEvent.source = source;
            attackEvent.target = target;

            OnPlayerAttack?.Invoke(source, attackEvent);
        }
    }
}