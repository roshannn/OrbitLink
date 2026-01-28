using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class RouteManager : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Prefab for the Route object. Must have a Route script and LineRenderer.")]
    [SerializeField] private GameObject _routePrefab;

    [Header("State")]
    [Tooltip("Currently active routes.")]
    [SerializeField] private List<Route> _activeRoutes = new List<Route>();

    public List<Route> ActiveRoutes => _activeRoutes;
    #region Delete this
    public GalaxyManager GalaxyManager;

    private void Start()
    {
        var (a, b) = GalaxyManager.ReturnPlanets();
        CreateRoute(a, b);
        var (c, d) = GalaxyManager.ReturnPlanets2();
        CreateRoute(c, d); 
        var (e, f) = GalaxyManager.ReturnPlanets3();
        CreateRoute(e, f);
    }
    #endregion
    /// <summary>
    /// Creates a new route between two planets if valid and unique.
    /// </summary>
    /// <returns>The created Route, or null if failed.</returns>
    public Route CreateRoute(Planet a, Planet b)
    {
        // 1. Basic validation
        if (a == null || b == null) return null;
        if (a == b) return null;

        // 2. Check for duplicates (A-B is considered same as B-A)
        if (DoesRouteExist(a, b))
        {
            Debug.LogWarning($"Route already exists between {a.name} and {b.name}.");
            return null;
        }

        // 3. Instantiate and Initialize
        if (_routePrefab == null)
        {
            Debug.LogError("RoutePrefab is not assigned in RouteManager!");
            return null;
        }

        GameObject routeObj = Instantiate(_routePrefab, transform);
        routeObj.name = $"Route_{a.name}_{b.name}";

        Route newRoute = routeObj.GetComponent<Route>();
        if (newRoute != null)
        {
            newRoute.Initialize(a, b);
            _activeRoutes.Add(newRoute);
            return newRoute;
        }
        else
        {
            Debug.LogError("RoutePrefab does not contain a Route script!");
            Destroy(routeObj);
            return null;
        }
    }

    /// <summary>
    /// Deletes the specified route.
    /// </summary>
    public void DeleteRoute(Route r)
    {
        if (r == null) return;

        if (_activeRoutes.Contains(r))
        {
            _activeRoutes.Remove(r);
        }

        Destroy(r.gameObject);
    }

    /// <summary>
    /// Checks if a route already exists between two planets.
    /// </summary>
    private bool DoesRouteExist(Planet a, Planet b)
    {
        foreach (Route r in _activeRoutes)
        {
            if (r == null) continue;

            // Check A->B OR B->A
            bool match1 = (r.StartPlanet == a && r.EndPlanet == b);
            bool match2 = (r.StartPlanet == b && r.EndPlanet == a);

            if (match1 || match2) return true;
        }
        return false;
    }
}
