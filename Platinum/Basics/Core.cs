using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fluent.IO;
using Ionic.Zip;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Dynamics;
using FarseerPhysics.DebugView;
using FarseerPhysics;
using FarseerPhysics.Factories;

namespace Platinum
{
	public class Core : Game
	{
		public static Core instance;

		public static SpriteBatch spriteBatch;

		public GraphicsDeviceManager graphics;

		public static SpriteFont fontDebug;

		DebugViewXNA debugView;

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

			//spriteBatch = new SpriteBatch(this.GraphicsDevice);

			this.IsMouseVisible = true;
			Window.AllowUserResizing = true;

			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

			GameState.physWorld = new World(new Vector2(0, 0));
			GameState.physWorld.Enabled = true;

			fontDebug = Content.Load<SpriteFont>(/*"Font"*/"DebugFont");

			debugView = new DebugViewXNA(GameState.physWorld);
			debugView.LoadContent(this.GraphicsDevice, this.Content);
			debugView.AppendFlags(DebugViewFlags.Shape);
			debugView.AppendFlags(DebugViewFlags.PolygonPoints);

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
			GameDef.gameService.PreUpdate();

			GameState.physWorld.Step(1f / 60f);

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

			// particles

			GameDef.gameService.PostUpdate();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			//if (spriteBatch == null) return; // why would it even get here? seriously

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

			spriteBatch.Begin();

			Matrix proj = Matrix.CreateOrthographicOffCenter(0f, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
			Vector2 vcenter = new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f);
			Matrix view = Matrix.CreateTranslation(new Vector3((GameState.cameraPos) - (vcenter), 0f)) * Matrix.CreateTranslation(new Vector3((vcenter), 0f));
			debugView.RenderDebugData(ref proj, ref view);

			spriteBatch.End();
			spriteBatch.Begin();

			spriteBatch.DrawString(fontDebug, "Debug menu (F3 to close)\nEntities: " + GameState.entities.Count + " (" + Collision.collidable.Count + " collidable)", Vector2.One * 8f, Color.White);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
