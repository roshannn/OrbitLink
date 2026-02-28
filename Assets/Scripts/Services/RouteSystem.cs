using System.Collections.Generic;
using UnityEngine;
using OrbitLink.Data;

namespace OrbitLink.Services
{
    public struct RouteRuntimeCache
    {
        public int RouteID;
        public Vector2 WorldStartPos;
        public Vector2 WorldEndPos;
        public Vector2 Direction; // Normalized
        public float TotalDistance;
    }

    /// <summary>
    /// Processes route geometries dynamically as planets orbit.
    /// Caches the distances and directions so the ShipSystem doesn't have to evaluate square roots per ship.
    /// </summary>
    public class RouteSystem
    {
        // Lookup cache: Key = RouteID
        public Dictionary<int, RouteRuntimeCache> RouteCaches { get; private set; }

        public RouteSystem()
        {
            RouteCaches = new Dictionary<int, RouteRuntimeCache>();
        }

        public void RebuildCaches(RouteData[] routes, PlanetData[] planets)
        {
            if (routes == null || planets == null) return;

            // In a highly optimized iteration, we might use a flat array over a Dictionary,
            // but assuming a max of ~100 routes, Dictionary lookup is negligible and simpler right now.
            foreach (var route in routes)
            {
                Vector2 startPos = GetPlanetPosition(planets, route.SourcePlanetID);
                Vector2 endPos = GetPlanetPosition(planets, route.TargetPlanetID);

                // Settle route vectors
                Vector2 displacement = endPos - startPos;
                float distance = displacement.magnitude;
                
                // Prevent perfectly overlapping planets causing division by zero 
                Vector2 direction = distance > 0.0001f ? displacement / distance : Vector2.zero;

                RouteCaches[route.RouteID] = new RouteRuntimeCache
                {
                    RouteID = route.RouteID,
                    WorldStartPos = startPos,
                    WorldEndPos = endPos,
                    Direction = direction,
                    TotalDistance = distance
                };
            }
        }

        private Vector2 GetPlanetPosition(PlanetData[] planets, int planetID)
        {
            // Linear search given small N (< 25 planets). Can optimize to index mapping if IDs != indices.
            for (int i = 0; i < planets.Length; i++)
            {
                if (planets[i].ID == planetID)
                {
                    return planets[i].Position;
                }
            }
            return Vector2.zero;
        }
    }
}
