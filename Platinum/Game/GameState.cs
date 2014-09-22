using System;
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
		public static float cameraRot = 0f;
		public static float cameraZoom = 1f;
		public static VecRect cameraBox
		{
			get
			{
				if (cameraBoxCache != null) return cameraBoxCache.Value;
				float rot = cameraRot.WrapRot();
				if (rot == 0f) return new VecRect(-GameDef.screenSize / 2f, GameDef.screenSize / 2f) + cameraPos;
				//Matrix cmtx = cameraTransform;
				//return VecRect.FromPoints(Vector2.Transform(GameDef.screenSize.Scale(-0.5f, -0.5f), cmtx), Vector2.Transform(GameDef.screenSize.Scale(0.5f, -0.5f), cmtx), Vector2.Transform(GameDef.screenSize.Scale(0.5f, 0.5f), cmtx), Vector2.Transform(GameDef.screenSize.Scale(-0.5f, 0.5f), cmtx));
				var inverseViewMatrix = Matrix.Invert(cameraTransform);
				var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
				var tr = Vector2.Transform(new Vector2(GameDef.screenSize.X, 0), inverseViewMatrix);
				var bl = Vector2.Transform(new Vector2(0, GameDef.screenSize.Y), inverseViewMatrix);
				var br = Vector2.Transform(GameDef.screenSize, inverseViewMatrix);
				var min = new Vector2(
					MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
					MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
				var max = new Vector2(
					MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
					MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
				return new VecRect(min, max);
			}
		}
		internal static VecRect? cameraBoxCache = null; // for reasons of drawing fasterness
		public static Matrix cameraTransform
		{
			get
			{
				return Matrix.CreateTranslation(-cameraPos.Upcast()) * Matrix.CreateRotationZ(cameraRot) * Matrix.CreateScale(cameraZoom) * Matrix.CreateTranslation((GameDef.screenSize * 0.5f).Upcast());
				//return Matrix.CreateTranslation(cameraPos.Upcast()) * Matrix.CreateTranslation((GameDef.screenSize * -0.5f).Upcast()) * Matrix.CreateRotationZ(cameraRot);
			}
		}

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
