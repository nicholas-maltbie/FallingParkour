
using Mirror;
using Mirror.Tests.RemoteAttributeTest;
using Moq;
using NUnit.Framework;
using PropHunt.Animation;
using PropHunt.Utils;

namespace Tests.EditMode.Animation
{
    /// <summary>
    /// Tests to verify behaviour of commands in character push script
    /// </summary>
    public class NetworkIKControlTests : RemoteTestBase
    {
        [Test]
        public void TestSenderConnectionIsSetWhenCommandReceived()
        {
            NetworkIKControl networkIKControl = CreateHostObject<NetworkIKControl>(true);
            IKControl ikControl = networkIKControl.gameObject.AddComponent<IKControl>();
            Mock<INetworkService> networkServiceMock = new Mock<INetworkService>();
            ikControl.Start();
            networkIKControl.controller = ikControl;
            networkIKControl.Awake();
            networkIKControl.networkService = networkServiceMock.Object;

            NetworkConnectionToClient connectionToClient = NetworkServer.connections[0];
            UnityEngine.Debug.Assert(connectionToClient != null, $"connectionToClient was null, This means that the test is broken and will give the wrong results");

            // Test enable the settings
            networkServiceMock.Setup(e => e.isServer).Returns(false);
            networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            networkIKControl.SetLookState(true);
            networkIKControl.SetLookWeight(1.0f);
            networkIKControl.SetIKHintState(UnityEngine.AvatarIKHint.LeftElbow, true);
            networkIKControl.SetIKHintWeight(UnityEngine.AvatarIKHint.LeftElbow, 1.0f);
            networkIKControl.SetIKGoalState(UnityEngine.AvatarIKGoal.LeftHand, true);
            networkIKControl.SetIKGoalWeight(UnityEngine.AvatarIKGoal.LeftHand, 1.0f);
            Assert.IsTrue(networkIKControl.lookState == true);
            Assert.IsTrue(networkIKControl.lookWeight == 1.0f);
            Assert.IsTrue(ikControl.leftElbowTarget != null);
            Assert.IsTrue(ikControl.leftElbowWeight == 1.0f);
            Assert.IsTrue(ikControl.leftHandTarget != null);
            Assert.IsTrue(ikControl.leftHandWeight == 1.0f);

            // Test disable the settings
            networkServiceMock.Setup(e => e.isServer).Returns(true);
            networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            networkIKControl.SetLookState(false);
            networkIKControl.SetLookWeight(0.0f);
            networkIKControl.SetIKHintState(UnityEngine.AvatarIKHint.LeftElbow, false);
            networkIKControl.SetIKHintWeight(UnityEngine.AvatarIKHint.LeftElbow, 0.0f);
            networkIKControl.SetIKGoalState(UnityEngine.AvatarIKGoal.LeftHand, false);
            networkIKControl.SetIKGoalWeight(UnityEngine.AvatarIKGoal.LeftHand, 0.0f);
            Assert.IsTrue(networkIKControl.lookState == false);
            Assert.IsTrue(networkIKControl.lookWeight == 0.0f);
            Assert.IsTrue(ikControl.leftElbowTarget == null);
            Assert.IsTrue(ikControl.leftElbowWeight == 0.0f);
            Assert.IsTrue(ikControl.leftHandTarget == null);
            Assert.IsTrue(ikControl.leftHandWeight == 0.0f);

            // Verify the dictionary operations
            //   Test simulating removal and addition of objects to dictionary
            networkIKControl.avatarIKGoalStates.Clear();
            networkIKControl.avatarIKGoalWeights.Clear();
            networkIKControl.avatarIKHintStates.Clear();
            networkIKControl.avatarIKHintWeights.Clear();

            // Test the NetworkIKControl as well
            NetworkIKControlTest control = networkIKControl.gameObject.AddComponent<NetworkIKControlTest>();
            networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            control.Awake();
            control.networkService = networkServiceMock.Object;
            control.rightFootState = true;
            control.Update();
        }
    }
}