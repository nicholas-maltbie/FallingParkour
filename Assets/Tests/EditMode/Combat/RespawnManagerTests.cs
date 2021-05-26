
using System.Linq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Combat;
using PropHunt.Environment.Sound;
using Tests.EditMode.Environment.Sound;
using UnityEditor;
using UnityEngine;

namespace Tests.EditMode.Combat
{
    /// <summary>
    /// Tests to verify behaviour of respawn manager
    /// </summary>
    [TestFixture]
    public class RespawnManagerTests : SoundEffectManagerTestBase
    {
        [Test]
        public void RespawnPlayerOnAttacked()
        {
            // Setup a basic death sound effect
            library.sounds = library.sounds.Concat(new LabeledSFX[]{
                new LabeledSFX
                {
                    soundMaterial = SoundMaterial.Misc,
                    soundType = SoundType.Death,
                    audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/SFX/Hits/glass-hit-1.wav"),
                    soundId = "testSound1",
                }
            }).ToArray();
            library.ClearLookups();
            library.SetupLookups();

            // Setup respawn manager
            PlayerRespawnManager respawnManager = new GameObject().AddComponent<PlayerRespawnManager>();
            respawnManager.OnStartServer();

            // Create a test player attacker and attacked
            GameObject player = new GameObject();
            player.AddComponent<CharacterName>();
            // Handle a player attack and their respawn
            PlayerAttackEvent attackEvent = new PlayerAttackEvent();
            attackEvent.source = player;
            attackEvent.target = player;

            respawnManager.HandlePlayerAttack(attackEvent.source, attackEvent);

            // Cleanup from test
            respawnManager.OnDisable();
            respawnManager.OnDestroy();
            GameObject.DestroyImmediate(respawnManager.gameObject);
        }
    }
}