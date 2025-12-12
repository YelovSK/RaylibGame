using Raylib_CSharp;
using Raylib_CSharp.Interact;

namespace Engine;

public class InputManager : Singleton<InputManager>
{
    /// <summary>
    /// Index by (int)KeyboardKey, get key pressed time.
    /// </summary>
    private readonly double[] _inputTimes;
    
    /// <summary>
    /// Index by (int)KeyboardKey, get <see cref="FixedTime.Ticks"/>,
    /// </summary>
    private readonly long[] _inputTicks;
    
    private const double NEVER = -1;
    private const long NEVER_TICK = -1;

    public InputManager()
    {
        var maxKey = Enum.GetValues<KeyboardKey>().Max(x => (int)x);
        _inputTimes = Enumerable.Repeat(NEVER, maxKey + 1).ToArray();
        _inputTicks = Enumerable.Repeat(NEVER_TICK, maxKey + 1).ToArray();
    }
    
    internal void Gather()
    {
        var now = Time.GetTime();

        var key = Input.GetKeyPressed();
        while (key != 0)
        {
            _inputTimes[key] = now;
            _inputTicks[key] = FixedTime.Ticks;
            key = Input.GetKeyPressed();
        }
    }
    
    public bool IsKeyBuffered(KeyboardKey key, float bufferTime) => Time.GetTime() - _inputTimes[(int)key] <= bufferTime;
    
    /// <summary>
    /// Returns true if the key was pressed this fixed update tick.
    /// </summary>
    public bool IsKeyPressedFixedTick(KeyboardKey key) => _inputTicks[(int)key] == FixedTime.Ticks;

    public void ConsumeKeyPress(KeyboardKey key)
    {
        _inputTimes[(int)key] = NEVER;
        _inputTicks[(int)key] = NEVER_TICK;
    }   
}