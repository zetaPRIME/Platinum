using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum
{
	public struct VecRect
	{
		public Vector2 topLeft;
		public Vector2 bottomRight;

		//public VecRect() : this(Vector2.Zero, Vector2.Zero) { }
		public VecRect(Vector2 topLeft, Vector2 bottomRight)
		{
			this.topLeft = topLeft; this.bottomRight = bottomRight;
		}

		#region operators and static defs
		public static VecRect operator +(VecRect orig, Vector2 offset)
		{
			return new VecRect(orig.topLeft + offset, orig.bottomRight + offset);
		}
		public static VecRect operator -(VecRect orig, Vector2 offset)
		{
			return orig + (offset * -1f);
		}

		public static VecRect Zero { get { return new VecRect(Vector2.Zero, Vector2.Zero); } }
		public static VecRect Radius { get { return new VecRect(-Vector2.One, Vector2.One); } }

		public static VecRect FromPoints(params Vector2[] points)
		{
			if (points.Length == 0) return VecRect.Zero;
			VecRect v = new VecRect(points[0], points[0]);

			for (int i = 1; i < points.Length; i++) v = v.Expand(points[i]);

			return v;
		}

		public static VecRect Combine(params VecRect[] rects)
		{
			if (rects.Length == 0) return VecRect.Zero;
			VecRect v = rects[0];

			for (int i = 1; i < rects.Length; i++) v = v.Expand(rects[i]);

			return v;
		}
		#endregion

		public Vector2 Size
		{
			get { return bottomRight - topLeft; }
		}

		public Vector2 topRight { get { return new Vector2(bottomRight.X, topLeft.Y); } }
		public Vector2 bottomLeft { get { return new Vector2(topLeft.X, bottomRight.Y); } }

		public float left { get { return topLeft.X; } }
		public float right { get { return bottomRight.X; } }
		public float top { get { return topLeft.Y; } }
		public float bottom { get { return bottomRight.Y; } }

		public Rectangle AsRectangle { get { return new Rectangle((int)left, (int)top, (int)(right - left), (int)(bottom - top)); } }

		public bool Intersects(VecRect other)
		{
			return !(other.topLeft.X > bottomRight.X || other.bottomRight.X < topLeft.X || other.topLeft.Y > bottomRight.Y || other.bottomRight.Y < topLeft.Y); // wow, that's so obvious

			// old
			/* if (Contains(other.topLeft) || Contains(other.bottomRight) || Contains(other.topRight) || Contains(other.bottomLeft)
				|| other.Contains(topLeft) || other.Contains(bottomRight) || other.Contains(topRight) || other.Contains(bottomLeft)) return true;

			return false; */
		}

		public bool Contains(Vector2 vec)
		{
			if (vec.X >= topLeft.X && vec.X <= bottomRight.X && vec.Y >= topLeft.Y && vec.Y <= bottomRight.Y) return true;
			return false;
		}

		public VecRect Expand(VecRect with)
		{
			return new VecRect(new Vector2(Math.Min(topLeft.X, with.topLeft.X), Math.Min(topLeft.Y, with.topLeft.Y)), new Vector2(Math.Max(bottomRight.X, with.bottomRight.X), Math.Max(bottomRight.Y, with.bottomRight.Y)));
		}

		public VecRect Expand(Vector2 vec)
		{
			return new VecRect(new Vector2(Math.Min(topLeft.X, vec.X), Math.Min(topLeft.Y, vec.Y)), new Vector2(Math.Max(bottomRight.X, vec.X), Math.Max(bottomRight.Y, vec.Y)));
		}
	}
}
