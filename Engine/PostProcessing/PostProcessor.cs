using Raylib_CSharp.Textures;

namespace Engine.PostProcessing;

public class PostProcessor
{
    private readonly List<IPostProcessPass> _passes = [];
    private readonly RenderTexture2D _bufferA;
    private readonly RenderTexture2D _bufferB;

    public PostProcessor(int width, int height)
    {
        _bufferA = RenderTexture2D.Load(width, height);
        _bufferB = RenderTexture2D.Load(width, height);

        _bufferA.Texture.SetFilter(TextureFilter.Point);
        _bufferB.Texture.SetFilter(TextureFilter.Point);
    }

    public void AddPass(IPostProcessPass pass)
    {
        _passes.Add(pass);
    }

    public Texture2D Apply(Texture2D input)
    {
        var sourceTex = input;
    
        var destBuffer = _bufferA;
        var otherBuffer = _bufferB;

        foreach (var pass in _passes)
        {
            if (!pass.IsEnabled()) continue;
            
            pass.Apply(sourceTex, destBuffer);

            sourceTex = destBuffer.Texture;

            // Swap buffers for the next pass
            (destBuffer, otherBuffer) = (otherBuffer, destBuffer);
        }

        return sourceTex; 
    }
}
