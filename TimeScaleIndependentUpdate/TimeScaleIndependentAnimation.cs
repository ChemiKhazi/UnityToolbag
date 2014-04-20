/*
 * Copyright (c) 2014, Nick Gravelyn.
 * Code in this file based on code provided by Asteroid Base in their blog:
 * http://www.asteroidbase.com/devlog/9-pausing-without-pausing/
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 *    1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 *
 *    2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 *
 *    3. This notice may not be removed or altered from any source
 *    distribution.
 */

using UnityEngine;
using System;

namespace UnityToolbag
{
    /// <summary>
    /// A component that can be attached to an object with an Animation to update it using real-time,
    /// to allow it to continue updating when Time.timeScale is set to 0.
    /// </summary>
    [AddComponentMenu("UnityToolbag/Time Scale Independent Animation")]
    public class TimeScaleIndependentAnimation : TimeScaleIndependentUpdate
    {
        // Cache the animation so we can minimize calls to GetComponent which happen
        // implicitly each time you call the MonoBehaviour.animation property.
        private Animation _animation;

        private AnimationState _currentState;
        private Action _callback0;
        private Action<TimeScaleIndependentAnimation> _callback1;
        private float _elapsedTime;
        private bool _isPlaying;

        [SerializeField]
        private bool _playOnStart;

        [SerializeField]
        private string _playOnStartStateName;

        void Start()
        {
            if (_playOnStart) {
                Play(_playOnStartStateName);
            }
        }

        protected override void Update()
        {
            // Always update the base first when subclassing TimeScaleIndependentUpdate
            base.Update();

            if (_isPlaying) {
                // If for some reason the state goes away (maybe the animation is destroyed), log a warning and stop playing.
                if (!_currentState) {
                    Debug.LogWarning("Animation is playing but the AnimationState isn't valid.", this);
                    Stop();
                    return;
                }

                _elapsedTime += deltaTime;
                _currentState.normalizedTime = _elapsedTime / _currentState.length;

                if (_elapsedTime >= _currentState.length) {
                    _isPlaying = false;

                    if (_currentState.wrapMode == WrapMode.Loop) {
                        Play(_currentState.name);
                    }
                    else if (_callback0 != null) {
                        _callback0();
                    }
                    else if (_callback1 != null) {
                        _callback1(this);
                    }
                }
            }
        }

        /// <summary>
        /// Plays a given animation.
        /// </summary>
        /// <param name="name">The name of the animation to play.</param>
        public void Play(string name)
        {
            // Validate that we have an animation
            if (!_animation) {
                _animation = animation;

                if (!_animation) {
                    Debug.LogWarning("No valid animation attached to object.", this);
                    return;
                }
            }

            // Get and validate the animation state
            var state = animation[name];
            if (!state) {
                Debug.LogWarning(string.Format("No animation state named '{0}' found.", name), this);
                return;
            }

            _elapsedTime = 0f;
            _currentState = state;
            _currentState.normalizedTime = 0;
            _currentState.enabled = true;
            _currentState.weight = 1;
            _isPlaying = true;

            _callback0 = null;
            _callback1 = null;
        }

        /// <summary>
        /// Plays a given animation with an action to invoke when the animation completes.
        /// </summary>
        /// <param name="name">The name of the animation to play.</param>
        /// <param name="callback">The action to invoke when the animation completes.</param>
        public void Play(string name, Action callback)
        {
            Play(name);
            _callback0 = callback;
            _callback1 = null;
        }

        /// <summary>
        /// Plays a given animation with an action to invoke when the animation completes.
        /// </summary>
        /// <param name="name">The name of the animation to play.</param>
        /// <param name="callback">The action to invoke when the animation completes.</param>
        public void Play(string name, Action<TimeScaleIndependentAnimation> callback)
        {
            Play(name);
            _callback0 = null;
            _callback1 = callback;
        }

        /// <summary>
        /// Stops playback of the animation.
        /// </summary>
        public void Stop()
        {
            _elapsedTime = 0f;
            _currentState = null;
            _callback0 = null;
            _callback1 = null;
            _isPlaying = false;
        }
    }
}
