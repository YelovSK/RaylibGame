namespace Engine;

public static class FixedTime
{
    public const double TICK_RATE = 1.0 / 50.0;

    public static long Ticks { get; internal set; }

    public static double GetTime() => Ticks * TICK_RATE;
}