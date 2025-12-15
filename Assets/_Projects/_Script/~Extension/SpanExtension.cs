using System;

public static class SpanExtension
{
    public static bool Contains<T>(this Span<T> span, T Value) where T : IEquatable<T>
    {
        foreach(var v in span)
        {
            if(v.Equals(Value)) return true;
        }
        return false;
    }
}