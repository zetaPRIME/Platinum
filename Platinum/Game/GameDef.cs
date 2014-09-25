using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using LitJson;

namespace Platinum
{
	public static class GameDef
	{
		public static GameService gameService = new GameService();

		public static int defaultPixelScale = 2;
		public static int gridSize = 32;

		public static Vector2 screenSize = new Vector2(640, 480);

		public static string defaultScene = "init";

		public static void Load()
		{
			JsonData j = PackageManager.globalPackage.def;
			if (j == null) j = new JsonData();

			// defaults
			defaultPixelScale = 2;
			gridSize = 32;
			screenSize = new Vector2(640, 480);
			defaultScene = "init";

			// and load
			j.Read("defaultPixelScale", ref defaultPixelScale);
			j.Read("gridSize", ref gridSize);
			j.Read("screenSize", ref screenSize);
			j.Read("defaultScene", ref defaultScene);

			// apply things that need to be applied
			GameState.SetGameSize((int)screenSize.X, (int)screenSize.Y);
		}
	}
}
