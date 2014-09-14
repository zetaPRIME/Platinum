using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision;

namespace Platinum
{
	public static class Collision
	{
		public struct CollisionPair
		{
			Collider c1, c2;

			public CollisionPair(Collider p1, Collider p2) { c1 = p1; c2 = p2; }
		}
		public struct CollisionPairEntity
		{
			Entity c1, c2;

			public CollisionPairEntity(Entity p1, Entity p2) { c1 = p1; c2 = p2; }
		}

		public static List<Entity> collidable = new List<Entity>();

		static List<CollisionPair> logPrevFrame = new List<CollisionPair>();
		static List<CollisionPair> logCurFrame = new List<CollisionPair>();

		static List<CollisionPairEntity> logPrevFrameEntity = new List<CollisionPairEntity>();
		static List<CollisionPairEntity> logCurFrameEntity = new List<CollisionPairEntity>();

		public static void PreUpdate()
		{
			collidable.Clear();
			foreach (Entity e in GameState.entities)
			{
				if (e.colliders.Count > 0) collidable.Add(e);
			}

			// step frames
			logPrevFrame = logCurFrame; logCurFrame = new List<CollisionPair>();
		}

		//

		#region log checks
		public static bool CollidingNow(Collider c1, Collider c2)
		{
			return (logCurFrame.Contains(new CollisionPair(c1, c2)) || logCurFrame.Contains(new CollisionPair(c2, c1)));
		}
		public static bool CollidingLastFrame(Collider c1, Collider c2)
		{
			return (logPrevFrame.Contains(new CollisionPair(c1, c2)) || logPrevFrame.Contains(new CollisionPair(c2, c1)));
		}

		public static bool CollidingNow(Entity c1, Entity c2)
		{
			return (logCurFrameEntity.Contains(new CollisionPairEntity(c1, c2)) || logCurFrameEntity.Contains(new CollisionPairEntity(c2, c1)));
		}
		public static bool CollidingLastFrame(Entity c1, Entity c2)
		{
			return (logPrevFrameEntity.Contains(new CollisionPairEntity(c1, c2)) || logPrevFrameEntity.Contains(new CollisionPairEntity(c2, c1)));
		}
		#endregion

		public static void TestEntity(Entity e)
		{
			//foreach
		}

		public static float Raycast(Vector2 start, Vector2 end, BitField layers, params Entity[] ignore)
		{
			VecRect testRect = new VecRect(new Vector2(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y)), new Vector2(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y)));

			float rayDist = (start - end).Length();
			float dist = float.MaxValue;

			foreach (Entity e in collidable)
			{
				if (ignore.Contains(e)) continue;
				if (!testRect.Intersects(e.WorldBounds)) continue;

				foreach (Collider col in e.GetCollidersFor(testRect))
				{
					foreach (Fixture f in col.physBody.FixtureList)
					{
						RayCastInput inp = new RayCastInput();
						inp.Point1 = start; inp.Point2 = end;
						inp.MaxFraction = 1f;

						for (int i = 0; i < f.Shape.ChildCount; i++)
						{
							RayCastOutput output;
							if (!f.RayCast(out output, ref inp, i)) continue;
							float fdist = rayDist * output.Fraction;
							if (fdist < dist) dist = fdist;
						}
					}
				}
			}

			if (dist == float.MaxValue) return -1;
			return dist;
		}

		public static bool TextFixture(Fixture a, Fixture b)
		{
			if (a == null || b == null) return false;

			for (int i = 0; i < a.Shape.ChildCount; i++)
			{
				for (int j = 0; j < b.Shape.ChildCount; j++)
				{
					Transform ta, tb;
					a.Body.GetTransform(out ta);
					b.Body.GetTransform(out tb);
					
					if (FarseerPhysics.Collision.Collision.TestOverlap(a.Shape, i, b.Shape, j, ref ta, ref tb)) return true;
				}
			}

			return false;
		}
	}
}
