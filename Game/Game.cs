using Engine;
using Engine.Helpers;
using Engine.PostProcessing;
using Game.Persistence;
using Game.Scenes;
using Raylib_CSharp;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Windowing;

namespace Game;

public class Game : Application
{
    public const string TITLE = "Platformer";
    public const int VIRTUAL_WIDTH = 640;
    public const int VIRTUAL_HEIGHT = 360;
    
    private static Game _instance;
    private static readonly Lock _lockObj = new();
    public static Game Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lockObj)
                {
                    _instance ??= new Game();
                }
            }

            return _instance;
        }
    }

    private Game() : base(VIRTUAL_WIDTH, VIRTUAL_HEIGHT, TITLE) { }

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
    }

    protected override void AfterWindowInit()
    {
        Input.SetExitKey(KeyboardKey.Null);
        
        SceneManager.Instance.LoadScene(new MenuScene());
    }
    
    protected override void Update(float dt)
    {
        var scale = Math.Min(
            Window.GetScreenWidth() / (float)VIRTUAL_WIDTH,
            Window.GetScreenHeight() / (float)VIRTUAL_HEIGHT
        );
    
        var offsetX = (Window.GetScreenWidth() - VIRTUAL_WIDTH * scale) / 2;
        var offsetY = (Window.GetScreenHeight() - VIRTUAL_HEIGHT * scale) / 2;
        Input.SetMouseScale(1f / scale, 1f/ scale);
        Input.SetMouseOffset((int)offsetX, -(int)offsetY);
        Console.WriteLine(Input.GetMousePosition());
        
        SceneManager.Instance.Update(dt);
    }
    
    protected override void Draw()
    {
        Graphics.ClearBackground(Color.SkyBlue);
        SceneManager.Instance.Draw();
    }

    protected override void DrawFinalFrame(Texture2D finalTexture)
    {
        base.DrawFinalFrame(finalTexture);
        
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
    
    protected override IEnumerable<IPostProcessPass> InitializeShaders()
    {
        return
        [
            new FullscreenShaderPass("bloom.fs"),
            new FullscreenShaderPass("crt.fs"),
        ];
    }
}