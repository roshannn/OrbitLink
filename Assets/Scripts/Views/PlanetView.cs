using UnityEngine;
using OrbitLink.Data;
using OrbitLink.Services;
using Zenject;

namespace OrbitLink.Views
{
    public class PlanetView : MonoBehaviour
    {
        [Header("Assets")]
        [SerializeField] private PlanetData _data;
        [SerializeField] private OrbitView _orbitPrefab;
        
        private ICelestialService _celestialService;
        private ISimulationService _simulationService;
        private OrbitView _orbitInstance;

        private PlanetConfig Config => _data != null ? _data.Config : null;

        [Inject]
        public void Construct(ICelestialService celestialService, ISimulationService simulationService)
        {
            _celestialService = celestialService;
            _simulationService = simulationService;
        }

        private void Start()
        {
            if (_data == null || Config == null)
            {
                Debug.LogError($"PlanetData or Config is missing on {name}!");
                return;
            }

            _celestialService.RegisterPlanet(Config);
            _simulationService.OnLateTick += HandleLateTick;
            InitializeOrbitVisual();
            UpdatePosition();
        }

        private void OnDestroy()
        {
            if (_celestialService != null && Config != null)
            {
                _celestialService.UnregisterPlanet(Config);
            }

            if (_simulationService != null)
            {
                _simulationService.OnLateTick -= HandleLateTick;
            }
        }

        private void HandleLateTick(float deltaTime)
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (Config == null || _celestialService == null || _simulationService == null) return;

            transform.position = _celestialService.GetPositionAt(Config, _simulationService.GameTime);
        }

        private void InitializeOrbitVisual()
        {
            if (Config == null || _orbitPrefab == null) return;

            // Instantiate and initialize the dedicated OrbitView
            _orbitInstance = Instantiate(_orbitPrefab, Vector3.zero, Quaternion.identity, transform.parent);
            _orbitInstance.name = $"{_data.name}_OrbitRing";
            
            _orbitInstance.SetOrbit(Config.OrbitRadius, Config.OrbitWidth, Config.ThemeColor);
        }
    }
}
