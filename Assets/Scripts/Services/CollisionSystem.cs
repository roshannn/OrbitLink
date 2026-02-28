using UnityEngine;
using OrbitLink.Data;

namespace OrbitLink.Services
{
    /// <summary>
    /// Implements O(N) Spatial Hash Grid strictly for O(1) adjacency checks.
    /// Completely bypasses Unity Physics/Colliders. 0 allocations per frame.
    /// </summary>
    public class CollisionSystem
    {
        private const float CELL_SIZE = 2f;
        private const float GRID_HALF_EXTENTS = 30f; // Assumes play space relates to -30 to +30 units
        private const int GRID_WIDTH = (int)((GRID_HALF_EXTENTS * 2f) / CELL_SIZE) + 1; // E.g., 61
        private const int GRID_HEIGHT = GRID_WIDTH;
        private const int TOTAL_CELLS = GRID_WIDTH * GRID_HEIGHT;

        private GridCell[] _gridCells;

        public CollisionSystem()
        {
            _gridCells = new GridCell[TOTAL_CELLS];
            for (int i = 0; i < TOTAL_CELLS; i++)
            {
                _gridCells[i].Initialize();
            }
        }

        public void Evaluate(ShipData[] activeShips, int activeShipCount, float jamRadius)
        {
            if (activeShipCount == 0) return;

            // 1. Clear Grid
            for (int i = 0; i < TOTAL_CELLS; i++)
            {
                _gridCells[i].Clear();
            }

            // 2. Populate Grid
            for (int i = 0; i < activeShipCount; i++)
            {
                int cellIndex = GetCellIndex(activeShips[i].CurrentPosition);
                if (cellIndex >= 0 && cellIndex < TOTAL_CELLS)
                {
                    // Store the Array INDEX (i), NOT the ShipID, for fast access
                    _gridCells[cellIndex].AddShip(i);
                }
            }

            float jamRadiusSq = jamRadius * jamRadius;

            // 3. Evaluate Collisions
            for (int i = 0; i < activeShipCount; i++)
            {
                // Reset jam state initially. 
                // Alternatively, could keep them jammed permanently until manually cleared.
                activeShips[i].IsJammed = false; 

                int cx, cy;
                GetCellCoords(activeShips[i].CurrentPosition, out cx, out cy);

                // Check 9 neighbors (including current cell)
                bool isColliding = false;
                for (int y = cy - 1; y <= cy + 1 && !isColliding; y++)
                {
                    for (int x = cx - 1; x <= cx + 1 && !isColliding; x++)
                    {
                        int index = GetCellIndexFromCoords(x, y);
                        if (index < 0 || index >= TOTAL_CELLS) continue;

                        GridCell cell = _gridCells[index];
                        for (int k = 0; k < cell.Count; k++)
                        {
                            int otherShipIndex = cell.ShipIDs[k];
                            // Don't collide with self
                            if (otherShipIndex == i) continue;

                            // Fast distance sq check
                            Vector2 diff = activeShips[i].CurrentPosition - activeShips[otherShipIndex].CurrentPosition;
                            float distSq = diff.sqrMagnitude;

                            if (distSq < jamRadiusSq)
                            {
                                activeShips[i].IsJammed = true;
                                isColliding = true;
                                break; // No need to check the rest
                            }
                        }
                    }
                }
            }
        }

        private void GetCellCoords(Vector2 position, out int cx, out int cy)
        {
            cx = Mathf.FloorToInt((position.x + GRID_HALF_EXTENTS) / CELL_SIZE);
            cy = Mathf.FloorToInt((position.y + GRID_HALF_EXTENTS) / CELL_SIZE);
        }

        private int GetCellIndex(Vector2 position)
        {
            GetCellCoords(position, out int cx, out int cy);
            return GetCellIndexFromCoords(cx, cy);
        }

        private int GetCellIndexFromCoords(int cx, int cy)
        {
            if (cx < 0 || cx >= GRID_WIDTH || cy < 0 || cy >= GRID_HEIGHT) return -1;
            return cy * GRID_WIDTH + cx;
        }
    }
}
