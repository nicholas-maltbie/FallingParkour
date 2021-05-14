using PropHunt.Game.Flow;
using UnityEngine;

namespace PropHunt.UI
{
    public class ChangeGamePhaseButton : MonoBehaviour
    {
        public void ExitLobby()
        {
            UnityEngine.Debug.Log($"{GameManager.Instance} {GameManager.Instance == null}");
            // If host and current phase is lobby, go to next phase
            if (GameManager.Instance.gamePhase == GamePhase.Lobby)
            {
                GameManager.Instance.ChangePhase(GamePhase.Setup);
            }
            else
            {
                UnityEngine.Debug.Log("Not in lobby now");
            }
        }

        public void ExitGame()
        {
            // If host and current phase is lobby, go to next phase
            if (GameManager.Instance.gamePhase == GamePhase.InGame)
            {
                GameManager.Instance.ChangePhase(GamePhase.Score);
            }
            else
            {
                UnityEngine.Debug.Log("Not in game now");
            }
        }

        public void ExitScore()
        {
            // If host and current phase is lobby, go to next phase
            if (GameManager.Instance.gamePhase == GamePhase.Score)
            {
                GameManager.Instance.ChangePhase(GamePhase.Reset);
            }
            else
            {
                UnityEngine.Debug.Log("Not in score screen now");
            }
        }
    }
}