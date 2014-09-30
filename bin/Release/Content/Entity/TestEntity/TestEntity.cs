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

		float dscale = 1.0f;

		Vector2 testVec = Vector2.Zero;

		public override bool DrawOffScreen { get { return true; } }
		public override void Update()
		{
			if (!init)
			{
				bounds = new VecRect(Vector2.One * -32, Vector2.One * 32);
				
				//position.X = 320f;

				//Collider col = new Collider(this);
				//new ColliderShapeCircle(col, Vector2.Zero, 16f);
				//Collider col = new ColliderShapeCircle(new Collider(this), Vector2.Zero, 16f).parent;
				Collider col = new ColliderShapePolygon(new Collider(this), new Vector2(-16, -16), new Vector2(16, -16), new Vector2(16, 16), new Vector2(-16, 16)).parent;

				col.collidesWith = UInt32.MaxValue;

				//collisionPassive = true;

				init = true;
			}

			PlayerInput p = Input.players[0];

			float rspeed = 0.032f;
			if (p.Held(Button.R)) rspeed *= 2;
			if (p.Held(Button.L)) rspeed *= -1;
			rotation += rspeed;
			if (p.Held(Button.A)) rotation = 0f;

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

			Vector2 correction = Vector2.Zero;

			/*dscale = 1f;
			colliders[0].Update();
			List<Collider> collis = CollisionManager.TestCollider(colliders[0], true, out correction);
			if (collis.Count != 0)
			{
				dscale = 1.2f;

				//Position += Vector2.Reflect(velocity, correction);
				Position += correction;
			}*/

			LineSegment line = new LineSegment(Position, Position + new Vector2(0, 128));
			Collider blah;
			float cast = CollisionManager.Raycast(line, out blah, 255, UInt32.MaxValue, this);
			if (cast != -1) testVec = line.PointAlong(cast);
			else testVec = line.end;

			if (p.Held(Button.Select)) GameState.cameraZoom += 0.01f;
			if (p.Pressed(Button.Start)) GameState.cameraZoom = 1;
			
			frame++;
		}

		public override bool MoveUpdate()
		{
			//position += velocity;
			if (colliders.Count == 0) return true;

			int cycles = 4;

			for (int i = 0; i < cycles; i++)
			{
				position += velocity / cycles;
				colliders[0].TestIndividual(velocity.Length(), (ts, os, mtv) =>
				{
					Position += mtv; colliders[0].Update();

					return CollisionState.Continue;
				});
			}

			return true;
		}

		public override void Draw(SpriteBatch sb)
		{
			if (!OnScreen) return;

			//velocity = Vector2.One;

			ExtTexture tex = mainTexture; // PackageManager.globalPackage.GetTexture("TestImage");

			sb.Draw(tex, Position, null, Color.White, Rotation, tex.center, dscale, SpriteEffects.None);

			sb.Draw(Core.txPixel, testVec.Pixelize(), null, Color.White, 0f, Vector2.One * 0.5f, 5f, SpriteEffects.None, 0f);

			sb.DrawRect(new Rectangle(600, 440, 40, 40), Color.Green);
			
			//float cast = Collision.Raycast(new Vector2(320, 0), new Vector2(320, 480), 255);
			//sb.DrawString(Core.fontDebug, "raycast " + cast, Vector2.One * 16, Color.White);
		}
	}
}
