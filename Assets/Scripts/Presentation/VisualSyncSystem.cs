using UnityEngine;
using OrbitLink.Core;
using OrbitLink.Data;

namespace OrbitLink.Presentation
{
    /// <summary>
    /// Strictly separates logic from rendering.
    /// Runs in LateUpdate to interpolate or snap visuals to the perfectly deterministic data states.
    /// </summary>
    public class VisualSyncSystem : MonoBehaviour
    {
        [SerializeField] private GameManager _gameManager;
        
        // Example references assuming an ObjectPool exists for visuals
        [SerializeField] private Transform[] _planetTransforms;
        [SerializeField] private Transform[] _shipTransforms;

        private void LateUpdate()
        {
            if (_gameManager == null || _gameManager.Session == null) return;

            // 1. Sync Planet Transforms
            var planets = _gameManager.Session.State.Planets;
            if (planets != null && _planetTransforms != null)
            {
                for (int i = 0; i < planets.Length && i < _planetTransforms.Length; i++)
                {
                    if (_planetTransforms[i] != null)
                    {
                        // Direct positional snap based on deterministic Math
                        _planetTransforms[i].position = planets[i].Position;
                    }
                }
            }

            // 2. Sync Route LineRenderers
            // (Assumes you have a LineRenderer manager iterating the RouteSystem caches)

            // 3. Sync Ship Transforms
            var ships = _gameManager.ShipSystem?.ActiveShips;
            int shipCount = _gameManager.ShipSystem?.ActiveShipCount ?? 0;

            if (ships != null && _shipTransforms != null)
            {
                // Simple pool mapping: Map visual instance [i] to active logic entity [i]
                // (In a real scenario, you'd match ShipID to a Dictionary or pooled object)
                for (int i = 0; i < shipCount && i < _shipTransforms.Length; i++)
                {
                    if (_shipTransforms[i] != null)
                    {
                        if (!_shipTransforms[i].gameObject.activeSelf)
                            _shipTransforms[i].gameObject.SetActive(true);

                        _shipTransforms[i].position = ships[i].CurrentPosition;
                    }
                }

                // Hide unused visual pool items
                for (int i = shipCount; i < _shipTransforms.Length; i++)
                {
                    if (_shipTransforms[i] != null && _shipTransforms[i].gameObject.activeSelf)
                    {
                        _shipTransforms[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
