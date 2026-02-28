namespace OrbitLink.Data
{
    /// <summary>
    /// Singleton owner of the persistent state.
    /// Can be instantiated via Zenject or accessed globally.
    /// </summary>
    public class GameSession
    {
        public static GameSession Instance { get; private set; }
        
        public PersistentState State { get; private set; }

        public GameSession()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            State = new PersistentState();
        }

        public void LoadState(PersistentState savedState)
        {
            State = savedState ?? new PersistentState();
        }
    }
}
