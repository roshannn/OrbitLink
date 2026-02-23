using UnityEngine;

namespace OrbitLink.Views
{
    [RequireComponent(typeof(LineRenderer))]
    public class OrbitView : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;

        private void Awake()
        {
            InitializeLineRendererIfNull();
        }

        private void InitializeLineRendererIfNull()
        {
            if (_lineRenderer == null)
            {
                _lineRenderer = GetComponent<LineRenderer>();
            }
        }

        public void SetOrbit(float radius, float width, Color color)
        {
            InitializeLineRendererIfNull();

            _lineRenderer.useWorldSpace = true;
            _lineRenderer.loop = true;
            _lineRenderer.startWidth = width;
            _lineRenderer.endWidth = width;

            // Set colors
            Color c = color;
            c.a = 0.3f;
            _lineRenderer.startColor = c;
            _lineRenderer.endColor = c;

            // Dynamic Segment Calculation
            // Base 60 segments, plus 20 per unit of radius for smoothness
            int segments = Mathf.Max(60, (int)(radius * 25));
            _lineRenderer.positionCount = segments;
            
            Vector3[] points = new Vector3[segments];
            for (int i = 0; i < segments; i++)
            {
                float progress = (float)i / segments;
                float angle = progress * 2f * Mathf.PI;
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                points[i] = new Vector3(x, y, 0f);
            }

            _lineRenderer.SetPositions(points);
        }
    }
}
