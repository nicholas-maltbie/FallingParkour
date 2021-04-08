using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.EditMode.Character
{
    /// <summary>
    /// Tests to verify behaviour of character animator script
    /// </summary>
    [TestFixture]
    public class CharacterAnimatorTests
    {
        /// <summary>Character animator component for this test </summary>
        CharacterAnimator characterAnimator;
        /// <summary>Character movement for reading character info for this test</summary>
        KinematicCharacterController kcc;
        /// <summary>Mock of network service attached to the player game object script</summary>
        Mock<INetworkService> networkServiceMock;
        /// <summary>Mock of camera controller attached to the player game object script</summary>
        CameraController cameraController;

        [SetUp]
        public void Setup()
        {
            // Setup character movement player
            GameObject characterGo = new GameObject();
            this.kcc = characterGo.AddComponent<KinematicCharacterController>();
            this.characterAnimator = characterGo.AddComponent<CharacterAnimator>();
            this.cameraController = characterGo.AddComponent<CameraController>();
            Animator anim = characterGo.AddComponent<Animator>();
            this.kcc.Start();
            this.networkServiceMock = new Mock<INetworkService>();
            this.characterAnimator.animator = anim;
            this.characterAnimator.kcc = kcc;
            this.characterAnimator.cameraController = cameraController;
            this.characterAnimator.Start();
            this.characterAnimator.networkService = this.networkServiceMock.Object;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.characterAnimator.gameObject);
        }

        [Test]
        public void CharacterMovementNotLocal()
        {
            this.characterAnimator.LateUpdate();
        }

        [Test]
        public void CharacterMovementNotGrounded()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            this.characterAnimator.LateUpdate();
        }
    }
}