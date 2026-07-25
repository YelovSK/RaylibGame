using System.Numerics;

using Raylib_cs;

namespace Engine.PostProcessing;

public class FullscreenShaderPass(string shaderName, Func<bool> enabledFunc) : IPostProcessPass
{
    public virtual bool IsEnabled() => enabledFunc();

    public FullscreenShaderPass(string shaderName) : this(shaderName, () => true)
    {
    }

    protected readonly Shader Shader = ResourceManager.Instance.LoadShader(null, shaderName);

    public void Apply(Texture2D input, RenderTexture2D output)
    {
        Raylib.BeginTextureMode(output);
        Raylib.BeginShaderMode(Shader);

        SetUniforms(Shader);

        Raylib.DrawTexturePro(
            input,
            new Rectangle(0, 0, input.Width, -input.Height),
            new Rectangle(0, 0, output.Texture.Width, output.Texture.Height),
            Vector2.Zero,
            0f,
            Color.White
        );

        Raylib.EndShaderMode();
        Raylib.EndTextureMode();
    }

    /// <summary>
    /// Set uniform values here.
    /// </summary>
    protected virtual void SetUniforms(Shader shader) { }
    
    public void Dispose()
    {
        ResourceManager.Instance.UnloadShader(null, shaderName);
    }
}
