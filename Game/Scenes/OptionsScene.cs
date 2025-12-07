using Engine;
using Engine.Components;
using Engine.Helpers;
using Game.Persistence;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Windowing;

namespace Game.Scenes;

public class OptionsScene : Scene
{
    private const float SETTING_HEIGHT = Layout.VIRTUAL_HEIGHT * 0.05f;
    private const float SETTING_OFFSET = SETTING_HEIGHT * 1.5f;

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
        var middle = Layout.Center(0, 0);
        middle.Y += SETTING_OFFSET * _settingsCount;
        
        var textGo = new Entity(this, middle);
        var textComponent = textGo.AddComponent(new TextComponent()
        {
            Text = text,
        });
        textGo.Transform.Position.X -= textComponent.TextSize() + SETTING_HEIGHT * 0.7f;
        AddEntity(textGo);
        
        var go = new Entity(this, middle);
        _ = go.AddComponent(new CheckboxComponent()
        {
            IsChecked = defaultValue,
            OnClick = setter,
            Size = SETTING_HEIGHT,
        });
        AddEntity(go);
        _settingsCount++;
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if (Input.IsKeyPressed(KeyboardKey.Escape))
        {
            SceneManager.Instance.LoadScene(new MenuScene());
        }
    }
}