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

		public static string name = "Platinum";

		public static int defaultPixelScale = 2;
		public static int gridSize = 32;

		public static Vector2 screenSize = new Vector2(640, 480);

		public static string defaultScene = "init";
		public static Vector4 defaultBackColorVec;
		public static Color defaultBackColor { get { return new Color(defaultBackColorVec); } set { defaultBackColorVec = value.ToVector4(); } }

		public static void Load()
		{
			JsonData j = PackageManager.globalPackage.def;
			if (j == null) j = new JsonData();

			// defaults
			name = "Platinum";
			defaultPixelScale = 2;
			gridSize = 32;
			screenSize = new Vector2(640, 480);
			defaultScene = "init";
			defaultBackColorVec = new Vector4(0.5f, 0f, 1f, 1f);

			// and load
			j.Read("name", ref name);
			j.Read("defaultPixelScale", ref defaultPixelScale);
			j.Read("gridSize", ref gridSize);
			j.Read("screenSize", ref screenSize);
			j.Read("defaultScene", ref defaultScene);

			j.Read("defaultBackColor", ref defaultBackColorVec);

			// apply things that need to be applied
			GameState.SetGameSize((int)screenSize.X, (int)screenSize.Y);
			Core.instance.Window.Title = name;
		}
	}
}
