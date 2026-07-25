using System.Numerics;
using Engine.PostProcessing;

using Raylib_cs;

namespace Engine;

public abstract class Application
{
    public string Title { get; private set; }

    /// <summary>
    /// Exits cleanly after finishing the current frame, cleanup etc.
    /// </summary>
    public void Close() => _closeRequested = true;
    private bool _closeRequested;

    public double UpdateTimeMs { get; private set; }
    public double DrawTimeMs { get; private set; }

    private RenderTexture2D _virtualRenderTarget;
    private RenderTexture2D _renderTarget;

    private PostProcessor _virtualPostProcessor;
    private PostProcessor _postProcessor;

    protected Application(int virtualWidth, int virtualHeight, string title)
    {
        VirtualViewport.Initialize(virtualWidth, virtualHeight);
        Title = title;
    }

    // Abstract
    protected abstract void BeforeWindowInit();
    protected abstract void AfterWindowInit();
    protected virtual void Update(float dt) => SceneManager.Instance.Update(dt);
    protected virtual void FixedUpdate() => SceneManager.Instance.FixedUpdate();
    protected virtual void Draw(float alpha, float dt)
    {
        Raylib.ClearBackground(Color.Black);
        SceneManager.Instance.Draw(alpha, dt);
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
        _closeRequested = false;

        BeforeWindowInit();

        Raylib.InitWindow(VirtualViewport.Width * 2, VirtualViewport.Height * 2, Title);

        _virtualRenderTarget = Raylib.LoadRenderTexture(VirtualViewport.Width, VirtualViewport.Height);
        Raylib.SetTextureFilter(_virtualRenderTarget.Texture, TextureFilter.Point);

        _renderTarget = Raylib.LoadRenderTexture(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        Raylib.SetTextureFilter(_renderTarget.Texture, TextureFilter.Bilinear);

        _virtualPostProcessor = new PostProcessor(
            VirtualViewport.Width,
            VirtualViewport.Height,
            GetVirtualShaders(),
            TextureFilter.Point
        );
        _postProcessor = new PostProcessor(Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), GetShaders(), TextureFilter.Bilinear);

        try
        {
            AfterWindowInit();

            double accumulator = 0;
            while (!Raylib.WindowShouldClose() && !_closeRequested)
            {
                try
                {
                    if (Raylib.IsWindowResized())
                    {
                        OnWindowResized();
                    }

                    var dt = Raylib.GetFrameTime();

                    // Update
                    var updateStart = Raylib.GetTime();
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

                    SceneManager.Instance.EndFrame();

                    var updateEnd = Raylib.GetTime();
                    var alpha = (float)(accumulator / FixedTime.TICK_RATE);

                    UpdateTimeMs = (updateEnd - updateStart) * 1000;

                    // Draw in virtual resolution
                    var drawStart = Raylib.GetTime();
                    Raylib.BeginTextureMode(_virtualRenderTarget);
                    Draw(alpha, dt);
                    Raylib.EndTextureMode();

                    // Apply shaders to low res texture
                    var virtualRenderTargetPp = _virtualPostProcessor.Apply(_virtualRenderTarget.Texture);

                    // Scale virtual res texture up
                    Raylib.BeginTextureMode(_renderTarget);
                    BlitToScreen(virtualRenderTargetPp);
                    Raylib.EndTextureMode();

                    // Apply shaders to full res texture
                    var renderTargetPp = _postProcessor.Apply(_renderTarget.Texture);

                    Raylib.BeginDrawing();
                    Raylib.ClearBackground(Color.Black);
                    Raylib.DrawTexturePro(
                        renderTargetPp,
                        new Rectangle(0, 0, Raylib.GetScreenWidth(), -Raylib.GetScreenHeight()),
                        new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
                        Vector2.Zero,
                        0.0f,
                        Color.White
                    );
                    BeforeDrawEnd();
                    Raylib.EndDrawing();

                    var drawEnd = Raylib.GetTime();
                    DrawTimeMs = (drawEnd - drawStart) * 1000;
                }
                catch (Exception e)
                {
                    if (OnException(e)) throw;
                }
            }
        }
        finally
        {
            OnExit();
            Raylib.CloseWindow();
        }
    }

    private void OnWindowResized()
    {
        Raylib.UnloadRenderTexture(_renderTarget);
        _renderTarget = Raylib.LoadRenderTexture(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        Raylib.SetTextureFilter(_renderTarget.Texture, TextureFilter.Bilinear);

        _postProcessor.Dispose();
        _postProcessor = new PostProcessor(Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), GetShaders(), TextureFilter.Bilinear);
    }

    private void BlitToScreen(Texture2D finalTexture)
    {
        Raylib.ClearBackground(Color.Black);
        Rectangle source = new(0, 0, VirtualViewport.Width, -VirtualViewport.Height);

        Raylib.DrawTexturePro(finalTexture, source, VirtualViewport.Destination, Vector2.Zero, 0.0f, Color.White);
    }
}
