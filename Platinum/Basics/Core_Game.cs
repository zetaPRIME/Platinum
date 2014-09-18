using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fluent.IO;
using Ionic.Zip;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platinum
{
	public partial class Core
	{
		protected void Update_Game(GameTime gameTime)
		{
			#region Debug keys
			if (Input.KeyPressed(Keys.F3)) debugDisplay = !debugDisplay;
			#endregion

			GameDef.gameService.PreUpdate();

			// entities
			List<Entity> shouldUpdate = GameState.entities.FindAll(e => !e.Asleep);

			foreach (Entity e in shouldUpdate) e.UpdatePhysics();

			CollisionManager.PreUpdate();
			//CollisionManager.TestAll();

			foreach (Entity e in shouldUpdate) e.Update();

			// todo: particles

			GameDef.gameService.PostUpdate();
		}

		protected void Draw_Game(GameTime gameTime)
		{
			spriteBatch.GraphicsDevice.Clear(new Color(0.5f, 0f, 1f));
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

			GameDef.gameService.PreDraw(spriteBatch);

			List<Entity> drawList = GameState.entities.FindAll(e => (e.DrawOffScreen || e.OnScreen));
			drawList = drawList.OrderBy(e => e.drawLayer).ToList(); // wow, that's easy
			foreach (Entity e in drawList) e.Draw(spriteBatch);

			GameDef.gameService.PostDraw(spriteBatch);

			spriteBatch.End();

			#region debug display
			if (debugDisplay)
			{
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

				string debugText = "Debug display (F3)";
				debugText += "\nEntities: " + GameState.entities.Count;// +" (" + CollisionManager.collidable.Count + " collidable)";
				debugText += "\nInput: " + (UInt32)Input.players[0].down;

				/*debugText += "\n";
				IntPtr dev = Tao.Sdl.Sdl.SDL_JoystickOpen(0);
				debugText += Joystick.GrabJoysticks()[0]. + ": ";
				debugText += Tao.Sdl.Sdl.SDL_JoystickNumBalls(dev) + ": ";
				for (int i = -8; i < 8; i++)
				{
					if (Tao.Sdl.Sdl.SDL_JoystickGetButton(dev, i) > 0) debugText += "" + i + ", ";
				}
				Tao.Sdl.Sdl.SDL_JoystickClose(dev);*/

				spriteBatch.DrawString(fontDebug, debugText, Vector2.One * 8f, Color.White);

				spriteBatch.End();
			}
			#endregion
		}
	}
}
