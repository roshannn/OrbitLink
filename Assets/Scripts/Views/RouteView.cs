using UnityEngine;
using OrbitLink.Data;
using OrbitLink.Services;
using Zenject;

namespace OrbitLink.Views
{
    [RequireComponent(typeof(LineRenderer))]
    public class RouteView : MonoBehaviour
    {
        [SerializeField] private PlanetData _planetData;
        
        private ICelestialService _celestial;
        private ISimulationService _simulation;
        private LineRenderer _lineRenderer;

        private PlanetConfig Config => _planetData != null ? _planetData.Config : null;

        [Inject]
        public void Construct(ICelestialService celestial, ISimulationService simulation)
        {
            _celestial = celestial;
            _simulation = simulation;
        }

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
            _lineRenderer.useWorldSpace = true;

            _simulation.OnLateTick += HandleLateTick;

            if (Config != null)
            {
                Color c = Config.ThemeColor;
                c.a = 0.3f;
                _lineRenderer.startColor = c;
                _lineRenderer.endColor = c;
            }
        }

        private void OnDestroy()
        {
            if (_simulation != null)
            {
                _simulation.OnLateTick -= HandleLateTick;
            }
        }

        private void HandleLateTick(float deltaTime)
        {
            if (Config == null) return;

            // Anchor A: The Star
            _lineRenderer.SetPosition(0, Vector3.zero);
            
            // Anchor B: The Moving Planet
            Vector3 planetPos = _celestial.GetPositionAt(Config, _simulation.GameTime);
            _lineRenderer.SetPosition(1, planetPos);
        }
    }
}
