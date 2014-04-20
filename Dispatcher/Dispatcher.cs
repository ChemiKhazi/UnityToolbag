/*
 * Copyright (c) 2014, Nick Gravelyn.
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
using System.Collections.Generic;
using System.Threading;

namespace UnityToolbag
{
    /// <summary>
    /// A system for dispatching code to execute on the main thread.
    /// </summary>
    [AddComponentMenu("UnityToolbag/Dispatcher")]
    public class Dispatcher : MonoBehaviour
    {
        private static Dispatcher _instance;

        // We can't use the behaviour reference from other threads, so we use a separate bool
        // to track the instance so we can use that on the other threads.
        private static bool _instanceExists;

        private static Thread _mainThread;
        private static object _lockObject = new object();
        private static readonly Queue<Action> _actions = new Queue<Action>();

        /// <summary>
        /// Gets a value indicating whether or not the current thread is the game's main thread.
        /// </summary>
        public static bool isMainThread
        {
            get
            {
                return Thread.CurrentThread == _mainThread;
            }
        }

        /// <summary>
        /// Queues an action to be invoked on the main game thread.
        /// </summary>
        /// <param name="action">The action to be queued.</param>
        public static void InvokeAsync(Action action)
        {
            if (!_instanceExists) {
                Debug.LogError("No Dispatcher exists in the scene. Actions will not be invoked!");
                return;
            }

            if (isMainThread) {
                // Don't bother queuing work on the main thread; just execute it.
                action();
            }
            else {
                lock (_lockObject) {
                    _actions.Enqueue(action);
                }
            }
        }

        /// <summary>
        /// Queues an action to be invoked on the main game thread and blocks the
        /// current thread until the action has been executed.
        /// </summary>
        /// <param name="action">The action to be queued.</param>
        public static void Invoke(Action action)
        {
            if (!_instanceExists) {
                Debug.LogError("No Dispatcher exists in the scene. Actions will not be invoked!");
                return;
            }

            bool hasRun = false;

            InvokeAsync(() =>
            {
                action();
                hasRun = true;
            });

            // Lock until the action has run
            while (!hasRun) {
                Thread.Sleep(5);
            }
        }

        void Awake()
        {
            if (_instance) {
                DestroyImmediate(this);
            }
            else {
                _instance = this;
                _instanceExists = true;
                _mainThread = Thread.CurrentThread;
            }
        }

        void OnDestroy()
        {
            if (_instance == this) {
                _instance = null;
                _instanceExists = false;
            }
        }

        void Update()
        {
            lock (_lockObject) {
                while (_actions.Count > 0) {
                    _actions.Dequeue()();
                }
            }
        }
    }
}
