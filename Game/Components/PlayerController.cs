using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Extensions;
using Game.Persistence;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;

namespace Game.Components;

public class PlayerController : Component, IFixedUpdatable
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

    private Rectangle Bounds =>
        new Rectangle(
            Entity.Transform.Position.X,
            Entity.Transform.Position.Y,
            GetComponent<SpriteComponent>().Width,
            GetComponent<SpriteComponent>().Height 
        );

    public override void Start()
    {
        _sceneColliders = Entity.Scene.Entities
            .AsEnumerable()
            .Where(go => go != Entity)
            .Select(go => go.GetComponent<BoxColliderComponent>())
            .Where(col => col != null)
            .ToList();
    }

    public void FixedUpdate()
    {
        var dt = (float)FixedTime.TICK_RATE;
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
            if (FixedTime.GetTime() > _startedDashOn + DASH_LENGTH)
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

        if (!InputManager.Instance.IsKeyPressedFixedTick(Settings.Instance.DashKey))
        {
            return;
        }

        if (_dashesCount == 0)
        {
            return;
        }

        _startedDashOn = FixedTime.GetTime();
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
        var wasJumpPressed = InputManager.Instance.IsKeyBuffered(Settings.Instance.JumpKey, JUMP_BUFFER_TIME);
        if (!wasJumpPressed)
        {
            return;
        }

        if (_jumpCount == 0)
        {
            return;
        }

        _consumed = false;
        if (_canHyperDashUntil > FixedTime.GetTime())
        {
            Console.WriteLine("Hyper dashing");
            Velocity.X *= 6;
        }
        Velocity.Y = -JUMP_ACCELERATION;

        InputManager.Instance.ConsumeKeyPress(Settings.Instance.JumpKey);
        _jumpCount--;

        //_jumpToConsume = false;
    }

    private void CheckCollisions()
    {
        var hitGround = false;
        var hitCeiling = false;

        foreach (var collider in _sceneColliders)
        {
            var overlap = ShapeHelper.GetCollisionRec(collider.Bounds, Bounds);

            if (overlap.Width == 0 || overlap.Height == 0)
            {
                continue;
            }

            
            var dirX = Velocity.X > 0
                ? -1
                : 1;

            var dirY = Velocity.Y > 0
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
                _isDashing = false;
            }

            break;
        }

        if (_isGrounded && !hitGround)
        {
            _timeLeftGrounded = FixedTime.GetTime();
            _jumpCount = 1;
        }

        if (!_isGrounded && hitGround)
        {
            _jumpCount = 2;
            _dashesCount = 1;
            if (_isDashing)
            {
                _canHyperDashUntil = FixedTime.GetTime() + (FixedTime.TICK_RATE * 3);
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