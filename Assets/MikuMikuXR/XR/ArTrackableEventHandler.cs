using UnityEngine;
using Vuforia;

namespace MikuMikuXR.XR
{
    public class ArTrackableEventHandler : DefaultTrackableEventHandler
    {
        private const int ArCameraLayer = 8;
        
        private Camera _controlCamera;

        protected override void Start()
        {
            base.Start();
            _controlCamera = Camera.main;
            if (_controlCamera == null)
            {
                return;
            }
            _controlCamera.transform.Find("BackgroundPlane").gameObject.layer = ArCameraLayer;
            _controlCamera.cullingMask = 1 << ArCameraLayer;   
        }

        protected override void OnTrackingFound()
        {
            if (_controlCamera == null)
            {
                return;
            }
            _controlCamera.cullingMask = -1;
        }


        protected override void OnTrackingLost()
        {
            if (_controlCamera == null)
            {
                return;
            }
            _controlCamera.cullingMask = 1 << ArCameraLayer;   
        }
    }
}