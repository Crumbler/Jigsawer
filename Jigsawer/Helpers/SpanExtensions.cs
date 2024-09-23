
namespace Jigsawer.Helpers;

public static class SpanExtensions {
    public static (int index, T element) GetMinElement<T>(this ReadOnlySpan<T> span) 
        where T : IComparable<T> {
        int index = 0;
        T minElement = span[0];

        for (int i = 1; i < span.Length; ++i) {
            T element = span[i];
            if (element.CompareTo(minElement) < 0) {
                minElement = element;
                index = i;
            }
        }

        return (index, minElement);
    }

    public static (int index, T element) GetMinElement<T>(this T[] array)
        where T : IComparable<T> {
        ReadOnlySpan<T> span = array;
        return GetMinElement(span);
    }
}
