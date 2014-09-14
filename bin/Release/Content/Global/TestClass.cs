using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Platinum;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace ExampleBase
{
	public class TestClass : GameService
	{
		float blah = 0;

		bool first = true;

		Body tbody;

		public override void PostDraw(SpriteBatch sb)
		{
			ExtTexture tex = PackageManager.globalPackage.GetTexture("TestImage");
			//sb.Draw(tex, new Vector2(320, 240), null, Color.White, blah, tex.center, 1f + blah, SpriteEffects.None);

			blah += 0.025f;
		}

		public override void PreUpdate()
		{
			if (first)
			{
				EntityDef.NewEntity("TestEntity");

				tbody = BodyFactory.CreateRectangle(GameState.physWorld, 320, 16, 1f);
				tbody.Position = new Vector2(320, 320);
				//tbody.IsSensor = true;
				tbody.SleepingAllowed = false;
			}
			first = false;

			tbody.Rotation += 0.025f;
		}
	}
}
