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
	public class Core : Game
	{
		public static Core instance;

		public static SpriteBatch spriteBatch;

		public GraphicsDeviceManager graphics;

		public static SpriteFont fontDebug;

		// flags
		public static bool debugDisplay = false;

		public Core()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content/Native";
		}

		protected override void Initialize()
		{
			573.ToString(); // because why not

			// making sure of a few things
			IsFixedTimeStep = true;

			graphics.PreferredBackBufferWidth = 640;
			graphics.PreferredBackBufferHeight = 480;
			graphics.IsFullScreen = false;

			graphics.ApplyChanges();

			this.IsMouseVisible = true;
			Window.AllowUserResizing = true;

			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

			fontDebug = Content.Load<SpriteFont>("DebugFont");

			Input.players.Add(new PlayerInput());
			Input.players[0].LoadDefaults(0);

			// start loading in content
			PackageManager.FindPackages();

			Package global = PackageManager.LoadPackage("Global", true);
			if (global.assembly != null)
			{
				Type[] types = global.assembly.GetTypes();

				foreach (Type t in types)
				{
					if (t.IsSubclassOf(typeof(GameService)))
					{
						GameDef.gameService = (t.GetConstructor(new Type[0]).Invoke(new object[0])) as GameService;
						break;
					}
				}
			}

		}

		protected override void Update(GameTime gameTime)
		{
			Input.Update();
			#region Debug keys
			if (Input.KeyPressed(Keys.F3)) debugDisplay = !debugDisplay;
			#endregion

			GameDef.gameService.PreUpdate();

			// entities
			List<Entity> entities = new List<Entity>(GameState.entities);

			foreach (Entity e in entities) e.UpdatePhysics();

			Collision.PreUpdate();
			Collision.TestAll();

			foreach (Entity e in entities) e.Update();

			foreach (Entity e in GameState.entityDel)
			{
				e.OnKill();
				GameState.entities.Remove(e);
			}

			// todo: particles

			GameDef.gameService.PostUpdate();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			spriteBatch.GraphicsDevice.SetRenderTarget(null);

			spriteBatch.GraphicsDevice.Clear(new Color(0.5f, 0f, 1f));
			spriteBatch.Begin();

			GameDef.gameService.PreDraw(spriteBatch);

			foreach (Entity e in GameState.entities)
			{
				e.Draw(spriteBatch);
			}

			GameDef.gameService.PostDraw(spriteBatch);

			spriteBatch.End();

			#region debug display
			if (debugDisplay)
			{
				spriteBatch.Begin();

				spriteBatch.DrawString(fontDebug, "Debug display (F3)\nEntities: " + GameState.entities.Count + " (" + Collision.collidable.Count + " collidable)", Vector2.One * 8f, Color.White);

				spriteBatch.End();
			}
			#endregion

			base.Draw(gameTime);
		}
	}
}
