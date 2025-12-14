using Engine.Attributes;
using Engine.Components;

namespace Engine.Systems.Update;

// This is psychotic.
[UpdateAfter(typeof(UiPointerSystem))]
public class CheckboxInputSystem : ISystem
{
    public void Update(World world, float dt)
    {
        world.View<UiPointerStateComponent, CheckboxComponent>().ForEach((entity, ref pointerState, ref checkbox) =>
        {
            if (pointerState.IsClicked)
            {
                checkbox.IsChecked = !checkbox.IsChecked;
                checkbox.Action?.Invoke(checkbox.IsChecked);
            }
        });
    }
}