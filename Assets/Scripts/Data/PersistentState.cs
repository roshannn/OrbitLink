using System;

namespace OrbitLink.Data
{
    [Serializable]
    public class PersistentState
    {
        public double WalletBalance;
        public double DarkMatterBalance;
        public PlanetData[] Planets;
        public RouteData[] Routes;
        
        public PersistentState()
        {
            WalletBalance = 0;
            DarkMatterBalance = 0;
            Planets = new PlanetData[0];
            Routes = new RouteData[0];
        }
    }
}
