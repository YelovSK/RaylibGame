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

    private readonly Shader _shader = ResourceManager.Instance.LoadShader(null, shaderName);

    public void Apply(Texture2D input, RenderTexture2D output)
    {
        Graphics.BeginTextureMode(output);
        Graphics.BeginShaderMode(_shader);

        SetUniforms(_shader);

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

    protected virtual void SetUniforms(Shader shader) { }
}
