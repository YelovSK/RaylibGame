using System.Numerics;
using Engine.Helpers;
using Raylib_CSharp.Camera.Cam2D;

namespace Engine.Components;

public class CameraComponent : Component, ILateUpdatable
{
    public Camera2D Camera;
    public Entity? Target { get; set; }
    public float FollowSpeed = 7f;

    public CameraComponent()
    {
        Camera.Offset = VirtualLayout.Center();
        Camera.Zoom = 1f;
    }

    public override void Start()
    {
        Entity.Scene.RegisterCamera(this);
    }

    public void LateUpdate(float dt)
    {
        if (Target is null)
        {
            return;
        }

        var newTarget = Vector2.Lerp(Camera.Target, Target.Transform.RenderPosition, FollowSpeed * dt);

        Camera.Target = newTarget;
    }

    public override void OnDestroy()
    {
        Entity.Scene.UnregisterCamera(this);
    }
}