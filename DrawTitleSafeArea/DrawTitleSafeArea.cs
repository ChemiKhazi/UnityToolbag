using UnityEngine;

namespace UnityToolbag
{
    public enum TitleSafeSizeMode
    {
        Pixels,
        Percentage
    }

    [ExecuteInEditMode]
    [AddComponentMenu("UnityToolbag/Draw Title Safe Area")]
    public class DrawTitleSafeArea : MonoBehaviour
    {
#if UNITY_EDITOR
        private Texture2D _blank;
#endif

        [SerializeField]
        private Color _innerColor = new Color(1, 1, 0, 0.15f);

        [SerializeField]
        private Color _outerColor = new Color(1, 0, 0, 0.15f);

        [SerializeField]
        private TitleSafeSizeMode _sizeMode = TitleSafeSizeMode.Percentage;

        [SerializeField]
        private int _sizeX = 5;

        [SerializeField]
        private int _sizeY = 5;

#if UNITY_EDITOR
        void OnValidate()
        {
            if (_sizeX < 0) {
                _sizeX = 0;
            }

            if (_sizeY < 0) {
                _sizeY = 0;
            }

            if (_sizeMode == TitleSafeSizeMode.Percentage) {
                if (_sizeX > 25) {
                    _sizeX = 25;
                }

                if (_sizeY > 25) {
                    _sizeY = 25;
                }
            }
        }

        void OnDestroy()
        {
            if (_blank)
            {
                DestroyImmediate(_blank);
                _blank = null;
            }
        }

        void OnGUI()
        {
            var camera = GetComponent<Camera>();
            if (!camera) {
                return;
            }

            if (!_blank) {
                _blank = new Texture2D(1, 1);
                _blank.SetPixel(0, 0, Color.white);
                _blank.hideFlags = HideFlags.HideAndDontSave;
            }

            float w = camera.pixelWidth;
            float h = camera.pixelHeight;

            // Compute the actual sizes based on the size mode and our sizes
            float wMargin = 0;
            float hMargin = 0;
            switch (_sizeMode) {
                case TitleSafeSizeMode.Percentage: {
                    wMargin = w * (_sizeX / 100.0f);
                    hMargin = h * (_sizeY / 100.0f);
                    break;
                }
                case TitleSafeSizeMode.Pixels: {
                    // Clamp to 1/4 the screen size so we never overlap the other side
                    wMargin = Mathf.Clamp(_sizeX, 0, w / 4);
                    hMargin = Mathf.Clamp(_sizeY, 0, h / 4);
                    break;
                }
            }

            // Draw the outer region first
            GUI.color = _outerColor;
            GUI.DrawTexture(new Rect(0,           0,           w,       hMargin),         _blank, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(new Rect(0,           h - hMargin, w,       hMargin),         _blank, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(new Rect(0,           hMargin,     wMargin, h - hMargin * 2), _blank, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(new Rect(w - wMargin, hMargin,     wMargin, h - hMargin * 2), _blank, ScaleMode.StretchToFill, true);

            // Then the inner region
            GUI.color = _innerColor;
            GUI.DrawTexture(new Rect(wMargin,         hMargin,         w - wMargin * 2, hMargin),         _blank, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(new Rect(wMargin,         h - hMargin * 2, w - wMargin * 2, hMargin),         _blank, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(new Rect(wMargin,         hMargin * 2,     wMargin,         h - hMargin * 4), _blank, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(new Rect(w - wMargin * 2, hMargin * 2,     wMargin,         h - hMargin * 4), _blank, ScaleMode.StretchToFill, true);
        }
#endif // UNITY_EDITOR
    }
}
