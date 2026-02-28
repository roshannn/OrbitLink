using Zenject;
using OrbitLink.Core;
using OrbitLink.Data;
using OrbitLink.Services;

namespace OrbitLink.Installers
{
    /// <summary>
    /// Binds all pure data services and logic systems as singletons for dependency injection.
    /// Attach this script to a SceneContext or ProjectContext GameObject.
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Data / State ownership
            Container.Bind<GameSession>().AsSingle();

            // Core Deterministic Services
            Container.Bind<TimeManager>().AsSingle();
            Container.Bind<OrbitSystem>().AsSingle();
            Container.Bind<RouteSystem>().AsSingle();
            Container.Bind<ShipSystem>().AsSingle();
            Container.Bind<CollisionSystem>().AsSingle();

            // Meta Services
            Container.Bind<EconomySystem>().AsSingle();
            Container.Bind<PrestigeSystem>().AsSingle();
            Container.Bind<SaveLoadSystem>().AsSingle();
            
            // Note: GameManager will be injected with these via [Inject]
        }
    }
}
