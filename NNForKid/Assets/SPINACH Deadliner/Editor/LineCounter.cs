using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ClottlyCode
{

	public class LineCounter
	{

		static private int currentLineCount = -1;
		static private AssetImportListener listener;

		[MenuItem("Clottly Codes 1st/显示当前行数",false,0)]
		static public void DisplayDialog()
		{
			EditorUtility.DisplayDialog("Clottly Codes 1st", "你已经写了 " + Count().ToString() + " 行程序。", "OK");
		}

		[MenuItem("Clottly Codes 1st/无耻的广告/[免费开源] 在UGUI中使用向量图标集", false, 5)]
		static public void OpenAD1()
		{
			Application.OpenURL("https://github.com/SPINACHCEO/UGUI-Material-Design-Icon");
		}

		static private List<string> DirSearch(string sDir)
		{
			List<string> files = new List<string>();

			foreach (string f in Directory.GetFiles(sDir))
			{
				files.Add(f);
			}
			foreach (string d in Directory.GetDirectories(sDir))
			{
				files.AddRange(DirSearch(d));
			}

			return files;
		}

		static public int Count()
		{
			if (listener == null) (listener = new AssetImportListener()).SetCallback(() => currentLineCount = -1);

			if (currentLineCount < 0)
			{
				int count = 0;
				var files = DirSearch(Application.dataPath);
				foreach (string path in files)
				{
					if (!path.EndsWith(".cs") && !path.EndsWith(".js")) continue;
					count += File.ReadAllText(path).Count(x => x == ';');
				}

				currentLineCount = count;
			}

			return currentLineCount;
		}
	}

	internal class AssetImportListener : AssetPostprocessor
	{
		static public AssetImportListener self;
		public Action cb;

		public AssetImportListener()
		{
			self = this;
		}

		public void SetCallback(Action callback)
		{
			cb = callback;
		}

		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			bool scriptsChanged = false;
			foreach (string path in importedAssets)
			{
				if (!path.EndsWith(".cs") && !path.EndsWith(".js")) continue;
				scriptsChanged = true;
			}
			foreach (string path in deletedAssets)
			{
				if (!path.EndsWith(".cs") && !path.EndsWith(".js")) continue;
				scriptsChanged = true;
			}
			foreach (string path in movedAssets)
			{
				if (!path.EndsWith(".cs") && !path.EndsWith(".js")) continue;
				scriptsChanged = true;
			}

			if (self.cb != null && scriptsChanged) self.cb();
		}
	}
}
