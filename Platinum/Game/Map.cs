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
		public Vector4 backColorVec;
		public Color backColor { get { return new Color(backColorVec); } set { backColorVec = value.ToVector4(); } }
		public List<Entity> entities;
		public List<EntityPlacement> placements;

		public JsonData def;

		internal void Load()
		{
			// defaults
			size = GameDef.screenSize;
			backColorVec = GameDef.defaultBackColorVec;

			// read in
			def.Read("size", ref size);
			def.Read("backColor", ref backColorVec);

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

		internal void Save()
		{
			// attributes
			def.Write("size", size);
			def.Write("backColor", backColorVec, true);

			// don't need includes, if they're there they're there

			def.Remove("entities"); // going to remake
			JsonData e = new JsonData();
			e.SetJsonType(JsonType.Array);

			List<EntityPlacement> saveEntities = placements.Where((p) => !p.fromInclude && p.parent == null).ToList();
			foreach (EntityPlacement ep in saveEntities)
			{
				ep.Save();
				e.Add(ep.def);
			}
			if (e.Count > 0) def["entities"] = e;
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
				p.Parent = parent;
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

			CollisionManager.Reset();

			if (Core.mode == EngineMode.Editor)
			{
				GameWindow window = Editor.EditorCore.Window;

				window.Title = GameDef.name + " - Editor - " + GameState.scene.displayName + " (" + GameState.scene.name + ") - Map: " + name;
			}
		}
	}
}
