using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;
using Timberborn.EntityPanelSystem;

namespace Clutch
{
  [Configurator(SceneEntrypoint.InGame)]
  public class ClutchUIConfigurator : IConfigurator
  {
    public void Configure(IContainerDefinition containerDefinition)
    {
      containerDefinition.Bind<ClutchUIFragment>().AsSingleton();
      containerDefinition.MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
    }

    private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
    {
      private readonly ClutchUIFragment _clutchUIFragment;

      public EntityPanelModuleProvider(ClutchUIFragment clutchUIFragment)
      {
        _clutchUIFragment = clutchUIFragment;
      }

      public EntityPanelModule Get()
      {
        EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
        builder.AddTopFragment(_clutchUIFragment);
        return builder.Build();
      }
    }
  }
}