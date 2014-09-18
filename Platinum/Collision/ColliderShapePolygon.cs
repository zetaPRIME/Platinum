using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Platinum
{
	public class ColliderShapePolygon : ColliderShape
	{
		public ColliderShapePolygon(Collider parent) { this.parent = parent; }

		public Vector2[] points;

		public Vector2[] pointsCache;
		public VecRect boundsCache;

		public Vector2 centroid;

		public int NumPoints { get { return points.Length; } }
		public Vector2[] Points
		{
			get
			{
				if (!parent.dirty) return pointsCache;
				Matrix matrix = parent.Transform; bool flip = parent.flip;

				Vector2 orient = Vector2.One; if (flip) orient = new Vector2(-1, 1);

				centroid = Vector2.Zero;

				List<Vector2> np = new List<Vector2>();
				Vector2 v;
				for (int i = 0; i < points.Length; i++)
				{
					v = Vector2.Transform(points[i] * orient, matrix);
					np.Add(v);

					centroid += v;
				}
				centroid /= points.Length;
				if (flip) np.Reverse();
				pointsCache = np.ToArray();

				return pointsCache;
			}
		}

		public List<LineSegment> Faces
		{
			get
			{
				List<LineSegment> lst = new List<LineSegment>();
				Vector2[] pts = Points;
				for (int i = 0; i < pts.Length; i++)
				{
					lst.Add(new LineSegment(pts[i], pts[(i + 1) % pts.Length]));
				}
				return lst;
			}
		}

		public override VecRect Bounds
		{
			get
			{
				if (!parent.dirty) return boundsCache;
				boundsCache = VecRect.FromPoints(Points);
				return boundsCache;
			}
		}

		public Range Project(Vector2 normal)
		{
			Vector2[] pts = Points;

			Range r = new Range(Vector2.Dot(normal, pts[0]));

			for (int i = 1; i < pts.Length; i++) r = r.Extend(Vector2.Dot(normal, pts[i]));

			return r.Pad(CollisionManager.SATPadding);
		}
	}
}
