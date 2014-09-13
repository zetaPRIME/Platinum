using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Reflection;

using System.CodeDom.Compiler;
using Microsoft.CSharp;

using Fluent.IO;
using Path = Fluent.IO.Path;
using Ionic.Zip;
using LitJson;

namespace Platinum
{
	public enum PackageType
	{
		Global,
		Library,
		SceneMode,
		Scene,
		Entity
	}

	public class Package
	{
		public PackageType type = PackageType.Library;

		public string path;

		public JsonData def;

		public Dictionary<string, ExtTexture> textures = new Dictionary<string, ExtTexture>();
		public Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

		public Assembly assembly;

		public List<Package> inherit = new List<Package>();

		public string StripPath()
		{
			string p = path;
			p = p.Replace("/", "_");
			p = p.Replace("\\", "_");
			return p;
		}

		public ExtTexture GetTexture(string name)
		{
			if (textures.ContainsKey(name)) return textures[name];
			foreach (Package pkg in inherit) if (pkg.textures.ContainsKey(name)) return pkg.textures[name];
			if (type != PackageType.Global && PackageManager.globalPackage.textures.ContainsKey(name)) return PackageManager.globalPackage.textures[name];
			return null;
		}

		public byte[] GetFile(string name)
		{
			if (files.ContainsKey(name)) return files[name];
			foreach (Package pkg in inherit) if (pkg.files.ContainsKey(name)) return pkg.files[name];
			if (type != PackageType.Global && PackageManager.globalPackage.files.ContainsKey(name)) return PackageManager.globalPackage.files[name];
			return null;
		}

		public void ReadDef()
		{
			if (def == null) return; // no definition supplied

			// inheritance
			if (def.Has("inherit") && def["inherit"].IsArray)
			{
				foreach (JsonData j in def["inherit"])
				{
					if (!j.IsString) continue;
					Inherit((string)j);
				}
			}
		}
		void Inherit(string name)
		{
			string ipath = "";
			// literal
			if (name.StartsWith("/")) ipath = name.Substring(1);
			// subpackage
			else if (PackageManager.availablePackages.Contains(path + "/" + name)) ipath = path + "/" + name;
			// library
			else if (PackageManager.availablePackages.Contains("Library/" + name)) ipath = "Library/" + name;

			if (ipath == "") return;
			if (!PackageManager.loadedPackages.ContainsKey(ipath)) PackageManager.LoadPackage(ipath);
			if (PackageManager.loadedPackages.ContainsKey(ipath)) inherit.Add(PackageManager.loadedPackages[ipath]);

		}

		public void AddFiles(Path path)
		{
			Path pfiles = path.Files();

			foreach (Path p in pfiles)
			{
				string subName = p.ToString().Replace("\\", "/");
				if (subName.StartsWith("Content/")) subName = subName.Substring("Content/".Length);
				string qpath = this.path + "/";
				if (subName.StartsWith(qpath)) subName = subName.Substring(qpath.Length);

				if (p.Extension == ".zip") continue; // zips are assumed to be subpackages
				if (p.Extension == ".png")
				{
					// todo: load in textures!
				}
				else
				{
					byte[] file = null;
					p.Open((FileStream fs) =>
					{
						file = new byte[fs.Length];
						fs.Read(file, 0, (int)fs.Length);
					});
					files.Add(subName, file);
				}
			}

			Path pdirs = path.Directories();

			foreach (Path p in pdirs)
			{
				if (PackageManager.IsPackage(p)) continue; // don't read into subpackages
				AddFiles(p);
			}
		}
		public void AddFiles(ZipFile zf, string subpath)
		{
			foreach (string s in zf.EntryFileNames)
			{
				if (!s.StartsWith(subpath)) continue;
				string dir = s.Substring(0, s.LastIndexOf('/'));
				if (dir != subpath && PackageManager.IsPackage(zf, dir)) continue; // don't read into subpackages

				string subName = s.Substring(subpath.Length);

				if (subName.EndsWith(".zip")) throw new Exception("Subpackage zip " + subName + " found in zipped package " + path + "; nested zipped packages are not supported."); // YOU'RE DOIN' IT WRONG
				if (subName.EndsWith(".png"))
				{
					// todo: load in textures!
				}
				else
				{
					using (MemoryStream ms = new MemoryStream())
					{
						zf[s].Extract(ms);
						files.Add(subName, ms.ToArray());
					}
				}
			}
		}

		public void MakeAssembly()
		{
			if (PackageManager.loadedAssemblies.ContainsKey(path))
			{
				assembly = PackageManager.loadedAssemblies[path];
				return;
			}

			CSharpCodeProvider provider = new CSharpCodeProvider();
			CompilerParameters parameters = new CompilerParameters();

			// set assembly path
			parameters.OutputAssembly = "tmp\\assembly\\" + StripPath() + ".dll";

			// set up compiler parameters
			parameters.GenerateInMemory = false;
			parameters.GenerateExecutable = false;
			
			// reference relevant libraries
			parameters.ReferencedAssemblies.AddRange(new string[]{
				"mscorlib.dll",
				"System.dll",
				"System.Core.dll",
				"System.Drawing.dll",
				"System.Windows.Forms.dll",
				"System.Numerics.dll",
				"System.Xml.dll",
				"Ionic.Zip.Reduced.dll",
				"Microsoft.Xna.Framework.dll",
				"Microsoft.Xna.Framework.Xact.dll",
				"Microsoft.Xna.Framework.Game.dll",
				"Microsoft.Xna.Framework.Graphics.dll"
			});

			// add Platinum itself
			parameters.ReferencedAssemblies.Add(System.Reflection.Assembly.GetExecutingAssembly().Location);

			// reference global assembly
			if (type != PackageType.Global && PackageManager.globalPackage.assembly != null) parameters.ReferencedAssemblies.Add(PackageManager.globalPackage.assembly.Location);

			// reference inherited
			foreach (Package pkg in inherit) if (pkg.assembly != null) parameters.ReferencedAssemblies.Add(pkg.assembly.Location);

			// assemble code to compile
			List<string> codeFiles = new List<string>();
			foreach (KeyValuePair<string, byte[]> kvp in files)
			{
				if (kvp.Key.EndsWith(".cs")) codeFiles.Add(System.Text.Encoding.UTF8.GetString(kvp.Value));
			}

			// abort if no code to compile
			if (codeFiles.Count == 0) return;

			// compile!
			CompilerResults results = provider.CompileAssemblyFromSource(parameters, codeFiles.ToArray());

			// finally, load in and apply
			assembly = results.CompiledAssembly;

			// and store into the thing
			PackageManager.loadedAssemblies.Add(path, assembly);
		}
	}
}
