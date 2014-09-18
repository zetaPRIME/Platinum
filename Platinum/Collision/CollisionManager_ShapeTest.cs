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
			if (!s1.Bounds.Intersects(s2.Bounds)) return false;

			if (s1 is ColliderShapePolygon && s2 is ColliderShapePolygon) return TestCollision(s1 as ColliderShapePolygon, s2 as ColliderShapePolygon);

			return false;
		}

		public static bool TestCollision(ColliderShapePolygon s1, ColliderShapePolygon s2)
		{
			if (s2.NumPoints > s1.NumPoints) { ColliderShapePolygon sw = s1; s1 = s2; s2 = sw; } // make sure s1 has least faces

			List<LineSegment> s1f = s1.Faces;
			int numFaces = s1f.Count;

			int mlf = 0; // most likely face; calculated in the following block by comparing normals' differences
			Vector2 diff = s2.centroid - s1.centroid; diff.Normalize();
			float td = 573;
			float tdn;
			for (int i = 0; i < numFaces; i++)
			{
				tdn = (diff - s1f[i].Normal).Length();
				if (tdn < td) { td = tdn; mlf = i; }
			}

			// now to SAT
			Range r1, r2;

			int cf = 0;
			Vector2 normal;
			for (int i = 0; i < numFaces; i++)
			{
				cf = (i + mlf) % numFaces;
				normal = s1f[cf].Normal;
				r1 = s1.Project(normal);
				r2 = s2.Project(normal);
				if (!r1.Overlaps(r2)) return false; // ruled out collision
			}

			return true; // if you get here, then you tested correct on all accounts!
		}
	}
}
