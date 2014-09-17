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

		public static void Update()
		{
			foreach (PlayerInput pi in players) pi.Update();
		}
	}

	public enum Button { A, B, X, Y, L, R, Start, Select, Up, Down, Left, Right, END }
}
