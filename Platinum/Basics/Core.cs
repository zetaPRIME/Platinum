using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Fluent.IO;
using Ionic.Zip;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Platinum
{
	public enum EngineMode
	{
		Game, Editor, Config, Exception
	}

	public partial class Core : Game
	{
		public static Core instance;
		//public static EngineMode mode = EngineMode.Game;
		public static EngineMode mode = EngineMode.Editor;

		public static SpriteBatch spriteBatch;
		public GraphicsDeviceManager graphics;

		public static SpriteFont fontDebug;
		public static Texture2D txPixel;

		public static RenderTarget2D screenTarget;

		// for command line things
		public static string forceScene = "";

		// flags
		public static bool debugDisplay = false;

		public DateTime initStart;

		public Core()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content/Native";
		}

		protected override void Initialize()
		{
			initStart = DateTime.Now;

			573.ToString(); // because why not

			// making sure of a few things
			IsFixedTimeStep = true;

			graphics.PreferredBackBufferWidth = 640;
			graphics.PreferredBackBufferHeight = 480;
			graphics.IsFullScreen = false;

			graphics.ApplyChanges();

			this.IsMouseVisible = true;
			Window.AllowUserResizing = !(mode == EngineMode.Game || mode == EngineMode.Exception);

			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

			fontDebug = Content.Load<SpriteFont>("DebugFont");
			txPixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
			txPixel.SetData<Color>(new Color[] { Color.White });

			Input.Init();
			Window.TextInput += Input.OnTextInput;

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

			GameDef.Load();

			CollisionManager.ConfirmTree();
		}

		bool init = false;
		protected override void Update(GameTime gameTime)
		{
			if (!init)
			{
				GameState.SetGameSize((int)GameDef.screenSize.X, (int)GameDef.screenSize.Y);

				if (mode == EngineMode.Game) Init_Game();
				else if (mode == EngineMode.Editor) Init_Editor();

				TimeSpan span = DateTime.Now - initStart;
				Console.WriteLine("Init took " + span.TotalSeconds + " seconds");
				init = true;
			}
			//Console.WriteLine("Resolution is " + Window.ClientBounds.Width + "x" + Window.ClientBounds.Height);

			Input.Update();

			if (mode == EngineMode.Game) Update_Game(gameTime);
			else if (mode == EngineMode.Editor) Update_Editor(gameTime);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			spriteBatch.GraphicsDevice.SetRenderTarget(null);

			if (mode == EngineMode.Game) Draw_Game(gameTime);
			else if (mode == EngineMode.Editor) Draw_Editor(gameTime);

			base.Draw(gameTime);
		}

		internal void PrepareTarget()
		{
			if (screenTarget == null || screenTarget.Bounds != Window.ClientBounds)
			{
				screenTarget = new RenderTarget2D(GraphicsDevice, Window.ClientBounds.Width, Window.ClientBounds.Height, false,
					GraphicsDevice.PresentationParameters.BackBufferFormat, GraphicsDevice.PresentationParameters.DepthStencilFormat, 0, RenderTargetUsage.PreserveContents);
			}
			GraphicsDevice.SetRenderTarget(screenTarget);
		}

		internal void BakeToScreen()
		{
			spriteBatch.End();
			spriteBatch.GraphicsDevice.SetRenderTarget(null);
			spriteBatch.Begin();
			spriteBatch.Draw(screenTarget, Vector2.Zero, Color.White);
			spriteBatch.End();
		}
	}
}
