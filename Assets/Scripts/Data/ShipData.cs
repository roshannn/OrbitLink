using UnityEngine;

namespace OrbitLink.Data
{
    /// <summary>
    /// Pure runtime struct. Not saved to disk.
    /// Needs to be kept very lightweight as we may have 1500 of these actively iterating.
    /// </summary>
    public struct ShipData
    {
        public int ShipID; // Unique identifier for pooling and spatial hashing
        public Vector2 CurrentPosition;
        public Vector2 TargetPosition;
        public float Progress;
        public int RouteID;
        public bool IsJammed;
    }
}
