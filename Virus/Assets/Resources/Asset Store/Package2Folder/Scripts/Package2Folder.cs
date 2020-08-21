// new argument was added in 19.1.4

#if (UNITY_2019_1_OR_NEWER && !UNITY_2019_1_0 && !UNITY_2019_1_1 && !UNITY_2019_1_2 && !UNITY_2019_1_3) || (UNITY_2018_4_OR_NEWER && !UNITY_2018_4_0 && !UNITY_2018_4_1 && !UNITY_2018_4_2)
#define CS_P2F_NEW_ARGUMENT
#endif

using System;
using System.IO;
using System.Reflection;
using UnityEditor;

namespace CodeStage.PackageToFolder
{
	public class Package2Folder
	{
		///////////////////////////////////////////////////////////////
		// Delegates and properties with caching for reflection stuff
		///////////////////////////////////////////////////////////////

		#region reflection stuff

#if CS_P2F_NEW_ARGUMENT
		private delegate object[] ExtractAndPrepareAssetListDelegate(string packagePath, out string packageIconPath, out bool allowReInstall, out string packageManagerDependenciesPath);
#else
		private delegate object[] ExtractAndPrepareAssetListDelegate(string packagePath, out string packageIconPath, out bool allowReInstall);
#endif

		private static Type _packageUtilityType;
		private static Type PackageUtilityType
		{
			get
			{
				if (_packageUtilityType == null)
				{
					_packageUtilityType = typeof(MenuItem).Assembly.GetType("UnityEditor.PackageUtility");
				}
				return _packageUtilityType;
			}
		}

		private static ExtractAndPrepareAssetListDelegate _extractAndPrepareAssetList;
		private static ExtractAndPrepareAssetListDelegate ExtractAndPrepareAssetList
		{
			get
			{
				if (_extractAndPrepareAssetList == null)
				{
					var method = PackageUtilityType.GetMethod("ExtractAndPrepareAssetList");
					if (method == null)
					{
						throw new Exception("Couldn't extract method with ExtractAndPrepareAssetListDelegate delegate!");
					}

					_extractAndPrepareAssetList = (ExtractAndPrepareAssetListDelegate)Delegate.CreateDelegate(
					   typeof(ExtractAndPrepareAssetListDelegate),
					   null,
					   method);
				}

				return _extractAndPrepareAssetList;
			}
		}
		
		private static FieldInfo _destinationAssetPathFieldInfo;
		private static FieldInfo DestinationAssetPathFieldInfo
		{
			get
			{
				if (_destinationAssetPathFieldInfo == null)
				{
					var importPackageItem = typeof(MenuItem).Assembly.GetType("UnityEditor.ImportPackageItem");
					_destinationAssetPathFieldInfo = importPackageItem.GetField("destinationAssetPath");
				}
				return _destinationAssetPathFieldInfo;
			}
		}

		private static MethodInfo _importPackageAssetsMethodInfo;
		private static MethodInfo ImportPackageAssetsMethodInfo
		{
			get
			{
				if (_importPackageAssetsMethodInfo == null)
				{
					_importPackageAssetsMethodInfo = PackageUtilityType.GetMethod("ImportPackageAssets");
				}

				return _importPackageAssetsMethodInfo;
			}
		}

		private static MethodInfo _showImportPackageMethodInfo;
		private static MethodInfo ShowImportPackageMethodInfo
		{
			get
			{
				if (_showImportPackageMethodInfo == null)
				{
					var packageImport = typeof(MenuItem).Assembly.GetType("UnityEditor.PackageImport");
					_showImportPackageMethodInfo = packageImport.GetMethod("ShowImportPackage");
				}

				return _showImportPackageMethodInfo;
			}
		}

		#endregion reflection stuff

		///////////////////////////////////////////////////////////////
		// Unity Editor menus integration
		///////////////////////////////////////////////////////////////

		[MenuItem("Assets/Import Package/Here...", true, 10)]
		private static bool IsImportToFolderCheck()
		{
			var selectedFolderPath = GetSelectedFolderPath();
			return !string.IsNullOrEmpty(selectedFolderPath);
		}

		[MenuItem("Assets/Import Package/Here...", false, 10)]
		private static void Package2FolderCommand()
		{
			var packagePath = EditorUtility.OpenFilePanel("Import package ...", "",  "unitypackage");
			if (string.IsNullOrEmpty(packagePath)) return;
			if (!File.Exists(packagePath)) return;
			
			var selectedFolderPath = GetSelectedFolderPath();
			ImportPackageToFolder(packagePath, selectedFolderPath, true);
		}

		///////////////////////////////////////////////////////////////
		// Main logic
		///////////////////////////////////////////////////////////////
		
		/// <summary>
		/// Allows to import package to the specified folder either via standard import window or silently.
		/// </summary>
		/// <param name="packagePath">Native path to the package.</param>
		/// <param name="selectedFolderPath">Path to the target folder where you wish to import package into.
		/// Relative to the project folder (should start with 'Assets')</param>
		/// <param name="interactive">If true - imports using standard import window, otherwise does this silently.</param>
		public static void ImportPackageToFolder(string packagePath, string selectedFolderPath, bool interactive)
		{
			string packageIconPath;
			bool allowReInstall;

#if CS_P2F_NEW_ARGUMENT
			string packageManagerDependenciesPath;
			var assetsItems = ExtractAndPrepareAssetList(packagePath, out packageIconPath, out allowReInstall, out packageManagerDependenciesPath);
#else
			var assetsItems = ExtractAndPrepareAssetList(packagePath, out packageIconPath, out allowReInstall);
#endif

			if (assetsItems == null) return;

			foreach (var item in assetsItems)
			{
				ChangeAssetItemPath(item, selectedFolderPath);
			}

			if (interactive)
			{
				ShowImportPackageWindow(packagePath, assetsItems, packageIconPath, allowReInstall);
			}
			else
			{
				ImportPackageSilently(assetsItems);
			}
		}

		private static void ChangeAssetItemPath(object assetItem, string selectedFolderPath)
		{
			var destinationPath = (string)DestinationAssetPathFieldInfo.GetValue(assetItem);
			destinationPath = selectedFolderPath + destinationPath.Remove(0, 6);
			DestinationAssetPathFieldInfo.SetValue(assetItem, destinationPath);
		}

		public static void ShowImportPackageWindow(string path, object[] array, string packageIconPath, bool allowReInstall)
		{
			ShowImportPackageMethodInfo.Invoke(null, new object[] { path, array, packageIconPath, allowReInstall });
		}

		public static void ImportPackageSilently(object[] assetsItems)
		{
			ImportPackageAssetsMethodInfo.Invoke(null, new object[] { assetsItems, false });
		}

		///////////////////////////////////////////////////////////////
		// Utility methods
		///////////////////////////////////////////////////////////////

		private static string GetSelectedFolderPath()
		{
			var obj = Selection.activeObject;
			if (obj == null) return null;

			var path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
			return !Directory.Exists(path) ? null : path;
		}
	}
}