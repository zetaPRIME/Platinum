using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum
{
	public static class GameState
	{
		public static List<Entity> entities = new List<Entity>();
		public static List<Entity> entityDel = new List<Entity>();

		public static Vector2 worldSize = new Vector2(640, 480);

		//
	}
}
