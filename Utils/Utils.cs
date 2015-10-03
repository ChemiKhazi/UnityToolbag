using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class Utils
{
	public static Vector3[] TransformToVectorArray(Transform[] transformArray)
	{
		Vector3[] vectorArray = new Vector3[transformArray.Length];
		for (int i = 0; i < transformArray.Length; i++)
		{
			vectorArray[i] = transformArray[i].position;
		}
		return vectorArray;
	}
	
	public static void SetLayerRecursively(GameObject gameObject, int layer)
	{
		if (gameObject == null)
			return;
		
		gameObject.layer = layer;
		
		foreach (Transform child in gameObject.transform)
		{
			if (child == null)
				continue;
			SetLayerRecursively(child.gameObject, layer);
		}
	}
	
	public static IEnumerator WaitAndSendMessage(float waitTime, GameObject gameObject, string function)
	{
		yield return new WaitForSeconds(waitTime);
		gameObject.SendMessage(function);
	}
	
	public static IEnumerator WaitAndCallback(float waitTime, System.Action callback)
	{
		yield return new WaitForSeconds(waitTime);
		callback();
	}

	//public static IEnumerator WaitAndCallback(float waitTime, System.Action callback, params object[] paramsObjects)
	//{
	//	yield return new WaitForSeconds(waitTime);
	//	callback(paramsObjects);
	//}
	
	/// <summary>
	/// Coroutine that simulates Invoke without the suck
	/// </summary>
	/// <param name='delayTime'>
	/// Time to delay before first call
	/// </param>
	/// <param name='repeatTime'>
	/// Time to delay between calls
	/// </param>
	/// <param name='routine'>
	/// A function that returns a bool. If true, functions continues
	/// If false, function stops
	/// </param>
	public static IEnumerator RepeatingFunction(float delayTime, float repeatTime, System.Func<bool> routine)
	{
		yield return new WaitForSeconds(delayTime);
		bool doRepeat = true;
		while (doRepeat)
		{
			doRepeat = routine();
			if (doRepeat)
				yield return new WaitForSeconds(repeatTime);
		}
	}

	/// <summary>
	/// Perform a deep Copy of the object.
	/// </summary>
	/// <typeparam name="T">The type of object being copied.</typeparam>
	/// <param name="source">The object instance to copy.</param>
	/// <returns>The copied object.</returns>
	public static T Clone<T>(T source)
	{
		if (!typeof(T).IsSerializable)
		{
			throw new ArgumentException("The type must be serializable.", "source");
		}

		// Don't serialize a null object, simply return the default for that object
		if (System.Object.ReferenceEquals(source, null))
		{
			return default(T);
		}

		IFormatter formatter = new BinaryFormatter();
		Stream stream = new MemoryStream();
		using (stream)
		{
			formatter.Serialize(stream, source);
			stream.Seek(0, SeekOrigin.Begin);
			return (T)formatter.Deserialize(stream);
		}
	}
}

