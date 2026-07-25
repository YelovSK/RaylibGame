using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Extensions;
using Game.Components;
using Game.Persistence;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Windowing;

namespace Game.Scenes;

public class OptionsScene : Scene
{
    private readonly float SETTING_HEIGHT = VirtualViewport.Height * 0.05f;
    private float SETTING_OFFSET => SETTING_HEIGHT * 1.5f;

    private int _settingsCount;
    
    public override void Load()
    {
        CreateEntity().AddComponent<OptionsSceneController>();
        
        var background = CreateEntity();
        var backgroundSprite = background.AddComponent<SpriteComponent>();
        backgroundSprite.Width = VirtualViewport.Width;
        backgroundSprite.Height = VirtualViewport.Height;
        backgroundSprite.Color = Color.SkyBlue;
        
        // Vsync
        AddSetting("VSync", Settings.Instance.IsVsyncEnabled, (isChecked) =>
        {
            Settings.Instance.IsVsyncEnabled = isChecked;
            if (isChecked)
            {
                Window.SetState(ConfigFlags.VSyncHint);
            }
            else
            {
                Window.ClearState(ConfigFlags.VSyncHint);
            }
        });
        
        // FPS
        AddSetting("Show FPS", Settings.Instance.ShowFps, isChecked => Settings.Instance.ShowFps = isChecked);
        
        // Shaders
        AddSetting("Enable Shaders", Settings.Instance.EnableShaders, isChecked => Settings.Instance.EnableShaders = isChecked);
        
        AddSetting("Fullscreen", Settings.Instance.IsFullScreen, isChecked =>
        {
            if (isChecked)
            {
                Window.SetState(ConfigFlags.BorderlessWindowMode);
            }
            else
            {
                Window.ClearState(ConfigFlags.BorderlessWindowMode);
            }
            Settings.Instance.IsFullScreen = isChecked;
        });
    }

    private void AddSetting(string text, bool defaultValue, Action<bool> setter)
    {
        var offset = new Vector2(0, SETTING_OFFSET * _settingsCount);

        var textGo = CreateEntity();
        var textComponent = textGo.AddComponent<TextComponent>();
        textComponent.Text = text;
        var textSize = textComponent.TextSize();
        textComponent.Rect.Anchor = new Vector2(0.5f);
        textComponent.Rect.Offset = offset - Vector2.X(textSize + SETTING_HEIGHT * 0.7f);
        textComponent.Rect.Size = new Vector2(textSize, SETTING_HEIGHT);

        var go = CreateEntity();
        var checkbox = go.AddComponent<CheckboxComponent>();
        checkbox.Rect.Anchor = new Vector2(0.5f);
        checkbox.Rect.Offset = offset;
        checkbox.Rect.Size = new Vector2(SETTING_HEIGHT);
        checkbox.IsChecked = defaultValue;
        checkbox.OnClick = setter;
        
        _settingsCount++;
    }
}
