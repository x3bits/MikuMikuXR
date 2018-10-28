using UnityEngine;

namespace MikuMikuXR.Components
{
    public class CameraInvertCulling : MonoBehaviour
    {
        private bool _oldInvertCulling;
        
        private void OnPreRender()
        {
            _oldInvertCulling = GL.invertCulling; 
            GL.invertCulling = true;
        }        
        
        private void OnPostRender()
        {
            GL.invertCulling = _oldInvertCulling;
        }
    }
}