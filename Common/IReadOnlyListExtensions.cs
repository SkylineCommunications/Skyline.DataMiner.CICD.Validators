namespace Skyline.DataMiner.CICD.Validators.Common
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Defines extension methods for the <see cref="IReadOnlyList{T}"/> interface.
    /// </summary>
    internal static class IReadOnlyListExtensions
    {
        public static int IndexOf<T>(this IReadOnlyList<T> source, T value)
        {
            if (source is IList list)
            {
                return list.IndexOf(value);
            }

            if (source is IList<T> listT)
            {
                return listT.IndexOf(value);
            }

            int i = 0;
            foreach (T element in source)
            {
                if (Equals(element, value))
                {
                    return i;
                }

                i++;
            }

            return -1;
        }
    }
}
