using System;
using System.Collections;
using LibMMD.Reader;
using LibMMD.Unity3D.BonePose;
using MikuMikuXR.SceneController;
using UnityEngine;
using UnityEngine.UI;

namespace MikuMikuXR.UI.Page
{
    public class PhysicsCalculating : HideOtherPage
    {
        private Text _tipText;
        private BonePoseFileGenerator _generator;

        public class Context
        {
            public string ModelPath { get; set; }
            public string MotionPath { get; set; }
            public string SavePath { get; set; }
        }

        public PhysicsCalculating()
        {
            uiPath = PrefabPaths.PhysicsCalculatingPath;
        }

        public override void Awake(GameObject go)
        {
            _tipText = transform.Find("Content/Tip").GetComponent<Text>();
            SetButtonListener("Bottom/BtnCancel", () =>
            {
                if (_generator != null)
                {
                    _generator.Cancel();
                }
                ClosePage();
            });
        }

        public override void Active()
        {
            base.Active();
            _tipText.text = "准备中...";
            MainSceneController.Instance.StartCoroutine(GenerateBonePoseFile());
        }

        private IEnumerator GenerateBonePoseFile()
        {
            yield return null;
            var context = (Context) data;
            var model = ModelReader.LoadMmdModel(context.ModelPath, new ModelReadConfig {GlobalToonPath = ""});
            var motion = new VmdReader().Read(context.MotionPath);
            Debug.Log("motion load finished" + motion.Length + " frames");
            var generator =
                BonePoseFileGenerator.GenerateAsync(model, motion, context.SavePath, 1.0f / 60.0f, 5.0f, 1.0f / 120.0f);
            _generator = generator;
            while (true)
            {
                var generatorStatus = generator.Status;
                if (generatorStatus == BonePoseFileGenerator.GenerateStatus.Failed
                    || generatorStatus == BonePoseFileGenerator.GenerateStatus.Finished
                    || generatorStatus == BonePoseFileGenerator.GenerateStatus.Canceled)
                {
                    break;
                }
                if (generatorStatus == BonePoseFileGenerator.GenerateStatus.CalculatingFrames)
                {
                    _tipText.text = "已计算" + generator.CalculatedFrames + "帧, 共" + generator.TotalFrames + "帧。 已完成"
                                    + 100L * generator.CalculatedFrames / generator.TotalFrames + "%";
                }
                yield return null;
            }
            if (generator.Status == BonePoseFileGenerator.GenerateStatus.Finished)
            {
                MainSceneController.Instance.ChangeCurrentBonePoseFile(context.SavePath);
            }
            ClosePage();
            switch (generator.Status)
            {
                case BonePoseFileGenerator.GenerateStatus.Finished:
                    ShowPage<OkDialog>(new OkDialog.Context
                    {
                        Title = "提示",
                        Tip =
                            "计算完成。现在可以流畅地播放了。以后如果需要再次播放同模型配同动作，选择模型、动作后可以再载入已保存的动作结果文件。记得在选择结果文件时可能需要先点“扫描”才能看到计算结果文件。",
                        OnOk = () => { OnceTipPage.ShowOnceTip(TipNames.WhereIsBonePoseFile); }
                    });
                    break;
                case BonePoseFileGenerator.GenerateStatus.Failed:
                    ShowPage<OkDialog>(new OkDialog.Context
                    {
                        Title = "提示",
                        Tip = "计算发生错误。"
                    });
                    break;
            }
        }
    }
}