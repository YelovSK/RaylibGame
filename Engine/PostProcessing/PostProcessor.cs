
using Raylib_cs;

namespace Engine.PostProcessing;

public class PostProcessor : IDisposable
{
    private readonly List<IPostProcessPass> _passes = [];
    private readonly RenderTexture2D _bufferA;
    private readonly RenderTexture2D _bufferB;

    public PostProcessor(int width, int height)
    {
        _bufferA = Raylib.LoadRenderTexture(width, height);
        _bufferB = Raylib.LoadRenderTexture(width, height);
    }
    
    public PostProcessor(int width, int height, IEnumerable<IPostProcessPass> passes, TextureFilter textureFilter) : this(width, height)
    {
        _passes = passes.ToList();
        
        Raylib.SetTextureFilter(_bufferA.Texture, textureFilter);
        Raylib.SetTextureFilter(_bufferB.Texture, textureFilter);
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
    
    public void Dispose()
    {
        Raylib.UnloadRenderTexture(_bufferA);
        Raylib.UnloadRenderTexture(_bufferB);
    }
}
