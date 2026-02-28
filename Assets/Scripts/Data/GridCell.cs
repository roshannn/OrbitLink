namespace OrbitLink.Data
{
    /// <summary>
    /// Runtime representation of a single cell in the Spatial Hash Grid.
    /// Uses pre-allocated fixed arrays to prevent GC allocations during spatial queries.
    /// </summary>
    public struct GridCell
    {
        // Max ships allowed in a generic cell before ignoring the overflow
        public const int MAX_SHIPS_PER_CELL = 8;
        
        // Pre-allocated fixed array to avoid garbage generation
        public int[] ShipIDs;
        
        // The current number of active ships tracked in this cell
        public int Count;

        public void Initialize()
        {
            if (ShipIDs == null || ShipIDs.Length != MAX_SHIPS_PER_CELL)
            {
                ShipIDs = new int[MAX_SHIPS_PER_CELL];
            }
            Count = 0;
        }

        public void Clear()
        {
            Count = 0;
        }

        public void AddShip(int shipId)
        {
            if (Count < MAX_SHIPS_PER_CELL)
            {
                ShipIDs[Count] = shipId;
                Count++;
            }
        }
    }
}
