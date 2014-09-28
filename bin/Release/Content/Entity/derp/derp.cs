using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Platinum;

namespace ExampleBase
{
	public class derp : Entity
	{
		bool init = true;
		public override void Update()
		{
			if (init)
			{
				init = false;
				bounds = VecRect.Radius * 16f;
				new ColliderShapePolygon(new Collider(this), bounds.topLeft, bounds.topRight, bounds.bottomRight, bounds.bottomLeft);
				this.colliders[0].categories[0] = true;
				UpdateColliders();
			}
		}

		public override void Draw(SpriteBatch sb)
		{
			sb.DrawRect(WorldBounds.AsRectangle, Color.Pink);
		}
	}
}
