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
	public enum EngineMode
	{
		Game, Editor, Config, Exception
	}

	public partial class Core : Game
	{
		public static Core instance;
		public static EngineMode mode = EngineMode.Game;

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

			Input.Init();

			Console.WriteLine("A ID: " + Input.padSettings[0].Button_A.ID);
			Console.WriteLine("B ID: " + Input.padSettings[0].Button_B.ID);

			Input.players.Add(new PlayerInput());
			Input.players[0].LoadDefaults(0);
			Console.WriteLine("" + GamePad.GetCapabilities(PlayerIndex.Two).IsConnected);

			PadConfig pc = Input.padSettings[0];
			pc.Button_A.ID = 1;
			pc.Button_B.ID = 5;
			pc.Button_X.ID = 0;
			pc.Button_Y.ID = 4;
			pc.Button_Start.ID = 3;
			pc.Button_Back.ID = 2;
			pc.Button_LB.ID = 6;
			pc.Button_RB.ID = 7;

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
			
			if (mode == EngineMode.Game) Update_Game(gameTime);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			spriteBatch.GraphicsDevice.SetRenderTarget(null);

			if (mode == EngineMode.Game) Draw_Game(gameTime);

			base.Draw(gameTime);
		}
	}
}
