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
        /// <summary>Character controller for character movement management</summary>
        CharacterController characterController;
        /// <summary>Character movement for reading character info for this test</summary>
        CharacterMovement characterMovement;
        /// <summary>Mock of network service attached to the player game object script</summary>
        Mock<INetworkService> networkServiceMock;

        [SetUp]
        public void Setup()
        {
            // Setup character movement player
            GameObject characterGo = new GameObject();
            this.characterController = characterGo.AddComponent<CharacterController>();
            this.characterMovement = characterGo.AddComponent<CharacterMovement>();
            this.characterAnimator = characterGo.AddComponent<CharacterAnimator>();
            Animator anim = characterGo.AddComponent<Animator>();
            this.characterMovement.Start();
            this.networkServiceMock = new Mock<INetworkService>();
            this.characterAnimator.animator = anim;
            this.characterAnimator.movement = characterMovement;
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