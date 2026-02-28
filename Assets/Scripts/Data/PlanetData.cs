using System;

namespace OrbitLink.Data
{
    [Serializable]
    public struct PlanetData
    {
        public int ID;
        public int Level; // Represents capacity or spawn rate upgrade
        public double Phase; // Current mathematical phase/angle
        public double Speed; // Base orbital speed
        public double Radius; // Distance from center
        public UnityEngine.Vector2 Position; // Computed runtime position
    }
}
