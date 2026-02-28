using OrbitLink.Data;

namespace OrbitLink.Services
{
    /// <summary>
    /// Resets the game state while awarding meta-currency.
    /// Executes completely outside the normal update loop to prevent data tearing.
    /// </summary>
    public class PrestigeSystem
    {
        private GameSession _session;

        // Example baseline: 1 trillion credits -> 1 Dark Matter
        private const double PRESTIGE_THRESHOLD = 1e12; 

        public PrestigeSystem(GameSession session)
        {
            _session = session;
        }

        public bool CanPrestige()
        {
            return _session.State.WalletBalance >= PRESTIGE_THRESHOLD;
        }

        public void TriggerPrestige()
        {
            if (!CanPrestige()) return;

            // Calculate reward (e.g., logarithmic or linear based on threshold)
            double currentWallet = _session.State.WalletBalance;
            double darkMatterEarned = System.Math.Floor(currentWallet / PRESTIGE_THRESHOLD);

            // Retain meta progression
            double retainedDarkMatter = _session.State.DarkMatterBalance + darkMatterEarned;

            // Wipe simulation state and inject retained currency
            PersistentState newState = new PersistentState
            {
                DarkMatterBalance = retainedDarkMatter,
                WalletBalance = 0
            };

            // Force session to reload cleanly
            _session.LoadState(newState);
        }
    }
}
