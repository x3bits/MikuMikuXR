using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MikuMikuXR.Components;
using MikuMikuXR.SceneController;
using MikuMikuXR.UI.ListItem;
using MikuMikuXR.UI.Resource;
using MikuMikuXR.UserConfig.Path;
using MikuMikuXR.UserConfig.Resource;
using MikuMikuXR.Utils;
using NSUListView;
using TinyTeam.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MikuMikuXR.UI.Page
{
    public class MmdFileSelector : HideOtherPage
    {
        private USimpleListView _fileListView;
        private Text _title;
        private Context _context;
        private Text _scanTipText;
        private RectTransform _pathBar;
        private RectTransform _viewPort;
        private RectTransform _titleBar;
        private Button _btnSwitchMode;
        private Button _btnScan;
        private Text _directoryName;

        public delegate void FileSelectHandler(string path);

        public class Context
        {
            public string Title { get; set; }
            public FileType Type { get; set; }
            public FileSelectHandler OnFileSelect { get; set; }
            public bool PathMode { get; set; }
            public string Path { get; set; }
        }

        public enum FileType
        {
            Model,
            Motion,
            Music,
            BonePose
        }

        public MmdFileSelector()
        {
            uiPath = PrefabPaths.MmdFileSelectorPath;
        }

        public override void Awake(GameObject go)
        {
            SetButtonListener("Bottom/BtnClose", ClosePage);
            _btnScan = FindCompoment<Button>("Bottom/BtnScan");
            UiUtils.SetButtonListener(_btnScan, ShowPage<ScanDialog>);
            _btnSwitchMode = FindCompoment<Button>("Bottom/BtnSwitchMode");
            UiUtils.SetButtonListener(_btnSwitchMode, () =>
            {
                _context.PathMode = !_context.PathMode;
                Refresh();
            });
            SetButtonListener("Path/DirectoryUp", () =>
            {
                var parent = new DirectoryInfo(_context.Path).Parent;
                if (parent == null) return;
                var oldPath = _context.Path;
                _context.Path = parent.FullName;
                try
                {
                    Refresh();
                }
                catch (Exception e)
                {
                    _context.Path = oldPath;
                    Refresh();
                }
                
            });
            _fileListView = FindCompoment<USimpleListView>("Viewport/Content/FileList");
            _title = FindCompoment<Text>("Title");
            _scanTipText = FindCompoment<Text>("Viewport/Content/ScanTip");
            _pathBar = FindCompoment<RectTransform>("Path");
            _viewPort = FindCompoment<RectTransform>("Viewport");
            _titleBar = FindCompoment<RectTransform>("Header");
            _directoryName = FindCompoment<Text>("Path/DirectoryName");
        }

        public override void Refresh()
        {
            base.Refresh();
            var context = (Context) data;
            if (context != null)
            {
                _context = context;
            }
            if (_context == null)
            {
                throw new ArgumentException("null context");
            }
            if (string.IsNullOrEmpty(_context.Path))
            {
                _context.Path = Paths.Getter().Home();
            }
            _title.text = _context.Title;
            RefreshPathBar();
            RefreshButtons();
            if (_context.PathMode)
            {
                RefreshByPath();
            }
            else
            {
                RefreshByResourceList();
            }
        }

        private void RefreshButtons()
        {
            _btnSwitchMode.transform.Find("Text").GetComponent<Text>().text = _context.PathMode ? "资源列表" : "文件夹";
        }

        private void RefreshByPath()
        {
            _fileListView.SetData(new List<object>());
            var currentDirectory = new DirectoryInfo(_context.Path);
            var directories = currentDirectory.GetDirectories();
            var files = currentDirectory.GetFiles();
            var list = (from dir in directories
                    let dir1 = dir
                    select new MmdFileListItemData
                    {
                        FileName = dir.Name.Replace(' ', '\u00A0'),
                        FilePath = dir.FullName.Replace(' ', '\u00A0'),
                        Icon = Sprites.FileIconDirectory,
                        OnClick = () =>
                        {
                            _context.Path = dir1.FullName;
                            Refresh();
                        }
                    }).Cast<object>()
                .ToList();
            list.AddRange((from file in files
                let file1 = file
                where GetFileExtsByFileType(_context.Type).Contains(file.Extension.ToLower())
                select new MmdFileListItemData
                {
                    FileName = file.Name.Replace(' ', '\u00A0'),
                    FilePath = file.FullName.Replace(' ', '\u00A0'),
                    Icon = GetIconByFileType(_context.Type),
                    OnClick = () =>
                    {
                        ClosePage();
                        _context.OnFileSelect(file1.FullName);
                    }
                }).Cast<object>());
            _fileListView.SetData(list);
        }

        private void RefreshByResourceList()
        {
            var listPath = Paths.Getter().ResourceList();
            AllResources allResources;
            try
            {
                allResources = ResourceListStore.Instance.Load(listPath);
            }
            catch (Exception e)
            {
                _scanTipText.gameObject.SetActive(true);
                Debug.LogWarning("load allResources exception. " + e);
                _fileListView.SetData(new List<object>());
                return;
            }
            _scanTipText.gameObject.SetActive(false);
            ResourceList resourceList;
            switch (_context.Type)
            {
                case FileType.Model:
                    resourceList = allResources.ModelList;
                    break;
                case FileType.Motion:
                    resourceList = allResources.MotionList;
                    break;
                case FileType.Music:
                    resourceList = allResources.MusicList;
                    break;
                case FileType.BonePose:
                    resourceList = allResources.BonePoseList;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _fileListView.SetData(ConvertDataForUi(resourceList));
        }

        private void RefreshPathBar()
        {
            if (_context.PathMode)
            {
                _pathBar.gameObject.SetActive(true);
                _viewPort.offsetMax = new Vector2(_viewPort.offsetMax.x, -_titleBar.rect.height - _pathBar.rect.height);
                var directoryNameText = Path.GetFileName(_context.Path) ?? "";
                _directoryName.text = directoryNameText.Replace(' ', '\u00A0');
            }
            else
            {
                _pathBar.gameObject.SetActive(false);
                _viewPort.offsetMax = new Vector2(_viewPort.offsetMax.x, -_titleBar.rect.height);
            }
        }

        private List<object> ConvertDataForUi(ResourceList resourceList)
        {
            var list = resourceList.List;
            return list.Select(info => new MmdFileListItemData
                {
                    FileName = info.Title.Replace(' ', '\u00A0'), //防止换行
                    FilePath = info.FilePath.Replace(' ', '\u00A0'),
                    Icon =  GetIconByFileType(_context.Type),
                    OnClick = () =>
                    {
                        ClosePage();
                        _context.OnFileSelect(info.FilePath);
                    }
                })
                .Cast<object>()
                .ToList();
        }

        private static Sprite GetIconByFileType(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Model:
                    return Sprites.FileIconModel;
                case FileType.Motion:
                    return Sprites.FileIconMotion;
                case FileType.Music:
                    return Sprites.FileIconMusic;
                case FileType.BonePose:
                    return Sprites.FileIconMotion;
                default:
                    throw new ArgumentOutOfRangeException("fileType", fileType, null);
            }
        }

        private HashSet<string> GetFileExtsByFileType(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Model:
                    return Constants.ModelExts;
                case FileType.Motion:
                    return Constants.MotionExts;
                case FileType.Music:
                    return Constants.MusicExts;
                case FileType.BonePose:
                    return Constants.BonePoseExts;
                default:
                    throw new ArgumentOutOfRangeException("fileType", fileType, null);
            }
        }
    }
}