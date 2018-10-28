using System;
using System.Collections;
using System.Collections.Generic;
using MikuMikuXR.UI.ListItem;
using MikuMikuXR.UserConfig.Path;
using MikuMikuXR.UserConfig.Resource;
using NSUListView;
using TinyTeam.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MikuMikuXR.UI.Page
{
    public class ScanDialog : HideOtherPage
    {
        private Button _btnClose;
        private Button _btnStart;
        private MonoBehaviour _behaviour;
        private Text _tipText;
        
        private volatile bool _isScanning;
        private volatile string _scanningPath = "";
        
        public ScanDialog()
        {
            uiPath = PrefabPaths.ScanDialogPath;
        }

        public override void Awake(GameObject go)
        {
            SetButtonListener("Bottom/BtnClose", ClosePage);
            SetButtonListener("Bottom/BtnStart", ScanResourceFiles);
            _btnClose = FindCompoment<Button>("Bottom/BtnClose");
            _btnStart = FindCompoment<Button>("Bottom/BtnStart");
            _tipText = FindCompoment<Text>("Content/Tip");
            _behaviour = gameObject.GetComponent<MonoBehaviour>();
        }

        public override void Refresh()
        {
            _tipText.text = "点击“开始”开始扫描。";
        }

        private void ScanResourceFiles()
        {
            PrepareStartScan();
            ResourceScanner.Instance.ScanAsync(Paths.Getter().Home(), new ScanListener(this));
        }

        private void PrepareStartScan()
        {
            _btnStart.enabled = false;
            _btnClose.enabled = false;
            _isScanning = true;
            _behaviour.StartCoroutine(UpdateScanningFile());
        }

        private IEnumerator UpdateScanningFile()
        {
            while (true)
            {
                if (!_isScanning)
                {
                    OnScanFinish();
                    yield break;
                } 
                _tipText.text = _scanningPath;
                yield return null;
            }
        }

        private void OnScanFinish()
        {
            _btnStart.enabled = true;
            _btnClose.enabled = true;
            _tipText.text = "扫描完成";
            ClosePage();
        }

        private class ScanListener: ResourceScanner.IScanListener
        {
            private readonly ScanDialog _outerInstance;

            public ScanListener(ScanDialog outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public void OnScanFile(string path)
            {
                _outerInstance._scanningPath = path;
            }

            public void OnFinish(AllResources allResources)
            {
                try
                {
                    ResourceListStore.Instance.Save(Paths.Getter().ResourceList(), allResources);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                _outerInstance._isScanning = false;
            }
        }
    }
}