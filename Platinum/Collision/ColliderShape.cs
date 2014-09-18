using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platinum
{
	public abstract class ColliderShape
	{
		protected Collider parent;

		public float RaycastAgainst(LineSegment line) { return float.MaxValue; }

		public virtual VecRect Bounds { get { return VecRect.Zero; } }
	}
}
