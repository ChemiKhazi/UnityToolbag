/*
 * Code in this file based on code provided by Asteroid Base in their blog:
 * http://www.asteroidbase.com/devlog/9-pausing-without-pausing/
 */

using UnityEngine;
using System;
using System.Collections;
using System.Diagnostics;

namespace UnityToolbag
{
    /// <summary>
    /// A script that can be added to an object to allow for tracking elapsed time even when Time.timeScale is 0.
    /// </summary>
    public class TimeScaleIndependentUpdate : MonoBehaviour
    {
        /// <summary>
        /// Whether or not the game is paused. While having the scripts poll for this is nicer, this is still global
        /// to all objects and lets this script not require more dependencies than necessary.
        /// </summary>
        public static bool IsGamePaused;

        private long _previousTicks;

        /// <summary>
        /// Whether or not this instance pauses when IsGamePaused is set to true.
        /// </summary>
        public bool pauseWhenGameIsPaused = true;

        /// <summary>
        /// Gets the elapsed time.
        /// </summary>
        public float deltaTime { get; private set; }

        protected virtual void Awake()
        {
            _previousTicks = Stopwatch.GetTimestamp();
        }

        protected virtual void Update()
        {
            long currentTicks = Stopwatch.GetTimestamp();
            deltaTime = (float)(currentTicks - _previousTicks) / Stopwatch.Frequency;
            _previousTicks = currentTicks;

            if (pauseWhenGameIsPaused && IsGamePaused)
            {
                // If this update pauses with the game and the game has been marked as paused, disregard the delta time.
                deltaTime = 0;
            }
            else if (deltaTime < 0)
            {
                // It is possible (especially if this script is attached to an object that is created when the
                // scene is loaded) that the calculated delta time is less than zero. In that case, discard this update.
                UnityEngine.Debug.LogWarning(string.Format("Delta time less than zero, discarding (delta time was {0})", deltaTime));
                deltaTime = 0;
            }
        }

        /// <summary>
        /// An enumerator used to wait for a given amount of seconds using the real-time based delta time.
        /// </summary>
        /// <param name="seconds">The amount of time to wait, in seconds.</param>
        /// <returns>An enumerator that can be used as a coroutine.</returns>
        public IEnumerator TimeScaleIndependentWaitForSeconds(float seconds)
        {
            float elapsedTime = 0;
            while (elapsedTime < seconds)
            {
                yield return null;
                elapsedTime += this.deltaTime;
            }
        }
    }
}
