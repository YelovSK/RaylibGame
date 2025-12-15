using System.Numerics;
using Engine.Collections;

namespace Engine.Components;

public class TransformComponent : Component, IInterpolatable
{
    private readonly List<TransformComponent> _children = [];
    public ReadOnlyCollections<TransformComponent> Children => new(_children);

    // Cache
    private Matrix4x4 _worldMatrix;
    private bool _isDirty = true;

    // Interpolation state
    private Vector2 _previousLocalPosition;
    private float _previousLocalRotation;
    
    /// <summary>
    /// Interpolated position for rendering. Only valid during Draw phase.
    /// </summary>
    public Vector2 RenderPosition { get; private set; }
    
    /// <summary>
    /// Interpolated rotation for rendering. Only valid during Draw phase.
    /// </summary>
    public float RenderRotation { get; private set; }

    // Local transform
    public Vector2 LocalPosition
    {
        get;
        set { field = value; SetDirty(); }
    }

    public float LocalRotation
    {
        get;
        set { field = value; SetDirty(); }
    }

    public Vector2 LocalScale
    {
        get;
        set { field = value; SetDirty(); }
    }
    
    // World transform
    public TransformComponent? Parent
    {
        get;
        set
        {
            if (field == value || value == this)
            {
                return;
            }

            field?._children.Remove(this);
            field = value;
            field?._children.Add(this);
            
            SetDirty();
        }
    }

    public Vector2 Position
    {
        get => Parent != null
            ? GetWorldMatrix().Translation.AsVector2()
            : LocalPosition;
        set
        {
            if (Parent != null)
            {
                var parentMatrix = Parent.GetWorldMatrix();
                Matrix4x4.Invert(parentMatrix, out var parentInverse);

                LocalPosition = Vector2.Transform(value, parentInverse);
            }
            else
            {
                LocalPosition = value;
            }
        }
    }

    public float Rotation
    {
        get => Parent != null
            ? Parent.Rotation + LocalRotation
            : LocalRotation;
        set
        {
            if (Parent != null)
            {
                LocalRotation = value - Parent.Rotation;
            }
            else
            {
                LocalRotation = value;
            }
        }
    }
    
    public Vector2 Scale
    {
        get => Parent != null
            ? Parent.Scale * LocalScale
            : LocalScale;
        set
        {
            if (Parent != null)
            {
                var parentScale = Parent.Scale;
                var x = parentScale.X == 0
                    ? 0
                    : value.X / parentScale.X;
                var y = parentScale.Y == 0
                    ? 0
                    : value.Y / parentScale.Y;
                LocalScale = new Vector2(x, y);
            }
            else
            {
                LocalScale = value;
            }
        }
    }
    
    public Matrix4x4 GetWorldMatrix()
    {
        if (!_isDirty)
        {
            return _worldMatrix;
        }
        
        var localMat = Matrix4x4.CreateScale(new Vector3(LocalScale.X, LocalScale.Y, 1)) *
                       Matrix4x4.CreateRotationZ(LocalRotation) *
                       Matrix4x4.CreateTranslation(LocalPosition.AsVector3());

        _worldMatrix = Parent != null
            ? localMat * Parent.GetWorldMatrix()
            : localMat;

        _isDirty = false;
        return _worldMatrix;
    }
    
    // IInterpolatable implementation
    public void SavePreviousState()
    {
        _previousLocalPosition = Position;
        _previousLocalRotation = Rotation;
    }

    public void ComputeRenderState(float alpha)
    {
        RenderPosition = Vector2.Lerp(_previousLocalPosition, Position, alpha);
        RenderRotation = LerpAngle(_previousLocalRotation, Rotation, alpha);
        Console.WriteLine("Computing");
    }
    
    private static float LerpAngle(float a, float b, float t)
    {
        // shortest-path lerp for angles (radians). Adjust if you use degrees.
        float diff = WrapAngle(b - a);
        return a + diff * t;
    }

    private static float WrapAngle(float angle)
    {
        while (angle > MathF.PI) angle -= MathF.Tau;
        while (angle < -MathF.PI) angle += MathF.Tau;
        return angle;
    }
    
    private void SetDirty()
    {
        _isDirty = true;
        foreach (var child in _children)
        {
            child.SetDirty();
        }
    }
}