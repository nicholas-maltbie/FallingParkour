using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests.Common.Utils
{
    /// <summary>
    /// Wait until a scene is loaded
    /// </summary>
    public class WaitForSceneLoaded : CustomYieldInstruction
    {
        readonly string sceneName;
        readonly float timeout;
        readonly float startTime;
        bool timedOut;

        public bool TimedOut => timedOut;

        public override bool keepWaiting
        {
            get
            {
                var scene = SceneManager.GetSceneByName(sceneName);
                var sceneLoaded = scene.IsValid() && scene.isLoaded;

                if (Time.realtimeSinceStartup - startTime >= timeout)
                {
                    timedOut = true;
                }

                return !sceneLoaded && !timedOut;
            }
        }

        public WaitForSceneLoaded(string newSceneName, float newTimeout = 10)
        {
            sceneName = newSceneName;
            timeout = newTimeout;
            startTime = Time.realtimeSinceStartup;
        }
    }
}