using UnityEngine;

namespace OrbitLink.Services
{
    /// <summary>
    /// Core time tracker for deterministic simulation.
    /// Separated from Unity's Time.time to allow pausing, fast-forwarding, or logic isolation.
    /// </summary>
    public class TimeManager
    {
        public double GlobalTime { get; private set; }

        public TimeManager()
        {
            GlobalTime = 0.0;
        }

        public void Tick(float deltaTime)
        {
            GlobalTime += deltaTime;
        }

        public void ResetTime()
        {
            GlobalTime = 0.0;
        }
    }
}
