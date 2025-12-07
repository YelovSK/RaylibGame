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
    protected PostProcessor PostProcessor { get; private set; }
    protected RenderTexture2D RenderTarget { get; private set; }

    protected readonly int VirtualWidth;
    protected readonly int VirtualHeight;
    protected readonly string Title;
    
    /// <summary>
    /// Exits cleanly after finishing the current frame, cleanup etc.
    /// </summary>
    public void Close() => _closeRequested = true;
    private bool _closeRequested;

    public Application(int virtualWidth, int virtualHeight, string title)
    {
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
    protected abstract IEnumerable<IPostProcessPass> InitializeShaders();

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
            var dt = Time.GetFrameTime();

            // Update
            InputBuffer.Instance.Gather();
            Update(dt);

            // Draw
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
        }
        
        // Cleanup
        RenderTarget.Unload();
        Window.Close();
    }
}