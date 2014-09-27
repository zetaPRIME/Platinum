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
				if (parent != null) pmt = parent.Transform;
				return Vector2.Transform(velocity, pmt);
			}
			set
			{
				Matrix pmt = Matrix.Identity;
				if (parent != null) pmt = parent.Transform;
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
				return pmt * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(position, 0));
			}
		}
		#endregion

		#region loading, creating entity
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

		}

		public Entity MakeEntity(Map map, Entity parent = null)
		{
			Entity e = EntityDef.NewEntity(typeName, true);
			if (e == null) return null;

			e.parent = parent;
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

		public Vector2 oldPosition;
		#endregion
	}
}
