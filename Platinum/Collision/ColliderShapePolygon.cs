using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum
{
	public class ColliderShapePolygon : ColliderShape
	{
		public ColliderShapePolygon(Collider parent) { this.parent = parent; parent.shapes.Add(this); dirty = true; }
		public ColliderShapePolygon(Collider parent, params Vector2[] points) : this(parent) { this.points = points; }

		public Vector2[] points;

		public Vector2[] pointsCache;
		public VecRect boundsCache;

		public Vector2 centroid;

		public int NumPoints { get { return points.Length; } }
		public Vector2[] Points
		{
			get
			{
				if (dirty) Update();
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
				if (dirty) Update();
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

		public override void Update()
		{
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

			boundsCache = VecRect.FromPoints(pointsCache);
			dirty = false;
		}

		public override void Draw(SpriteBatch sb)
		{
			Update();

			if (points == null) return;
			List<LineSegment> faces = Faces;
			foreach (LineSegment line in faces) sb.DrawLine(line, Color.LightGreen, 1f); // sb.Draw(Core.txPixel, line.start, null, Color.LightGreen, (float)Math.Atan2(line.Direction.Y, line.Direction.X), new Vector2(0f, 0.5f), new Vector2(line.Length, 1f / GameState.cameraZoom), SpriteEffects.None, 0f);
			foreach (LineSegment line in faces) sb.Draw(Core.txPixel, line.start, null, Color.Yellow, 0f, new Vector2(0.5f, 0.5f), 3f / GameState.cameraZoom, SpriteEffects.None, 0f);
		}

		public override float RaycastAgainst(LineSegment line)
		{
			List<LineSegment> faces = Faces;
			float closest = float.MaxValue;
			foreach (LineSegment face in faces)
			{
				if (face.DotTo(line.start) <= 0) continue; // don't count away-faces
				float inter = line.Intersection(face);
				if (inter < 0) continue;
				if (inter < closest) closest = inter;
			}

			return closest;
		}
	}
}
