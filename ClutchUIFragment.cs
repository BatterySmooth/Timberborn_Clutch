using System;
using TimberApi.UiBuilderSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
// using Timberborn.SliderToggleSystem;
using Timberborn.AssetSystem;

// using System.Reflection;
// using TimberApi.UiBuilderSystem.ElementSystem;
using Timberborn.Localization;


namespace Clutch
{
  public class ClutchUIFragment : IEntityPanelFragment
  {
    private readonly UIBuilder _uiBuilder;
    // private readonly VisualElementLoader _visualElementLoader;
    private readonly IResourceAssetLoader _resourceAssetLoader;
    private readonly ILoc _loc;
    
    private VisualElement _root;
    private ClutchSystem _clutch;
    // public ClutchSystem ClutchSystem => _clutch;

    private Button _clutchEnableButton;
    private string _uiEnableText;
    private string _uiDisableText;
    private StyleBackground _closedBackground;
    private StyleBackground _fragmentBackground;

    public ClutchUIFragment(UIBuilder uiBuilder, IResourceAssetLoader resourceAssetLoader, ILoc loc)
    {
      _uiBuilder = uiBuilder;
      // _visualElementLoader = visualElementLoader;
      _resourceAssetLoader = resourceAssetLoader;
      _loc = loc;
    }

    public VisualElement InitializeFragment()
    {
      // Fetch Localisation strings
      _uiEnableText = _loc.T("battery.Clutch.UIButtonEnable");
      _uiDisableText = _loc.T("battery.Clutch.UIButtonDisable");
      // Set disabled background variable
      _closedBackground = new StyleBackground(_resourceAssetLoader.Load<Sprite>("battery.Clutch/battery_clutch/button-red"));
      _fragmentBackground = new StyleBackground(_resourceAssetLoader.Load<Sprite>("battery.Clutch/battery_clutch/background"));
      
      // Build root element
      _root = _uiBuilder.CreateFragmentBuilder()
        .SetBackground(TimberApiStyle.Backgrounds.Bg3)
        // .ModifyWrapper(builder => builder.SetBackgroundImage(_fragmentBackground))
        .AddComponent(builder =>
        {
          builder
            .SetName("ClutchButtonContainer")
            .SetFlexDirection(FlexDirection.Row)
            .SetFlexWrap(Wrap.Wrap)
            .SetJustifyContent(Justify.SpaceAround)
            .SetBackgroundImage(_fragmentBackground)
            .AddComponent(BuildButton());
        })
        .BuildAndInitialize();
      
      // Set up Button and Callbacks
      _clutchEnableButton = _root.Q<Button>("ClutchEnableButton");
      _clutchEnableButton.clicked += () => ButtonToggle();

      // Hide root element and return
      _root.ToggleDisplayStyle(false);

      return _root;
    }
    private VisualElement BuildButton()
    {
      VisualElement root = _uiBuilder.CreateComponentBuilder().CreateVisualElement()
        .AddComponent(_uiBuilder.CreateComponentBuilder().CreateButton()
          .SetName("ClutchEnableButton")
          .AddClass(TimberApiStyle.Buttons.Normal.Button)
          .AddClass(TimberApiStyle.Sounds.Click)
          .SetHeight(new Length(44, LengthUnit.Pixel))
          .SetWidth(new Length(254, LengthUnit.Pixel))
          .SetMargin(new Length(10, LengthUnit.Pixel))
          .SetColor(new StyleColor(Color.white))
          .Build()
        )
        .Build();
      return root;
    }
    
    public void ShowFragment(BaseComponent entity)
    {
      _clutch = entity.GetComponentFast<ClutchSystem>();
      UpdateUI();
    }

    public void ClearFragment()
    {
      _root.ToggleDisplayStyle(false);
      _clutch = null;
    }

    public void UpdateFragment()
    {
      Plugin.Log.LogInfo($"Refresh Fragment. Clutch: {(bool)_clutch}");
      if (!(bool)_clutch)
        return;
      UpdateUI();
    }

    private void ButtonToggle()
    {
      _clutch.SetClutchValue(!_clutch.Closed);
    }

    private void UpdateUI()
    {
      if (!(bool)(Object)_clutch || !_clutch.enabled) // .enabled is used to only when building is complete
      {
        _root.ToggleDisplayStyle(false);
        return;
      }
      _clutchEnableButton.text = _clutch.Closed ? _uiDisableText : _uiEnableText;
      _clutchEnableButton.style.backgroundImage = _clutch.Closed ? _closedBackground : StyleKeyword.Null;
      Plugin.Log.LogInfo($"Updated UI From Show Fragment. Closed: {_clutch.Closed}");
      _root.ToggleDisplayStyle(true);
    }
  }
}

// OLD BUTTON/ FRAGMENT GENERATION
// _root = _uiBuilder.CreateFragmentBuilder()
//   // .SetFlexDirection(FlexDirection.Row)
//   // .SetFlexWrap(Wrap.Wrap)
//   // .SetJustifyContent(Justify.SpaceAround)
//   // .SetName("ClutchButtonContainer")
//   .SetBackground(TimberApiStyle.Backgrounds.Bg3)
//   .AddComponent(BuildGreenButton())
//   .AddComponent(BuildRedButton())
//   .Build();
      
// _root = _uiBuilder.CreateComponentBuilder().CreateVisualElement()
//   .AddComponent(_uiBuilder.CreateComponentBuilder().CreateButton()
//     .SetName("ClutchEnableButton")
//     .AddClass(TimberApiStyle.Buttons.Normal.Button)
//     .AddClass(TimberApiStyle.Buttons.Hover.ButtonHover)
//     .AddClass(TimberApiStyle.Sounds.Click)
//     .SetJustifyContent(Justify.Center)
//     .SetAlignItems(Align.Center)
//     .SetFlexDirection(FlexDirection.Row)
//   ).Build();

// TOBBERT'S METHOD OF GENERATING THE UI
// var clutchFragment = _uiBuilder.CreateFragmentBuilder()
//   .AddComponent(builder => builder
//     .SetFlexDirection(FlexDirection.Column)
//     .SetWidth(new Length(100, LengthUnit.Percent))
//     .SetJustifyContent(Justify.Center)
//     .AddPreset(builder =>
//     {
//       var button = builder.Buttons().Button();
//       button.name = "ClutchButton";
//       button.text = _uiEnableText;
//       return button;
//     }))
//   .BuildAndInitialize();
//
// _root = new VisualElement();
// _root.Add(clutchFragment);
//
// clutchFragment.Q<Button>("ClutchButton").clicked += () => ButtonToggle();

// OTHER BUTTON GENERATION SYNTAX
// _root = _uiBuilder.CreateFragmentBuilder()
//   .AddComponent(_uiBuilder
//     .CreateComponentBuilder()
//     .CreateVisualElement()
//     .SetName("ClutchContainer")
//     .BuildAndInitialize()
//   )
//   .AddComponent(_uiBuilder
//     .CreateComponentBuilder()
//     .CreateButton()
//     .AddClass("entity-fragment__button--green")
//     .SetName("ClutchButton")
//     .Build()
//   )
//   .BuildAndInitialize();