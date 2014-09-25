using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using LitJson;

namespace Platinum
{
	public class Scene
	{
		public Package package;
		public SceneService sceneService;
		public SceneMode sceneMode;

		public JsonData def { get { return package.def; } }

		public Dictionary<string, Map> maps = new Dictionary<string, Map>();
		public Map currentMap;
		public string defaultMap;

		public string displayName;
		public string name
		{
			get
			{
				if (package == null) return "_default";
				string n = package.path;
				if (n.StartsWith("Scene/")) n = n.Substring("Scene/".Length);
				return n;
			}
		}


		internal void Load()
		{
			JsonData j = def;
			
			// defaults
			displayName = "Scene";
			defaultMap = "default";

			// and read json attributes
			j.Read("displayName", ref displayName);
			j.Read("defaultMap", ref defaultMap);

			if (j.Has("maps") && j["maps"].IsObject)
			{
				foreach (KeyValuePair<string, JsonData> kvp in j["maps"])
				{
					Map m = new Map();
					m.name = kvp.Key;
					m.def = kvp.Value;
					m.Load();
					maps.Add(kvp.Key, m);
				}
			}

			string smode = "";
			j.Read("sceneMode", ref smode);
			if (smode != "")
			{
				// load scenemode
				string smpath = "SceneMode/" + smode;
				if (PackageManager.availablePackages.ContainsKey(smpath))
				{
					Package pkg = PackageManager.LoadPackage(smpath);

					if (pkg.assembly != null)
					{
						Type[] types = pkg.assembly.GetTypes();

						foreach (Type t in types)
						{
							if (t.IsSubclassOf(typeof(SceneMode)))
							{
								sceneMode = (t.GetConstructor(new Type[0]).Invoke(new object[0])) as SceneMode;
								break;
							}
						}
					}
					if (sceneMode == null) sceneMode = new SceneMode();
					sceneMode.package = pkg;
					// todo: onload
				}
			}
			if (sceneMode == null) sceneMode = new SceneMode(); // blank if failed to load

			// and sceneservice
			if (package.assembly != null)
			{
				Type[] types = package.assembly.GetTypes();

				foreach (Type t in types)
				{
					if (t.IsSubclassOf(typeof(SceneService)))
					{
						sceneService = (t.GetConstructor(new Type[0]).Invoke(new object[0])) as SceneService;
						break;
					}
				}
			}
			if (sceneService == null) sceneService = new SceneService();
			// todo: onload
		}

		internal void Unload()
		{
			if (sceneMode.package != null) sceneMode.package.Unload();
			package.Unload();
		}
	}
}
