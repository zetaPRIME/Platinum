using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using LitJson;

namespace Platinum
{
	public class Map
	{
		public string name;
		public Vector2 size;
		public Color backColor;
		public List<Entity> entities;
		public List<EntityPlacement> placements;

		public JsonData def;

		internal void Load()
		{
			// defaults
			size = GameDef.screenSize;
			backColor = GameDef.defaultBackColor;

			// read in
			def.Read("size", ref size);
			def.Read("backColor", ref backColor);

			// set up some stuff
			placements = new List<EntityPlacement>();

			// entity set includes ...
			if (def.Has("include") && def["include"].IsArray)
			{
				foreach (JsonData j in def["include"])
				{
					if (!j.IsString) continue;
					byte[] file = GameState.scene.package.GetFile((string)j + ".json");
					if (file == null) continue;
					LoadEntitySet(JsonMapper.ToObject(Encoding.UTF8.GetString(file)), true);
				}
			}

			// and the actual entity set
			if (def.Has("entities") && def["entities"].IsArray) LoadEntitySet(def["entities"]);
		}

		internal void LoadEntitySet(JsonData j, bool fromInclude = false, EntityPlacement parent = null)
		{
			if (j == null || !j.IsArray) return; // noap

			foreach (JsonData e in j)
			{
				if (!e.IsObject) continue;
				EntityPlacement p = new EntityPlacement();
				p.def = e;
				p.fromInclude = fromInclude;
				p.parent = parent;
				p.Load();
				placements.Add(p);
				if (e.Has("children")) LoadEntitySet(e["children"], fromInclude, p);
			}
		}

		internal void Apply(bool reset = false)
		{
			GameState.worldSize = size;
			GameState.backColor = backColor;
			if (Core.mode != EngineMode.Editor)
			{
				if (reset || entities == null)
				{
					entities = new List<Entity>();
					// apply entity set
					foreach (EntityPlacement p in placements)
					{
						if (p.parent != null) continue;
						p.MakeEntity(this);
					}
				}
				// todo: do the lift-and-set
				GameState.entities = entities;
			}

			GameState.scene.currentMap = this;
		}
	}
}
