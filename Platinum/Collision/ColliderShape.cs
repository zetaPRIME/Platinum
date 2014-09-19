using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum
{
	public abstract class ColliderShape
	{
		public Collider parent { get; protected set; }

		public virtual float RaycastAgainst(LineSegment line) { return float.MaxValue; }

		public virtual VecRect Bounds { get { return VecRect.Zero; } }

		public bool dirty { get; protected set; }

		public void SetDirty() { dirty = true; }

		public virtual void Update() { }

		public virtual void Draw(SpriteBatch sb) { }
	}
}
