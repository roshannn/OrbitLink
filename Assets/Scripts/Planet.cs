using UnityEngine;

public class Planet : MonoBehaviour
{
    [Header("Orbit Settings")]
    [Tooltip("Radius of the orbit in world units.")]
    [SerializeField] private float _orbitRadius = 5f;

    [Tooltip("Time in seconds to complete one full orbit.")]
    [SerializeField] private float _orbitPeriod = 10f;

    [Tooltip("Starting angle in degrees (0 = Right, 90 = Up).")]
    [SerializeField, Range(0f, 360f)] private float _startAngle = 0f;

    [Tooltip("Color of the orbit path visualization.")]
    [SerializeField] private Color _orbitColor = Color.white;

    [Tooltip("Width of the orbit line.")]
    [SerializeField, Range(0.01f, 1f)] private float _orbitWidth = 0.05f;

    private void Start()
    {
        InitializeOrbitVisual();
    }

    /// <summary>
    /// Calculates and sets the position of the planet based on the deterministic game time.
    /// </summary>
    /// <param name="gameTime">The total elapsed game time.</param>
    public void SetPosition(float gameTime)
    {
        // Avoid division by zero if period is somehow 0
        float period = Mathf.Max(_orbitPeriod, 0.001f);

        // Formula: Angle = (StartAngle * Deg2Rad) + (2PI / Period * gameTime)
        float angleRad = (_startAngle * Mathf.Deg2Rad) + ((2f * Mathf.PI / period) * gameTime);

        float x = Mathf.Cos(angleRad) * _orbitRadius;
        float y = Mathf.Sin(angleRad) * _orbitRadius;

        transform.position = new Vector3(x, y, 0f);
    }

    private void InitializeOrbitVisual()
    {
        // Spawn a child object for the visuals
        GameObject ringObj = new GameObject($"{name}_OrbitRing");

        // The LineRenderer needs to be in world space to remain static while the parent moves.
        ringObj.transform.SetParent(transform);
        ringObj.transform.localPosition = Vector3.zero;

        LineRenderer lr = ringObj.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.loop = true;
        lr.startWidth = _orbitWidth;
        lr.endWidth = _orbitWidth;

        // Set Color with low alpha
        Color c = _orbitColor;
        c.a = 0.25f; // Low alpha
        lr.startColor = c;
        lr.endColor = c;

        // Use a default shader so it's not magenta
        Material lineMat = new Material(Shader.Find("Sprites/Default"));
        lr.material = lineMat;

        // Draw the circle
        int segments = 128; // Higher count for smoother circle
        lr.positionCount = segments;
        Vector3[] points = new Vector3[segments];

        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * 2f * Mathf.PI;
            // The orbit is centered at World Zero (0,0,0) as per requirements
            float x = Mathf.Cos(angle) * _orbitRadius;
            float y = Mathf.Sin(angle) * _orbitRadius;

            points[i] = new Vector3(x, y, 0f);
        }

        lr.SetPositions(points);
    }
}
