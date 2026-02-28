using System;

namespace OrbitLink.Data
{
    [Serializable]
    public struct RouteData
    {
        public int RouteID;
        public int SourcePlanetID;
        public int TargetPlanetID;
        public int Level; // Represents transit speed upgrade
    }
}
