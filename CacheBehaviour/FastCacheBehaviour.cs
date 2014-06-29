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

namespace UnityToolbag
{
    /// <summary>
    /// A replacement for MonoBehaviour as your base class that stores fields for all common components that are normally properties
    /// on MonoBehaviour. You must use one of the many Reset methods to get the components you want, or manually assign the values yourself.
    /// However this allows for the fastest access to the components since they are just plain fields with no property call or GetComponent
    /// overhead.
    /// </summary>
    public abstract class FastCacheBehaviour : MonoBehaviour
    {
        /// <summary>
        /// The cached Animation component attached to the object.
        /// Will be null if you haven't called ResetAnimationCache(), ResetCache(), ResetCacheDestroyed(), or manually stored the component yourself.
        /// </summary>
        [HideInInspector, NonSerialized]
        public new Animation animation;

        /// <summary>
        /// Caches the current Animation attached to the object.
        /// </summary>
        public void ResetAnimationCache() { animation = GetComponent<Animation>(); }

        /// <summary>
        /// The cached AudioSource component attached to the object.
        /// Will be null if you haven't called ResetAudioSourceCache(), ResetCache(), ResetCacheDestroyed(), or manually stored the component yourself.
        /// </summary>
        [HideInInspector, NonSerialized]
        public new AudioSource audio;

        /// <summary>
        /// Caches the current AudioSource attached to the object.
        /// </summary>
        public void ResetAudioSourceCache() { audio = GetComponent<AudioSource>(); }

        /// <summary>
        /// The cached Camera component attached to the object.
        /// Will be null if you haven't called ResetCameraCache(), ResetCache(), ResetCacheDestroyed(), or manually stored the component yourself.
        /// </summary>
        [HideInInspector, NonSerialized]
        public new Camera camera;

        /// <summary>
        /// Caches the current Camera attached to the object.
        /// </summary>
        public void ResetCameraCache() { camera = GetComponent<Camera>(); }

        /// <summary>
        /// The cached Collider component attached to the object.
        /// Will be null if you haven't called ResetColliderCache(), ResetCache(), ResetCacheDestroyed(), or manually stored the component yourself.
        /// </summary>
        [HideInInspector, NonSerialized]
        public new Collider collider;

        /// <summary>
        /// Caches the current Collider attached to the object.
        /// </summary>
        public void ResetColliderCache() { collider = GetComponent<Collider>(); }

        /// <summary>
        /// The cached Collider2D component attached to the object.
        /// Will be null if you haven't called ResetCollider2DCache(), ResetCache(), ResetCacheDestroyed(), or manually stored the component yourself.
        /// </summary>
        [HideInInspector, NonSerialized]
        public new Collider2D collider2D;

        /// <summary>
        /// Caches the current Collider2D attached to the object.
        /// </summary>
        public void ResetCollider2DCache() { collider2D = GetComponent<Collider2D>(); }

        /// <summary>
        /// The cached ConstantForce component attached to the object.
        /// Will be null if you haven't called ResetConstantForceCache(), ResetCache(), ResetCacheDestroyed(), or manually stored the component yourself.
        /// </summary>
        [HideInInspector, NonSerialized]
        public new ConstantForce constantForce;

        /// <summary>
        /// Caches the current ConstantForce attached to the object.
        /// </summary>
        public void ResetConstantForceCache() { constantForce = GetComponent<ConstantForce>(); }

        /// <summary>
        /// The cached GUIText component attached to the object.
        /// Will be null if you haven't called ResetGUITextCache(), ResetCache(), ResetCacheDestroyed(), or manually stored the component yourself.
        /// </summary>
        [HideInInspector, NonSerialized]
        public new GUIText guiText;

        /// <summary>
        /// Caches the current GUIText attached to the object.
        /// </summary>
        public void ResetGUITextCache() { guiText = GetComponent<GUIText>(); }

        /// <summary>
        /// The cached GUITexture component attached to the object.
        /// Will be null if you haven't called ResetGUITextureCache(), ResetCache(), ResetCacheDestroyed(), or manually stored the component yourself.
        /// </summary>
        [HideInInspector, NonSerialized]
        public new GUITexture guiTexture;

        /// <summary>
        /// Caches the current GUITexture attached to the object.
        /// </summary>
        public void ResetGUITextureCache() { guiTexture = GetComponent<GUITexture>(); }

        /// <summary>
        /// The cached HingeJoint component attached to the object.
        /// Will be null if you haven't called ResetHingeJointCache(), ResetCache(), ResetCacheDestroyed(), or manually stored the component yourself.
        /// </summary>
        [HideInInspector, NonSerialized]
        public new HingeJoint hingeJoint;

        /// <summary>
        /// Caches the current HingeJoint attached to the object.
        /// </summary>
        public void ResetHingeJointCache() { hingeJoint = GetComponent<HingeJoint>(); }

        /// <summary>
        /// The cached Light component attached to the object.
        /// Will be null if you haven't called ResetLightCache(), ResetCache(), ResetCacheDestroyed(), or manually stored the component yourself.
        /// </summary>
        [HideInInspector, NonSerialized]
        public new Light light;

        /// <summary>
        /// Caches the current Light attached to the object.
        /// </summary>
        public void ResetLightCache() { light = GetComponent<Light>(); }

        /// <summary>
        /// The cached NetworkView component attached to the object.
        /// Will be null if you haven't called ResetNetworkViewCache(), ResetCache(), ResetCacheDestroyed(), or manually stored the component yourself.
        /// </summary>
        [HideInInspector, NonSerialized]
        public new NetworkView networkView;

        /// <summary>
        /// Caches the current NetworkView attached to the object.
        /// </summary>
        public void ResetNetworkViewCache() { networkView = GetComponent<NetworkView>(); }

        /// <summary>
        /// The cached ParticleEmitter component attached to the object.
        /// Will be null if you haven't called ResetParticleEmitterCache(), ResetCache(), ResetCacheDestroyed(), or manually stored the component yourself.
        /// </summary>
        [HideInInspector, NonSerialized]
        public new ParticleEmitter particleEmitter;

        /// <summary>
        /// Caches the current ParticleEmitter attached to the object.
        /// </summary>
        public void ResetParticleEmitterCache() { particleEmitter = GetComponent<ParticleEmitter>(); }

        /// <summary>
        /// The cached ParticleSystem component attached to the object.
        /// Will be null if you haven't called ResetParticleSystemCache(), ResetCache(), ResetCacheDestroyed(), or manually stored the component yourself.
        /// </summary>
        [HideInInspector, NonSerialized]
        public new ParticleSystem particleSystem;

        /// <summary>
        /// Caches the current ParticleSystem attached to the object.
        /// </summary>
        public void ResetParticleSystemCache() { particleSystem = GetComponent<ParticleSystem>(); }

        /// <summary>
        /// The cached Renderer component attached to the object.
        /// Will be null if you haven't called ResetRendererCache(), ResetCache(), ResetCacheDestroyed(), or manually stored the component yourself.
        /// </summary>
        [HideInInspector, NonSerialized]
        public new Renderer renderer;

        /// <summary>
        /// Caches the current Renderer attached to the object.
        /// </summary>
        public void ResetRendererCache() { renderer = GetComponent<Renderer>(); }

        /// <summary>
        /// The cached Rigidbody component attached to the object.
        /// Will be null if you haven't called ResetRigidbodyCache(), ResetCache(), ResetCacheDestroyed(), or manually stored the component yourself.
        /// </summary>
        [HideInInspector, NonSerialized]
        public new Rigidbody rigidbody;

        /// <summary>
        /// Caches the current Rigidbody attached to the object.
        /// </summary>
        public void ResetRigidbodyCache() { rigidbody = GetComponent<Rigidbody>(); }

        /// <summary>
        /// The cached Rigidbody2D component attached to the object.
        /// Will be null if you haven't called ResetRigidbody2DCache(), ResetCache(), ResetCacheDestroyed(), or manually stored the component yourself.
        /// </summary>
        [HideInInspector, NonSerialized]
        public new Rigidbody2D rigidbody2D;

        /// <summary>
        /// Caches the current Rigidbody2D attached to the object.
        /// </summary>
        public void ResetRigidbody2DCache() { rigidbody2D = GetComponent<Rigidbody2D>(); }

        /// <summary>
        /// The cached Transform component attached to the object.
        /// Will be null if you haven't called ResetTransformCache(), ResetCache(), ResetCacheDestroyed(), or manually stored the component yourself.
        /// </summary>
        [HideInInspector, NonSerialized]
        public new Transform transform;

        /// <summary>
        /// Caches the current Transform attached to the object.
        /// </summary>
        public void ResetTransformCache() { transform = GetComponent<Transform>(); }

        /// <summary>
        /// Caches all of the current standard components attached to the object.
        /// </summary>
        public void ResetCache()
        {
            animation = GetComponent<Animation>();
            audio = GetComponent<AudioSource>();
            camera = GetComponent<Camera>();
            collider = GetComponent<Collider>();
            collider2D = GetComponent<Collider2D>();
            constantForce = GetComponent<ConstantForce>();
            guiText = GetComponent<GUIText>();
            guiTexture = GetComponent<GUITexture>();
            hingeJoint = GetComponent<HingeJoint>();
            light = GetComponent<Light>();
            networkView = GetComponent<NetworkView>();
            particleEmitter = GetComponent<ParticleEmitter>();
            particleSystem = GetComponent<ParticleSystem>();
            renderer = GetComponent<Renderer>();
            rigidbody = GetComponent<Rigidbody>();
            rigidbody2D = GetComponent<Rigidbody2D>();
            transform = GetComponent<Transform>();
        }

        /// <summary>
        /// Calls GetComponent for all cached components if the currently cached instance is no longer valid.
        /// </summary>
        public void ResetCacheDestroyed()
        {
            if (!animation)
            {
                animation = GetComponent<Animation>();
            }
            if (!audio)
            {
                audio = GetComponent<AudioSource>();
            }
            if (!camera)
            {
                camera = GetComponent<Camera>();
            }
            if (!collider)
            {
                collider = GetComponent<Collider>();
            }
            if (!collider2D)
            {
                collider2D = GetComponent<Collider2D>();
            }
            if (!constantForce)
            {
                constantForce = GetComponent<ConstantForce>();
            }
            if (!guiText)
            {
                guiText = GetComponent<GUIText>();
            }
            if (!guiTexture)
            {
                guiTexture = GetComponent<GUITexture>();
            }
            if (!hingeJoint)
            {
                hingeJoint = GetComponent<HingeJoint>();
            }
            if (!light)
            {
                light = GetComponent<Light>();
            }
            if (!networkView)
            {
                networkView = GetComponent<NetworkView>();
            }
            if (!particleEmitter)
            {
                particleEmitter = GetComponent<ParticleEmitter>();
            }
            if (!particleSystem)
            {
                particleSystem = GetComponent<ParticleSystem>();
            }
            if (!renderer)
            {
                renderer = GetComponent<Renderer>();
            }
            if (!rigidbody)
            {
                rigidbody = GetComponent<Rigidbody>();
            }
            if (!rigidbody2D)
            {
                rigidbody2D = GetComponent<Rigidbody2D>();
            }
            if (!transform)
            {
                transform = GetComponent<Transform>();
            }
        }
    }
}
