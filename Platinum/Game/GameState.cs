using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Dynamics;

namespace Platinum
{
	public static class GameState
	{
		public static List<Entity> entities = new List<Entity>();
		public static List<Entity> entityDel = new List<Entity>();

		public static Vector2 worldSize = new Vector2(640, 480);

		public static Vector2 cameraPos = Vector2.Zero;
		public static VecRect cameraBox { get { return new VecRect(Vector2.Zero, new Vector2(Core.instance.GraphicsDevice.Viewport.Width, Core.instance.GraphicsDevice.Viewport.Height)) + cameraPos; } }

		public static World physWorld;
	}
}
