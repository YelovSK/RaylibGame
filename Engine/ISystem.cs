using Raylib_CSharp.Textures;

namespace Engine;

public interface ISystem
{
    void Update(World world, float dt);
}

public interface IRenderSystem
{
    void Draw(World world);
}
