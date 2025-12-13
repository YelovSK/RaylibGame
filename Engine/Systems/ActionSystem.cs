using Engine.Components;

namespace Engine.Systems;

public class ActionSystem : ISystem
{
    public void Update(World world, float dt)
    {
        world.View<ActionComponent>().ForEach((_, ref action) => action.Action());
    }
}