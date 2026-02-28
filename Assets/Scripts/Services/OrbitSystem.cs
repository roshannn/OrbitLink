using System;
using UnityEngine;
using OrbitLink.Data;

namespace OrbitLink.Services
{
    /// <summary>
    /// Computes strictly deterministic planet positions using Math routines.
    /// Completely bypasses Unity transforms during simulation to avoid sync overhead.
    /// </summary>
    public class OrbitSystem
    {
        private const double TWO_PI = Math.PI * 2.0;

        /// <summary>
        /// Updates the positional vectors for all planets based on current global deterministic time.
        /// </summary>
        public void UpdatePositions(PlanetData[] planets, double globalTime)
        {
            if (planets == null) return;

            for (int i = 0; i < planets.Length; i++)
            {
                // Modulo by 2PI prevents floating-point precision loss over extreme simulated times
                double currentAngle = (planets[i].Phase + planets[i].Speed * globalTime) % TWO_PI;
                if (currentAngle < 0) currentAngle += TWO_PI; // Ensure positive angle

                float x = (float)(Math.Cos(currentAngle) * planets[i].Radius);
                float y = (float)(Math.Sin(currentAngle) * planets[i].Radius);

                // We write directly to the structs in the array
                planets[i].Position = new Vector2(x, y);
            }
        }
    }
}
