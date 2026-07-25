using Engine.PostProcessing;
using Game.Persistence;

using Raylib_cs;

namespace Game.PostProcessing;

public class GrainPass : FullscreenShaderPass
{
    private readonly int _timeUniformLoc;
    
    public GrainPass() : base("grain.fs", () => Settings.Instance.EnableShaders)
    {
        _timeUniformLoc = Raylib.GetShaderLocation(Shader, "iTime");
    }
    
    protected override void SetUniforms(Shader shader)
    {
        Raylib.SetShaderValue(shader, _timeUniformLoc, (float)Raylib.GetTime(), ShaderUniformDataType.Float);
    }
}
