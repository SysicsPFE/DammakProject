using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace Movinarc
{
    public class PuSelection : EditorWindow
    {
      
        TreeView _fileTree = null;
        Texture2D _fold1 = null;
        Texture2D _filepic = null;
        Texture2D _line = null;
        GUIStyle _boldStyle = null;
        Rect _rectScroll;
        Vector2 _scrollPos = Vector2.zero;
        public List<PuManager.AssetPath> fileList;
        public List<string> foundList;
        public List<string> directories;
        public string tempPath;
        public string packageName = "";
        public string packagePath;

        public static List<string> DirectoriesPathList(List<PuManager.AssetPath> paths)
        {
            List<string> possibleDirs = new List<string>();
            List<TreeView.NodePath> files = new List<TreeView.NodePath>();
            paths.ForEach(
                (p) =>
                {
                    var ph = TreeView.PathHierarchy(p.filePath);
                    foreach (var item in ph)
                    {
                        if (!possibleDirs.Exists((d) => d.Equals(item.path, System.StringComparison.OrdinalIgnoreCase)) &&
                            !item.path.Equals(p.filePath, System.StringComparison.OrdinalIgnoreCase))
                        {
                            possibleDirs.Add(item.path);
                        }
                    }
                    files.Add(new TreeView.NodePath(){ name = p.name, isDirectory = false, path = p.filePath });
                }
            );
            return possibleDirs;
        }

        void Awake()
        {
            _boldStyle = new GUIStyle(GUI.skin.button);
            _boldStyle.fontStyle = FontStyle.Bold;

        }

        void OnGUI()
        {
            if (_fileTree == null)
            {
                _fileTree = ScriptableObject.CreateInstance<TreeView>();
                if (fileList != null)
                {
                    _fold1 = Resources.Load("pu_fold") as Texture2D;
                    _filepic = Resources.Load("pu_file2") as Texture2D;
                    _line = Resources.Load("pu_line") as Texture2D;
                   
                    fileList.ForEach((f) => _fileTree.AddNodeFromPath(f.filePath, directories, foundList));
                    _rectScroll = new Rect(0, 0, this.position.width, 50);
                }
                _fileTree.ParentalCheck();
                _fileTree.EmptyFolderCheck(directories);
            }

            GUILayout.Box(packageName, GUILayout.ExpandWidth(true));
            GUILayout.Label(_line);
            GUILayout.Label("");
            var rect = GUILayoutUtility.GetLastRect();
            if (fileList != null)
            {
                _rectScroll.x = rect.x;
                _rectScroll.y = rect.y;
                _scrollPos = GUI.BeginScrollView(new Rect(rect.x, rect.y + rect.height, this.position.width - rect.x, this.position.height - rect.height - rect.y - 40), 
                    _scrollPos,
                    _rectScroll);
                _fileTree.PopulateTree(_fileTree.treeroot, ref rect, _fold1, _filepic);

                GUI.EndScrollView();
                if (_rectScroll.width < rect.x)
                    _rectScroll.width += rect.x;
                if (_rectScroll.height < rect.y)
                    _rectScroll.height += rect.y;
                
                var recBtn = new Rect(5, this.position.height - 30, 50, 20);
                if (GUI.Button(recBtn, "All"))
                {
                    _fileTree.CheckAll(_fileTree.treeroot, true);
                }
                recBtn.x += 55;
                if (GUI.Button(recBtn, "None"))
                {
                    _fileTree.CheckNone(_fileTree.treeroot);
                }
                recBtn.x = this.position.width - 130;
                if (GUI.Button(recBtn, "Cancel"))
                {
                    if (Directory.Exists(this.tempPath))
                        RemoveMess(this.tempPath);
                    this.Close();
                }
                recBtn.x = this.position.width - 75;
                recBtn.width = 70;
                if (_fileTree.selectedNodes != null && _fileTree.selectedNodes.Count > 0)
                    GUI.enabled = true;
                else
                    GUI.enabled = false;
                if (GUI.Button(recBtn, "Uninstall", _boldStyle))
                {
                    Uninstall(this.packagePath);
                   
                }
                if (position.width < rect.x)
                    position = new Rect(position.x, position.y, rect.x + 30, position.height);
            }
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        void Uninstall(string customPackage)
        {
            ClearWarnings();
            var fileName = System.IO.Path.GetFileNameWithoutExtension(customPackage);
            if (EditorUtility.DisplayDialog("Delete Imported Unitypackage", string.Format("You're about to uninstall the '{0}'. Are you sure you want to delete all the files related to this package?", fileName),
                    "Yes", "No"))
            {
                try
                {
                    if (EditorUtility.DisplayDialog("Delete Imported Unitypackage", string.Format("The operation can not be undone! Are you sure?"), "Yes. Do It!", "No"))
                    {
                        int delCnt = RemoveFiles(_fileTree.selectedNodes);
                        
                        EditorUtility.DisplayProgressBar("Uninstalling Package", "Finalizing...", 1f);
                        if (Directory.Exists(this.tempPath))
                            RemoveMess(this.tempPath);
                        
                        EditorUtility.ClearProgressBar();
                        string msg = string.Format("{0} files/folders related to '{1}' deleted from project.", delCnt, fileName);
                        if (EditorUtility.DisplayDialog("Package Uninstaller", msg, "Ok"))
                        {
                            Debug.Log(msg);
                            EditorWindow.GetWindow(typeof(PuSelection)).Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    EditorUtility.ClearProgressBar();
                    Debug.LogError(ex.Message);
                }
            }
        }

        private int RemoveFiles(List<TreeNode> filelist)
        {
            float step = .5f / filelist.Count;
            float progress = .5f + step;
            var dirs = new List<string>();
            int deleted = 0;
            string appPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf(@"Assets"));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            foreach (var f in filelist)
            {

                EditorUtility.DisplayProgressBar("Uninstalling Package", string.Format("Removing {0}", f.name), progress);
                progress += step;
                try
                {
                    string fullPath = Path.Combine(appPath, f.path);

                    var phList = TreeView.PathHierarchy(f.path);
                    phList.ForEach((ph) =>
                        {
                            if (!dirs.Exists(d => d.Equals(ph.path, StringComparison.OrdinalIgnoreCase)))
                                dirs.Add(ph.path);
                        });

                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                        deleted++;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);

                }
                finally
                {
                    try
                    {
                        //deleting meta file
                        string fullPath = Path.Combine(appPath, f.path);
                        var meta = fullPath + ".meta";
                        if (File.Exists(meta))
                            File.Delete(meta);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                    catch
                    {
                    }
                }
            }
            dirs = dirs.OrderByDescending(i => i.Count(x => x == '/')).ToList();
            foreach (var item in dirs)
            {
                var fullpath = Path.Combine(appPath, item);
                try
                {
                    //deleted += (AssetDatabase.DeleteAsset(item) ? 1 : 0);
                    if (Directory.Exists(fullpath))
                    {
                        if (Directory.GetFiles(fullpath).Length <= 0)
                        {
                            Directory.Delete(fullpath);
                            deleted++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
                finally
                {
                    try
                    {
                        var meta = fullpath + ".meta";
                        if (File.Exists(meta))
                            File.Delete(meta);
                    }
                    catch
                    {
                    }
                }
            }
            AssetDatabase.Refresh();
            return deleted;
        }

        public static void RemoveMess(string tempPath)
        {
            try
            {
                var tempPathInfo = new DirectoryInfo(tempPath);

                foreach (FileInfo file in tempPathInfo.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in tempPathInfo.GetDirectories())
                {
                    dir.Delete(true);
                }
                if (Directory.GetFiles(tempPath).Length <= 0)
                    Directory.Delete(tempPath);
            }
            catch
            {
            }
        }

        private void GetDirectoryNames(string path, List<string> list)
        {
            var parent = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(parent) && !list.Contains(parent.ToLower()) && string.Compare(parent, "assets", true) != 0)
            {
                list.Add(parent.ToLower());
                if (path.Contains("/"))
                {
                    GetDirectoryNames(parent, list);
                }
            }
        }

        public static void ClearWarnings()
        {
            var logs = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            var clear = logs.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clear.Invoke(null, null);
        }
    }
}
