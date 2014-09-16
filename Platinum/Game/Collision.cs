using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
			List<CollisionPair> logSwap = logPrevFrame;
			logPrevFrame = logCurFrame;
			logCurFrame = logSwap;
			logCurFrame.Clear();

			List<CollisionPairEntity> logSwapEntity = logPrevFrameEntity;
			logPrevFrameEntity = logCurFrameEntity;
			logCurFrameEntity = logSwapEntity;
			logCurFrameEntity.Clear();
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

		public static void TestAll()
		{
			foreach (Entity e1 in collidable)
			{
				if (e1.collisionPassive) continue;

				foreach (Entity e2 in collidable)
				{
					if (e1 == e2) continue;
					if (!e1.WorldBounds.Intersects(e2.WorldBounds)) continue;
					if (!e1.CanCollideWith(e2) || !e2.CanCollideWith(e1)) continue;

					foreach (Collider c1 in e1.GetCollidersFor(e2.WorldBounds))
					{
						foreach (Collider c2 in e2.GetCollidersFor(e1.WorldBounds))
						{
							bool first = true;
							if (c1.CollidesWith(c2))
							{
								// log!
								logCurFrame.Add(new CollisionPair(c1, c2));
								if (first) logCurFrameEntity.Add(new CollisionPairEntity(e1, e2));

								// event
								bool firstCont = CollidingLastFrame(c1, c2);
								e1.CollisionEventCollider(c1, c2, firstCont);
								e2.CollisionEventCollider(c2, c1, firstCont);
								if (first)
								{
									firstCont = CollidingLastFrame(e1, e2);
									e1.CollisionEventEntity(e2, firstCont);
									e2.CollisionEventEntity(e1, firstCont);
								}

								first = false;
							}
						}
					}
				}
			}
		}

		public static void TestEntity(Entity et, bool logCollision = false)
		{
			foreach (Entity e in collidable)
			{
				if (e == et) continue;
				if (!et.WorldBounds.Intersects(e.WorldBounds)) continue;

				foreach (Collider ct in et.GetCollidersFor(e.WorldBounds))
				{
					foreach (Collider col in e.GetCollidersFor(et.WorldBounds))
					{
						if (ct.CollidesWith(col))
						{
							if (logCollision)
							{
								logCurFrame.Add(new CollisionPair(ct, col));
								CollisionPairEntity cpe = new CollisionPairEntity(et, e);
								if (!logCurFrameEntity.Contains(cpe)) logCurFrameEntity.Add(cpe);
							}


						}
					}
				}
				
			}
		}

		public static float Raycast(Vector2 start, Vector2 end, byte layers = 255, bool solidOnly = true, params Entity[] ignore)
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
					if (solidOnly && !col.solid) continue;
					//
				}
			}

			if (dist == float.MaxValue) return -1;
			return dist;
		}
	}
}
