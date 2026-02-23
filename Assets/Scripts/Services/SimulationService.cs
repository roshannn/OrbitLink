using System;
using UnityEngine;
using Zenject;

namespace OrbitLink.Services
{
    public interface ISimulationService
    {
        float GameTime { get; }
        float TimeScale { get; set; }
        event Action<float> OnTick;
        event Action<float> OnLateTick;
        void Tick(float deltaTime);
    }

    public class SimulationService : ISimulationService, ITickable, ILateTickable
    {
        public float GameTime { get; private set; }
        public float TimeScale { get; set; } = 1.0f;

        public event Action<float> OnTick;
        public event Action<float> OnLateTick;

        // Zenject's ITickable.Tick is called every frame
        public void Tick()
        {
            float delta = Time.deltaTime * TimeScale;
            GameTime += delta;
            OnTick?.Invoke(delta);
        }

        public void LateTick()
        {
            float delta = Time.deltaTime * TimeScale;
            OnLateTick?.Invoke(delta);
        }

        // Manual tick for catch-up or specific simulation steps
        public void Tick(float deltaTime)
        {
            float delta = deltaTime * TimeScale;
            GameTime += delta;
            OnTick?.Invoke(delta);
        }
    }
}
