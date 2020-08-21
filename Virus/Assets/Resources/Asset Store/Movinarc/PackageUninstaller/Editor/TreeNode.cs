using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

namespace Movinarc
{
    public class TreeNode
    {
        public TreeNode()
        {
            children = new List<TreeNode>();
            lastCheckedState = isChecked;
        }
        //dynamic
        public TreeNode root;

        public bool IsTreeEmpty
        {
            get
            {
                return root.children == null || root.children.Count <= 0;
            }
        }

        public bool isExpanded = false;
        public bool isRoot = false;

        public TreeNode parent;
        public string name;
        public bool isChecked;
        public bool lastCheckedState;
        //Primary Key - dynamic
        public string path;

        //dynamic
        public bool isDirectory;

        public List<TreeNode> children;

        public List<string> PathList()
        {
            return new List<string>(path.Split(new []{ '/' }, System.StringSplitOptions.RemoveEmptyEntries));
        }

        public List<string> PathList(string path)
        {
            return new List<string>(path.Split(new []{ '/' }, System.StringSplitOptions.RemoveEmptyEntries));
        }

        public List<TreeNode> Parents
        {
            get
            {
                return new List<TreeNode>(GetParents(this));
            }
        }

        private IEnumerable<TreeNode> GetParents(TreeNode node)
        {
            if (parent != null)
            {
                yield return parent;
                GetParents(parent);
            }
        }

        public TreeNode AddNode(TreeNode node)
        {
            children.Add(node);
            return node;
        }

        public TreeNode AddNode(string name, bool isChecked)
        {
            TreeNode node = new TreeNode();
            node.parent = this;
            node.name = name;
            node.isChecked = isChecked;
            node.lastCheckedState = isChecked;
            children.Add(node);
            return node;
        }

        bool Exists()
        {
            return false;
        }

        public bool PathExists(string path)
        {
            return CheckPathExists(PathList(path), root);
        }

        private bool CheckPathExists(List<string> pathList, TreeNode fromNode)
        {
            if (pathList != null)
            {
                if (pathList.Count > 0)
                {
                    if (IsTreeEmpty)
                        return false;
                    foreach (var ch in fromNode.children)
                    {
                        string n0 = pathList[0];
                        if (ch.name.Equals(n0, StringComparison.OrdinalIgnoreCase))
                        {
                            pathList.RemoveAt(0);
                            return CheckPathExists(pathList, ch);
                      
                        }
                    }
                }

            }
            if (pathList.Count == 0)
                return true;
            else
                return false;
        }

        public TreeNode GetNodeInPath(string path, TreeNode root)
        {
            return CheckGetNodeInPath(PathList(path), root);
        }

        private TreeNode CheckGetNodeInPath(List<string> pathList, TreeNode fromNode)
        {

            if (pathList != null)
            {
                if (pathList.Count > 0)
                {
                    string n0 = pathList[0];

                    foreach (var child in fromNode.children)
                    {
                        if (child.name.Equals(n0, StringComparison.OrdinalIgnoreCase))
                        {
                            pathList.RemoveAt(0);
                            return CheckGetNodeInPath(pathList, child);
                        }
                    }
                }

            }
            if (pathList.Count == 0)
                return fromNode;
            else
                return null;
        }

        public TreeNode GetParentNodeInPath(string path, TreeNode root)
        {
            return CheckGetParentNodeInPath(PathList(path), root);
        }

        private TreeNode CheckGetParentNodeInPath(List<string> pathList, TreeNode fromNode)
        {

            if (pathList != null)
            {
                if (pathList.Count > 0)
                {
                    pathList.RemoveAt(pathList.Count - 1);
                    return GetNodeInPath(ListToPath(pathList), fromNode);
                }
            }
            return root;
        }

        public static string ListToPath(List<string> list)
        {
            string joint = String.Join("/", list.ToArray());
            joint = IHateSlashes(joint);
            return joint;
        }

        public static string IHateSlashes(string which)
        {
            if (which.Length > 0)
            {
                if (which[which.Length - 1] == '/')
                    which = which.Remove(which.Length - 1);
                if (which[0] == '/')
                    which = which.Remove(0);
            }
            return which;
        }

        public bool ExistsInChildren(string name, bool recursive)
        {
            return false;
        }

        public List<TreeNode> FindByName(string name)
        {
            return null;
        }
    }

 }
