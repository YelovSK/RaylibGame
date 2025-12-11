using System.Numerics;
using Engine.Helpers;
using Raylib_CSharp.Camera.Cam2D;

namespace Engine.Components;

public class CameraComponent : Component
{
    public Camera2D Camera = new();
    public Entity? Target { get; set; }
    public float FollowSpeed = 5.0f;

    public CameraComponent()
    {
        Camera.Offset = VirtualLayout.Center(0f, 0f);
        Camera.Zoom = 1f;
    }

    public override void Start()
    {
        Entity.Scene.RegisterCamera(this);
    }

    public override void Update(float dt)
    {
        if (Target is null)
        {
            return;
        }
        
        var currentTarget = Camera.Target;
        var desiredTarget = Target.Transform.Position;

        var newTarget = Vector2.Lerp(currentTarget, desiredTarget, FollowSpeed * dt);

        Camera.Target = newTarget;
    }

    public override void OnDestroy()
    {
        Entity.Scene.UnregisterCamera(this);
    }
}