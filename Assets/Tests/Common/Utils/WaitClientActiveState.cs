using Mirror;
using UnityEngine;

namespace Tests.Common.Utils
{
    /// <summary>
    /// Wait until client is active state or until time expires
    /// </summary>
    public class WaitClientActiveState : CustomYieldInstruction
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

                return NetworkClient.active != targetState && !timedOut;
            }
        }

        public WaitClientActiveState(bool state = true, float newTimeout = 10)
        {
            this.targetState = state;
            this.timeout = newTimeout;
            this.startTime = Time.realtimeSinceStartup;
        }
    }
}