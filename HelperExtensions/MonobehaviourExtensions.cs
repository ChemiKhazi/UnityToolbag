using System;
using System.Security.Cryptography;
using UnityEngine;
using System.Collections;

namespace kontrabida.utils.extensions
{
	public static class MonobehaviourExtensions
	{
		public static Coroutine TweenFloat(this MonoBehaviour routineParent,
			System.Action<float> recieverFunction,
			float start, float end, float time)
		{
			return routineParent.StartCoroutine(FloatTweener(recieverFunction, start, end, time));
		}

		private static IEnumerator FloatTweener(System.Action<float> recieverFunction, float start, float end, float time)
		{
			float distance = end - start;
			float speed = distance / time;
			distance = Mathf.Abs(distance);

			float current = start;
			while (current != end)
			{
				float delta = speed * Time.deltaTime;
				current += delta;

				if (Mathf.Abs(start - current) >= distance)
				{
					current = end;
				}
				recieverFunction(current);
				yield return null;
			}
		}

		/// <summary>
		/// Add the component to this GameObject and copy the values from the original
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="destination"></param>
		/// <param name="original"></param>
		/// <returns></returns>
		public static T CopyComponent<T>(this GameObject destination, T original) where T : Component
		{
			Type type = original.GetType();
			Component copy = destination.AddComponent(type);
			System.Reflection.FieldInfo[] fields = type.GetFields();
			foreach (System.Reflection.FieldInfo field in fields)
			{
				field.SetValue(copy, field.GetValue(original));
			}
			return copy as T;
		}

		/// <summary>
		/// Runs a function repeatedly until it returns false. Similar to InvokeRepeating
		/// </summary>
		/// /// <param name='delayTime'>
		/// Time to delay before first function call
		/// </param>
		/// <param name='repeatTime'>
		/// Time between repeated function calls
		/// </param>
		/// <param name='routine'>
		/// A function that returns a bool. If true, functions repeats.
		/// If false, function stops
		/// </param>
		public static Coroutine RepeatFunction(this MonoBehaviour routineParent,
												float delayTime,
												float repeatTime,
												System.Func<bool> routine)
		{
			return routineParent.StartCoroutine(RepeatCoroutine(delayTime, repeatTime, routine));
		}

		private static IEnumerator RepeatCoroutine(float delayTime, float repeatTime, System.Func<bool> routine)
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

		public static Coroutine WaitFrames(this MonoBehaviour routineParent, int frames, System.Action callback)
		{
			return routineParent.StartCoroutine(FrameDelay(frames, callback));
		}

		private static IEnumerator FrameDelay(int frames, System.Action callback)
		{
			for (int i = 0; i < frames; i++)
			{
				yield return null;
			}
			callback();
		}

		/// <summary>
		/// Call a function after an amount of time
		/// </summary>
		/// <param name="routineParent"></param>
		/// <param name="time">Delay</param>
		/// <param name="callback">Function to call</param>
		/// <returns></returns>
		public static Coroutine StartTimer(this MonoBehaviour routineParent, float time, System.Action callback)
		{
			return routineParent.StartCoroutine(TimedCall(time, callback));
		}

		/// <summary>
		/// Call a function after an amount of time
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="routineParent"></param>
		/// <param name="time">Delay</param>
		/// <param name="callback">Function to call</param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static Coroutine StartTimer<T>(this MonoBehaviour routineParent, float time, System.Action<T> callback, T arg)
		{
			return routineParent.StartCoroutine(TimedCall(time, callback, arg));
		}

		/// <summary>
		/// Call a function after an amount of time
		/// </summary>
		/// <param name="time">Delay</param>
		public static Coroutine StartTimer<T1, T2>(this MonoBehaviour routineParent,
											float time, System.Action<T1, T2> callback, T1 arg, T2 arg2)
		{
			return routineParent.StartCoroutine(TimedCall(time, callback, arg, arg2));
		}

		/// <summary>
		/// Call a function after an amount of time
		/// </summary>
		/// <param name="time">Delay</param>
		public static Coroutine StartTimer<T1, T2, T3>(this MonoBehaviour routineParent,
											float time, System.Action<T1, T2, T3> callback,
											T1 arg1, T2 arg2, T3 arg3)
		{
			return routineParent.StartCoroutine(TimedCall(time, callback, arg1, arg2, arg3));
		}

		/// <summary>
		/// Call a function after an amount of time
		/// </summary>
		/// <param name="time">Delay</param>
		public static Coroutine StartTimer<T1, T2, T3, T4>(this MonoBehaviour routineParent,
											float time, System.Action<T1, T2, T3, T4> callback,
											T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			return routineParent.StartCoroutine(TimedCall(time, callback, arg1, arg2, arg3, arg4));
		}

		#region Time delay coroutines
		private static IEnumerator TimedCall(float time, System.Action callback)
		{
			yield return new WaitForSeconds(time);
			callback();
		}

		private static IEnumerator TimedCall<T>(float time, System.Action<T> callback, T arg)
		{
			yield return new WaitForSeconds(time);
			callback(arg);
		}

		private static IEnumerator TimedCall<T1, T2>(float time, System.Action<T1, T2> callback, T1 arg1, T2 arg2)
		{
			yield return new WaitForSeconds(time);
			callback(arg1, arg2);
		}

		private static IEnumerator TimedCall<T1, T2, T3>(float time,
														System.Action<T1, T2, T3> callback,
														T1 arg1, T2 arg2, T3 arg3)
		{
			yield return new WaitForSeconds(time);
			callback(arg1, arg2, arg3);
		}

		private static IEnumerator TimedCall<T1, T2, T3, T4>(float time,
															System.Action<T1, T2, T3, T4> callback,
															T1 arg1, T2 arg2,
															T3 arg3, T4 arg4)
		{
			yield return new WaitForSeconds(time);
			callback(arg1, arg2, arg3, arg4);
		}
		#endregion
	}
}