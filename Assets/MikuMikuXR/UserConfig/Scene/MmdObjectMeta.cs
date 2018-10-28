using System.Collections.Generic;
using UnityEngine;

namespace MikuMikuXR.UserConfig.Scene
{
    public class MmdObjectMeta
    {
        public string ModelPath { get; set; }
        public string MotionPath { get; set; }
        public string BonePosePath { get; set; }
        public List<float> Position { get; set; }
        public List<float> Rotation { get; set; }
        public float Scale { get; set; }
    }
}