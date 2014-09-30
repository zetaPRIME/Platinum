using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using LitJson;

using Platinum.Editor;

namespace Platinum
{
	public class EntityDef
	{
		#region Statics
		public static Dictionary<string, EntityDef> defs = new Dictionary<string, EntityDef>();

		/*public static EntityDef this[string name]
		{
			get
			{
				if (!defs.ContainsKey(name)) return null;
				return defs[name];
			}

			set
			{
				if (value == null && defs.ContainsKey(name)) defs.Remove(name);
			}
		}*/

		public static EntityDef GetDef(string name)
		{
			LoadEntity(name);
			if (!defs.ContainsKey(name)) return null;
			return defs[name];
		}

		public static void LoadEntity(string name)
		{
			if (defs.ContainsKey(name)) return; // don't try double-loading!

			string path = name;

			if (false) { } // TODO: scene subpackage
			if (PackageManager.availablePackages.ContainsKey("Entity/" + name)) path = "Entity/" + name;
			else return; // none found

			Package pkg;
			if (PackageManager.loadedPackages.ContainsKey(path)) pkg = PackageManager.loadedPackages[path];
			else pkg = PackageManager.LoadPackage(path);

			EntityDef def = new EntityDef();
			def.source = pkg;

			if (pkg.assembly == null) return; // why would you even

			def.LoadJson();

			// codetype
			Type[] types = pkg.assembly.GetTypes();

			foreach (Type t in types)
			{
				if (t.IsSubclassOf(typeof(Entity)))
				{
					//GameDef.gameService = (t.GetConstructor(new Type[0]).Invoke(new object[0])) as GameService;
					def.codeType = t;
					break;
				}
			}

			if (Core.mode == EngineMode.Editor)
			{
				foreach (Type t in types)
				{
					if (t.IsSubclassOf(typeof(EditorEntity)))
					{
						def.editorEntity = (t.GetConstructor(new Type[0]).Invoke(new object[0])) as EditorEntity;
						break;
					}
				}
				if (def.editorEntity == null) def.editorEntity = new EditorEntity();
				def.editorEntity.def = def;
				def.editorEntity.LoadJson();
			}

			defs.Add(name, def);
		}

		public static Entity NewEntity(string name, bool placed = false)
		{
			LoadEntity(name);
			if (!defs.ContainsKey(name)) return null;
			EntityDef def = EntityDef.defs[name];

			Entity e = (def.codeType.GetConstructor(new Type[0]).Invoke(new object[0])) as Entity;
			e.def = def;

			e.mainTexture = def.mainTexture;

			e.bounds = def.bounds;

			GameState.entities.Add(e);
			if (!placed) e.Init();
			return e;
		}
		#endregion

		public bool BuiltIn { get; internal set; }

		public string displayName;

		public Type codeType;
		public Package source;

		public EditorEntity editorEntity;

		public ExtTexture mainTexture;
		public VecRect bounds;

		public void LoadJson()
		{
			// note: can't load in editorentity things here, called before it's made!

			JsonData j = source.def;

			string txname = "";
			j.Read("mainTexture", ref txname);
			if (txname != "") mainTexture = source.GetTexture(txname);

			// defaults
			displayName = source.path.Substring(source.path.LastIndexOf('/') + 1);
			bounds = VecRect.Radius * 16f;

			// and read in
			j.Read("displayName", ref displayName);
			j.Read("bounds", ref bounds);
		}
	}
}
