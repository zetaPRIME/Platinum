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

			if (s1 is ColliderShapeCircle && s2 is ColliderShapeCircle) return TestCollision(s1 as ColliderShapeCircle, s2 as ColliderShapeCircle, out mtv);
			if (s1 is ColliderShapePolygon && s2 is ColliderShapePolygon) return TestCollision(s1 as ColliderShapePolygon, s2 as ColliderShapePolygon, calculateMTV, out mtv);
			if (s1 is ColliderShapeCircle && s2 is ColliderShapePolygon) return TestCollision(s1 as ColliderShapeCircle, s2 as ColliderShapePolygon, false, out mtv);
			if (s2 is ColliderShapeCircle && s1 is ColliderShapePolygon) return TestCollision(s2 as ColliderShapeCircle, s1 as ColliderShapePolygon, true, out mtv);

			mtv = Vector2.Zero;
			return false;
		}

		public static bool TestCollision(ColliderShapeCircle s1, ColliderShapeCircle s2, out Vector2 mtv)
		{
			if ((s2.Center - s1.Center).Length() > s1.Radius + s2.Radius) { mtv = Vector2.Zero; return false; }

			Vector2 vec = (s1.Center - s2.Center);
			float length = vec.Length() - (s1.Radius + s2.Radius);
			vec.Normalize();

			mtv = vec * length;
			return true;
		}
		public static bool TestCollision(ColliderShapePolygon s1, ColliderShapePolygon s2, bool calculateMTV, out Vector2 mtv)
		{
			float overlap = float.MaxValue;
			Vector2 ovec = Vector2.Zero;

			if (!calculateMTV && s1.NumPoints > s2.NumPoints) { ColliderShapePolygon sw = s1; s1 = s2; s2 = sw; } // make sure s1 has least faces

			List<LineSegment> s1f = s1.Faces;
			int numFaces = s1f.Count;

			int mlf = 0; // most likely face; calculated in the following block by comparing normals' dot products (in other words, side which is most facing the other's centroid)
			Vector2 vOff = s1.centroid - s2.centroid; vOff.Normalize();
			//Vector2 diff = s2.centroid - s1.centroid; diff.Normalize();
			float td = -1;
			float tdn;
			for (int i = 0; i < numFaces; i++)
			{
				//tdn = Vector2.Dot(s1f[i].Normal, diff);
				tdn = s1f[i].DotTo(s2.centroid);
				if (tdn > td) { td = tdn; mlf = i; }
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

		public static bool TestCollision(ColliderShapeCircle s1, ColliderShapePolygon s2, bool flip, out Vector2 mtv)
		{
			List<LineSegment> faces = s2.Faces;

			int face = -1;
			for (int i = 0; i < faces.Count; i++)
			{
				if (faces[i].DotTo(s1.Center + faces[i].Normal * s1.Radius) <= 0) continue; // don't even bother if it's facing away!
				
				// project
				Range r = new Range(Vector2.Dot(faces[i].Direction, faces[i].start)).Extend(Vector2.Dot(faces[i].Direction, faces[i].end));
				float p = Vector2.Dot(faces[i].Direction, s1.Center);
				if (!r.Contains(p)) continue;

				face = i; break;
			}

			if (face != -1) // process by face
			{
				float lp = Vector2.Dot(faces[face].Normal, faces[face].Center);
				float sp = Vector2.Dot(faces[face].Normal, s1.Center);

				float penetration = s1.Radius - (sp - lp);

				if (penetration < 0) { mtv = Vector2.Zero; return false; }
				if (flip) penetration *= -1;
				mtv = faces[face].Normal * penetration;
				return true;
			}
			else // process by nearest point
			{
				int cpt = -1;
				float closest = float.MaxValue;

				for (int i = 0; i < s2.Points.Length; i++)
				{
					float c = (s1.Center - s2.Points[i]).Length();
					if (c < closest)
					{
						cpt = i; closest = c;
					}
				}
				if (cpt == -1) { mtv = Vector2.Zero; return false; } // something has gone kind of horribly wrong here if this happens; just smile and nod, and hope the eldritch abomination doesn't eat your eyeballs
				Vector2 pt = s2.Points[cpt];

				bool inside = (s1.Center - s2.centroid).Length() < (pt - s2.centroid).Length();
				Vector2 normal = s1.Center - pt; normal.Normalize(); if (inside) normal *= -1;

				float pp = Vector2.Dot(normal, pt);
				float sp = Vector2.Dot(normal, s1.Center);

				float penetration = s1.Radius - (sp - pp);

				if (penetration < 0) { mtv = Vector2.Zero; return false; }
				if (flip) penetration *= -1;
				mtv = normal * penetration;
				return true;
			}
		}
	}
}
