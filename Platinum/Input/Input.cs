using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;

namespace Platinum
{
	public static class Input
	{
		public static List<PlayerInput> players = new List<PlayerInput>();

		public static KeyboardState keyStateNow;
		public static KeyboardState keyStateLast;

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
