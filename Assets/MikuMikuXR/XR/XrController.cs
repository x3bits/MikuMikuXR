namespace MikuMikuXR.XR
{
    public interface IXrController
    {
        void Create();

        void Destroy();

        XrType GetType();

        bool EnableGesture();
    }
}