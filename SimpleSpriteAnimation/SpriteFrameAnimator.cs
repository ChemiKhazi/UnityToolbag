using UnityEngine;
using System.Collections;

namespace UnityToolbag
{
    public enum SpriteFrameAnimationResetOption
    {
        DontReset,
        ResetIfNew,
        ResetAlways
    }

    [AddComponentMenu("UnityToolbag/Sprite Frame Animator")]
    public class SpriteFrameAnimator : MonoBehaviour
    {
        private SpriteFrameAnimation _currentAnimation;

        private int _frame;
        private float _timer;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private SpriteFrameAnimation[] _animations;

        [SerializeField]
        private string _startAnimation = string.Empty;

        [SerializeField]
        private bool _playOnStart = false;

        public bool isAnimationComplete
        {
            get
            {
                if (_currentAnimation == null) {
                    return true;
                }

                if (_currentAnimation.loop) {
                    return false;
                }

                return _frame == _currentAnimation.frames.Length - 1 &&
                       _timer == _currentAnimation.frameDuration;
            }
        }

        public void Play(string name)
        {
            Play(name, SpriteFrameAnimationResetOption.ResetIfNew);
        }

        public void Play(string name, SpriteFrameAnimationResetOption resetOption)
        {
            SpriteFrameAnimation newAnimation = null;

            foreach (var anim in _animations){
                if (anim.name == name) {
                    newAnimation = anim;
                    break;
                }
            }

            if (newAnimation == null) {
                _currentAnimation = null;
                return;
            }

            switch (resetOption) {
                case SpriteFrameAnimationResetOption.ResetIfNew: {
                    if (newAnimation != _currentAnimation) {
                        _frame = 0;
                        _timer = 0;
                    }
                    break;
                }
                case SpriteFrameAnimationResetOption.ResetAlways: {
                    _frame = 0;
                    _timer = 0;
                    break;
                }
                default: {
                    break;
                }
            }

            _currentAnimation = newAnimation;
            if (_currentAnimation != null) {
                UpdateWithFrameData();
            }
        }

        public void Stop()
        {
            _currentAnimation = null;
        }

        public void Stop(int stopOnIndex)
        {
            if (_currentAnimation != null) {
                _frame = stopOnIndex;
                UpdateWithFrameData();
                _currentAnimation = null;
            }
        }

        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            if (_playOnStart) {
                Play(_startAnimation);
            }
        }

        void Update()
        {
            if (_currentAnimation != null) {
                if (!isAnimationComplete) {
                    _timer += Time.deltaTime * 1000f;
                }

                if (_timer >= _currentAnimation.frameDuration) {
                    _timer -= _currentAnimation.frameDuration;

                    if (!isAnimationComplete || _currentAnimation.loop) {
                        if (_currentAnimation.loop) {
                            _frame = (_frame + 1) % _currentAnimation.frames.Length;
                        }
                        else if (_frame < _currentAnimation.frames.Length) {
                            _frame++;
                        }

                        UpdateWithFrameData();
                    }
                }
            }
        }

        void UpdateWithFrameData()
        {
            _spriteRenderer.sprite = _currentAnimation.frames[_frame];
        }
    }
}
