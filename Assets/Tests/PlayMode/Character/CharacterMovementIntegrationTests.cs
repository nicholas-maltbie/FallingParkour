using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode.Character
{
    public class CharacterMovementIntegrationTests : NetworkIntegrationTest
    {
        [UnityTest]
        public IEnumerator TestPlayerMoveForward()
        {
            KinematicCharacterController characterMovement = GameObject.FindObjectOfType<KinematicCharacterController>();
            Vector3 forward = characterMovement.transform.forward;
            Vector3 start = characterMovement.transform.position;

            // Simulate the movement of the character
            Mock<IUnityService> unityServiceMock = new Mock<IUnityService>();
            unityServiceMock.Setup(e => e.GetAxis("Vertical")).Returns(1.0f);
            unityServiceMock.Setup(e => e.deltaTime).Returns(() => Time.deltaTime);
            characterMovement.unityService = unityServiceMock.Object;

            // Move the character forward for 1 seconds
            yield return new WaitForSeconds(1.0f);

            // Assert that there is some positive movement along the forward axis
            Vector3 movement = characterMovement.transform.position - start;
            Assert.IsTrue(Vector3.Project(movement, forward).magnitude > 0);

            // Move the character forward for 3 more seconds (have it hit the teleport box)
            yield return new WaitForSeconds(3.0f);
        }
    }
}
