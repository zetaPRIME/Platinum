using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Platinum;

namespace ExampleBase
{
	public class TestEntity : Entity
	{
		public override void Draw(SpriteBatch sb)
		{
			velocity = Vector2.One;

			ExtTexture tex = PackageManager.globalPackage.GetTexture("TestImage");

			sb.Draw(tex, Position, null, Color.White, 0f, tex.center, 1f, SpriteEffects.None);
		}
	}
}
