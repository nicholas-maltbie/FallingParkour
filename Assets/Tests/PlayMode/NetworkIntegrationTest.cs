using Mirror;
using NUnit.Framework;
using System.Collections;
using Tests.PlayMode.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class NetworkIntegrationTest
    {
        NetworkManager manager;

        [UnitySetUp]
        public IEnumerator UnitySetup()
        {
            SceneManager.LoadScene("TestRoom", LoadSceneMode.Single);
            yield return new WaitForSceneLoaded("TestRoom", newTimeout: 10);
            yield return null;
            // Get Network Manager from scene
            manager = GameObject.FindObjectOfType<NetworkManager>();
            // Simulate starting the host
            manager.StartHost();

            yield return null;

            // Wait for connected
            yield return new WaitForConnected();

            // Assert that we are now connected
            Assert.IsTrue(NetworkClient.isConnected);
        }

        [UnityTearDown]
        public IEnumerator UnityTearDown()
        {
            // now disconnect, wait until we are connected
            manager.StopHost();

            // Wait until we disconnect
            yield return new WaitForConnected(state: false);

            // Assert that we are not connected
            Assert.IsFalse(NetworkClient.isConnected);
        }
    }
}
