/*
 * Code in this file based on code provided by Asteroid Base in their blog:
 * http://www.asteroidbase.com/devlog/9-pausing-without-pausing/
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
                _animation = GetComponent<Animation>();

                if (!_animation) {
                    Debug.LogWarning("No valid animation attached to object.", this);
                    return;
                }
            }

            // Get and validate the animation state
            var state = GetComponent<Animation>()[name];
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
