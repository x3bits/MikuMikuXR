using UnityEngine;
using UnityEngine.Events;

namespace MikuMikuXR.Components
{
    public class TouchEventDetector : MonoBehaviour
    {
        private bool _isEnabled;
        private int _lastCount;
        private TouchData _downTouch;
        private TouchData _lastTouch;

        private class TouchData
        {
            public float Time;
            public int Frame;
            public Vector2 Pos;
        }

        public bool Enabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled == value)
                {
                    return;
                }
                _isEnabled = value;
                if (_isEnabled)
                {
                    Update();
                }
            }
        }

        public UnityEvent OnShortClick;

        private void Update()
        {
            if (!Enabled)
            {
                return;
            }
            var touchCount = GetTouchCount();
            if (touchCount == 1)
            {
                _lastTouch = GetTouchData();
                if (_lastCount == 0)
                {
                    _downTouch = _lastTouch;
                }
            }
            if (_lastCount == 1 && touchCount == 0)
            {
                var upTouch = _lastTouch;
                if ((upTouch.Time - _downTouch.Time < 0.5f || upTouch.Frame - _downTouch.Frame < 2)
                    && (upTouch.Pos - _downTouch.Pos).magnitude <
                    Mathf.Max(20.0f, Mathf.Min(Screen.width, Screen.height) / 100.0f))
                {
                    OnShortClick.Invoke();
                }
            }
            _lastCount = touchCount;
        }

        private static TouchData GetTouchData()
        {
            var ret = new TouchData
            {
                Time = Time.time,
                Frame = Time.frameCount,
                Pos = Input.mousePosition
            };
        #if UNITY_EDITOR
            ret.Pos = Input.mousePosition;
        #else 
		    ret.Pos = Input.GetTouch (0).position;
		#endif
            return ret;
        }

        private static int GetTouchCount()
        {
            #if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                return 1;
            }
            return 0;
            #else
		    return Input.touchCount;
		    #endif
        }
    }
}