using Raylib_CSharp.Textures;

namespace Engine.PostProcessing;

public interface IPostProcessPass
{
    Texture2D Apply(Texture2D input, RenderTexture2D output);
}
