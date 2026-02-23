using UnityEngine;
using OrbitLink.Data;
using OrbitLink.Services;
using Zenject;
using System.Collections.Generic;
using System.Linq;

namespace OrbitLink.Views
{
    public class ShipManager : MonoBehaviour
    {
        private const int MAX_SHIPS = 1024;
        private const float DEAD_ZONE_RADIUS = 0.5f;
        private const float GHOST_DURATION = 3.0f;

        [Header("Assets")]
        [SerializeField] private Mesh _shipMesh;
        [SerializeField] private Material _shipMaterial;

        private ISimulationService _simulation;
        private ICelestialService _celestial;
        private ILogisticsService _logistics;

        private ShipData[] _ships = new ShipData[MAX_SHIPS];
        private Matrix4x4[] _matrices = new Matrix4x4[MAX_SHIPS];
        private Vector4[] _colors = new Vector4[MAX_SHIPS];
        private MaterialPropertyBlock _propertyBlock;

        [Inject]
        public void Construct(ISimulationService simulation, ICelestialService celestial, ILogisticsService logistics)
        {
            _simulation = simulation;
            _celestial = celestial;
            _logistics = logistics;
        }

        private void Start()
        {
            _propertyBlock = new MaterialPropertyBlock();
            _logistics.OnShipSpawned += HandleSpawn;
            _simulation.OnTick += UpdateShips;
            _simulation.OnLateTick += HandleLateTick;

            // Initialize pool
            for (int i = 0; i < MAX_SHIPS; i++)
            {
                _ships[i].IsActive = false;
            }
        }

        private void OnDestroy()
        {
            if (_logistics != null) _logistics.OnShipSpawned -= HandleSpawn;
            if (_simulation != null)
            {
                _simulation.OnTick -= UpdateShips;
                _simulation.OnLateTick -= HandleLateTick;
            }
        }

        private void HandleLateTick(float deltaTime)
        {
            RenderShips();
        }

        private void HandleSpawn(string planetId)
        {
            for (int i = 0; i < MAX_SHIPS; i++)
            {
                if (!_ships[i].IsActive)
                {
                    var config = _celestial.ActivePlanets.FirstOrDefault(p => p.Id == planetId);
                    if (config == null) return;

                    _ships[i].IsActive = true;
                    _ships[i].PlanetId = planetId;
                    _ships[i].Progress = 0f;
                    _ships[i].Speed = 0.1f; // Target 10s travel time
                    _ships[i].State = ShipState.Moving;
                    _ships[i].GhostTimer = 0f;
                    break;
                }
            }
        }

        private void UpdateShips(float dt)
        {
            float time = _simulation.GameTime;

            for (int i = 0; i < MAX_SHIPS; i++)
            {
                if (!_ships[i].IsActive) continue;

                var planet = _celestial.ActivePlanets.FirstOrDefault(p => p.Id == _ships[i].PlanetId);
                if (planet == null)
                {
                    _ships[i].IsActive = false;
                    continue;
                }

                // 1. Progress Logic
                if (_ships[i].State != ShipState.Jammed)
                {
                    _ships[i].Progress += _ships[i].Speed * dt;
                }

                // 2. Arrival Logic
                if (_ships[i].Progress >= 1.0f)
                {
                    _logistics.NotifyShipArrival(_ships[i].PlanetId);
                    _ships[i].IsActive = false;
                    continue;
                }

                // 3. Ghosting Logic
                if (_ships[i].State == ShipState.Ghosting)
                {
                    _ships[i].GhostTimer -= dt;
                    if (_ships[i].GhostTimer <= 0) _ships[i].State = ShipState.Moving;
                }

                // 4. Calculate Mathematical Position (The "Clock Hand" Slide)
                Vector3 planetPos = _celestial.GetPositionAt(planet, time);
                _ships[i].WorldPosition = Vector3.Lerp(planetPos, Vector3.zero, _ships[i].Progress);
                
                // Rotation looks toward the Star (0,0,0)
                Vector3 direction = (Vector3.zero - planetPos).normalized;
                _ships[i].WorldRotation = Quaternion.LookRotation(Vector3.forward, direction);

                // 5. Build Matrix for Instancing
                _matrices[i] = Matrix4x4.TRS(_ships[i].WorldPosition, _ships[i].WorldRotation, Vector3.one * 0.2f);
                
                // Color mapping: Jammed = Orange, Ghosting = Dim, Moving = Planet Color
                Color col = planet.ThemeColor;
                if (_ships[i].State == ShipState.Jammed) col = Color.Lerp(col, Color.red, 0.8f);
                if (_ships[i].State == ShipState.Ghosting) col.a = 0.4f;
                _colors[i] = col;
            }
        }

        private void RenderShips()
        {
            // Only draw up to 1023 because that's the limit for some GPUs/Shaders in a single call
            // Using DrawMeshInstanced for maximum performance
            _propertyBlock.SetVectorArray("_BaseColor", _colors);
            Graphics.DrawMeshInstanced(_shipMesh, 0, _shipMaterial, _matrices, MAX_SHIPS, _propertyBlock);
        }
    }
}
