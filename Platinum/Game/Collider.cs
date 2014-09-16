using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Platinum
{
	public class Collider
	{
		public Entity parent;

		public Vector2 offset = Vector2.Zero;
		public float rotOffset = 0f;

		public BitField layers = 255;
		public bool solid = false;

		public void Update()
		{
			//
		}
		
		public bool CollidesWith(Collider other)
		{
			if ((layers & other.layers) == 0) return false;

			return false;
		}
	}
}
