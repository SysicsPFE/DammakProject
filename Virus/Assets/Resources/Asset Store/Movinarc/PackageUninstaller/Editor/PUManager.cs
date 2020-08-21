//(c) 2016 Movinarc
//http://movinarc.com/unity-package-uninstaller
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using System.IO.Compression;
using System.Reflection;

//SharpCompress: https://sharpcompress.codeplex.com/
using SharpCompress;
using SharpCompress.Reader;
using SharpCompress.Common;
using System.Collections;
using System.ComponentModel;

namespace Movinarc
{
    public class PuManager : EditorWindow
    {
        Vector2 _scrollPos = Vector2.zero;
        Texture2D _putitle = null;
        Texture2D _puicon = null;
        Texture2D _line = null;
        Texture2D _imgInfo;
        Texture2D _imgQuestion;
        Texture2D _openicon = null;
        string _customPackage = string.Empty;
        GUIStyle _horAlignStyle = null;
        GUIStyle _fontStyle = null;
        GUIStyle[] _searchStyles = null;
        string _searchText = "";
        List<Package> _packages = null;
        List<Package> _assetStoreList = null;
        Rect _rectScrollView;
        double _lastTime = 0;
        bool _blinkOn = true;
        string _unityPath = "";
        string _infoToShow = "";

        struct Package
        {
            public string fullPath;
            public string fileName;
            public string dateString;
            public DateTime date;
        }

        public class AssetPath
        {
            public string name = "";
            public string filePath = "";
            public bool isDirectory = false;
        }

        [MenuItem("Assets/Uninstall Package...")]
        static void CreateWindow()
        {
            var win = EditorWindow.GetWindow(typeof(PuManager));
            win.minSize = new Vector2(410, 450);
            win.titleContent.text = "Uninstall Package";
            win.titleContent.tooltip = "v1.3";
            win.Focus();
        }

        public void Update()
        {
            if (EditorApplication.timeSinceStartup - _lastTime > .25)
            {
                _blinkOn = !_blinkOn;
                _lastTime = EditorApplication.timeSinceStartup;
                Repaint();
            }
        }

        public void OnGUI()
        {
            #region init
            if (_packages == null || _packages.Count <= 0)
            {
                TreeView tv = CreateInstance<TreeView>();
                tv.allCheckedByDefault = true;
                _packages = new List<Package>();

                #if UNITY_EDITOR_WIN
                _unityPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                _unityPath = Path.Combine(_unityPath, "Unity");
 
                #elif UNITY_EDITOR_OSX

                unityPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Library/Unity";
                #endif
                var info = new DirectoryInfo(_unityPath);
                var files = info.GetFiles("*.unitypackage", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    _packages.Add(new Package()
                        { fileName = Path.GetFileNameWithoutExtension(file.FullName), 
                            fullPath = file.FullName,
                            date = file.CreationTime, dateString = file.CreationTime.ToString("g")
                        });
                }
                _packages = _packages.OrderBy(i => i.fileName).ToList();
                _assetStoreList = new List<Package>(_packages);


                _putitle = Resources.Load("PU_title") as Texture2D;
                _puicon = Resources.Load("PU_icon") as Texture2D;
                _line = Resources.Load("pu_line") as Texture2D;
                _imgInfo = Resources.Load("pu_i") as Texture2D;
                _imgQuestion = Resources.Load("pu_q") as Texture2D;

                _openicon = Resources.Load("Open_icon") as Texture2D;
                _horAlignStyle = new GUIStyle();
                _horAlignStyle.padding = new RectOffset(0, 5, 15, 0);

                _fontStyle = new GUIStyle();
                _fontStyle.fontSize = 9;
                _fontStyle.normal.textColor = new Color(.2f, .2f, .2f);
                _fontStyle.padding = new RectOffset(5, 0, 5, 0);
                _fontStyle.wordWrap = true;


                _searchStyles = new GUIStyle[3];
                _searchText = "";
            }
            #endregion

            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label(_putitle);
            #region Search/Select
            _searchStyles[0] = GUI.skin.FindStyle("Toolbar");
            _searchStyles[1] = GUI.skin.FindStyle("ToolbarSeachTextField");
            _searchStyles[2] = GUI.skin.FindStyle("ToolbarSeachCancelButton");

            GUILayout.BeginHorizontal();
            GUILayout.Box("Search / Select ", GUILayout.ExpandWidth(true));
            GUILayout.Label(_imgQuestion, GUILayout.ExpandWidth(false));
            var qrect = GUILayoutUtility.GetLastRect();
            if (qrect.Contains(Event.current.mousePosition))
            {
                _infoToShow = "[Search / Select] \nType the name of the package you are looking for to filter the list of packages.";
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(_searchStyles[0]);
            GUI.SetNextControlName("txtSearch");
            _searchText = GUILayout.TextField(_searchText, _searchStyles[1]);
        
            if (GUILayout.Button("", _searchStyles[2]))
            {
                _searchText = "";
                GUI.FocusControl(null);
            }

            GUI.FocusControl("txtSearch");
            GUILayout.EndHorizontal();

            GUILayout.Space(15.0f);
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.BeginVertical();

            if (_assetStoreList != null && _assetStoreList.Count > 0)
            {
                foreach (var item in _assetStoreList)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(new GUIContent(_puicon, "Uninstall"), GUILayout.ExpandWidth(false)))
                    {
                        CheckUninstall(item.fullPath);
                    }
                    var irect = GUILayoutUtility.GetLastRect();
                    irect.width = position.width;
                    if (irect.Contains(Event.current.mousePosition) && _rectScrollView.Contains(Event.current.mousePosition))
                    {
                        _infoToShow = String.Format("[Name] {0}\n[Path] {1}\n[Downladed] {2}", item.fileName, item.fullPath, item.dateString);
                    }
                    GUILayout.Label(item.fileName, this._horAlignStyle, GUILayout.MinWidth(230));


                    GUILayout.EndHorizontal();
                    GUILayout.Space(5.0f);
                    GUILayout.Label(_line);
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            if (Event.current.type == EventType.Repaint)
            {
                _rectScrollView = GUILayoutUtility.GetLastRect();
                _rectScrollView.x = _scrollPos.x;
                _rectScrollView.y = _scrollPos.y;
            }
            #endregion

            GUILayout.Space(10.0f);

            #region Browse
            GUILayout.BeginHorizontal();
            GUILayout.Box("Browse ", GUILayout.ExpandWidth(true));
            GUILayout.Label(_imgQuestion, GUILayout.ExpandWidth(false));
            qrect = GUILayoutUtility.GetLastRect();
            if (qrect.Contains(Event.current.mousePosition))
            {
                _infoToShow = "[Browse] \nClick Open to select a unity package file, from your computer. Then click uninstall to unimport the package.";
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent(_openicon, "Open"), GUILayout.ExpandWidth(false))) // Custom Open Button
            {
                var f = EditorUtility.OpenFilePanel("Select the .unitypackage file you are going to remove from project", "", "unitypackage");
                if (System.IO.File.Exists(f))
                {
                    this._customPackage = f;
                }
            }
            if (string.IsNullOrEmpty(_customPackage))
                GUI.enabled = false;
            else
                GUI.enabled = true;
            if (GUILayout.Button(new GUIContent(_puicon, "Uninstall"), GUILayout.ExpandWidth(false))) // Custom Uninstall Button
            {
                CheckUninstall(this._customPackage);
            }
            GUI.enabled = true;
            GUILayout.BeginVertical();
            if (string.IsNullOrEmpty(_customPackage))
                GUILayout.Label("---", _horAlignStyle);
            else
            {
                FileInfo fi = new FileInfo(_customPackage);
                var pkgName = Path.GetFileNameWithoutExtension(fi.FullName);
                GUILayout.BeginHorizontal();
                GUILayout.Label(pkgName, this._horAlignStyle, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
                var irect = GUILayoutUtility.GetLastRect();
                if (irect.Contains(Event.current.mousePosition))
                {
                    _infoToShow = String.Format("[Name] {0}\n[Path] {1}\n[Downladed] {2}", pkgName, fi.FullName, fi.CreationTime.ToString("g"));
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(10.0f);
            GUILayout.Label(_line, GUILayout.ExpandWidth(true));

            if (GUI.changed)
            {
                FilterAssetStoreList(_searchText);
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label(_imgInfo);

            if (!String.IsNullOrEmpty(_infoToShow))
            {
                GUILayout.Label(_infoToShow, _fontStyle, GUILayout.Height(70), GUILayout.ExpandWidth(true));
            }
            else
            {
                GUILayout.Label("", _fontStyle, GUILayout.Height(70), GUILayout.ExpandWidth(true));
            }
            GUILayout.EndHorizontal();

        }

        void FilterAssetStoreList(string what)
        {
            if (_packages != null)
                _assetStoreList = _packages.FindAll(x => x.fileName.ToLowerInvariant().Contains(what.ToLowerInvariant()));
        }

        public void PrepareForUnimport(string file2Import, string tempPath)
        {
            // test absolute
            if (!File.Exists(file2Import))
            {
                // test relative
                file2Import = Path.Combine(Environment.CurrentDirectory, file2Import);
                if (!File.Exists(file2Import))
                    throw new FileNotFoundException(file2Import);
            }

            if (!file2Import.ToLower().EndsWith(".unitypackage"))
                throw new Exception("You must select *.unitypackage files.");

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            EditorUtility.DisplayProgressBar("Uninstalling Package", "Initializing...", .25f);
            HolyGZip(file2Import, tempPath);

            EditorUtility.DisplayProgressBar("Uninstalling Package", "Fetching Package Structure...", .5f);
            List<AssetPath> fileList = GenerateAssetList(tempPath);


            var assetList = AssetDatabase.GetAllAssetPaths().ToList();
            var foundList = new List<string>();
            foreach (var item in assetList)
            {
                if (fileList.Exists((f) => f.filePath.Equals(item, StringComparison.OrdinalIgnoreCase)))
                {
                    foundList.Add(item);
                }
            }
            var dirs = PuSelection.DirectoriesPathList(fileList);

            EditorUtility.ClearProgressBar();
            bool goOn = true;
            if (foundList.Count <= 0)
            {
                if (!EditorUtility.DisplayDialog("No files found.", "It seems that '" + Path.GetFileNameWithoutExtension(file2Import) + "' doesn't exist in your project. Do you want to continue anyway?", "Continue Anyway", "No"))
                {
                    goOn = false;
                    PuSelection.RemoveMess(tempPath);
                }
            }
            if (goOn)
            {
                PuSelection pus = ScriptableObject.CreateInstance<PuSelection>();
                pus.fileList = fileList;
                pus.directories = dirs;
                pus.foundList = foundList;
                pus.packageName = Path.GetFileNameWithoutExtension(file2Import);
                pus.packagePath = file2Import;
                pus.tempPath = tempPath;
                pus.titleContent = new GUIContent("");
                pus.minSize = new Vector2(300, 200);

                pus.ShowUtility();
            }
        }

        void CheckUninstall(string customPackage)
        {
            try
            {

                UninstallPackage(customPackage);

            }
            catch (Exception ex)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError(ex.Message);
            }
        }

        public void UninstallPackage(string path)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "PU" + DateTime.Now.Ticks.ToString());

            PrepareForUnimport(path, tempPath);
        }

        public void HolyGZip(string gzipFileName, string targetDir)
        {
            using (Stream stream = File.OpenRead(gzipFileName))
            {
                var reader = ReaderFactory.Open(stream);
                var progress = .25f;

                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        reader.WriteEntryToDirectory(targetDir, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                        progress += .75f * reader.Entry.Size / stream.Length;
                        EditorUtility.DisplayProgressBar("Uninstalling Package", "Analysing... ", progress);

                    }
                }
            }
        }

        private List<AssetPath> GenerateAssetList(string contentPath)
        {
            string appPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf(@"Assets"));
            var lst = new List<AssetPath>();
            var directoryInfo = new DirectoryInfo(contentPath).GetDirectories();
            foreach (var item in directoryInfo)
            {
                string pathnameFromFile = File.ReadAllLines(Path.Combine(item.FullName, "pathname")).ToList().First();
                //ensure path correction
                pathnameFromFile = pathnameFromFile.Replace(@"\\", "/");
                pathnameFromFile = pathnameFromFile.Replace(@"\", "/");

                var asset = new AssetPath();
                if (Directory.Exists(Path.Combine(appPath, pathnameFromFile)))
                {
                    asset.name = Path.GetDirectoryName(pathnameFromFile);

                    if (!pathnameFromFile.EndsWith("/"))
                    {
                        pathnameFromFile += "/";
                    }
                }
                else
                {
                    asset.name = Path.GetFileName(pathnameFromFile);
                }
                asset.filePath = pathnameFromFile;

                lst.Add(asset);
            }
            var ordered = lst.OrderByDescending(i => i.filePath.Count(x => x == '/'));

            return ordered.ToList();
        }
    }
}