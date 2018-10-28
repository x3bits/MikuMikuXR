using System;

namespace MikuMikuXR.UserConfig.Path
{
    public class MacEditorPathGetter:BasePathGetter
    {
        public override string Home()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/MikuMikuAR";
        }
    }
}