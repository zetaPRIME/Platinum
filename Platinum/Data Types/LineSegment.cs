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
		public Vector2 Center { get { return (start + end) / 2f; } }
		public VecRect Bounds { get { return VecRect.FromPoints(start, end); } }
		public Vector2 Direction { get { Vector2 d = end - start; d.Normalize(); return d; } }
		public Vector2 Normal { get { Vector2 d = Direction; return new Vector2(d.Y, -d.X); } }

		public Vector2 PointAlong(float length)
		{
			return start + (Direction * length);
		}

		public float DotTo(Vector2 point)
		{
			Vector2 offset = point - Center; offset.Normalize();
			return Vector2.Dot(offset, Normal);
		}

		public float Intersection(LineSegment other)
		{
			// borrowed from example at http://thirdpartyninjas.com/blog/2008/10/07/line-segment-intersection/
			float ua = (other.end.X - other.start.X) * (start.Y - other.start.Y) - (other.end.Y - other.start.Y) * (start.X - other.start.X);
			float ub = (end.X - start.X) * (start.Y - other.start.Y) - (end.Y - start.Y) * (start.X - other.start.X);
			float denominator = (other.end.Y - other.start.Y) * (end.X - start.X) - (other.end.X - other.start.X) * (end.Y - start.Y);

			//bool intersection, coincident;
			//intersection = coincident = false;

			if (Math.Abs(denominator) <= 0.00001f)
			{
				if (Math.Abs(ua) <= 0.00001f && Math.Abs(ub) <= 0.00001f)
				{
					//intersection = coincident = true;
					//intersectionPoint = (point1 + point2) / 2;
				}
			}
			else
			{
				ua /= denominator;
				ub /= denominator;

				if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
				{
					//intersection = true;
					return ua * Length;
				}
			}

			return -1;
		}

		public static LineSegment operator *(LineSegment seg, Matrix mtx) { return new LineSegment(Vector2.Transform(seg.start, mtx), Vector2.Transform(seg.end, mtx)); }
	}
}
