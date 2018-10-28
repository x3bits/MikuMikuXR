using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MikuMikuXR.DTO;
using MikuMikuXR.UI.Page;
using MikuMikuXR.UserConfig;
using MikuMikuXR.UserConfig.Path;
using MikuMikuXR.UserConfig.Scene;
using MikuMikuXR.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TinyTeam.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace MikuMikuXR.SceneController
{
    public class StartSceneController : MonoBehaviour
    {
        private bool _firstRun;

        private static volatile bool _updateChecked;

        public static StartSceneController Instance;

        private void Start()
        {
            Instance = this;
            Screen.orientation = ScreenOrientation.Portrait;
            UpdateFirstRun();
            TTUIPage.ShowPage<StartBackground>();
            TTUIPage.ShowPage<StartPage>();
            if (!_firstRun && AppConfig.RuleAgreed)
            {
                InitResource();
            }
            else
            {
                TTUIPage.ShowPage<RulePage>();
            }
            if (_updateChecked) return;
            StartCoroutine(CheckUpdate());
            _updateChecked = true;
        }

        private void OnDestroy()
        {
            TTUIPage.ClearNodes();
        }

        public void InitResource()
        {
            if (NeedExtractResource())
            {
                StartCoroutine(ExtractResourceFile());
            }
        }

        private void UpdateFirstRun()
        {
            var version = Application.version;
            Debug.LogFormat("Application Version: {0}, Last Application Version: {1}", version,
                AppConfig.LastApplicationVersion);
            _firstRun = !version.Equals(AppConfig.LastApplicationVersion);
            AppConfig.LastApplicationVersion = Application.version;
        }

        private bool NeedExtractResource()
        {
            if (_firstRun)
            {
                return true;
            }
            var path = Paths.Getter().Home() + "/Default_Resource/Model/XR_1_0";
            return !(new DirectoryInfo(path).Exists);
        }

        private static IEnumerator ExtractResourceFile()
        {
            TtuiUtils.GetPage<StartPage>().SetStartButtonEnable(false);
            TTUIPage.ShowPage<LoadingTip>();
            TtuiUtils.GetPage<LoadingTip>().SetText("第一次运行解压中...");
            var textAsset = Resources.Load("LocalFiles/DefaultResource") as TextAsset;
            if (textAsset != null)
            {
                var bytes = textAsset.bytes;
                var tempFilePath = Application.temporaryCachePath + "/MikuMikuARTemp.zip";
                var appDataPath = Paths.Getter().Home();
                var extractDataThread = new Thread(() =>
                {
                    Directory.CreateDirectory(appDataPath);
                    FileUtils.ExtractZipBytesToFolder(appDataPath, bytes, tempFilePath);
                });
                extractDataThread.Start();
                while (extractDataThread.IsAlive)
                {
                    yield return null;
                }
            }
            else
            {
                Directory.CreateDirectory(Paths.Getter().Home());
                Debug.Log("No resource file under LocalFiles/DefaultResource. Skip extracting.");
            }

            TTUIPage.ClosePage<LoadingTip>();
            TtuiUtils.GetPage<StartPage>().SetStartButtonEnable(true);
        }

        private static IEnumerator CheckUpdate()
        {
            var url = "http://xrappdata.mikumikuar.com/api/update?crrVerName="
                      + WWW.EscapeURL(Application.version)
                      + "&crrVerCode=" + AppConfig.VersionCode
                      + "&platform=" + WWW.EscapeURL(Application.platform.ToString())
                      + "&deviceId=" + WWW.EscapeURL(SystemInfo.deviceUniqueIdentifier)
                      + "&deviceModel=" + WWW.EscapeURL(SystemInfo.deviceModel)
                      + "&opSys=" + WWW.EscapeURL(SystemInfo.operatingSystem);

            using (var www = new WWW(url))
            {
                yield return www;
                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogErrorFormat("error access update url, error message = {0}", www.error);
                    yield break;
                }
                var serializerSettings =
                    new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};
                try
                {
                    var updateVersionInfo =
                        JsonConvert.DeserializeObject<UpdateVersionDto>(www.text, serializerSettings);
                    if (updateVersionInfo.VersionCode > AppConfig.VersionCode)
                    {
                        ShowUpdateDialog(updateVersionInfo);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private static void ShowUpdateDialog(UpdateVersionDto updateVersionInfo)
        {
            TTUIPage.ShowPage<UpdateDialog>(updateVersionInfo);
        }

    }
}