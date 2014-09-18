using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum
{
	public static partial class CollisionManager
	{
		public static bool TestCollision(ColliderShape s1, ColliderShape s2)
		{
			Vector2 blah; return TestCollision(s1, s2, false, out blah);
		}
		public static bool TestCollision(ColliderShape s1, ColliderShape s2, bool calculateMTV, out Vector2 mtv)
		{
			if (!s1.Bounds.Intersects(s2.Bounds)) { mtv = Vector2.Zero; return false; }

			if (s1 is ColliderShapePolygon && s2 is ColliderShapePolygon) return TestCollision(s1 as ColliderShapePolygon, s2 as ColliderShapePolygon, calculateMTV, out mtv);

			mtv = Vector2.Zero;
			return false;
		}

		public static bool TestCollision(ColliderShapePolygon s1, ColliderShapePolygon s2, bool calculateMTV, out Vector2 mtv)
		{
			float overlap = float.MaxValue;
			Vector2 ovec = Vector2.Zero;

			if (!calculateMTV && s1.NumPoints > s2.NumPoints) { ColliderShapePolygon sw = s1; s1 = s2; s2 = sw; } // make sure s1 has least faces

			List<LineSegment> s1f = s1.Faces;
			int numFaces = s1f.Count;

			int mlf = 0; // most likely face; calculated in the following block by comparing normals' differences
			Vector2 vOff = s1.centroid - s2.centroid; vOff.Normalize();
			Vector2 diff = s2.centroid - s1.centroid; diff.Normalize();
			float td = 573;
			float tdn;
			for (int i = 0; i < numFaces; i++)
			{
				tdn = (diff - s1f[i].Normal).Length();
				if (tdn < td) { td = tdn; mlf = i; }
			}

			// now to SAT
			Range r1 = new Range(0), r2 = new Range(0);

			int cf = 0;
			Vector2 normal = Vector2.Zero;
			for (int i = 0; i < numFaces; i++)
			{
				cf = (i + mlf) % numFaces;
				normal = s1f[cf].Normal;
				r1 = s1.Project(normal);
				r1 += Vector2.Dot(normal, vOff);
				r2 = s2.Project(normal);
				if (!r1.Overlaps(r2)) { mtv = Vector2.Zero; return false; } // ruled out collision
				if (calculateMTV)
				{
					float o = r1.GetOverlap(r2);
					if (o < overlap)
					{
						overlap = o; ovec = normal;
					}
				}
			}

			if (calculateMTV) // second pass
			{
				List<LineSegment> s2f = s2.Faces;
				for (int i = 0; i < s2f.Count; i++)
				{
					normal = s2f[i].Normal;
					r1 = s1.Project(normal);
					r1 += Vector2.Dot(normal, vOff);
					r2 = s2.Project(normal);
					if (!r1.Overlaps(r2)) { mtv = Vector2.Zero; return false; } // ruled out collision

					float o = r1.GetOverlap(r2);
					if (o < overlap)
					{
						overlap = o; ovec = normal;
					}
				}

				overlap = Math.Abs(overlap);

				if (Vector2.Dot(vOff, ovec) < 0) ovec *= -1f;

				ovec.Normalize();
				mtv = ovec * overlap;
			}
			else mtv = Vector2.Zero;

			return true; // if you get here, then you tested correct on all accounts!
		}
	}
}
