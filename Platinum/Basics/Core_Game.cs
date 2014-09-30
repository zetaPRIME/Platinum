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
		protected void Init_Game()
		{
			if (forceScene != "") GameState.EnterScene(forceScene);
			else GameState.EnterScene(GameDef.defaultScene);
		}

		protected void Update_Game(GameTime gameTime)
		{
			#region Debug keys
			if (Input.KeyPressed(Keys.F2)) GameState.EnterScene(GameState.scene.name, GameState.scene.currentMap.name, true);
			if (Input.KeyPressed(Keys.F3)) debugDisplay = !debugDisplay;
			#endregion

			GameDef.gameService.PreUpdate();
			GameState.scene.sceneMode.PreUpdate();
			GameState.scene.sceneService.PreUpdate();

			// entities
			List<Entity> shouldUpdate = GameState.entities.FindAll(e => !e.Asleep && !e.Disabled);

			CollisionManager.PreUpdate();
			shouldUpdate.Operate(e => e.UpdatePhysics()).Operate(e => e.Update());
			//foreach (Entity e in shouldUpdate) e.UpdatePhysics();
			//foreach (Entity e in shouldUpdate) e.Update();

			// todo: particles

			GameState.scene.sceneService.PostUpdate();
			GameState.scene.sceneMode.PostUpdate();
			GameDef.gameService.PostUpdate();
		}

		protected void Draw_Game(GameTime gameTime)
		{
			PrepareTarget();

			spriteBatch.GraphicsDevice.Clear(GameState.backColor);

			VecRect cameraBox = GameState.cameraBox; GameState.cameraBoxCache = cameraBox;
			Vector2 worldSize = GameState.worldSize;
			#region clamp camera position
			if (worldSize.X < cameraBox.Size.X)
			{
				GameState.cameraPos.X = worldSize.X / 2f;
			}
			else
			{
				float left = cameraBox.left;
				if (left < 0) GameState.cameraPos.X += -left;
				float right = cameraBox.right - worldSize.X;
				if (right > 0) GameState.cameraPos.X += -right;
			}
			if (worldSize.Y < cameraBox.Size.Y)
			{
				GameState.cameraPos.Y = worldSize.Y / 2f;
			}
			else
			{
				float top = cameraBox.top;
				if (top < 0) GameState.cameraPos.Y += -top;
				float bottom = cameraBox.bottom - worldSize.Y;
				if (bottom > 0) GameState.cameraPos.Y += -bottom;
			}
			#endregion

			spriteBatch.CameraOn(false);
			
			GameDef.gameService.PreDraw(spriteBatch);
			GameState.scene.sceneService.PreDraw(spriteBatch);

			List<Entity> drawList = GameState.entities.FindAll(e => (e.DrawOffScreen || e.OnScreen) && !e.Disabled);
			drawList = drawList.OrderBy(e => e.drawLayer).ToList(); // wow, that's easy
			foreach (Entity e in drawList) e.Draw(spriteBatch);

			GameState.scene.sceneService.PostDraw(spriteBatch);
			GameDef.gameService.PostDraw(spriteBatch);

			#region debug display
			if (debugDisplay)
			{
				spriteBatch.CameraOn(); // bake

				const int maxDraw = 500;
				int numDraw = 0;
				List<Collider> colOnScreen = CollisionManager.quadTree.GetObjects(GameState.cameraBox.AsRectangle);
				foreach (Collider c in colOnScreen)
				{
					foreach (ColliderShape s in c.shapes)
					{
						s.Draw(spriteBatch);
						numDraw++;
						if (numDraw == maxDraw) break;
					}
					if (numDraw == maxDraw) break;
				}

				spriteBatch.CameraOff();

				string debugText = "Debug display (F3)";
				debugText += "\nEntities: " + GameState.entities.Count;// +" (" + CollisionManager.collidable.Count + " collidable)";
				debugText += "\nDrawing " + numDraw + " collider shapes (out of " + maxDraw + " max)";
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
			}
			#endregion

			BakeToScreen();
			GameState.cameraBoxCache = null;
		}
	}
}
