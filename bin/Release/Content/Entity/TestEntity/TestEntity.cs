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
		bool init = false;

		int frame = 0;

		public override void Update()
		{
			if (!init)
			{
				bounds = new VecRect(Vector2.One * -32, Vector2.One * 32);
				
				//position.X = 320f;

				Collider col = new Collider();
				colliders.Add(col);

				//collisionPassive = true;

				init = true;
			}

			PlayerInput p = Input.players[0];

			float rspeed = 0.032f;
			if (p.Held(Button.R)) rspeed *= 2;
			if (p.Held(Button.L)) rspeed *= -1;
			rotation += rspeed;

			if (Parent == null)
			{
				

				float speed = 3f;
				if (p.Held(Button.X) || p.Held(Button.Y)) speed = 6f;
				velocity = Vector2.Zero;
				if (p.Held(Button.Up)) velocity.Y = -speed;
				if (p.Held(Button.Down)) velocity.Y = speed;
				if (p.Held(Button.Left)) velocity.X = -speed;
				if (p.Held(Button.Right)) velocity.X = speed;
			}
			
			frame++;
		}

		public override void Draw(SpriteBatch sb)
		{
			if (!OnScreen) return;

			//velocity = Vector2.One;

			ExtTexture tex = PackageManager.globalPackage.GetTexture("TestImage");

			sb.Draw(tex, ScreenPosition, null, Color.White, Rotation, tex.center, 1f, SpriteEffects.None);

			//float cast = Collision.Raycast(new Vector2(320, 0), new Vector2(320, 480), 255);
			//sb.DrawString(Core.fontDebug, "raycast " + cast, Vector2.One * 16, Color.White);
		}

		public override bool CanCollideWith(Entity e)
		{
			return false;
		}
	}
}
