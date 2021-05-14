using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using PropHunt.Environment.Sound;
using Tests.EditMode.Game.Flow;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.TestTools;

namespace Tests.EditMode.Environment.Sound
{
    public class SoundEffectManagerTestBase : CustomNetworkManagerTestBase
    {
        protected GameObject soundEffectPrefab;
        protected SoundEffectLibrary library;
        protected SoundEffectManager manager;

        [SetUp]
        public override void SetUp()
        {
            // Load up an empty scene
#if UNITY_EDITOR
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
#endif
            base.SetUp();
            // Clear out all audio sources
            foreach (AudioSource source in GameObject.FindObjectsOfType<AudioSource>())
            {
                GameObject.DestroyImmediate(source);
            }
            SetupSoundEffectManager(out this.soundEffectPrefab, out this.library, out this.manager);
        }

        public static void SetupSoundEffectManager(out GameObject soundEffectPrefab,
            out SoundEffectLibrary library, out SoundEffectManager manager)
        {
            soundEffectPrefab = new GameObject();
            soundEffectPrefab.AddComponent<AudioSource>();

            AudioMixer mixer = (AudioMixer)AssetDatabase.LoadAssetAtPath("Assets/Sound/Settings/AudioMixer.mixer", typeof(AudioMixer));

            library = ScriptableObject.CreateInstance<SoundEffectLibrary>();
            LabeledSFX glassHit = new LabeledSFX
            {
                soundMaterial = SoundMaterial.Glass,
                soundType = SoundType.Hit,
                audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/SFX/Hits/glass-hit-1.wav"),
                soundId = "testSound1",
            };
            library.sounds = new LabeledSFX[] { glassHit };
            GameObject go = new GameObject();
            manager = go.AddComponent<SoundEffectManager>();
            manager.soundEffectLibrary = library;
            manager.soundEffectPrefab = soundEffectPrefab;
            manager.audioMixer = mixer;
            manager.Awake();
            Assert.IsTrue(manager.AvailableSources == manager.maxSFXSources);
        }

        public override void TearDown()
        {
            base.TearDown();
            manager.OnDestroy();
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
                if (source.gameObject != soundEffectPrefab && source.gameObject.activeInHierarchy)
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
            // check there was no created game object
            Assert.IsTrue(manager.UsedSources == 0);
            SoundEffectManager.CreateNetworkedSoundEffectAtPoint(
                new SoundEffectEvent()
            );
            // check there was no created game object
            Assert.IsTrue(manager.UsedSources == 0);
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
            Assert.IsTrue(manager.UsedSources == 1);
            GameObject.DestroyImmediate(created2);
        }

        [Test]
        public void TestPlaySFXEnumerator()
        {
            AudioSource source = SoundEffectManager.CreateSoundEffectAtPoint(Vector3.zero, library.sounds[0].audioClip).GetComponent<AudioSource>();
            IEnumerator enumerator = SoundEffectManager.PlaySFX(source, SoundEffectManager.defaultAudioMixerGroup);
            while (enumerator.MoveNext()) { }
            Assert.IsTrue(source.isPlaying);
        }

        [Test]
        public void TestFillQueue()
        {
            GameObject[] created = new GameObject[manager.maxSFXSources];
            for (int i = 0; i < manager.maxSFXSources; i++)
            {
                created[i] = SoundEffectManager.CreateSoundEffectAtPoint(Vector3.zero, library.sounds[0].audioClip);
            }
            GameObject fullQueue = SoundEffectManager.CreateSoundEffectAtPoint(Vector3.zero, library.sounds[0].audioClip);
            Assert.IsTrue(fullQueue == null);

            GameObject.DestroyImmediate(fullQueue);
            for (int i = 0; i < manager.maxSFXSources; i++)
            {
                GameObject.DestroyImmediate(created[i]);
            }
        }

        [UnityTest]
        public IEnumerator TestCreateSoundFromEffect()
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
            Assert.IsTrue(manager.UsedSources == 1);
            List<AudioSource> createdSources = GetNonPrefabSources();
            int maxLoops = 100;
            int iters = 0;
            while (GetNonPrefabSources().Count == 0)
            {
                yield return null;
                createdSources = GetNonPrefabSources();
                iters++;
                Assert.IsTrue(iters <= maxLoops);
            }
            GameObject created = createdSources[0].gameObject;
            // Assert that object was created with correct settings
            AudioSource createdSound = created.GetComponent<AudioSource>();
            Assert.IsTrue(createdSound.pitch == 0.5f);
            Assert.IsTrue(createdSound.volume == 0.8f);
            Assert.IsTrue(created.transform.position == new Vector3(1, 5, 1));

            GameObject.DestroyImmediate(created);
        }
    }
}