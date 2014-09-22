using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum
{
	public class ColliderShapeCircle : ColliderShape
	{
		public ColliderShapeCircle(Collider parent) { this.parent = parent; parent.shapes.Add(this); dirty = true; }
		public ColliderShapeCircle(Collider parent, Vector2 center, float radius) : this(parent) { this.center = center; this.radius = radius; }

		public Vector2 center = Vector2.Zero;
		public float radius = 1f;

		Vector2 worldCenter = Vector2.Zero;

		public Vector2 Center { get { if (dirty) Update(); return worldCenter; } }
		public float Radius { get { return radius; } }

		VecRect boundsCache;

		public override VecRect Bounds { get { if (dirty) Update(); return boundsCache; } }

		public override void Update()
		{
			worldCenter = Vector2.Transform(center, parent.Transform);

			boundsCache = VecRect.FromPoints(worldCenter).ExpandOut(radius);

			dirty = false;
		}

		const int drawPoints = 16;
		public override void Draw(SpriteBatch sb)
		{
			Update();

			Vector2 screenCenter = worldCenter.Pixelize();
			Vector2[] points = new Vector2[drawPoints];
			Vector2 rad = new Vector2(0, -radius);

			for (int i = 0; i < drawPoints; i++)
			{
				points[i] = screenCenter + Vector2.Transform(rad, Matrix.CreateRotationZ((((float)Math.PI * 2f) / (float)drawPoints) * i));
			}

			for (int i = 0; i < drawPoints; i++)
			{
				LineSegment line = new LineSegment(points[i], points[(i + 1) % drawPoints]);
				sb.Draw(Core.txPixel, line.start, null, Color.LightBlue, (float)Math.Atan2(line.Direction.Y, line.Direction.X), new Vector2(0f, 0.5f), new Vector2(line.Length, 1f / GameState.cameraZoom), SpriteEffects.None, 0f);
			}
		}

		public override float RaycastAgainst(LineSegment line)
		{
			float pl = Vector2.Dot(line.Normal, line.start);
			float pc = Vector2.Dot(line.Normal, Center);
			float dist = Math.Abs(pl - pc);

			if (dist > Radius) return float.MaxValue; // no collision

			float chord = (float)Math.Sqrt((Radius * Radius) - (dist * dist));

			pl = Vector2.Dot(line.Direction, line.start);
			pc = Vector2.Dot(line.Direction, Center);
			dist = Math.Abs(pl - pc);

			dist -= chord;// / 2;

			if (dist > line.Length) return float.MaxValue;
			return dist;
		}
	}
}
