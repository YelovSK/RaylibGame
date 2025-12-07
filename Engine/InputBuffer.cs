using Raylib_CSharp;
using Raylib_CSharp.Interact;

namespace Engine;

public class InputBuffer : Singleton<InputBuffer>
{
    /// <summary>
    /// Key -> time last pressed
    /// </summary>
    private readonly Dictionary<KeyboardKey, double> _buffer = new();
    
    internal void Gather()
    {
        var now = Time.GetTime();

        var key = Input.GetKeyPressed();
        while (key != 0)
        {
            _buffer[(KeyboardKey) key] = now;
            key = Input.GetKeyPressed();
        }
    }
    
    public bool WasKeyPressedRecently(KeyboardKey key, float bufferTime)
    {
        if (_buffer.TryGetValue(key, out var pressTime))
        {
            return (Time.GetTime() - pressTime) <= bufferTime;
        }
        
        return false;
    }
    
    public void ConsumeKeyPress(KeyboardKey key)
    {
        _buffer.Remove(key);
    }   
}