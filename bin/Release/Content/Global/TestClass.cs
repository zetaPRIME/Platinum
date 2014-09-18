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

		bool first = true;

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

				e = EntityDef.NewEntity("TestEntity");
				e.position = new Vector2(128, 128);

				/*Entity e2 = EntityDef.NewEntity("TestEntity");
				e2.Parent = e;
				e2.position = new Vector2(32, 0);*/

				e = new Entity();
				GameState.entities.Add(e);
				e.position = new Vector2(320, 240);

				Collider nc = new Collider(e);
				new ColliderShapePolygon(nc, new Vector2(-128, -16), new Vector2(128, -16), new Vector2(128, 16), new Vector2(-128, 16));
				new ColliderShapePolygon(nc, new Vector2(96, -16), new Vector2(128, -48), new Vector2(128, -16));

				nc.categories = 1;
				//nc.Update();
			}
			first = false;

			if (true || Input.players[0].Held(Button.A)) return;
			e = new Entity();
			GameState.entities.Add(e);
			e.position = new Vector2(500, 400);
			new ColliderShapePolygon(new Collider(e), new Vector2(-16, -16), new Vector2(16, -16), new Vector2(16, 16), new Vector2(-16, 16));

			/*Random rand2 = new Random();
			e = EntityDef.NewEntity("TestEntity");
			e.position = new Vector2(rand2.Next(640), rand2.Next(480));
			const float range2 = 4f * 2f;
			e.velocity = new Vector2(((float)rand2.NextDouble() - 0.5f) * range2, ((float)rand2.NextDouble() - 0.5f) * range2);

			e.collisionPassive = true;*/

		}
	}
}
