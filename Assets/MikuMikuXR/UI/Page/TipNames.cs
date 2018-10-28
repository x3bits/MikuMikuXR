using System.Collections.Generic;
using MikuMikuXR.UserConfig.Path;

namespace MikuMikuXR.UI.Page
{
    public class TipNames
    {
        public const string Pyramid = "Pyramid";
        public const string TipText = "TipText";
        public const string ExportCustomMmdFiles = "ExportCustomMmdFiles";
        public const string HowToStart = "HowToStart";
        public const string WhereIsBonePoseFile = "WhereIsBonePoseFile";

        public static readonly IDictionary<string, string> TipNameToText = new Dictionary<string, string>
        {
            {ExportCustomMmdFiles, "你可以把自己的MikuMikuDance模型、动作文件放至手机存储器的MikuMikuAR目录下，然后点击这里的\"扫描\"按钮就可以找到它们了。"},
            {HowToStart, "先添加模型，选择动作，然后选择音乐，就可以播放了。出于版权原因本应用不自带示例模型、动作。" +
                         "你可以把自己的MikuMikuDance模型、动作文件放至手机存储器的MikuMikuAR目录下。" +
                         "如果你不知道MikuMikuDance是什么，从哪里获得模型，可以到网上搜索一下。"},
            {WhereIsBonePoseFile, "计算结果文件保存在" + Paths.Getter().BonePoseFolder() + ", 结果比较占空间。如果不需要了，你可以通过手机的文件浏览器删掉它们。"},
        };
    }
}