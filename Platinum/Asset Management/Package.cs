using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Reflection;

using System.CodeDom.Compiler;
using Microsoft.CSharp;

using FluentPath;
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
			return null;
		}

		public byte[] GetFile(string name)
		{
			if (files.ContainsKey(name)) return files[name];
			foreach (Package pkg in inherit) if (pkg.files.ContainsKey(name)) return pkg.files[name];
			return null;
		}

		public void AddFiles(FPath path)
		{
			FPathCollection pfiles = path.GetFiles();

			foreach (FPath p in pfiles)
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

			FPathCollection pdirs = path.GetDirectories();

			foreach (FPath p in pdirs)
			{
				if (PackageManager.IsPackage(p)) continue; // don't read into subpackages
				AddFiles(p);
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
			parameters.OutputAssembly = "tmp\\assembly\\" + StripPath();

			// set up compiler parameters
			parameters.GenerateInMemory = false;
			parameters.GenerateExecutable = false;
			
			// reference relevant libraries
			parameters.ReferencedAssemblies.Add("Ionic.Zip.Reduced.dll");
			parameters.ReferencedAssemblies.Add("Microsoft.Xna.Framework.dll");
			parameters.ReferencedAssemblies.Add("Microsoft.Xna.Framework.Game.dll");
			parameters.ReferencedAssemblies.Add("Microsoft.Xna.Framework.Graphics.dll");
			parameters.ReferencedAssemblies.Add("Microsoft.Xna.Framework.Xact.dll");

			// add Platinum itself
			parameters.ReferencedAssemblies.Add(System.Reflection.Assembly.GetExecutingAssembly().Location);

			// todo: reference global assemblies

			// reference inherited
			foreach (Package pkg in inherit) parameters.ReferencedAssemblies.Add(pkg.assembly.Location);

			// assemble code to compile
			List<string> codeFiles = new List<string>();
			foreach (KeyValuePair<string, byte[]> kvp in files)
			{
				if (kvp.Key.EndsWith(".cs")) codeFiles.Add(System.Text.Encoding.UTF8.GetString(kvp.Value));
			}

			// compile!
			CompilerResults results = provider.CompileAssemblyFromFile(parameters, codeFiles.ToArray());

			// finally, load in and apply
			assembly = results.CompiledAssembly;

			// and store into the thing
			PackageManager.loadedAssemblies.Add(path, assembly);
		}
	}
}
