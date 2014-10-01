using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using LitJson;

using Platinum;

namespace ExampleBase
{
	public class Rotoshredder : Entity
	{
		public float radius = 96f;

		public float rotationSpeed = 3f * (float)(Math.PI / 180.0);

		public override void LoadJson(JsonData j)
		{
			j.Read("radius", ref radius);
			rotationSpeed = 3f;
			j.Read("rotationSpeed", ref rotationSpeed);
			rotationSpeed *= (float)(Math.PI / 180.0);
		}

		public override void Init()
		{
			ColliderShapeCircle c = new ColliderShapeCircle(new Collider(this), new Vector2(0, -radius), 16f);
		}

		public override bool MoveUpdate()
		{
			rotation += rotationSpeed;
			return false;
		}

		public override void Draw(SpriteBatch sb)
		{
			Vector2 rad = new Vector2(0, -radius);

			SpriteEffects fx = SpriteEffects.None;
			if (rotationSpeed < 0) fx = SpriteEffects.FlipHorizontally;
			sb.Draw(mainTexture, rad.Transform(Transform), 0, Color.White, Rotation * 4f, mainTexture.center, 1f, fx);
		}
	}
}
