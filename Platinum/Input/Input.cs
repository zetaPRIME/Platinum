using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Microsoft.Xna.Framework.Input;

namespace Platinum
{
	public static class Input
	{
		public static List<PlayerInput> players = new List<PlayerInput>();

		public static KeyboardState keyStateNow;
		public static KeyboardState keyStateLast;

		public static Settings padSettings;

		public static void Init()
		{
			// for some stupid reason you need to bash reflection against MonoGame if you want to configure controllers
			Type gpt = typeof(GamePad);
			MethodInfo minfo = gpt.GetMethod("PrepSettings", BindingFlags.NonPublic | BindingFlags.Static);
			minfo.Invoke(null, new object[] { });
			FieldInfo finfo = gpt.GetField("settings", BindingFlags.NonPublic | BindingFlags.Static);
			padSettings = finfo.GetValue(null) as Settings;

		}

		public static void Update()
		{
			keyStateLast = keyStateNow;
			keyStateNow = Keyboard.GetState();

			foreach (PlayerInput pi in players) pi.Update();
		}

		public static bool KeyPressed(Keys key)
		{
			return keyStateNow.IsKeyDown(key) && (keyStateLast == null || keyStateLast.IsKeyUp(key));
		}
	}

	public enum Button { A, B, X, Y, L, R, Start, Select, Up, Down, Left, Right, END }
}
