using Engine.PostProcessing;
using Game.Persistence;
using Raylib_CSharp;
using Raylib_CSharp.Shaders;

namespace Game.PostProcessing;

public class GrainPass : FullscreenShaderPass
{
    private readonly int _timeUniformLoc;
    
    public GrainPass() : base("grain.fs", () => Settings.Instance.EnableShaders)
    {
        _timeUniformLoc = Shader.GetLocation("iTime");
    }
    
    protected override void SetUniforms(Shader shader)
    {
        shader.SetValue(_timeUniformLoc, (float)Time.GetTime(), ShaderUniformDataType.Float);
    }
}