using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Route : MonoBehaviour
{
    // Public properties to read the planets
    public GameObject StartPlanet { get; private set; }
    public GameObject EndPlanet { get; private set; }
    
    // Publicly readable distance for gameplay logic
    public float Distance { get; private set; }

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Initializes the route with two planets.
    /// </summary>
    /// <param name="a">The starting planet.</param>
    /// <param name="b">The ending planet.</param>
    public void Initialize(GameObject a, GameObject b)
    {
        StartPlanet = a;
        EndPlanet = b;

        // Optionally set initial positions immediately to avoid 1-frame glitches
        if (StartPlanet != null && EndPlanet != null)
        {
            UpdateVisuals();
        }
    }

    // LateUpdate is critical to run AFTER the Planet's Update/movement logic
    // Assuming Planets move in Update or via GalaxyManager which runs in Update.
    private void LateUpdate()
    {
        if (StartPlanet == null || EndPlanet == null) return;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        Vector3 startPos = StartPlanet.transform.position;
        Vector3 endPos = EndPlanet.transform.position;

        // Update LineRenderer positions
        // Ensure the LineRenderer has at least 2 points configured in the prefab or via code if needed,
        // but typically just setting positions 0 and 1 works if count is 2.
        if (_lineRenderer.positionCount < 2) _lineRenderer.positionCount = 2;
        
        _lineRenderer.SetPosition(0, startPos);
        _lineRenderer.SetPosition(1, endPos);

        // Calculate and sort distance
        Distance = Vector3.Distance(startPos, endPos);
    }
}
