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
    
    private RenderTexture2D _virtualRenderTarget;
    private RenderTexture2D _renderTarget;
    
    private PostProcessor _virtualPostProcessor;
    private PostProcessor _postProcessor;

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
    protected virtual void Update(float dt) => WorldManager.Instance.Update(dt);
    protected virtual void FixedUpdate() { }
    protected virtual void LateUpdate(float dt) { }
    protected virtual void Draw()
    {
        Graphics.ClearBackground(Color.Black);
        WorldManager.Instance.Draw();
    }
    /// <summary>
    /// Do the final drawing here.
    /// </summary>
    protected virtual void BeforeDrawEnd() { }
    protected abstract void OnExit();
    /// <returns>Return true to throw exception, and false to continue.</returns>
    protected virtual bool OnException(Exception exception) => true;
    /// <summary>
    /// Shaders get applied to the low res render texture with virtual resolution.
    /// </summary>
    protected virtual IEnumerable<IPostProcessPass> GetVirtualShaders() => [];
    /// <summary>
    /// Shaders get applied to the final high resolution texture.
    /// </summary>
    protected virtual IEnumerable<IPostProcessPass> GetShaders() => [];

    public void Run()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            OnExit();
        }; 
        
        BeforeWindowInit();

        Window.Init(Window.GetScreenWidth(), Window.GetScreenHeight(), Title);
        
        _virtualRenderTarget  = RenderTexture2D.Load(VirtualWidth, VirtualHeight);
        _virtualRenderTarget.Texture.SetFilter(TextureFilter.Point);
        
        _renderTarget = RenderTexture2D.Load(Window.GetScreenWidth(), Window.GetScreenHeight());
        _renderTarget.Texture.SetFilter(TextureFilter.Bilinear);
        
        _virtualPostProcessor = new PostProcessor(VirtualWidth, VirtualHeight, GetVirtualShaders(), TextureFilter.Point);
        _postProcessor = new PostProcessor(Window.GetScreenWidth(), Window.GetScreenHeight(), GetShaders(), TextureFilter.Bilinear);
        
        AfterWindowInit();

        double accumulator = 0;
        while (!Window.ShouldClose() && !_closeRequested)
        {
            try
            {
                if (Window.IsResized())
                {
                    OnWindowResized();
                }
                
                var dt = Time.GetFrameTime();

                // Update
                var updateStart = Time.GetTime();
                InputManager.Instance.Gather();
                Update(dt);
                
                // Fixed update
                accumulator += dt;
                while (accumulator >= FixedTime.TICK_RATE)
                {
                    FixedUpdate();
                    FixedTime.Ticks++;
                    accumulator -= FixedTime.TICK_RATE;
                }
                
                LateUpdate(dt);
                
                var updateEnd = Time.GetTime();
                var alpha = (float)(accumulator / FixedTime.TICK_RATE);
                
                UpdateTimeMs = (updateEnd - updateStart) * 1000;

                // Draw in virtual resolution
                var drawStart = Time.GetTime();
                Graphics.BeginTextureMode(_virtualRenderTarget);
                Draw();
                Graphics.EndTextureMode();

                // Apply shaders to low res texture
                var virtualRenderTargetPp = _virtualPostProcessor.Apply(_virtualRenderTarget.Texture);
                
                // Scale virtual res texture up
                Graphics.BeginTextureMode(_renderTarget);
                BlitToScreen(virtualRenderTargetPp);
                Graphics.EndTextureMode();
                
                // Apply shaders to full res texture
                var renderTargetPp = _postProcessor.Apply(_renderTarget.Texture); 
                
                Graphics.BeginDrawing();
                Graphics.ClearBackground(Color.Black);
                Graphics.DrawTexturePro(
                    renderTargetPp,
                    new Rectangle(0, 0, Window.GetScreenWidth(), -Window.GetScreenHeight()),
                    new Rectangle(0, 0, Window.GetScreenWidth(), Window.GetScreenHeight()),
                    Vector2.Zero,
                    0.0f,
                    Color.White
                );
                BeforeDrawEnd();
                Graphics.EndDrawing(); 

                // Scale up
                var drawEnd  = Time.GetTime();
                DrawTimeMs = (drawEnd - drawStart) * 1000;
            }
            catch (Exception e)
            {
                if (OnException(e)) throw;
            }
        }
        
        // Cleanup
        _renderTarget.Unload();
        _virtualRenderTarget.Unload();
        Window.Close();
    }

    private void OnWindowResized()
    {
        _renderTarget.Unload();
        _renderTarget = RenderTexture2D.Load(Window.GetScreenWidth(), Window.GetScreenHeight());
        _renderTarget.Texture.SetFilter(TextureFilter.Bilinear);
        
        _postProcessor.Dispose();
        _postProcessor = new PostProcessor(Window.GetScreenWidth(), Window.GetScreenHeight(), GetShaders(), TextureFilter.Bilinear);
    }
    
    private void BlitToScreen(Texture2D finalTexture)
    {
        Graphics.ClearBackground(Color.Black);
        
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
}