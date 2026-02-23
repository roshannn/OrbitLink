using System.Collections.Generic;
using UnityEngine;
using OrbitLink.Data;

namespace OrbitLink.Services
{
    public interface ICelestialService
    {
        Vector3 GetPositionAt(PlanetConfig config, float time);
        void RegisterPlanet(PlanetConfig config);
        void UnregisterPlanet(PlanetConfig config);
        IReadOnlyList<PlanetConfig> ActivePlanets { get; }
    }

    public class CelestialService : ICelestialService
    {
        private readonly List<PlanetConfig> _activePlanets = new List<PlanetConfig>();
        public IReadOnlyList<PlanetConfig> ActivePlanets => _activePlanets;

        public void RegisterPlanet(PlanetConfig config)
        {
            if (!_activePlanets.Contains(config))
            {
                _activePlanets.Add(config);
            }
        }

        public void UnregisterPlanet(PlanetConfig config)
        {
            _activePlanets.Remove(config);
        }

        public Vector3 GetPositionAt(PlanetConfig config, float time)
        {
            if (config == null) return Vector3.zero;

            float period = Mathf.Max(config.OrbitPeriod, 0.001f);
            float angleRad = (config.StartAngle * Mathf.Deg2Rad) + ((2f * Mathf.PI / period) * time);

            float x = Mathf.Cos(angleRad) * config.OrbitRadius;
            float y = Mathf.Sin(angleRad) * config.OrbitRadius;

            return new Vector3(x, y, 0f);
        }
    }
}
