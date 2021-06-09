using PropHunt.Game.Flow;
using UnityEngine;

namespace PropHunt.UI.Actions
{
    public class ChangeGamePhaseButton : MonoBehaviour
    {
        public void ExitLobby()
        {
            // If host and current phase is lobby, go to next phase
            if (GameManager.gamePhase == GamePhase.Lobby)
            {
                GameManager.ChangePhase(GamePhase.Setup);
            }
            else
            {
                UnityEngine.Debug.Log("Not in lobby now");
            }
        }

        public void ExitGame()
        {
            // If host and current phase is lobby, go to next phase
            if (GameManager.gamePhase == GamePhase.InGame)
            {
                GameManager.ChangePhase(GamePhase.Score);
            }
            else
            {
                UnityEngine.Debug.Log("Not in game now");
            }
        }

        public void ExitScore()
        {
            // If host and current phase is lobby, go to next phase
            if (GameManager.gamePhase == GamePhase.Score)
            {
                GameManager.ChangePhase(GamePhase.Reset);
            }
            else
            {
                UnityEngine.Debug.Log("Not in score screen now");
            }
        }
    }
}