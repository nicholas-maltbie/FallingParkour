using NUnit.Framework;
using PropHunt.Game.Flow;
using PropHunt.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.UI
{
    [TestFixture]
    public class GameManagerButtonTests : CustomNetworkManager
    {
        [Test]
        public void GamePhaseButtonTests()
        {
            GameObject testObj = new GameObject();
            ChangeGamePhaseButton buttonPhase = testObj.AddComponent<ChangeGamePhaseButton>();
            GameManager.playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Tests/EditMode/TestPlayer.prefab");
            // Test exiting lobby while in or not in lobby phase
            GameManager.ChangePhase(GamePhase.Lobby);
            LogAssert.Expect(LogType.Error, "ServerChangeScene empty scene name");
            buttonPhase.ExitLobby();
            GameManager.ChangePhase(GamePhase.Setup);
            buttonPhase.ExitLobby();
            // Test exiting while in and not in game
            GameManager.ChangePhase(GamePhase.InGame);
            buttonPhase.ExitGame();
            GameManager.ChangePhase(GamePhase.Lobby);
            buttonPhase.ExitGame();
            // Test exiting while in and not in score
            GameManager.ChangePhase(GamePhase.Score);
            LogAssert.Expect(LogType.Error, "ServerChangeScene empty scene name");
            LogAssert.Expect(LogType.Error, "ServerChangeScene empty scene name");
            buttonPhase.ExitScore();
            GameManager.ChangePhase(GamePhase.Lobby);
            buttonPhase.ExitScore();

            GameObject.DestroyImmediate(testObj);
        }
    }
}