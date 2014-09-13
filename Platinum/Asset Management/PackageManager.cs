using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using FluentPath;
using Ionic.Zip;

namespace Platinum
{
	public class PackageManager
	{
		public static RootPackage rootPackage;

		public static List<String> availablePackages = new List<string>();
		public static Dictionary<string, Package> loadedPackages = new Dictionary<string, Package>();

		public static Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly>();

		public static void FindPackages()
		{
			FPath path = new FPath("Content");

			// libraries
			FindPackages(path.Combine("Library"));
			
			// entities
			FindPackages(path.Combine("Entity"));

			// scenemodes
			FindPackages(path.Combine("SceneMode"));

			// scenes
			FindPackages(path.Combine("Scene"));

			//
		}

		public static void FindPackages(FPath basePath)
		{
			if (!basePath.Exists) return;

			string[] matchDef = { "library.json", "entity.json", "scenemode.json", "scene.json" };

			// zips first because reasons
			FPathCollection zips = basePath.GetFiles("*.zip");
			foreach (FPath p in zips)
			{
				string pstr = p.ToString();

				using (ZipFile zf = ZipFile.Read(p.ToString()))
				{
					if (IsPackage(zf))
					{
						AddPackageEntry(pstr.Substring(0, pstr.Length - 4) + "/");
					}

					foreach (string s in zf.EntryFileNames)
					{
						if (s.IndexOf("/") == -1) continue;
						string test = s.Substring(0, s.LastIndexOf("/"));
						test = test;
						if (matchDef.Contains(s.Substring(s.LastIndexOf("/") + 1))) AddPackageEntry(pstr.Substring(0, pstr.Length - 4) + "/" + s.Substring(0, s.LastIndexOf("/") + 1));
					}
				}
			}
		}

		public static bool IsPackage(ZipFile zf, string dir = "")
		{
			string[] match = { "library.json", "entity.json", "scenemode.json", "scene.json" };

			foreach (string s in match) if (zf.ContainsEntry(dir + s)) return true;

			return false;
		}

		static void AddPackageEntry(string str)
		{
			str = str.Replace("\\", "/");
			if (str.StartsWith("Content/")) str = str.Substring("Content/".Length);
			availablePackages.Add(str);
		}
	}
}
