using UnityEngine;

namespace OrbitLink.Data
{
    [CreateAssetMenu(fileName = "NewPlanetData", menuName = "OrbitLink/Planet Data")]
    public class PlanetData : ScriptableObject
    {
        public PlanetConfig Config;
    }
}
