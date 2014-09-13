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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
		public List<Package> inheritedBy = new List<Package>();

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

		public void Free(Package from)
		{
			if (inheritedBy.Contains(from)) inheritedBy.Remove(from);
			if (type == PackageType.Library && inheritedBy.Count == 0) Unload();
		}

		public void Unload()
		{
			foreach (Package pkg in inherit) pkg.Free(this);
			PackageManager.loadedPackages.Remove(path);
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
			if (PackageManager.loadedPackages.ContainsKey(ipath))
			{
				Package pkg = PackageManager.loadedPackages[ipath];
				inherit.Add(pkg);
				pkg.inheritedBy.Add(this);
			}

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
					p.Open((FileStream fs) =>
					{
						LoadTexture(subName, fs);
					});
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
					using (MemoryStream ms = new MemoryStream())
					{
						zf[s].Extract(ms);
						LoadTexture(subName, ms);
					}
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

		void LoadTexture(string fileName, Stream stream)
		{
			ExtTexture ext = new ExtTexture();
			ext.texture = Texture2D.FromStream(Core.instance.GraphicsDevice, stream);
			ext.texture = Utils.ConvertToPreMultipliedAlpha(ext.texture);

			if (fileName.EndsWith(".png")) fileName = fileName.Substring(0, fileName.Length - ".png".Length);

			// defaults
			ext.pixelScale = 1;
			ext.baseScale = 1;

			if (fileName.EndsWith("]") && fileName.IndexOf('[') > -1)
			{
				string args = fileName.Substring(fileName.LastIndexOf('[') + 1);
				args = args.Substring(0, args.Length - 1);

				// trim
				fileName = fileName.Substring(0, fileName.LastIndexOf('['));

				string[] arg = args.Split(',');

				foreach (string p in arg)
				{
					if (p.StartsWith("px"))
					{
						int px = int.Parse(p.Substring(2));
						if (px < 1) px = 1;
						ext.pixelScale = px;
					}
					else if (p.StartsWith("x"))
					{
						string np = p;
						if (np.StartsWith(".")) np = "0" + np;
						float x = float.Parse(np.Substring(2));
						ext.baseScale = x;
					}
				}
			}

			ext.texture = Utils.PixelUpscale(ext.texture, ext.pixelScale);

			fileName = fileName.TrimEnd(' ');

			textures.Add(fileName, ext);
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
