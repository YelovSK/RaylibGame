using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Enums;
using Engine.Extensions;
using Engine.Helpers;
using Game.Persistence;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Windowing;

namespace Game.Scenes;

public class OptionsScene : Scene
{
    private readonly float SETTING_HEIGHT = Application.Instance.VirtualHeight * 0.05f;
    private float SETTING_OFFSET => SETTING_HEIGHT * 1.5f;

    private int _settingsCount;
    
    public override void Load()
    {
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
        var middle = VirtualLayout.Center(0, 0);
        middle.Y += SETTING_OFFSET * _settingsCount;
        
        var textGo = new Entity(this, middle);
        var textComponent = textGo.AddComponent(new TextComponent()
        {
            Text = text,
            //RenderSpace = RenderSpace.Screen
        });
        textGo.Transform.Position -= Vector2.X(textComponent.TextSize() + SETTING_HEIGHT * 0.7f);
        
        var go = new Entity(this, middle);
        _ = go.AddComponent(new CheckboxComponent()
        {
            IsChecked = defaultValue,
            OnClick = setter,
            Size = SETTING_HEIGHT,
            //RenderSpace = RenderSpace.Screen
        });
        _settingsCount++;
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if (Input.IsKeyPressed(KeyboardKey.Escape))
        {
            SceneManager.Instance.Pop();
        }
    }
}