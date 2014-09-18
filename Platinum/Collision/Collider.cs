using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using C3.XNA;

namespace Platinum
{
	public class Collider : IQuadStorable
	{
		public Entity parent;

		public Vector2 position = Vector2.Zero;
		public float rotation = 0f;
		public bool flip = false;

		public bool dirty = false;

		public BitField32 categories = 1;
		public BitField32 collidesWith = 1;
		public BitField layers = 255;

		public List<ColliderShape> shapes = new List<ColliderShape>();

		Matrix parentLastMatrix;
		Matrix matrixCache;
		public Matrix Transform
		{
			get {
				Matrix xform = parent.Transform;
				if (parentLastMatrix == xform) return matrixCache;
				parentLastMatrix = xform;
				matrixCache = xform * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(position, 0));

				return matrixCache;
			}
		}

		VecRect boundsCache;
		public VecRect Bounds
		{
			get
			{
				if (!dirty) return boundsCache;
				if (shapes.Count == 0) { boundsCache = VecRect.Zero; return boundsCache; }

				boundsCache = shapes[0].Bounds;
				for (int i = 1; i < shapes.Count; i++) boundsCache = boundsCache.Expand(shapes[i].Bounds);

				return boundsCache;
			}
		}

		public void Update()
		{
			dirty = true;

			if (!CollisionManager.quadTree.Move(this)) CollisionManager.quadTree.Add(this);
		}

		public void KillFromTree()
		{
			CollisionManager.quadTree.Remove(this);
		}
		
		public bool CollidesWith(Collider other)
		{
			if ((layers & other.layers) == 0) return false;

			return false;
		}

		public bool RaycastAgainst()
		{
			return false;
		}

		public Rectangle Rect { get { return Bounds.AsRectangle; } }
		public bool HasMoved { get { return dirty; } }
	}
}
