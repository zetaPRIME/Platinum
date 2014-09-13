using System;
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

		public VecRect bounds = VecRect.Zero;

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

		public virtual void Update() { }
		public virtual void Draw(SpriteBatch sb) { }

		public virtual void OnKill() { }
		public void Kill()
		{
			GameState.entityDel.Add(this);
		}
	}
}
