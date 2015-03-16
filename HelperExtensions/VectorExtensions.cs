using UnityEngine;
using System.Collections;

namespace kontrabida.utils.extensions
{
	public static class VectorExtensions
	{
		/// <summary>
		/// Multiplies the vector by a factor, returns the input or factored vector depending on which is smaller
		/// </summary>
		/// <param name="input"></param>
		/// <param name="factor"></param>
		/// <returns></returns>
		public static Vector3 MinFactored(this Vector3 input, float factor)
		{
			Vector3 factoredVector = input.normalized*factor;
			if (factoredVector.sqrMagnitude < input.sqrMagnitude)
				return factoredVector;
			return input;
		}

		/// <summary>
		/// Return this vector projected onto a normal
		/// </summary>
		/// <param name="input"></param>
		/// <param name="normal"></param>
		/// <returns>This vector projected to the normal</returns>
		public static Vector3 ProjectTo(this Vector3 input, Vector3 normal)
		{
			/// Solution from here http://forum.unity3d.com/threads/58325-Vector-Math-problem
			Vector3 projectionOnNormal = Vector3.Project(input, normal);
			return (input - projectionOnNormal);
		}

		/// <summary>
		/// Check if value is between the X/Y of this vector
		/// </summary>
		public static bool ValueWithin(this Vector2 check, float value)
		{
			float max = Mathf.Max(check.x, check.y);
			float min = Mathf.Min(check.x, check.y);

			return (value >= min && value <= max);
		}

		/// <summary>
		/// Clamp a value between the X/Y of this vector
		/// </summary>
		public static float ClampWithin(this Vector2 check, float value)
		{
			float max = Mathf.Max(check.x, check.y);
			float min = Mathf.Min(check.x, check.y);

			return Mathf.Clamp(value, min, max);
		}
	}
}