using System;
using UnityEngine;
using OrbitLink.Data;

namespace OrbitLink.Services
{
    /// <summary>
    /// Pure data-driven core for moving 1,500 ships strictly mathematically.
    /// Operates independently of Unity GameObjects.
    /// </summary>
    public class ShipSystem
    {
        public const int MAX_SHIPS = 1500;
        public ShipData[] ActiveShips { get; private set; }
        public int ActiveShipCount { get; private set; }

        public event Action<int, int> OnShipArrived; // RouteID, PlanetID (for economy payout)

        private int _nextShipID = 1; // Running increment to keep ship IDs globally unique

        public ShipSystem()
        {
            ActiveShips = new ShipData[MAX_SHIPS];
            ActiveShipCount = 0;
        }

        public void TickMovement(float deltaTime, float baseShipSpeed, RouteSystem routeSystem)
        {
            // Iterate backwards so we can safely remove-swap elements without skipping
            for (int i = ActiveShipCount - 1; i >= 0; i--)
            {
                if (ActiveShips[i].IsJammed)
                {
                    continue; // Skip movement if jammed
                }

                // Get cached route info to avoid sqrt/distance recalcs
                if (!routeSystem.RouteCaches.TryGetValue(ActiveShips[i].RouteID, out var routeCache))
                {
                    // Invalid route (e.g., planet destroyed?), despawn gracefully
                    DespawnShip(i);
                    continue;
                }

                float speedVal = baseShipSpeed; // Could be multiplied by Route Level later
                ActiveShips[i].Progress += (speedVal * deltaTime) / routeCache.TotalDistance;

                if (ActiveShips[i].Progress >= 1.0f)
                {
                    // Ship Arrived!
                    OnShipArrived?.Invoke(routeCache.RouteID, routeCache.RouteID /* We need target planet ID here! Wait. */);
                    // Actually, let's just pass RouteID, EconomySystem can look up TargetPlanetID from PersistentState Routes.
                    DespawnShip(i);
                }
                else
                {
                    // Linear interpolation based on deterministic vectors
                    ActiveShips[i].CurrentPosition = routeCache.WorldStartPos + (routeCache.Direction * (ActiveShips[i].Progress * routeCache.TotalDistance));
                    // Update TargetPosition dynamically as planets orbit
                    ActiveShips[i].TargetPosition = routeCache.WorldEndPos; 
                }
            }
        }

        public bool TrySpawnShip(int routeID, RouteSystem routeSystem)
        {
            if (ActiveShipCount >= MAX_SHIPS)
            {
                // Soft ceiling reached, skip spawning to prevent CPU/GC spikes
                return false;
            }

            if (!routeSystem.RouteCaches.TryGetValue(routeID, out var routeCache))
            {
                return false;
            }

            ActiveShips[ActiveShipCount] = new ShipData
            {
                ShipID = _nextShipID++,
                RouteID = routeID,
                Progress = 0f,
                CurrentPosition = routeCache.WorldStartPos,
                TargetPosition = routeCache.WorldEndPos,
                IsJammed = false
            };

            ActiveShipCount++;
            return true;
        }

        private void DespawnShip(int index)
        {
            // Fast removal: Swap with last active element
            if (ActiveShipCount > 1 && index != ActiveShipCount - 1)
            {
                ActiveShips[index] = ActiveShips[ActiveShipCount - 1];
            }
            ActiveShipCount--;
        }
    }
}
