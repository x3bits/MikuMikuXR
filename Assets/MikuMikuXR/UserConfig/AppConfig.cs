using UnityEngine;

namespace MikuMikuXR.UserConfig
{
    public class AppConfig
    {
        public static string LastApplicationVersion {
            get {
                return PlayerPrefs.GetString ("lastVersion", "");
            }
            set {
                PlayerPrefs.SetString ("lastVersion", value);
                PlayerPrefs.Save();
            }
        }

        public static bool RuleAgreed {
            get {
                return PlayerPrefs.GetInt ("ruleAgreed", 0) > 0;
            }
            set {
                PlayerPrefs.SetInt ("ruleAgreed", value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        public static long VersionCode {
            get { return 20181008000L; }
        }
    }
}