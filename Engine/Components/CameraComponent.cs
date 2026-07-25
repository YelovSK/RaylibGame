using Raylib_CSharp.Camera.Cam2D;

namespace Engine.Components;

public class CameraComponent : Component
{
    public Camera2D Camera;
    public Entity? Target { get; set; }
    public float FollowSpeed = 7f;

    private bool _isFollowing;

    public CameraComponent()
    {
        Camera.Offset = VirtualViewport.Center;
        Camera.Zoom = 1f;
    }

    public override void Start()
    {
        Entity.Scene.RegisterCamera(this);
    }

    internal void PrepareForDraw(float dt)
    {
        if (Target is null)
        {
            _isFollowing = false;
            return;
        }

        var targetPosition = Target.Transform.RenderPosition;
        if (!_isFollowing)
        {
            Camera.Target = targetPosition;
            _isFollowing = true;
            return;
        }

        var amount = 1f - MathF.Exp(-FollowSpeed * dt);
        Camera.Target += (targetPosition - Camera.Target) * amount;
    }

    public override void OnDestroy()
    {
        Entity.Scene.UnregisterCamera(this);
    }
}
