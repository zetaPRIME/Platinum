using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Platinum;
using FarseerPhysics.Factories;

namespace ExampleBase
{
	public class TestEntity : Entity
	{
		bool init = false;

		public override void Update()
		{
			if (!init)
			{
				bounds = new VecRect(Vector2.One * -32, Vector2.One * 32);
				
				position.X = 320f;

				Collider col = new Collider();
				colliders.Add(col);

				col.physBody = BodyFactory.CreateCircle(GameState.physWorld, 32, 1f);

				init = true;
			}

			//GameState.physWorld.Step(1f);
			//if (GameState.physWorld.RayCast(Position, Position + new Vector2(0, 32f)).Count > 0) velocity = new Vector2(0, -3f);
			//if (physBody.ContactList != null) velocity = new Vector2(0, -3f);

			velocity += new Vector2(0, 0.025f);
			rotation += 0.01f;
		}

		public override void Draw(SpriteBatch sb)
		{
			//velocity = Vector2.One;

			ExtTexture tex = PackageManager.globalPackage.GetTexture("TestImage");

			sb.Draw(tex, Position, null, Color.White, 0f, tex.center, 1f, SpriteEffects.None);

			float cast = Collision.Raycast(new Vector2(320, 0), new Vector2(320, 480), 255);
			sb.DrawString(Core.fontDebug, "raycast " + cast, Vector2.One * 16, Color.White);
		}
	}
}
