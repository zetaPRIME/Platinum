using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using LitJson;

namespace Platinum
{
	public class EntityPlacement
	{
		public EntityPlacement() { }
		public EntityPlacement(string type, Vector2 position)
		{
			typeName = type;
			this.type = EntityDef.GetDef(typeName);
			this.position = position;
			def = new JsonData();
			def.SetJsonType(JsonType.Object);

			if (this.type.editorEntity != null) this.type.editorEntity.BuildDefaultJson(def);
		}

		public string typeName;
		public EntityDef type;

		public string name = "";

		public Vector2 position = Vector2.Zero;
		public Vector2 velocity = Vector2.Zero;
		public float rotation = 0f;
		public int direction = 1;

		public int drawLayer = 0;

		public VecRect bounds = VecRect.Zero;

		//
		public JsonData def;
		public bool fromInclude = false;

		#region parenting etc.
		internal EntityPlacement parent = null;
		public EntityPlacement Parent
		{
			get { return parent; }
			set
			{
				Vector2 pos = Position;
				float rot = Rotation;
				Vector2 vel = Velocity;
				if (parent != null) parent.children.Remove(this);
				parent = value;
				if (parent != null) parent.children.Add(this);
				Position = pos;
				Rotation = rot;
				Velocity = vel;
			}
		}

		internal protected List<EntityPlacement> children = new List<EntityPlacement>();
		public List<EntityPlacement> Children
		{
			get { return new List<EntityPlacement>(children); } // new copy
		}

		public Vector2 Position
		{
			get
			{
				Matrix pmt = Matrix.Identity;
				if (parent != null) pmt = parent.Transform;
				return Vector2.Transform(position, pmt);
			}
			set
			{
				Matrix pmt = Matrix.Identity;
				if (parent != null) pmt = parent.Transform;
				position = Vector2.Transform(value, Matrix.Invert(pmt));
			}
		}
		public Vector2 Velocity
		{
			get
			{
				Matrix pmt = Matrix.Identity;
				if (parent != null) pmt = Matrix.CreateRotationZ(parent.Rotation);//parent.Transform;
				return Vector2.Transform(velocity, pmt);
			}
			set
			{
				Matrix pmt = Matrix.Identity;
				if (parent != null) pmt = Matrix.CreateRotationZ(parent.Rotation); //parent.Transform;
				velocity = Vector2.Transform(value, Matrix.Invert(pmt));
			}
		}
		public float Rotation
		{
			get
			{
				float par = 0f;
				if (parent != null) par = parent.Rotation;
				return (par + rotation).WrapRot();
			}
			set
			{
				float par = 0f;
				if (parent != null) par = parent.Rotation;
				rotation = (value - par).WrapRot();
			}
		}

		public Matrix Transform
		{
			get
			{
				Matrix pmt = Matrix.Identity;
				if (parent != null) pmt = parent.Transform;
				return Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(position, 0)) * pmt;
			}
		}
		#endregion

		#region loading, saving, creating entity
		public void Load()
		{
			// most important thing first!
			typeName = "";
			def.Read("type", ref typeName);
			type = EntityDef.GetDef(typeName);

			def.Read("name", ref name);
			def.Read("position", ref position);
			def.Read("velocity", ref velocity);
			def.Read("rotation", ref rotation);
			def.Read("direction", ref direction);

			def.Read("drawLayer", ref drawLayer);

			if (type.editorEntity != null) type.editorEntity.BuildDefaultJson(def);
		}

		public void Save()
		{
			def.Write("type", typeName);

			def.Write("name", name);
			def.Write("position", position);
			def.Write("velocity", velocity);
			def.Write("rotation", rotation);
			def.Write("direction", direction);

			def.Write("drawLayer", drawLayer);

			def.Remove("children"); // going to remake it
			JsonData ch = new JsonData();
			ch.SetJsonType(JsonType.Array);
			foreach (EntityPlacement ep in children)
			{
				ep.Save();
				ch.Add(ep.def);
			}
			if (ch.Count > 0) def["children"] = ch;
		}

		public Entity MakeEntity(Map map, Entity parent = null)
		{
			Entity e = EntityDef.NewEntity(typeName, true);
			if (e == null) return null;

			e.Parent = parent;
			e.position = position;
			e.velocity = velocity;
			e.rotation = rotation;
			e.direction = direction;

			e.drawLayer = drawLayer;

			e.LoadJson(def);
			e.Init();

			map.entities.Add(e);
			foreach (EntityPlacement c in children)
			{
				c.MakeEntity(map, e);
			}

			return e;
		}
		#endregion

		#region editor things
		public VecRect DrawBounds { get { return type.editorEntity.DrawBounds + Position; } }
		public VecRect SelectBounds { get { return type.editorEntity.SelectBounds + Position; } }

		public EntityPlacement Copy(Vector2 position)
		{
			EntityPlacement cp = new EntityPlacement();

			Save();
			cp.def = JsonMapper.ToObject(JsonMapper.ToJson(def));
			cp.LoadAndCreateChildren();

			cp.Position = position;

			return cp;
		}

		public void LoadAndCreateChildren()
		{
			Load();

			if (!def.Has("children") || !def["children"].IsArray) return;
			JsonData j = def["children"];
			foreach (JsonData c in j)
			{
				EntityPlacement e = new EntityPlacement();
				e.Parent = this;
				e.def = c;
				e.LoadAndCreateChildren();
			}
		}

		public void AddToMap(Map map)
		{
			if (!map.placements.Contains(this)) map.placements.Add(this);
			foreach (EntityPlacement e in children)
			{
				e.AddToMap(map);
			}
		}

		public void Snap(float gridSize)
		{
			Vector2 snap = type.editorEntity.SnapOffset;
			if (parent != null) snap -= parent.type.editorEntity.SnapOffset;

			position += Vector2.One * gridSize / 2;
			position += Vector2.One * gridSize * 1024;
			position += snap;
			position = new Vector2((int)(position.X / gridSize) * gridSize, (int)(position.Y / gridSize) * gridSize);
			position -= snap;
			position -= Vector2.One * gridSize * 1024;
		}

		public void Kill(Map map)
		{
			foreach (EntityPlacement e in Children) e.Parent = null;
			Parent = null;
			map.placements.Remove(this);
		}

		public Vector2 oldPosition;

		public void DrawParentLine(SpriteBatch sb, Color color, float lineWidth)
		{
			if (lineWidth < 1f) lineWidth = 1f;
			if (parent == null) return;
			sb.DrawLine(new LineSegment(Position, parent.Position), color, lineWidth);
			parent.DrawParentLine(sb, color.MultiplyBy(0.9f), lineWidth * 0.95f);
		}
		public void DrawChildLines(SpriteBatch sb, Color color, float lineWidth)
		{
			if (lineWidth < 1f) lineWidth = 1f;
			foreach (EntityPlacement c in children)
			{
				sb.DrawLine(new LineSegment(Position, c.Position), color, lineWidth);
				c.DrawChildLines(sb, color.MultiplyBy(0.9f), lineWidth * 0.95f);
			}
		}
		#endregion
	}
}
