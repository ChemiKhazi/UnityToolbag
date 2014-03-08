using UnityEngine;
using System.Collections;

public static class Vector3Extensions
{
	/// <summary>
	/// Gives a vector flattened to a given normal
	/// </summary>
	/// <param name="input"></param>
	/// <param name="normal"></param>
	/// <returns></returns>
	public static Vector3 FlattenedToNormal(this Vector3 input, Vector3 normal)
	{
		/// Solution from here http://forum.unity3d.com/threads/58325-Vector-Math-problem
		Vector3 projectionOnNormal = Vector3.Project(input, normal);
		return (input - projectionOnNormal);
	}

	public static void SetX(this Vector3 input, float x)
	{
		input = new Vector3(x, input.y, input.z);
	}
	public static void SetY(this Vector3 input, float y)
	{
		input = new Vector3(input.x, y, input.z);
	}
	public static void SetZ(this Vector3 input, float z)
	{
		input = new Vector3(input.x, input.y, z);
	}
}
