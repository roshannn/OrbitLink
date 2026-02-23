using System;
using UnityEngine;

namespace OrbitLink.Data
{
    [Serializable]
    public class PlanetConfig
    {
        public string Id;
        
        [Header("Orbit Settings")]
        public float OrbitRadius = 5f;
        public float OrbitPeriod = 10f;
        [Range(0f, 360f)]
        public float StartAngle = 0f;

        [Header("Visuals")]
        public Color ThemeColor = Color.cyan;
        [Range(0.01f, 1f)]
        public float OrbitWidth = 0.05f;

        [Header("Economy")]
        public float BaseIncome = 10f;
        public float BaseRouteCost = 100f;
    }
}
