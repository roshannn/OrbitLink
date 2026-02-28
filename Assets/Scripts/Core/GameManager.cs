using OrbitLink.Data;
using OrbitLink.Services;
using UnityEngine;
using Zenject;

namespace OrbitLink.Core
{
    /// <summary>
    /// The master loop controller. 
    /// Enforces strict deterministic tick ordering to ensure logic cannot desync from visuals.
    /// Can be bound via Zenject or added to a core GameObject.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // Dependencies injected via Zenject
        [Inject] private GameSession _session;
        [Inject] private TimeManager _timeManager;
        [Inject] private OrbitSystem _orbitSystem;
        [Inject] private RouteSystem _routeSystem;
        [Inject] private ShipSystem _shipSystem;
        [Inject] private CollisionSystem _collisionSystem;
        [Inject] private EconomySystem _economySystem;

        [Header("Simulation Settings")]
        public float BaseShipSpeed = 2f;
        public float CollisionJamRadius = 1.0f;
        public float SpawnCooldownTimer = 1.0f;

        private float _currentSpawnTimer = 0f;

        private void Start()
        {
            // Usually, data loading happens here or in an initializer class
            // Sample initialization if empty state 
            if (_session.State.Planets == null || _session.State.Planets.Length == 0)
            {
                _session.State.Planets = new PlanetData[]
                {
                    new PlanetData { ID = 1, Radius = 3f, Speed = 0.5f, Phase = 0f },
                    new PlanetData { ID = 2, Radius = 6f, Speed = 0.2f, Phase = 3.14f }
                };

                _session.State.Routes = new RouteData[]
                {
                    new RouteData { RouteID = 101, SourcePlanetID = 1, TargetPlanetID = 2 }
                };
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime; // Could use a fixed accumulated step for 100% determinism

            // 1. Accumulate Simulation Time
            _timeManager.Tick(dt);

            // 2. Process Input (Stubbed for now, e.g. Handle user clicking to connect routes)

            // 3. Update Planet Orbits
            _orbitSystem.UpdatePositions(_session.State.Planets, _timeManager.GlobalTime);

            // 4. Update Route Vectors
            _routeSystem.RebuildCaches(_session.State.Routes, _session.State.Planets);

            // 5. Spawn Ships
            _currentSpawnTimer -= dt;
            if (_currentSpawnTimer <= 0)
            {
                _currentSpawnTimer = SpawnCooldownTimer;
                // Exam ple: Try spawning on all routes (or use a dedicated pacing system)
                foreach (var route in _session.State.Routes)
                {
                    _shipSystem.TrySpawnShip(route.RouteID, _routeSystem);
                }
            }

            // 6. Advance Ship Positions
            _shipSystem.TickMovement(dt, BaseShipSpeed, _routeSystem);

            // 7. Resolve Spatial Hash Grid Collisions
            _collisionSystem.Evaluate(_shipSystem.ActiveShips, _shipSystem.ActiveShipCount, CollisionJamRadius);

            // 8. Process Economy
            // Handled automatically via OnShipArrived event firing synchronously inside TickMovement
        }

        private void OnDestroy()
        {
            _economySystem?.Dispose();
        }

        // Exposing systems for Presentation layer
        public GameSession Session => _session;
        public ShipSystem ShipSystem => _shipSystem;
    }
}
