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
		public static QuadTree<Collider> quadTree;// = //new QuadTree<Collider>(

		public static void PreUpdate()
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
			List<Collider> res = new List<Collider>();

			List<Collider> potential = quadTree.GetObjects(c.Rect)
				.FindAll(c2 => (c.layers & c2.layers) != 0) // if they share any layers
				.FindAll(c2 => (c.collidesWith & c2.categories) != 0); // if the collider being tested is looking for any of c2's categories

			foreach (Collider c2 in potential)
			{
				bool hitFound = false;

				foreach (ColliderShape cs1 in c.shapes)
				{
					foreach (ColliderShape cs2 in c2.shapes)
					{
						hitFound = TestCollision(cs1, cs2);
						if (hitFound) break;
					}
					if (hitFound) break;
				}

				if (hitFound) res.Add(c2);
			}

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
