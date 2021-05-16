using Mirror.Tests.RemoteAttributeTest;
using Moq;
using NUnit.Framework;
using PropHunt.Animation;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.EditMode.Animation
{
    /// <summary>
    /// Tests to verify network foot state
    /// </summary>
    public class NetworkFootGroundingTests : RemoteTestBase
    {
        [Test]
        public void TestSenderConnectionIsSetWhenCommandReceived()
        {
            NetworkFootGrounding networkFootGrounding = CreateHostObject<NetworkFootGrounding>(true);
            NetworkIKControl networkControl = networkFootGrounding.gameObject.GetComponent<NetworkIKControl>();
            GameObject playerObj = new GameObject();
            PlayerFootGrounded footGrounded = playerObj.AddComponent<PlayerFootGrounded>();
            IKControl ikControl = playerObj.AddComponent<IKControl>();
            networkFootGrounding.playerFootGrounded = footGrounded;
            networkControl.controller = ikControl;
            networkControl.Awake();

            Mock<INetworkService> networkServiceMock = new Mock<INetworkService>();

            networkFootGrounding.Awake();
            networkFootGrounding.networkService = networkServiceMock.Object;
            networkControl.networkService = networkServiceMock.Object;

            networkServiceMock.Setup(e => e.isServer).Returns(true);
            networkFootGrounding.Start();
            ProcessMessages();

            networkServiceMock.Setup(e => e.isServer).Returns(false);
            networkFootGrounding.SetFootGroundedState(true);
            ProcessMessages();

            GameObject.DestroyImmediate(playerObj);
        }
    }
}