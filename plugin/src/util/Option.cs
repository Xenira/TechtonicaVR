using System.Collections.Generic;

namespace TechtonicaVR.Util;

public class Option<T> : IEnumerable<T>
{
    private readonly T[] data;

    private Option(T[] data)
    {
        this.data = data;
    }

    public static Option<T> Some(T value)
    {
        return new Option<T>([value]);
    }

    public static Option<T> None()
    {
        return new Option<T>([]);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)data).GetEnumerator();
    }

    System.Collections.IEnumerator
        System.Collections.IEnumerable.GetEnumerator()
    {
        return data.GetEnumerator();
    }
}
