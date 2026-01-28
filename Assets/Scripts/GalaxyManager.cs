using UnityEngine;
using System.Collections.Generic;

public class GalaxyManager : MonoBehaviour
{
    [Header("Simulation Settings")]
    [Tooltip("List of planets to update.")]
    [SerializeField] private List<Planet> _planets = new List<Planet>();

    [Tooltip("Global time scale for the simulation.")]
    [SerializeField] private float _timeScale = 1.0f;

    // Private tracker for the deterministic time
    private float _gameTime = 0f;

    private void Update()
    {
        // Increment game time
        _gameTime += Time.deltaTime * _timeScale;

        // Update all planets
        if (_planets != null)
        {
            // Using a for loop prevents garbage generation from foreach in older Unity versions/configs,
            // though foreach is mostly fine nowadays. Sticking to simple foreach for readability.
            foreach (Planet planet in _planets)
            {
                if (planet != null)
                {
                    planet.SetPosition(_gameTime);
                }
            }
        }
    }
    #region delete this
    public (Planet, Planet) ReturnPlanets()
    {
        return (_planets[0], _planets[1]);
    }
    public (Planet, Planet) ReturnPlanets2() {
        return (_planets[1], _planets[2]);
    }

    public (Planet, Planet) ReturnPlanets3() {
        return (_planets[2], _planets[0]);
    }

    #endregion
}
