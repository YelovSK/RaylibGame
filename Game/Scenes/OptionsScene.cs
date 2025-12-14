using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Enums;
using Engine.Systems;
using Engine.Systems.Render;
using Engine.Systems.Update;
using Raylib_CSharp.Colors;

namespace Game.Scenes;

public class OptionsScene : Scene
{
    protected override void Load(World world)
    {
        var middle = VirtualLayout.AnchorToScreen(0, 0, Anchor.Center);
        
        var background = world.CreateEntity();
        world.AddComponent(background, new TransformComponent
        {
            Position = Vector2.Zero,
        });
        world.AddComponent(background, new SpriteComponent
        {
            Size = new Vector2(Application.Instance.VirtualWidth,  Application.Instance.VirtualHeight),
            Color = Color.SkyBlue
        });
        
        AddButton(world, "VSync", middle.Y, (isChecked) =>
        {
            Console.WriteLine(isChecked);
        });
        
        world.AddSystem<UiPointerSystem>();
        world.AddSystem<CheckboxInputSystem>();
        world.AddRenderSystem<SpriteRenderSystem>();
        world.AddRenderSystem<CheckboxRenderSystem>();
    }
    
    private static void AddButton(World world, string text, float y, Action<bool> action)
    {
        var middle = VirtualLayout.Center(32, 0); 
        
        var checkbox = world.CreateEntity();
        world.AddComponent(checkbox, new RectTransform()
        {
            Position = middle with { Y = y },
            Size = new Vector2(32, 32),
        });
        
        world.AddComponent(checkbox, new LabelComponent()
        {
            Text = text,
            TextColor = Color.White,
            FontSize = 20,
        });

        world.AddComponent(checkbox, new CheckboxComponent()
        {
            Action = action
        });

        world.AddComponent(checkbox, new UiPointerStateComponent()
        {
        });
    }
}