using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Platinum
{
	public struct LineSegment
	{
		public Vector2 start, end;

		public LineSegment(Vector2 start, Vector2 end)
		{
			this.start = start;
			this.end = end;
		}

		public float Length { get { return (end - start).Length(); } }
		public VecRect Bounds { get { return VecRect.FromPoints(start, end); } }
		public Vector2 Direction { get { Vector2 d = end - start; d.Normalize(); return d; } }
		public Vector2 Normal { get { Vector2 d = Direction; return new Vector2(d.Y, -d.X); } }

		public static LineSegment operator *(LineSegment seg, Matrix mtx) { return new LineSegment(Vector2.Transform(seg.start, mtx), Vector2.Transform(seg.end, mtx)); }
	}
}
