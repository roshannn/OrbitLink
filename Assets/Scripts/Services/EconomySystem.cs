using System;
using OrbitLink.Data;

namespace OrbitLink.Services
{
    /// <summary>
    /// Handles exponential logic and triggers for economy actions using double-precision math.
    /// Responds to ship arrivals directly via event routing.
    /// </summary>
    public class EconomySystem : IDisposable
    {
        private GameSession _session;
        private ShipSystem _shipSystem;

        // Base payout per ship arrival. In a real game this would scale by route/planet level.
        private const double BASE_PAYOUT = 10.0;

        public event Action<double> OnWalletChanged;

        public EconomySystem(GameSession session, ShipSystem shipSystem)
        {
            _session = session;
            _shipSystem = shipSystem;

            _shipSystem.OnShipArrived += HandleShipArrived;
        }

        public void Dispose()
        {
            if (_shipSystem != null)
            {
                _shipSystem.OnShipArrived -= HandleShipArrived;
            }
        }

        private void HandleShipArrived(int routeID, int planetID)
        {
            // Calculate dynamic payout based on route or planet level
            // using the exponential cost scaling formula for upgrades elsewhere.
            double payout = BASE_PAYOUT; 
            
            // Add to persistent state
            _session.State.WalletBalance += payout;
            
            // Notify UI
            OnWalletChanged?.Invoke(_session.State.WalletBalance);
        }

        public bool TryPurchaseUpgrade(double cost)
        {
            if (_session.State.WalletBalance >= cost)
            {
                _session.State.WalletBalance -= cost;
                OnWalletChanged?.Invoke(_session.State.WalletBalance);
                return true;
            }
            return false;
        }

        // Standard scaling formula from TDD
        public double CalculateUpgradeCost(double baseCost, double multiplier, int level)
        {
            return baseCost * Math.Pow(multiplier, level);
        }
    }
}
