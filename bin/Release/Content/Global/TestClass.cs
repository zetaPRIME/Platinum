using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Platinum;

namespace ExampleBase
{
	public class TestClass : GameService
	{
		float blah = 0;

		public override void PostDraw(SpriteBatch sb)
		{
			ExtTexture tex = PackageManager.globalPackage.GetTexture("TestImage");
			sb.Draw(tex, new Vector2(320, 240), null, Color.White, blah, tex.center, 1f + blah, SpriteEffects.None);

			blah += 0.025f;
		}
	}
}
