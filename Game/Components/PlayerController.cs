using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Extensions;
using Game.Persistence;
using Raylib_CSharp;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Interact;

namespace Game.Components;

public class PlayerController : Component
{
    // Helper cheats
    public const float COTOYE_TIME = 0.2f;
    public readonly float CEILING_BUMP_MAX_PX = Application.Instance.VirtualWidth * 0.01f;
    public const float JUMP_BUFFER_TIME = 0.1f;

    public const float MAX_HORIZONTAL_SPEED = 300f;
    public const float HORIZONTAL_ACCELERATION = 5000f;
    public const float JUMP_ACCELERATION = 500f;
    public const float FRICTION = 20f;
    public const float AIR_FRICTION = 2f;
    public const float GRAVITY = 1500f;

    public const float JUMP_HOLD_FORCE = 2000f;
    public const float MAX_JUMP_HOLD_TIME = 0.2f;

    public const float DASH_SPEED = 700f;
    public const float DASH_LENGTH = 0.15f;

    public Vector2 Velocity;

    private bool _jumpToConsume;
    private bool _isGrounded;
    private double _timeLeftGrounded;

    private int _jumpCount = 0;
    private int _dashesCount = 0;

    private List<BoxColliderComponent?> _sceneColliders = [];
    private BoxColliderComponent _collider;

    public override void Start()
    {
        _sceneColliders = Entity.Scene.Entities
            .AsEnumerable()
            .Where(go => go != Entity)
            .Select(go => go.GetComponent<BoxColliderComponent>())
            .Where(col => col != null)
            .ToList();
        _collider = Entity.GetComponent<BoxColliderComponent>()!;
    }

    public override void Update(float dt)
    {
        CheckCollisions();

        HandleJump();
        HandleJumpRelease();
        HandleDirection(dt);

        Velocity.Y += GRAVITY * dt;
        HandleDash();
        if (!_isGrounded)
        {
            Velocity.X -= Velocity.X * AIR_FRICTION * dt;
        }
        Entity.Transform.Position += Velocity * dt;
    }

    private bool _isDashing = false;
    private Vector2 _dashDirection;
    private double _startedDashOn;
    private void HandleDash()
    {
        if (_isDashing)
        {
            if (Time.GetTime() > _startedDashOn + DASH_LENGTH)
            {
                _isDashing = false;
                Velocity.Y /= 4;
                Velocity.X /= 4;
            }
            else
            {
                Velocity.X = _dashDirection.X * DASH_SPEED;
                Velocity.Y = _dashDirection.Y * DASH_SPEED;
            }
            return;
        }

        if (!Input.IsKeyPressed(Settings.Instance.DashKey))
        {
            return;
        }

        if (_dashesCount == 0)
        {
            return;
        }

        _startedDashOn = Time.GetTime();
        _isDashing = true;
        var direction = InputDirection();
        _dashDirection = direction;
        Velocity.X = direction.X * DASH_SPEED;
        Velocity.Y = direction.Y * DASH_SPEED;
        _dashesCount--;
    }

    private bool _consumed = false;
    private void HandleJumpRelease()
    {
        if (!_consumed && !Input.IsKeyDown(Settings.Instance.JumpKey) && Velocity.Y < -10f)
        {
            _consumed = true;
            Velocity.Y /= 2;
        }
    }

    private void HandleDirection(float dt)
    {
        switch (InputDirection().X)
        {
            case 0:
                if (_isGrounded)
                {
                    Velocity.X -= Velocity.X * FRICTION * dt;
                }
                break;
            case 1:
                if (Velocity.X < MAX_HORIZONTAL_SPEED)
                {
                    Velocity.X += HORIZONTAL_ACCELERATION * dt;
                }
                break;
            case -1:
                if (Velocity.X > -MAX_HORIZONTAL_SPEED)
                {
                    Velocity.X -= HORIZONTAL_ACCELERATION * dt;
                }
                break;
        }
    }

    private void HandleJump()
    {
        if (_isDashing)
        {
            return;
        }
        var wasJumpPressed = InputBuffer.Instance.WasKeyPressedRecently(Settings.Instance.JumpKey, JUMP_BUFFER_TIME);
        if (!wasJumpPressed)
        {
            return;
        }

        if (_jumpCount == 0)
        {
            return;
        }

        _consumed = false;
        if (_canHyperDashUntil > Time.GetTime())
        {
            Console.WriteLine("Hyper dashing");
            Velocity.X *= 6;
        }
        Velocity.Y = -JUMP_ACCELERATION;

        InputBuffer.Instance.ConsumeKeyPress(Settings.Instance.JumpKey);
        _jumpCount--;

        //_jumpToConsume = false;
    }

    private void CheckCollisions()
    {
        var hitGround = false;
        var hitCeiling = false;

        foreach (var collider in _sceneColliders)
        {
            var overlap = ShapeHelper.GetCollisionRec(collider.Bounds, _collider.Bounds);

            if (overlap.Width == 0 || overlap.Height == 0)
            {
                continue;
            }

            var dirX = _collider.Bounds.X < collider.Bounds.X
                ? -1
                : 1;

            var dirY = _collider.Bounds.Y < collider.Bounds.Y
                ? -1
                : 1;

            // Horizontal collision
            if (overlap.Width < overlap.Height)
            {
                Entity.Transform.Position += Vector2.X(overlap.Width * dirX);
                Velocity.X = 0;
            }
            // Vertical collision
            else
            {
                hitGround = dirY == -1;
                hitCeiling = dirY == 1;

                if (hitCeiling && overlap.Width < CEILING_BUMP_MAX_PX)
                {
                    Entity.Transform.Position += Vector2.X(overlap.Width * dirX);
                    break;
                }

                Entity.Transform.Position += Vector2.Y(overlap.Height * dirY);
                Velocity.Y = 0;
            }

            break;
        }

        if (_isGrounded && !hitGround)
        {
            _timeLeftGrounded = Time.GetTime();
            _jumpCount = 1;
        }

        if (!_isGrounded && hitGround)
        {
            _jumpCount = 2;
            _dashesCount = 1;
            if (_isDashing)
            {
                _canHyperDashUntil = Time.GetTime() + 0.2f;
            }
        }

        _isGrounded = hitGround;
    }

    private double _canHyperDashUntil;


    private Vector2 InputDirection()
    {
        var direction = new Vector2();

        if (Input.IsKeyDown(KeyboardKey.Up))
        {
            direction.Y -= 1;
        }
        if (Input.IsKeyDown(KeyboardKey.Down))
        {
            direction.Y += 1;
        }
        if (Input.IsKeyDown(KeyboardKey.Left))
        {
            direction.X -= 1;
        }
        if (Input.IsKeyDown(KeyboardKey.Right))
        {
            direction.X += 1;
        }

        return direction;
    }
}
