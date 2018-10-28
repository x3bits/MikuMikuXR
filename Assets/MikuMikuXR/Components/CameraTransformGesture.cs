using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MikuMikuXR.Components
{
    public class CameraTransformGesture : MonoBehaviour
    {
        private readonly float _rotateSpeed = 360.0f;
        private readonly float _transformXySpeed = 100.0f;
        private readonly float _transformZSpeed = 100.0f;

        private bool _controllingUi = false;

        public CameraTransformGesture()
        {
            EnableRotation = true;
            EnableTranslation = true;
        }

        private class TouchData
        {
            public int Count;
            public Vector2 CenterPos;
            public float Distance;
            public float Angle;
        }

        private TouchData _lastTouchData;

        private void Start()
        {
            _lastTouchData = GetCurrentTouchData();
        }

        public bool EnableRotation { get; set; }
        public bool EnableTranslation { get; set; }

        public Transform MovingChild { get; set; }

        private void Update()
        {
            var touchData = GetCurrentTouchData();
            if (_controllingUi)
            {
                return;
            }
            if (touchData.Count == 0 || touchData.Count != _lastTouchData.Count)
            {
                _lastTouchData = touchData;
                return;
            }
            if (touchData.Count == 1 && _lastTouchData.Count == 1)
            {
                if (EnableRotation) RotateXyByTouch(touchData, _lastTouchData);
            }
            if (touchData.Count == 2 && _lastTouchData.Count == 2)
            {
                if (EnableTranslation) TransByTouch(touchData, _lastTouchData);
                if (touchData.Distance > (Screen.width + Screen.height) / 20.0f)
                {
                    //双指过近不进行缩放旋转操作
                    if (EnableTranslation) ZoomByTouch(touchData, _lastTouchData);
                    if (EnableRotation) RotateZByTouch(touchData, _lastTouchData);
                }
            }
            _lastTouchData = touchData;
        }


        private void RotateXyByTouch(TouchData touch, TouchData lastTouch)
        {
            var touchMotion = touch.CenterPos - lastTouch.CenterPos;
            var rotation = Vector3.zero;
            var rotationFactor = _rotateSpeed / (Screen.width + Screen.height);
            rotation.x = touchMotion.y * rotationFactor;
            rotation.y = -touchMotion.x * rotationFactor;
            transform.Rotate(rotation);
        }

        private void TransByTouch(TouchData touch, TouchData lastTouch)
        {
            var centerMotion = touch.CenterPos - lastTouch.CenterPos;
            //var oldLocalPosition =  transform.localPosition;
            var transformFactor = _transformXySpeed / (Screen.width + Screen.height);
            transform.Translate(new Vector3(-centerMotion.x * transformFactor, -centerMotion.y * transformFactor,
                0.0f));
        }

        private void ZoomByTouch(TouchData touch, TouchData lastTouch)
        {
            var factor = (touch.Distance - lastTouch.Distance) / (Screen.width + Screen.height);
            transform.Translate(new Vector3(0.0f, 0.0f, factor * _transformZSpeed));
        }

        private void RotateZByTouch(TouchData touch, TouchData lastTouch)
        {
            transform.Rotate(new Vector3(0.0f, 0.0f, lastTouch.Angle - touch.Angle));
        }

#if UNITY_EDITOR

        private TouchData GetCurrentTouchData()
        {
            TouchData ret = new TouchData();
            var touchCount = MouseButtonToTouchCount(Input.GetMouseButton(0), Input.GetMouseButton(1));
            if (touchCount == 0)
            {
                _controllingUi = false;
            }
            if (touchCount == 0 || touchCount > 2)
            {
                return ret;
            }
            if (_lastTouchData.Count == 0 && touchCount > 0 && EventSystem.current.IsPointerOverGameObject())
            {
                _controllingUi = true;
            }
            if (touchCount == 1)
            {
                ret.Count = 1;
                ret.CenterPos = Input.mousePosition;
                return ret;
            }
            ret.Count = 2;
            ret.CenterPos = Input.mousePosition;
            ret.Distance = 0.0f;
            var posVec = Vector2.up;
            ret.Angle = Vector2.Angle(posVec, Vector2.right);
            if (posVec.y < 0)
            {
                ret.Angle = -ret.Angle;
            }
            return ret;
        }

        private int MouseButtonToTouchCount(bool left, bool right)
        {
            if (left)
            {
                return 1;
            }
            if (right)
            {
                return 2;
            }
            return 0;
        }

#else

        private TouchData GetCurrentTouchData()
        {
            TouchData ret = new TouchData();
            var touchCount = Input.touchCount;
            if (touchCount == 0)
            {
                _controllingUi = false;
            }
            if (touchCount == 0 || touchCount > 2)
            {
                return ret;
            }
            if (_lastTouchData.Count == 0 && touchCount > 0 && IsTouchOverGameObject())
            {
                _controllingUi = true;
            }
            if (touchCount == 1)
            {
                ret.Count = 1;
                ret.CenterPos = Input.GetTouch(0).position;
                return ret;
            }
            ret.Count = 2;
            var pos0 = Input.GetTouch(0).position;
            var pos1 = Input.GetTouch(1).position;

            ret.CenterPos = (pos0 + pos1) / 2;
            ret.Distance = Vector2.Distance(pos0, pos1);
            var posVec = pos1 - pos0;
            ret.Angle = Vector2.Angle(posVec, Vector2.right);
            if (posVec.y < 0)
            {
                ret.Angle = -ret.Angle;
            }
            return ret;
        }
        
        //参考https://answers.unity.com/questions/1115464/ispointerovergameobject-not-working-with-touch-inp.html
        private static bool IsTouchOverGameObject()
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

#endif
    }
}