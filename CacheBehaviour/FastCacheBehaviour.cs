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
    public abstract class FastCacheBehaviour : MonoBehaviour
    {
        private Animation _animation;
        public new Animation animation { get { return ReferenceEquals(_animation, null) ? (_animation = GetComponent<Animation>()) : _animation; } }

        private AudioSource _audio;
        public new AudioSource audio { get { return ReferenceEquals(_audio, null) ? (_audio = GetComponent<AudioSource>()) : _audio; } }

        private Camera _camera;
        public new Camera camera { get { return ReferenceEquals(_camera, null) ? (_camera = GetComponent<Camera>()) : _camera; } }

        private Collider _collider;
        public new Collider collider { get { return ReferenceEquals(_collider, null) ? (_collider = GetComponent<Collider>()) : _collider; } }

        private Collider2D _collider2D;
        public new Collider2D collider2D { get { return ReferenceEquals(_collider2D, null) ? (_collider2D = GetComponent<Collider2D>()) : _collider2D; } }

        private ConstantForce _constantForce;
        public new ConstantForce constantForce { get { return ReferenceEquals(_constantForce, null) ? (_constantForce = GetComponent<ConstantForce>()) : _constantForce; } }

        private GUIText _guiText;
        public new GUIText guiText { get { return ReferenceEquals(_guiText, null) ? (_guiText = GetComponent<GUIText>()) : _guiText; } }

        private GUITexture _guiTexture;
        public new GUITexture guiTexture { get { return ReferenceEquals(_guiTexture, null) ? (_guiTexture = GetComponent<GUITexture>()) : _guiTexture; } }

        private HingeJoint _hingeJoint;
        public new HingeJoint hingeJoint { get { return ReferenceEquals(_hingeJoint, null) ? (_hingeJoint = GetComponent<HingeJoint>()) : _hingeJoint; } }

        private Light _light;
        public new Light light { get { return ReferenceEquals(_light, null) ? (_light = GetComponent<Light>()) : _light; } }

        private NetworkView _networkView;
        public new NetworkView networkView { get { return ReferenceEquals(_networkView, null) ? (_networkView = GetComponent<NetworkView>()) : _networkView; } }

        private ParticleEmitter _particleEmitter;
        public new ParticleEmitter particleEmitter { get { return ReferenceEquals(_particleEmitter, null) ? (_particleEmitter = GetComponent<ParticleEmitter>()) : _particleEmitter; } }

        private ParticleSystem _particleSystem;
        public new ParticleSystem particleSystem { get { return ReferenceEquals(_particleSystem, null) ? (_particleSystem = GetComponent<ParticleSystem>()) : _particleSystem; } }

        private Renderer _renderer;
        public new Renderer renderer { get { return ReferenceEquals(_renderer, null) ? (_renderer = GetComponent<Renderer>()) : _renderer; } }

        private Rigidbody _rigidbody;
        public new Rigidbody rigidbody { get { return ReferenceEquals(_rigidbody, null) ? (_rigidbody = GetComponent<Rigidbody>()) : _rigidbody; } }

        private Rigidbody2D _rigidbody2D;
        public new Rigidbody2D rigidbody2D { get { return ReferenceEquals(_rigidbody2D, null) ? (_rigidbody2D = GetComponent<Rigidbody2D>()) : _rigidbody2D; } }

        private Transform _transform;
        public new Transform transform { get { return ReferenceEquals(_transform, null) ? (_transform = GetComponent<Transform>()) : _transform; } }
    }
}
