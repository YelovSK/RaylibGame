
using Raylib_cs;

namespace Engine.PostProcessing;

public interface IPostProcessPass
{
    void Apply(Texture2D input, RenderTexture2D output);
    bool IsEnabled();
}
