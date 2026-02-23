using UnityEngine;

namespace OrbitLink.Data
{
    public enum ShipState
    {
        Moving,
        Jammed,
        Ghosting
    }

    public struct ShipData
    {
        public bool IsActive;
        public string PlanetId;
        public float Progress; // 0.0 (Planet) to 1.0 (Star)
        public float Speed;
        public ShipState State;
        public float GhostTimer;

        // Cached values for the current frame
        public Vector3 WorldPosition;
        public Quaternion WorldRotation;
    }
}
