using System.Numerics;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Shaders;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;

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
        Graphics.BeginTextureMode(output);
        Graphics.BeginShaderMode(Shader);

        SetUniforms(Shader);

        Graphics.DrawTexturePro(
            input,
            new Rectangle(0, 0, input.Width, -input.Height),
            new Rectangle(0, 0, output.Texture.Width, output.Texture.Height),
            Vector2.Zero,
            0f,
            Color.White
        );

        Graphics.EndShaderMode();
        Graphics.EndTextureMode();
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
