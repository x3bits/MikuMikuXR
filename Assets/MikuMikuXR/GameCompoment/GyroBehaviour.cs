using UnityEngine;

namespace MikuMikuXR.GameCompoment
{
    public class GyroBehaviour : MonoBehaviour
    {
        //使得初始化相机在y轴上没有旋转，正对着要看的东西
        private Quaternion _adaptQuaternion = Quaternion.identity;

        private bool _needInitAdaption = false;
        
        private void Start()
        {
            Input.gyro.enabled = true;
            _needInitAdaption = true;
        }

        private void InitAdaptQuaternion()
        {
            var rotationGyro = GyroToUnity(Input.gyro.attitude);
            var initRotation = rotationGyro;
            initRotation.y = 0.0f;
            _adaptQuaternion = Quaternion.Inverse(rotationGyro) * initRotation;
            _adaptQuaternion = Quaternion.Euler(0.0f, _adaptQuaternion.eulerAngles.y, 0.0f);
        }

        protected void Update()
        {
            if (_needInitAdaption)
            {
                InitAdaptQuaternion();
                _needInitAdaption = false;
            }
            GyroModifyCamera();
        }

        private void GyroModifyCamera()
        {
            transform.rotation = _adaptQuaternion * GyroToUnity(Input.gyro.attitude);
        }

        private static Quaternion GyroToUnity(Quaternion q)
        {
            return Quaternion.Euler(90.0f, 0.0f, 0.0f) * new Quaternion(q.x, q.y, -q.z, -q.w);
        }
    }
}

