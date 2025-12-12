using Engine;
using Engine.PostProcessing;
using Game.Persistence;
using Game.PostProcessing;
using Game.Scenes;
using Raylib_CSharp;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Windowing;

namespace Game;

public class Game() : Application(VIRTUAL_WIDTH, VIRTUAL_HEIGHT, TITLE)
{
    private const string TITLE = "Platformer";
    private const int VIRTUAL_WIDTH = 640;
    private const int VIRTUAL_HEIGHT = 360;

    protected override void BeforeWindowInit()
    {
        Settings.Instance.Load();
        SaveData.Instance.Load();

        if (Settings.Instance.IsVsyncEnabled)
        {
            Window.SetState(ConfigFlags.VSyncHint);
        }

        // This doesn't actually do anything. Broken piece of shit.
        if (Settings.Instance.IsFullScreen)
        {
            Raylib.SetConfigFlags(ConfigFlags.BorderlessWindowMode);
        }
        
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
    }

    protected override void AfterWindowInit()
    {
        Input.SetExitKey(KeyboardKey.Null);

        //SceneManager.Instance.Push(new PerformanceTestScene());
        SceneManager.Instance.Push(new MenuScene());
    }

    protected override void BeforeDrawEnd()
    {
        if (Settings.Instance.ShowFps)
        {
            Graphics.DrawFPS(0, 0);
        }
    }

    protected override void OnExit()
    {
        Settings.Instance.Save();
        SaveData.Instance.Save();
    }

    protected override bool OnException(Exception exception)
    {
        // log or sth
        return true;
    }

    protected override IEnumerable<IPostProcessPass> GetVirtualShaders()
    {
        return
        [
        ];
    }

    protected override IEnumerable<IPostProcessPass> GetShaders()
    {
        return
        [
            new FullscreenShaderPass("crt.fs", () => Settings.Instance.EnableShaders),
            new FullscreenShaderPass("bloom.fs", () => Settings.Instance.EnableShaders),
            new GrainPass(),
            new FullscreenShaderPass("aces.fs", () => Settings.Instance.EnableShaders),
        ];
    }
}
