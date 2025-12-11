using System.Numerics;
using Engine.PostProcessing;
using Raylib_CSharp;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Windowing;

namespace Engine;

public abstract class Application
{
    // To explain, the issue is that I want the game itself to define
    // the virtual resolution, but I still need to have access to the virtual resolution
    // in the engine project, e.g. for getting the virtual mouse coordinates in components.
    /// <summary>
    /// Will be null if an instance wasn't created with the constructor.
    /// </summary>
    public static Application Instance { get; private set; }
    
    protected PostProcessor PostProcessor { get; private set; }
    protected RenderTexture2D RenderTarget { get; private set; }

    public int VirtualWidth { get; private set; }
    public int VirtualHeight  { get; private set; }
    public string Title  { get; private set; }
    
    /// <summary>
    /// Exits cleanly after finishing the current frame, cleanup etc.
    /// </summary>
    public void Close() => _closeRequested = true;
    private bool _closeRequested;
    
    public double UpdateTimeMs  { get; private set; }
    public double DrawTimeMs  { get; private set; }

    protected Application(int virtualWidth, int virtualHeight, string title)
    {
        Instance = this;
        
        VirtualWidth = virtualWidth;
        VirtualHeight = virtualHeight;
        Title = title;
    }

    // Abstract
    protected abstract void BeforeWindowInit();
    protected abstract void AfterWindowInit();
    protected abstract void Update(float dt);
    protected abstract void Draw();
    protected abstract void OnExit();
    /// <returns>Return true to throw exception, and false to continue.</returns>
    protected virtual bool OnException(Exception exception) => true;
    protected virtual IEnumerable<IPostProcessPass> InitializeShaders() => [];

    protected virtual void DrawFinalFrame(Texture2D finalTexture)
    {
        var screenWidth = Window.GetScreenWidth();
        var screenHeight = Window.GetScreenHeight();
        
        var scale = Math.Min((float)screenWidth / VirtualWidth, (float)screenHeight / VirtualHeight);
        Rectangle dest = new(
            (screenWidth - VirtualWidth * scale) / 2,
            (screenHeight - VirtualHeight * scale) / 2,
            VirtualWidth * scale,
            VirtualHeight * scale
        );
        Rectangle source = new(0, 0, VirtualWidth, -VirtualHeight);

        Graphics.DrawTexturePro(finalTexture, source, dest, Vector2.Zero, 0.0f, Color.White);
    }
    
    public void Run()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            OnExit();
        }; 
        
        BeforeWindowInit();

        Window.Init(Window.GetScreenWidth(), Window.GetScreenHeight(), Title);
        
        RenderTarget = RenderTexture2D.Load(VirtualWidth, VirtualHeight);
        RenderTarget.Texture.SetFilter(TextureFilter.Point);
        
        PostProcessor = new PostProcessor(VirtualWidth, VirtualHeight);
        foreach (var pass in InitializeShaders())
        {
            PostProcessor.AddPass(pass);
        }
        
        AfterWindowInit();

        while (!Window.ShouldClose() && !_closeRequested)
        {
            try
            {
                var dt = Time.GetFrameTime();

                // Update
                var updateStart = Time.GetTime();
                InputBuffer.Instance.Gather();
                Update(dt);
                var updateEnd = Time.GetTime();
                UpdateTimeMs = (updateEnd - updateStart) * 1000;

                // Draw
                var drawStart = Time.GetTime();
                Graphics.BeginTextureMode(RenderTarget);
                Draw();
                Graphics.EndTextureMode();

                // Post-process
                var postProcessedTexture = PostProcessor.Apply(RenderTarget.Texture);

                // Scale up
                Graphics.BeginDrawing();
                Graphics.ClearBackground(Color.Black);
                DrawFinalFrame(postProcessedTexture);
                Graphics.EndDrawing();
                var drawEnd  = Time.GetTime();
                DrawTimeMs = (drawEnd - drawStart) * 1000;
            }
            catch (Exception e)
            {
                if (OnException(e)) throw;
            }
        }
        
        // Cleanup
        RenderTarget.Unload();
        Window.Close();
    }
}