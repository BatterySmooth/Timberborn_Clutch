using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Persistence;
using Timberborn.ConstructibleSystem;
using Timberborn.MechanicalSystem;
using Bindito.Core;

namespace Clutch
{
  public class ClutchSystem : BaseComponent, IPersistentEntity, IFinishedStateListener
  {
    // Whether the clutch is Closed or open
    private bool _closed = true;

    public bool Closed => _closed;
    private static readonly ComponentKey ClutchManagerKey = new(nameof(ClutchSystem));

    private static readonly PropertyKey<bool> ClutchKey = new("Closed");
    private MechanicalNode _mechanicalNode;
    private MechanicalNode _mechanicalNodeCache;
    private MechanicalGraphManager _mechanicalGraphManager;

    [Inject]
    public void InjectDependencies(MechanicalGraphManager mechanicalGraphManager) => this._mechanicalGraphManager = mechanicalGraphManager;

    private void Awake()
    {
      _mechanicalNode = this.GetComponentFast<MechanicalNode>();
      // _mechanicalNodeCache = _mechanicalNode;
      // _mechanicalNode.enabled = false;
      // _mechanicalNode.Active = false;
      enabled = false;
      // TryUpdate("Awake");
    }

    void IPersistentEntity.Load(IEntityLoader entityLoader)
    {
      if (!entityLoader.HasComponent(ClutchManagerKey))
        return;
      var component = entityLoader.GetComponent(ClutchManagerKey);
      if (component.Has(ClutchKey))
      {
        _closed = component.Get(ClutchKey);
        Plugin.Log.LogInfo($"Load w/ Clutch key: {_closed}");
        // TryUpdate("Load");
      }
      // if (_closed)
      // {
      //   _mechanicalNode.Active = true;
      // }
    }

    void IPersistentEntity.Save(IEntitySaver entitySaver)
    {
      entitySaver.GetComponent(ClutchManagerKey).Set(ClutchKey, _closed);
    }

    void IFinishedStateListener.OnEnterFinishedState()
    {
      enabled = true;
      // TryUpdate("OnEnter");
    }

    void IFinishedStateListener.OnExitFinishedState()
    {
      // TryUpdate("OnExit");
      enabled = false;
    }
    
    // Externally set new Clutch value
    public void SetClutchValue(bool newValue)
    {
      _closed = newValue;
      UpdateClutch();
    }
    
    private void UpdateClutch()
    {
      if (_closed)
      {
        EnableClutch();
      }
      else
      {
        DisableClutch();
      }
    }

    private void EnableClutch()
    {
      _closed = true;
      // _mechanicalNode.Active = true;
      _mechanicalGraphManager.AddNode(_mechanicalNode);
      _mechanicalNode.enabled = true;
    }

    private void DisableClutch()
    {
      _closed = false;
      _mechanicalNode.enabled = false;
      _mechanicalGraphManager.RemoveNode(_mechanicalNode);
    }

    private void TryUpdate(string method)
    {
      try
      {
        UpdateClutch();
      }
      catch (Exception e)
      {
        Plugin.Log.LogWarning($"Failed to update in {method}:\n{e}");
      }
    }
  }
}
