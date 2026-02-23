using System;
using System.Collections.Generic;
using OrbitLink.Data;
using OrbitLink.Services;
using UnityEngine;
using Zenject;
using System.Linq;

namespace OrbitLink.Services
{
    public interface ILogisticsService
    {
        void SpawnShip(string planetId);
        event Action<string> OnShipSpawned;
        event Action<string> OnShipArrived;
        void NotifyShipArrival(string planetId);
    }

    public class LogisticsService : ILogisticsService, IInitializable, IDisposable
    {
        private ISimulationService _simulation;
        private ICelestialService _celestial;
        
        // Tracks spawn timers per planet ID
        private Dictionary<string, float> _spawnTimers = new Dictionary<string, float>();

        public event Action<string> OnShipSpawned;
        public event Action<string> OnShipArrived;

        [Inject]
        public void Construct(ISimulationService simulation, ICelestialService celestial)
        {
            _simulation = simulation;
            _celestial = celestial;
        }

        public void Initialize()
        {
            _simulation.OnTick += HandleTick;
        }

        public void Dispose()
        {
            if (_simulation != null) _simulation.OnTick -= HandleTick;
        }

        private void HandleTick(float deltaTime)
        {
            // Iterate through active planets and manage spawn rates
            foreach (var planet in _celestial.ActivePlanets)
            {
                if (!_spawnTimers.ContainsKey(planet.Id))
                {
                    _spawnTimers[planet.Id] = 0f;
                }

                // Data-driven spawn rate (default to 2s if config is weird, but ideally based on period/data)
                // For now, let's assume one ship per 1/5th of an orbit for some 'traffic'
                float spawnRate = Mathf.Max(planet.OrbitPeriod / 5f, 0.5f); 
                _spawnTimers[planet.Id] += deltaTime;

                if (_spawnTimers[planet.Id] >= spawnRate)
                {
                    SpawnShip(planet.Id);
                    _spawnTimers[planet.Id] = 0f;
                }
            }
        }

        public void SpawnShip(string planetId)
        {
            OnShipSpawned?.Invoke(planetId);
        }

        public void NotifyShipArrival(string planetId)
        {
            OnShipArrived?.Invoke(planetId);
        }
    }
}
