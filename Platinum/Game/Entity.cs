﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum
{
	public class Entity : EventPassable
	{
		public Vector2 position = Vector2.Zero;
		public Vector2 velocity = Vector2.Zero;
		public float rotation = 0f;
		public int direction = 1;

		public VecRect bounds = VecRect.Zero;

		//public Body physBody = null;
		public List<Collider> colliders = new List<Collider>();
		public bool collisionPassive = false;

		internal Entity parent = null;
		public Entity Parent
		{
			get { return parent; }
			set
			{
				Vector2 pos = Position;
				float rot = Rotation;
				if (parent != null) parent.children.Remove(this);
				parent = value;
				if (parent != null) parent.children.Add(this);
				Position = pos;
				Rotation = rot;
			}
		}

		internal protected List<Entity> children = new List<Entity>();
		public List<Entity> Children
		{
			get { return new List<Entity>(children); } // new copy
		}

		public EntityDef def;

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

		public float Rotation
		{
			get
			{
				float par = 0f;
				if (parent != null) par = parent.Rotation;
				return par + rotation;
			}
			set
			{
				float par = 0f;
				if (parent != null) par = parent.Rotation;
				rotation = value - par;
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

		public VecRect WorldBounds
		{
			get { return bounds + Position; }
		}

		public Vector2 ScreenPosition { get { return (Position - GameState.cameraPos).Pixelize(); } }
		public bool OnScreen { get { return WorldBounds.Intersects(GameState.cameraBox); } }

		public virtual void Update() { }
		public virtual void Draw(SpriteBatch sb) { }

		public virtual void OnKill() { }
		public void Kill()
		{
			GameState.entityDel.Add(this);
		}

		public void UpdatePhysics()
		{
			if (!MoveUpdate())
			{
				position += velocity;

				foreach (Collider col in colliders)
				{
					col.Update();
				}
			}
		}
		public virtual bool MoveUpdate() { return false; } // return true to override

		public virtual List<Collider> GetCollidersFor(VecRect rect) { return colliders; }

		public virtual bool CanCollideWith(Entity e) { return true; }

		public virtual void CollisionEventCollider(Collider thisCol, Collider col, bool firstContact) { }
		public virtual void CollisionEventEntity(Entity other, bool firstContact) { }
	}
}
