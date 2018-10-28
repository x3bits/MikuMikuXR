namespace MikuMikuXR.UserConfig.Path
{
    public interface IPathGetter
    {
        string Home();

        string ConfigFolder();
        
        string ResourceList();

        string SceneFolder();

        string BonePoseFolder();
    }
}