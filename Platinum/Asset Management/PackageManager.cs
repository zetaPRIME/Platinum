using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Reflection;

using Fluent.IO;
using Path = Fluent.IO.Path;
using Ionic.Zip;
using LitJson;

namespace Platinum
{
	public class PackageManager
	{
		//public static List<string> availablePackages = new List<string>();
		public static Dictionary<string, PackageInfo> availablePackages = new Dictionary<string, PackageInfo>();
		public static Dictionary<string, Package> loadedPackages = new Dictionary<string, Package>();

		public static Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly>();

		public static Package globalPackage
		{
			get { return loadedPackages["Global"]; }
		}

		public static void FindPackages()
		{
			Path path = new Path("Content");

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

		public static void FindPackages(Path basePath)
		{
			if (!basePath.Exists) return;

			string[] matchDef = { "library.json", "entity.json", "scenemode.json", "scene.json" };

			// zips first because reasons
			Path zips = basePath.Files("*.zip", false);
			foreach (Path p in zips)
			{
				string pstr = p.ToString();

				using (ZipFile zf = ZipFile.Read(p.ToString()))
				{
					if (IsPackage(zf))
					{
						GetPackageInfo(zf);//AddPackageEntry(pstr.Substring(0, pstr.Length - 4) + "/");
					}

					foreach (string s in zf.EntryFileNames)
					{
						if (s.IndexOf("/") == -1) continue;
						if (matchDef.Contains(s.Substring(s.LastIndexOf("/") + 1))) GetPackageInfo(zf, s.Substring(0, s.LastIndexOf("/") + 1), pstr.Substring(0, pstr.Length - 4) + "/" + s.Substring(0, s.LastIndexOf("/") + 1));// AddPackageEntry(pstr.Substring(0, pstr.Length - 4) + "/" + s.Substring(0, s.LastIndexOf("/") + 1));
					}
				}
			}

			// recursive subdirectory time!
			Path dirs = basePath.Directories();
			foreach (Path p in dirs)
			{
				if (IsPackage(p)) GetPackageInfo(p);//AddPackageEntry(p.ToString());
				/*else*/ FindPackages(p);
			}
		}

		public static bool IsPackage(Path path)
		{
			string[] match = { "library.json", "entity.json", "scenemode.json", "scene.json" };

			Path jsons = path.Files("*.json", false);

			foreach (Path p in jsons) if (match.Contains(p.FileName)) return true;

			return false;
		}
		public static bool IsPackage(ZipFile zf, string dir = "")
		{
			string[] match = { "library.json", "entity.json", "scenemode.json", "scene.json" };

			foreach (string s in match) if (zf.ContainsEntry(dir + s)) return true;

			return false;
		}
		public static void GetPackageInfo(Path path)
		{
			List<string> match = new List<string>(new string[]{ "library.json", "entity.json", "scenemode.json", "scene.json" });
			PackageType[] types = { PackageType.Library, PackageType.Entity, PackageType.SceneMode, PackageType.Scene };

			Path jsons = path.Files("*.json", false);

			string defName = "";
			PackageType type = PackageType.NotFound;
			foreach (Path p in jsons) if (match.Contains(p.FileName))
			{
				type = types[match.IndexOf(p.FileName)];
				defName = p.FileName;
			}
			if (type == PackageType.NotFound) return; // not package

			PackageInfo info = new PackageInfo();
			info.type = type;
			info.isZip = false;
			info.path = ProcessPackageName(path.ToString());
			info.fPath = path;

			JsonData def = null;
			path.Combine(defName).Open((FileStream fs) =>
			{
				byte[] contents = new byte[fs.Length];
				fs.Read(contents, 0, (int)fs.Length);
				def = JsonMapper.ToObject(Encoding.UTF8.GetString(contents));
			});//, System.IO.FileMode.Open);
			info.def = def;

			availablePackages.Add(info.path, info);
		}
		public static void GetPackageInfo(ZipFile zf, string dir = "", string pname = "")
		{
			List<string> match = new List<string>(new string[] { "library.json", "entity.json", "scenemode.json", "scene.json" });
			PackageType[] types = { PackageType.Library, PackageType.Entity, PackageType.SceneMode, PackageType.Scene };

			string defName = "";
			PackageType type = PackageType.NotFound;
			foreach (string s in match) if (zf.ContainsEntry(dir + s))
				{
					type = types[match.IndexOf(s)];
					defName = dir + s;
				}
			if (type == PackageType.NotFound) return; // not package

			PackageInfo info = new PackageInfo();
			info.type = type;
			info.isZip = true;
			info.path = ProcessPackageName(pname);

			JsonData def = null;
			using (MemoryStream ms = new MemoryStream())
			{
				zf[defName].Extract(ms);

				byte[] contents = ms.ToArray();
				def = JsonMapper.ToObject(Encoding.UTF8.GetString(contents));
			}
			info.def = def;

			availablePackages.Add(info.path, info);
		}
		static string ProcessPackageName(string str)
		{
			str = str.Replace("\\", "/");
			if (str.StartsWith("Content/")) str = str.Substring("Content/".Length);
			if (str.EndsWith("/")) str = str.TrimEnd('/');
			return str;
		}

		public static Package LoadPackage(string path, bool global = false)
		{
			string[] comp = path.Split('/');

			Path bpath = new Path("Content");
			
			for (int i = 0; i < comp.Length; i++)
			{
				Path npath = bpath.Combine(comp[i]);
				if (npath.Exists)
				{
					bpath = npath;
					continue;
				}

				Path zpath = bpath.Combine(comp[i] + ".zip");
				if (!zpath.Exists) throw new Exception("Package not found: " + path);

				string subpath = "";
				for (int j = i; j < comp.Length; j++) subpath = subpath + comp[j] + "/";
				using (ZipFile zf = ZipFile.Read(zpath.ToString()))
				{
					return LoadPackageZip(path, zf, subpath, global);
				}
			}

			Package pkg = new Package();
			pkg.path = path;
			string defName = "";
			if (global)
			{
				pkg.type = PackageType.Global;
				if (bpath.Combine("game.json").Exists) defName = "game.json";
			}
			else if (bpath.Combine("library.json").Exists)
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

			if (defName != "")
			{
				JsonData def = null;
				bpath.Combine(defName).Open((FileStream fs) =>
				{
					byte[] contents = new byte[fs.Length];
					fs.Read(contents, 0, (int)fs.Length);
					def = JsonMapper.ToObject(Encoding.UTF8.GetString(contents));
				});//, System.IO.FileMode.Open);
				pkg.def = def;
			}

			if (availablePackages.ContainsKey(pkg.path)) pkg.info = availablePackages[pkg.path];
			pkg.AddFiles(bpath);
			pkg.ReadDef();
			pkg.MakeAssembly();

			// add to loaded only if successful
			loadedPackages.Add(path, pkg);

			return pkg;
		}

		public static Package LoadPackageZip(string path, ZipFile zf, string subpath, bool global = false)
		{
			Package pkg = new Package();
			pkg.path = path;
			string defName = "";
			if (global)
			{
				pkg.type = PackageType.Global;
				if (zf.EntryFileNames.Contains(subpath + "game.json")) defName = "game.json";
			}
			else if (zf.EntryFileNames.Contains(subpath + "library.json"))
			{
				defName = "library.json";
				pkg.type = PackageType.Library;
			}
			else if (zf.EntryFileNames.Contains(subpath + "entity.json"))
			{
				defName = "entity.json";
				pkg.type = PackageType.Entity;
			}
			else if (zf.EntryFileNames.Contains(subpath + "scenemode.json"))
			{
				defName = "scenemode.json";
				pkg.type = PackageType.SceneMode;
			}
			else if (zf.EntryFileNames.Contains(subpath + "scene.json"))
			{
				defName = "scene.json";
				pkg.type = PackageType.Scene;
			}
			else throw new Exception("No package definition for package " + path);

			if (defName != "")
			{
				JsonData def = null;
				using (MemoryStream ms = new MemoryStream())
				{
					zf[subpath + defName].Extract(ms);
					
					byte[] contents = ms.ToArray();
					def = JsonMapper.ToObject(Encoding.UTF8.GetString(contents));
				}
				pkg.def = def;
			}

			pkg.info = availablePackages[pkg.path];
			pkg.AddFiles(zf, subpath);
			pkg.ReadDef();
			pkg.MakeAssembly();

			// add to loaded only if successful
			loadedPackages.Add(path, pkg);

			return pkg;
		}
	}
}
