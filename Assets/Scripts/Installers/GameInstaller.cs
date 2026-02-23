using OrbitLink.Services;
using Zenject;

namespace OrbitLink.Installers
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Bind the Master Clock
            Container.BindInterfacesAndSelfTo<SimulationService>()
                .AsSingle()
                .NonLazy();

            // Bind the Celestial Logic
            Container.BindInterfacesAndSelfTo<CelestialService>()
                .AsSingle()
                .NonLazy();

            // Bind Logistics (Shipping & Routes)
            Container.BindInterfacesAndSelfTo<LogisticsService>()
                .AsSingle()
                .NonLazy();
        }
    }
}
