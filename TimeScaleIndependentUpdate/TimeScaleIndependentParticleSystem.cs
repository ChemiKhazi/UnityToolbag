/*
 * Code in this file based on code provided by Asteroid Base in their blog:
 * http://www.asteroidbase.com/devlog/9-pausing-without-pausing/
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
                _particleSystem = GetComponent<ParticleSystem>();

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
