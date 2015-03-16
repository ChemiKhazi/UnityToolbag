using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace kontrabida.utils.extensions
{
	public static class RandomExtensions
	{
		public static T RandomValue<T>(this List<T> input)
		{
			return input[Random.Range(0, input.Count)];
		}

		public static T RandomValue<T>(this T[] input)
		{
			return input[Random.Range(0, input.Length)];
		}
	}
}
