﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace Platinum
{
	public static class GameState
	{
		public static List<Entity> entities = new List<Entity>();

		public static Vector2 worldSize = new Vector2(640, 480);

		public static Vector2 cameraPos = Vector2.Zero;
		public static VecRect cameraBox { get { return new VecRect(Vector2.Zero, new Vector2(Core.instance.GraphicsDevice.Viewport.Width, Core.instance.GraphicsDevice.Viewport.Height)) + cameraPos; } }

		public static Entity FindEntity(string name) { return entities.Find(e => e.Name == name); }
		public static List<Entity> FindEntityByTag(string tag) { return entities.FindAll(e => e.HasTag(tag)); }

		public static void SetGameSize(int width, int height)
		{
			Type otkgw = typeof(OpenTKGameWindow);
			FieldInfo wfield = otkgw.GetField("window", BindingFlags.NonPublic | BindingFlags.Instance);
			OpenTK.GameWindow wnd = (OpenTK.GameWindow)wfield.GetValue(Core.instance.Window);

			wnd.Width = width; wnd.Height = height;
		}
	}
}
