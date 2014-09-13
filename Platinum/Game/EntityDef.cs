using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platinum
{
	public class EntityDef
	{
		#region Statics
		public static Dictionary<string, EntityDef> defs = new Dictionary<string, EntityDef>();

		public static void LoadEntity(string name)
		{
			if (defs.ContainsKey(name)) return; // don't try double-loading!

			string path = name;

			if (false) { } // TODO: scene subpackage
			if (PackageManager.availablePackages.Contains("Entity/" + name)) path = "Entity/" + name;
			else return; // none found

			Package pkg;
			if (PackageManager.loadedPackages.ContainsKey(path)) pkg = PackageManager.loadedPackages[path];
			else pkg = PackageManager.LoadPackage(path);

			EntityDef def = new EntityDef();
			def.source = pkg;

			if (pkg.assembly == null) return; // why would you even

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

			defs.Add(name, def);
		}

		public static Entity NewEntity(string name)
		{
			LoadEntity(name);
			if (!defs.ContainsKey(name)) return null;
			EntityDef def = EntityDef.defs[name];

			Entity e = (def.codeType.GetConstructor(new Type[0]).Invoke(new object[0])) as Entity;
			e.def = def;

			GameState.entities.Add(e);
			return e;
		}
		#endregion

		public Type codeType;
		public Package source;
	}
}
