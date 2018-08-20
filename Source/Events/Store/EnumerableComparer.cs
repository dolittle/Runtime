using System.Collections.Generic;
using System.Linq;

namespace Dolittle.Runtime.Events.Store
{
	//TODO: This doesn't belong in the EventStore.  Should be put down into DotNet.Fundamentals

	/// <summary>
	/// Compares to enumerables and returns true if the same elements are in both collections in the same order
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public class EnumerableComparer<T> : IEqualityComparer<IEnumerable<T>>
	{
		/// http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
		
		/// <summary>
		/// Generates a hashcode for the enumberable, utilising the hashcode of each element
		/// </summary>
		/// <param name="enumerable">The enumerable to generte the hashcode for</param>
		/// <returns>The hashcode value</returns>
		public int GetHashCode(IEnumerable<T> enumerable)
		{
			if (enumerable != null)
			{
				unchecked
				{
					int hash = 17;
					foreach (var item in enumerable)
					{
						hash = hash * 23 + ((item != null) ? item.GetHashCode() : 0);
					}

					return hash;
				}
			}
			return 0;
		}

		/// <summary>
		/// Equates two IEnumerable{T}s 
		/// </summary>
		/// <param name="first">First Enumerable</param>
		/// <param name="second">Second Enumerable</param>
		/// <returns>True if the exact same elements, in the same order, are in both enumerables</returns>
		public bool Equals(IEnumerable<T> first, IEnumerable<T> second)
		{
			if (object.ReferenceEquals(first, second) )
			{
				return true;
			}

            var firstArray = first.ToArray();
            var secondArray = second.ToArray();

            if(firstArray == null || secondArray == null || (firstArray.Length != secondArray.Length))
                return false;

            for (int i = 0; i < firstArray.Count(); i++)
            {
                if (!object.Equals(firstArray[i], secondArray[i]))
                {
                    return false;
                }
            }

            return true;
		}
	} 
}


