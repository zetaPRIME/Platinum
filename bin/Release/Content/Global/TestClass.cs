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
			Entity e;
			if (first)
			{
				Random rand = new Random();
				//Entity e;
				for (int i = 0; i < -5000; i++)
				{
					e = EntityDef.NewEntity("TestEntity");
					e.position = new Vector2(rand.Next(640), rand.Next(480));
					const float range = 4f * 2f;
					e.velocity = new Vector2(((float)rand.NextDouble() - 0.5f) * range, ((float)rand.NextDouble() - 0.5f) * range);

					e.collisionPassive = true;
				}


				tbody = BodyFactory.CreateRectangle(GameState.physWorld, 320, 16, 1f);
				tbody.Position = new Vector2(320, 320);
				//tbody.IsSensor = true;
				tbody.SleepingAllowed = false;
			}
			first = false;

			Random rand2 = new Random();
			e = EntityDef.NewEntity("TestEntity");
			e.position = new Vector2(rand2.Next(640), rand2.Next(480));
			const float range2 = 4f * 2f;
			e.velocity = new Vector2(((float)rand2.NextDouble() - 0.5f) * range2, ((float)rand2.NextDouble() - 0.5f) * range2);

			e.collisionPassive = true;

			tbody.Rotation += 0.025f;
		}
	}
}
