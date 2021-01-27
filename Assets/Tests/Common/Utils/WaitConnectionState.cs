using Mirror;
using UnityEngine;

namespace Tests.Common.Utils
{
    /// <summary>
    /// Wait until client is connected or until a time expired
    /// </summary>
    public class WaitForConnected : CustomYieldInstruction
    {
        readonly float timeout;
        readonly float startTime;
        readonly bool targetState;
        bool timedOut;

        public bool TimedOut => timedOut;

        public override bool keepWaiting
        {
            get
            {
                if (Time.realtimeSinceStartup - startTime >= timeout)
                {
                    timedOut = true;
                }

                return NetworkClient.isConnected != targetState && !timedOut;
            }
        }

        public WaitForConnected(bool state = true, float newTimeout = 10)
        {
            this.targetState = state;
            this.timeout = newTimeout;
            this.startTime = Time.realtimeSinceStartup;
        }
    }
}