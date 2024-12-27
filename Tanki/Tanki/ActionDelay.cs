using System;
using System.Diagnostics;

namespace Tanki
{
    public class ActionDelay
    {
        private readonly TimeSpan delay;
        private DateTime lastActionTime;

        public ActionDelay(TimeSpan delay)
        {
            this.delay = delay;
            lastActionTime = DateTime.MinValue;
        }

        public bool CanPerformAction()
        {
            return DateTime.Now - lastActionTime >= delay;
        }

        public void Reset()
        {
            lastActionTime = DateTime.Now;
        }
    }
}