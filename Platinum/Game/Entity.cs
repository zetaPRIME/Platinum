using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics;
using FarseerPhysics.Dynamics;

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

		public Entity parent = null;

		public EntityDef def;

		public Vector2 Position
		{
			get
			{
				Vector2 par = Vector2.Zero;
				if (parent != null) par = parent.Position;
				return par + position;
			}
			set
			{
				Vector2 par = Vector2.Zero;
				if (parent != null) par = parent.Position;
				position = value - par;
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

		public VecRect WorldBounds
		{
			get { return bounds + Position; }
		}

		public virtual void Update() { }
		public virtual void Draw(SpriteBatch sb) { }

		public virtual void OnKill() { }
		public void Kill()
		{
			GameState.entityDel.Add(this);
		}

		public void UpdatePhysics()
		{
			position += velocity;

			foreach (Collider col in colliders)
			{
				col.parent = this;
				col.Update();
			}
		}

		public virtual List<Collider> GetCollidersFor(VecRect rect)
		{
			return colliders;
		}
	}
}
