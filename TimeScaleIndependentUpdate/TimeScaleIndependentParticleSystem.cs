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

namespace UnityToolbag
{
    /// <summary>
    /// A component that can be attached to an object with a ParticleSystem to update it using real-time,
    /// to allow it to continue updating when Time.timeScale is set to 0.
    /// </summary>
    [AddComponentMenu("UnityToolbag/Time Scale Independent ParticleSystem")]
    public class TimeScaleIndependentParticleSystem : TimeScaleIndependentUpdate
    {
        // Cache the particle system so we can minimize calls to GetComponent which happen
        // implicitly each time you call the MonoBehaviour.particleSystem property.
        private ParticleSystem _particleSystem;

        protected override void Update()
        {
            // Always update the base first when subclassing TimeScaleIndependentUpdate
            base.Update();

            // Update our particle system cache if our private field is null or the instance was destroyed.
            if (!_particleSystem) {
                _particleSystem = particleSystem;

                if (!_particleSystem) {
                    Debug.LogWarning("No valid particle system attached to object.", this);
                    return;
                }
            }

            // If we have a valid system, go ahead and simulate.
            _particleSystem.Simulate(deltaTime, true, false);
        }
    }
}
