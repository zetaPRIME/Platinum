using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Reflection;

using FluentPath;
using Ionic.Zip;
using LitJson;

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
						if (matchDef.Contains(s.Substring(s.LastIndexOf("/") + 1))) AddPackageEntry(pstr.Substring(0, pstr.Length - 4) + "/" + s.Substring(0, s.LastIndexOf("/") + 1));
					}
				}
			}

			// recursive subdirectory time!
			FPathCollection dirs = basePath.GetDirectories();
			foreach (FPath p in dirs)
			{
				if (IsPackage(p)) AddPackageEntry(p.ToString());
				else FindPackages(p);
			}
		}

		public static bool IsPackage(FPath path)
		{
			string[] match = { "library.json", "entity.json", "scenemode.json", "scene.json" };

			FPathCollection jsons = path.GetFiles("*.json");

			foreach (FPath p in jsons) if (match.Contains(p.FileName)) return true;

			return false;
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
			if (str.EndsWith("/")) str = str.TrimEnd('/');
			availablePackages.Add(str);
		}

		public static void LoadPackage(string path)
		{
			string[] comp = path.Split('/');

			FPath bpath = new FPath("Content");
			
			for (int i = 0; i < comp.Length; i++)
			{
				FPath npath = bpath.Combine(comp[i]);
				if (npath.Exists)
				{
					bpath = npath;
					continue;
				}

				FPath zpath = bpath.Combine(comp[i] + ".zip");
				if (!zpath.Exists) throw new Exception("Package not found: " + path);

				string subpath = "";
				for (int j = i; j < comp.Length; j++) subpath = subpath + comp[j] + "/";
				using (ZipFile zf = ZipFile.Read(zpath.ToString()))
				{
					LoadPackageZip(path, zf, subpath);
					return;
				}
			}

			Package pkg = new Package();
			pkg.path = path;
			string defName = "";
			if (bpath.Combine("library.json").Exists)
			{
				defName = "library.json";
				pkg.type = PackageType.Library;
			}
			else if (bpath.Combine("entity.json").Exists)
			{
				defName = "entity.json";
				pkg.type = PackageType.Entity;
			}
			else if (bpath.Combine("scenemode.json").Exists)
			{
				defName = "scenemode.json";
				pkg.type = PackageType.SceneMode;
			}
			else if (bpath.Combine("scene.json").Exists)
			{
				defName = "scene.json";
				pkg.type = PackageType.Scene;
			}
			else throw new Exception("No package definition for package " + path);

			JsonData def = null;
			bpath.Combine(defName).Open((FileStream fs) =>
			{
				byte[] contents = new byte[fs.Length];
				fs.Read(contents, 0, (int)fs.Length);
				def = JsonMapper.ToObject(Encoding.UTF8.GetString(contents));
			}, FileMode.Open);
			pkg.def = def;

			pkg.AddFiles(bpath);

			// add to loaded only if successful
			loadedPackages.Add(path, pkg);
		}

		public static void LoadPackageZip(string path, ZipFile zf, string subpath)
		{
			//
		}
	}
}
