using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using C3.XNA;

namespace Platinum
{
	public static partial class CollisionManager
	{
		// constants
		public const float SATPadding = 0f;//1f;

		// other things
		public static QuadTree<Collider> quadTree;// = //new QuadTree<Collider>(

		public static void PreUpdate()
		{
			ConfirmTree();
		}

		public static void ConfirmTree()
		{
			Rectangle wRect = worldRect();
			if (quadTree == null || quadTree.QuadRect != wRect)
			{
				quadTree = new QuadTree<Collider>(wRect);
				foreach (Entity e in GameState.entities) e.UpdateColliders();
				Console.WriteLine("Generating new QuadTree");
			}
		}

		static Rectangle worldRect()
		{
			return new Rectangle(0, 0, (int)GameState.worldSize.X, (int)GameState.worldSize.Y);
		}

		//

		public static List<Collider> TestCollider(Collider c)
		{
			Vector2 blah; return TestCollider(c, false, out blah);
		}
		public static List<Collider> TestCollider(Collider c, bool calculateMTV, out Vector2 mtv)
		{
			List<Collider> res = new List<Collider>();

			List<Collider> potential = quadTree.GetObjects(c.Rect)
				.FindAll(c2 => (c.layers & c2.layers) != 0) // if they share any layers
				.FindAll(c2 => (c.collidesWith & c2.categories) != 0); // if the collider being tested is looking for any of c2's categories

			Vector2 corr = Vector2.Zero;
			Vector2 ncorr = Vector2.Zero;

			foreach (Collider c2 in potential)
			{
				if (c2 == c) continue;
				bool hitFound = false;

				foreach (ColliderShape cs1 in c.shapes)
				{
					foreach (ColliderShape cs2 in c2.shapes)
					{
						hitFound = TestCollision(cs1, cs2, calculateMTV, out ncorr);
						if (calculateMTV && ncorr.Length() > corr.Length()) corr = ncorr;

						if (!calculateMTV && hitFound) break;
					}
					if (!calculateMTV && hitFound) break;
				}

				if (hitFound) res.Add(c2);
			}

			mtv = corr;
			return res;
		}

		public static float Raycast(LineSegment line, byte layers = 255, UInt32 lookFor = UInt32.MaxValue, params Entity[] ignore)
		{
			float rayDist = line.Length;
			float dist = float.MaxValue;

			//

			if (dist == float.MaxValue) return -1;
			return dist;
		}

		//
	}
}
