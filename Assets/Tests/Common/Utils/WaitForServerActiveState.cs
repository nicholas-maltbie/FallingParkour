using Mirror;
using UnityEngine;

namespace Tests.Common.Utils
{
    /// <summary>
    /// Wait until network server is active or until a time expired
    /// </summary>
    public class WaitForServerActiveState : CustomYieldInstruction
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

                return NetworkServer.active != targetState && !timedOut;
            }
        }

        public WaitForServerActiveState(bool state = true, float newTimeout = 10)
        {
            this.targetState = state;
            this.timeout = newTimeout;
            this.startTime = Time.realtimeSinceStartup;
        }
    }
}