using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Enums;
using Engine.Systems;
using Raylib_CSharp.Colors;

namespace Game;

class SimpleMovementSystem : ISystem
{
    public void Update(World world, float dt)
    {
        world.View<TransformComponent, SpriteComponent, ButtonComponent>().ForEach((_, ref transform, ref _, ref _) =>
        {
            transform.Position.X += 10 * dt;
        });
    }
}

public static class WorldsFactory
{
    private static readonly float BUTTON_WIDTH = Application.Instance.VirtualWidth * 0.2f;
    private static readonly float BUTTON_HEIGHT = Application.Instance.VirtualHeight * 0.1f;

    public static World CreatePerformanceTest()
    {
        var world = new World();

        for (int i = 0; i < 10_000; i++)
        {
            var entity = world.CreateEntity();
            world.AddComponent(entity, new TransformComponent());
            world.AddComponent(entity, new ButtonComponent());
            world.AddComponent(entity, new SpriteComponent()
            {
                Size = new Vector2(50, 50),
                Color = Color.Red
            });
        }

        world.AddSystem(new SimpleMovementSystem());
        world.AddRenderSystem(new SpriteRenderSystem());

        return world;
    }
    
    public static World CreateMenu()
    {
        var middle = VirtualLayout.AnchorToScreen((int)BUTTON_WIDTH, (int)BUTTON_HEIGHT, Anchor.Center); 
        var buttonOffset = BUTTON_HEIGHT * 1.2f; 
        
        var world = new World();
        
        // Background
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
        
        // Buttons
        AddButton(world, "Play", middle.Y - buttonOffset, () =>
        {
            
        });
        
        AddButton(world, "Options", middle.Y, () =>
        {
            
        });
        
        AddButton(world, "Quit", middle.Y + buttonOffset, () =>
        {
            
        });

        world.AddRenderSystem(new SpriteRenderSystem());
        world.AddSystem(new ButtonInputSystem());
        world.AddRenderSystem(new ButtonRenderSystem());

        return world;
    }
    
    private static void AddButton(World world, string text, float y, Action action)
    {
        var middle = VirtualLayout.Center(BUTTON_WIDTH, 0); 
        
        var button = world.CreateEntity();
        world.AddComponent(button, new TransformComponent()
        {
            Position = middle with { Y = y }
        });
        world.AddComponent(button, new ButtonComponent
        {
            Text = text,
            FontSize = (int)(0.04f * Application.Instance.VirtualWidth),
            Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT),
            StrokeWidth = BUTTON_HEIGHT * 0.06f,
            NormalColor = Color.DarkGray,
            HoverColor = Color.Gray,
            PressedColor = Color.LightGray,
            TextColor = Color.White,
        });
        world.AddComponent(button, new ButtonStateComponent());
    }
}