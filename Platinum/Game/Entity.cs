using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum
{
	public class Entity : MessagePassable
	{
		public string Name { get; protected set; }
		readonly List<string> tags = new List<string>();
		public bool HasTag(string tag) { return tags.Contains(tag); }

		public Vector2 position = Vector2.Zero;
		public Vector2 velocity = Vector2.Zero;
		public float rotation = 0f;
		public int direction = 1;

		public VecRect bounds = VecRect.Zero;

		public List<Collider> colliders = new List<Collider>();
		public bool collisionPassive = false;

		public int drawLayer = 0;
		public virtual bool DrawOffScreen { get { return false; } }

		public virtual bool Asleep { get { return Parent != null && Parent.Asleep; } set { /* just here so it can be overridden */ } }
		public virtual bool Disabled { get { return Parent != null && Parent.Disabled; } set { /* just here so it can be overridden */ } }

		internal Entity riding = null;
		public Entity Riding
		{
			get { return riding; }
			set
			{
				Vector2 pos = Position;
				float rot = Rotation;
				Vector2 vel = Velocity;
				if (riding != null) riding.riddenBy.Remove(this);
				riding = value;
				if (riding != null) riding.riddenBy.Add(this);
				Position = pos;
				Rotation = rot;
				Velocity = vel;
			}
		}
		internal Entity parent = null;
		public Entity Parent
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

		internal protected List<Entity> children = new List<Entity>();
		public List<Entity> Children
		{
			get { return new List<Entity>(children); } // new copy
		}
		internal protected List<Entity> riddenBy = new List<Entity>();
		public List<Entity> RiddenBy
		{
			get { return new List<Entity>(riddenBy); } // new copy
		}

		public EntityDef def;

		public Vector2 Position
		{
			get
			{
				Matrix pmt = Matrix.Identity;
				if (riding != null) pmt = riding.Transform;
				else if (parent != null) pmt = parent.Transform;
				return Vector2.Transform(position, pmt);
			}
			set
			{
				Matrix pmt = Matrix.Identity;
				if (riding != null) pmt = riding.Transform;
				else if (parent != null) pmt = parent.Transform;
				position = Vector2.Transform(value, Matrix.Invert(pmt));
			}
		}
		public Vector2 Velocity
		{
			get
			{
				Matrix pmt = Matrix.Identity;
				if (riding != null) pmt = riding.Transform;
				else if (parent != null) pmt = parent.Transform;
				return Vector2.Transform(velocity, pmt);
			}
			set
			{
				Matrix pmt = Matrix.Identity;
				if (riding != null) pmt = riding.Transform;
				else if (parent != null) pmt = parent.Transform;
				velocity = Vector2.Transform(value, Matrix.Invert(pmt));
			}
		}
		public float Rotation
		{
			get
			{
				float par = 0f;
				if (riding != null) par = riding.Rotation;
				else if (parent != null) par = parent.Rotation;
				return (par + rotation).WrapRot();
			}
			set
			{
				float par = 0f;
				if (riding != null) par = riding.Rotation;
				else if (parent != null) par = parent.Rotation;
				rotation = (value - par).WrapRot();
			}
		}

		public Matrix Transform
		{
			get
			{
				Matrix pmt = Matrix.Identity;
				if (riding != null) pmt = riding.Transform;
				else if (parent != null) pmt = parent.Transform;
				return pmt * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(position, 0));
			}
		}
		Matrix transformCache;
		public bool hasMoved { get { return transformCache != Transform; } }

		public VecRect WorldBounds
		{
			get { return bounds + Position; }
		}

		public Vector2 ScreenPosition { get { return (Position - GameState.cameraPos).Pixelize(); } }
		public bool OnScreen { get { return WorldBounds.Intersects(GameState.cameraBox); } }

		public virtual void Update() { }
		public virtual void Draw(SpriteBatch sb) { }

		public virtual void OnKill() { }
		public virtual void OnKillAny() { }
		public void Kill(bool silent = false)
		{
			if (!silent) OnKill();
			OnKillAny();

			// remove colliders from quadtree
			foreach (Collider col in colliders) col.KillFromTree();

			// deparent and free children
			Parent = null;
			Riding = null;
			foreach (Entity e in children) e.Parent = null; // todo: conditions where every child should also be deleted (though meanwhile OnKill can manually do that)
			foreach (Entity e in riddenBy) e.Riding = null;

			// remove from play
			if (GameState.entities.Contains(this)) GameState.entities.Remove(this);
		}

		public void UpdatePhysics()
		{
			if (!MoveUpdate())
			{
				position += velocity;

				if (hasMoved) UpdateColliders();
			}

			transformCache = Transform;
		}
		public virtual bool MoveUpdate() { return false; } // return true to override
		public virtual void UpdateColliders()
		{
			foreach (Collider col in colliders) col.Update();
		}
	}
}
