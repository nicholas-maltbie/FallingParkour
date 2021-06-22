using System.Linq;
using Mirror;
using Mirror.Tests.RemoteAttributeTest;
using Moq;
using NUnit.Framework;
using PropHunt.Animation;
using PropHunt.Character;
using PropHunt.Character.Footstep;
using PropHunt.Environment.Sound;
using PropHunt.Utils;
using Tests.EditMode.Environment.Sound;
using UnityEditor;
using UnityEngine;

namespace Tests.EditMode.Character.Footstep
{
    /// <summary>
    /// Tests to verify behaviour of player footstep sound tests
    /// </summary>
    [TestFixture]
    public class PlayerFootstepSoundsTests : SoundEffectManagerTestBase
    {

        Mock<INetworkService> networkServiceMock;
        Mock<IUnityService> unityServiceMock;
        PlayerFootstepSounds footstepSounds;
        KinematicCharacterController playerKcc;
        PlayerFootGrounded footGrounded;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            library.sounds = library.sounds.Concat(new LabeledSFX[]{
                new LabeledSFX
                {
                    soundMaterial = SoundMaterial.Concrete,
                    soundType = SoundType.Footstep,
                    audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/SFX/Footsteps/SFX Walk_Concrete_Shoes_Left.wav"),
                    soundId = "test-sound-floor"
                }
            }).ToArray();
            library.ClearLookups();
            library.SetupLookups();

            GameObject playerGo = new GameObject();
            playerKcc = playerGo.AddComponent<KinematicCharacterController>();
            footGrounded = playerGo.AddComponent<PlayerFootGrounded>();

            footstepSounds = playerGo.AddComponent<PlayerFootstepSounds>();
            footstepSounds.Awake();

            footstepSounds.footGrounded = footGrounded;

            footstepSounds.Start();

            networkServiceMock = new Mock<INetworkService>();
            unityServiceMock = new Mock<IUnityService>();

            networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);

            footstepSounds.networkService = networkServiceMock.Object;
            footstepSounds.unityService = unityServiceMock.Object;
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            GameObject.DestroyImmediate(footstepSounds.gameObject);
        }

        [Test]
        public void TestMakeCorrectSounds()
        {
            GameObject floor = new GameObject();
            // Test making a footstep sound
            this.footstepSounds.HandleFootstepEvent(footGrounded, new FootstepEvent(
                Vector3.zero, PlayerFoot.LeftFoot, FootstepState.Down, floor
            ));
            Assert.IsTrue(manager.UsedSources == 1);
            GameObject.DestroyImmediate(floor);
        }

        [Test]
        public void TestMakeCorrectSoundsServer()
        {
            GameObject floor = new GameObject();
            networkServiceMock.Setup(e => e.isServer).Returns(true);
            // Test making a footstep sound
            this.footstepSounds.HandleFootstepEvent(footGrounded, new FootstepEvent(
                Vector3.zero, PlayerFoot.LeftFoot, FootstepState.Down, floor
            ));
            Assert.IsTrue(manager.UsedSources == 1);
            GameObject.DestroyImmediate(floor);
        }

        [Test]
        public void TestDoNotMakeCorrectSounds()
        {
            GameObject floor = new GameObject();
            // Test making a footstep sound
            this.footstepSounds.HandleFootstepEvent(footGrounded, new FootstepEvent(
                Vector3.zero, PlayerFoot.LeftFoot, FootstepState.Up, floor
            ));
            Assert.IsTrue(manager.UsedSources == 0);
            GameObject.DestroyImmediate(floor);
        }

        [Test]
        public void TestUpdateMaximumTimeFootstepSounds()
        {
            GameObject floor = new GameObject();
            playerKcc.floor = floor;
            // Test making a footstep sound
            this.unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
            this.unityServiceMock.Setup(e => e.fixedDeltaTime).Returns(1.0f);
            this.playerKcc.inputMovement = new Vector3(0, 0, 1.0f);
            this.footstepSounds.unityService = this.unityServiceMock.Object;
            this.footstepSounds.networkService = this.networkServiceMock.Object;
            this.playerKcc.networkService = this.networkServiceMock.Object;
            this.playerKcc.unityService = this.unityServiceMock.Object;
            this.footstepSounds.Update();
            this.footstepSounds.maxFootstepSoundDelay = 1.5f;
            playerKcc.onGround = true;
            playerKcc.angle = 0.0f;
            playerKcc.distanceToGround = KinematicCharacterController.Epsilon;
            Assert.IsFalse(playerKcc.Falling);
            Assert.IsTrue(playerKcc.StandingOnGround);

            this.footstepSounds.Update();
            Assert.IsTrue(manager.UsedSources == 0);

            networkServiceMock.Setup(e => e.isLocalPlayer).Returns(false);
            this.footstepSounds.Update();
            Assert.IsTrue(manager.UsedSources == 0);

            networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            this.footstepSounds.Update();
            Assert.IsTrue(manager.UsedSources == 1);

            GameObject.DestroyImmediate(floor);
        }

        [Test]
        public void TestPlayAttachedSound()
        {
            AttachedFootstepSound attached = footstepSounds.gameObject.AddComponent<AttachedFootstepSound>();
            attached.audioSource = footstepSounds.gameObject.AddComponent<AudioSource>();

            attached.PlayFootstepSound(new SoundEffectEvent
            {
                volume = 1.0f,
                pitchValue = 1.0f,
                sfxId = SoundEffectManager.Instance.soundEffectLibrary.GetSFXClipBySoundType(SoundType.Footstep).soundId,
            });
            Assert.IsTrue(attached.audioSource.isPlaying);
        }

        [Test]
        public void TestHitGroundMiniumTimeFootstepSounds()
        {
            GameObject floor = new GameObject();
            playerKcc.floor = floor;
            // Test making a footstep sound
            this.footstepSounds.Update();
            unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
            this.footstepSounds.maxFootstepSoundDelay = 1.5f;
            playerKcc.onGround = false;

            // Test making a footstep sound
            this.footstepSounds.HandleFootstepEvent(footGrounded, new FootstepEvent(
                Vector3.zero, PlayerFoot.LeftFoot, FootstepState.Down, floor
            ));
            Assert.IsTrue(manager.UsedSources == 1);

            // Test making a footstep sound
            this.footstepSounds.HandleFootstepEvent(footGrounded, new FootstepEvent(
                Vector3.zero, PlayerFoot.LeftFoot, FootstepState.Down, floor
            ));
            Assert.IsTrue(manager.UsedSources == 1);

            unityServiceMock.Setup(e => e.time).Returns(footstepSounds.minFootstepSoundDelay * 2);

            // Test making a footstep sound
            this.footstepSounds.HandleFootstepEvent(footGrounded, new FootstepEvent(
                Vector3.zero, PlayerFoot.LeftFoot, FootstepState.Down, floor
            ));
            Assert.IsTrue(manager.UsedSources == 2);

            GameObject.DestroyImmediate(floor);
        }
    }

    [TestFixture]
    public class PlayerFootstepCommandValidation : RemoteTestBase
    {
        protected GameObject soundEffectPrefab;
        protected SoundEffectLibrary library;
        protected SoundEffectManager manager;
        Mock<INetworkService> networkServiceMock = new Mock<INetworkService>();
        PlayerFootstepSounds footstepSounds;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            SoundEffectManagerTestBase.SetupSoundEffectManager(out this.soundEffectPrefab, out this.library, out this.manager);
            footstepSounds = this.CreateHostObject<PlayerFootstepSounds>(true);
            footstepSounds.footGrounded = footstepSounds.gameObject.AddComponent<PlayerFootGrounded>();
            footstepSounds.Start();
            footstepSounds.networkService = networkServiceMock.Object;
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            GameObject.DestroyImmediate(manager.gameObject);
            GameObject.DestroyImmediate(soundEffectPrefab);
            ScriptableObject.DestroyImmediate(library);
        }

        [Test]
        public void VerifyPlayerFootstepCommands()
        {
            footstepSounds.CmdCreateFootstepSound(new SoundEffectEvent
            {
                sfxId = library.sounds[0].soundId
            });
            // Need to use this command to complete the [ClientRpc] methods
            ProcessMessages();
            GameObject.DestroyImmediate(footstepSounds);
        }

        [Test]
        public void VerifyPlayerFootstepCommandsNotLocal()
        {
            networkServiceMock.Setup(n => n.isLocalPlayer).Returns(true);
            footstepSounds.CmdCreateFootstepSound(new SoundEffectEvent
            {
                sfxId = library.sounds[0].soundId
            });
            // Need to use this command to complete the [ClientRpc] methods
            ProcessMessages();
            GameObject.DestroyImmediate(footstepSounds);
        }
    }
}