using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Tests;
using NUnit.Framework;
using PropHunt.Environment.Sound;
using PropHunt.Game.Flow;
using Tests.EditMode.Game.Flow;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Tests.EditMode.Environment.Sound
{
    public class SoundEffectManagerTestBase : CustomNetworkManagerTest
    {
        protected GameObject soundEffectPrefab;
        protected SoundEffectLibrary library;
        protected SoundEffectManager manager;

        [SetUp]
        public override void Setup()
        {
            // Load up an empty scene
#if UNITY_EDITOR
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
#endif
            base.Setup();
            // Clear out all audio sources
            foreach (AudioSource source in GameObject.FindObjectsOfType<AudioSource>())
            {
                GameObject.DestroyImmediate(source);
            }
            soundEffectPrefab = new GameObject();
            soundEffectPrefab.AddComponent<AudioSource>();

            AudioMixer mixer = (AudioMixer)AssetDatabase.LoadAssetAtPath("Assets/Sound/Settings/AudioMixer.mixer", typeof(AudioMixer));

            library = ScriptableObject.CreateInstance<SoundEffectLibrary>();
            LabeledSFX glassHit = new LabeledSFX
            {
                soundMaterial = SoundMaterial.Glass,
                soundType = SoundType.Hit,
                audioClip = null,
                soundId = "testSound1",
            };
            library.sounds = new LabeledSFX[] { glassHit };
            GameObject go = new GameObject();
            manager = go.AddComponent<SoundEffectManager>();
            manager.soundEffectLibrary = library;
            manager.soundEffectPrefab = soundEffectPrefab;
            manager.audioMixer = mixer;
            manager.Awake();
        }

        public override void TearDown()
        {
            base.TearDown();
            GameObject.DestroyImmediate(manager.gameObject);
            GameObject.DestroyImmediate(soundEffectPrefab);
            ScriptableObject.DestroyImmediate(library);
        }

        public List<AudioSource> GetNonPrefabSources()
        {
            AudioSource[] sources = GameObject.FindObjectsOfType<AudioSource>();
            List<AudioSource> filtered = new List<AudioSource>();
            foreach (AudioSource source in sources)
            {
                if (source.gameObject != soundEffectPrefab)
                {
                    filtered.Add(source);
                }
            }
            return filtered;
        }

    }

    [TestFixture]
    public class SoundEffectManagerTests : SoundEffectManagerTestBase
    {
        [Test]
        public void TestDoNotCreateNewtorkedSound()
        {
            SoundEffectManager.Instance = null;
            SoundEffectManager.CreateNetworkedSoundEffectAtPoint(
                Vector3.zero, SoundMaterial.Glass, SoundType.Hit
            );
            GameObject created = null;
            AudioSource[] sources = GameObject.FindObjectsOfType<AudioSource>();
            foreach (AudioSource source in sources)
            {
                if (source.gameObject != soundEffectPrefab)
                {
                    created = source.gameObject;
                }
            }
            // check there was no created game object
            List<AudioSource> createdSources = GetNonPrefabSources();
            Assert.IsTrue(createdSources.Count == 0);
        }

        [Test]
        public void TestCreateNewtorkedSound()
        {
            SoundEffectManager.CreateNetworkedSoundEffectAtPoint(
                Vector3.zero, SoundMaterial.Glass, SoundType.Hit
            );
        }

        [Test]
        public void TestCreateSoundFromPoint()
        {
            // Test creating via other methodology
            GameObject created2 = SoundEffectManager.CreateSoundEffectAtPoint(Vector3.zero,
                SoundMaterial.Glass, SoundType.Hit);
            GameObject.DestroyImmediate(created2);
        }

        [Test]
        public void TestCreateSoundFromEffect()
        {
            // Test creating via sound effect event
            SoundEffectManager.CreateSoundEffectAtPoint(new SoundEffectEvent
            {
                sfxId = "testSound1",
                point = new Vector3(1, 5, 1),
                pitchValue = 0.5f,
                volume = 0.8f,
            });
            // Find the created game object
            List<AudioSource> createdSources = GetNonPrefabSources();
            Assert.IsTrue(createdSources.Count > 0);
            GameObject created = createdSources[0].gameObject;
            // Assert that object was created with correct settings
            AudioSource createdSound = created.GetComponent<AudioSource>();
            Assert.IsTrue(createdSound.pitch == 0.5f);
            Assert.IsTrue(createdSound.volume == 0.8f);
            Assert.IsTrue(created.transform.position == new Vector3(1, 5, 1));

            // Ensure that the delayed start works as expected
            Assert.IsFalse(createdSound.isPlaying);
            IEnumerator eventEnum = SoundEffectManager.DelayedStartAudioClip(createdSound);
            while (eventEnum.MoveNext()) { }
            GameObject.DestroyImmediate(created);
        }
    }
}