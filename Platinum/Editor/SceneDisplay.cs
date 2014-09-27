using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Platinum.UIKit;

namespace Platinum.Editor
{
	public class SceneDisplay : UIElement
	{
		public Vector2 cameraPos;
		public float cameraZoom = 1f;
		public float cameraRot = 0f;

		public Matrix cameraTransform
		{
			get
			{
				return Matrix.CreateTranslation(-cameraPos.Upcast()) * Matrix.CreateRotationZ(cameraRot) * Matrix.CreateScale(cameraZoom) * Matrix.CreateTranslation((bounds.VecSize() * 0.5f).Upcast());
				//return Matrix.CreateTranslation(cameraPos.Upcast()) * Matrix.CreateTranslation((GameDef.screenSize * -0.5f).Upcast()) * Matrix.CreateRotationZ(cameraRot);
			}
		}
		public VecRect cameraBox
		{
			get
			{
				Vector2 blah = bounds.VecSize() / 2f;
				return new VecRect(-blah, blah) + cameraPos;
			}
		}

		//
		public List<EntityPlacement> selection = new List<EntityPlacement>();

		//
		public override bool InterceptsMouse { get { return true; } }
		public override bool InterceptsScrollwheel { get { return true; } }

		public override void Update()
		{
			if (UI.focusText == null && !Input.KeyHeld(Keys.LeftControl))
			{
				float moveSpeed = 8f;
				if (Input.KeyHeld(Keys.LeftShift)) moveSpeed *= 4f;
				if (Input.KeyHeld(Keys.W)) cameraPos += Vector2.UnitY * -moveSpeed;
				if (Input.KeyHeld(Keys.S)) cameraPos += Vector2.UnitY * moveSpeed;
				if (Input.KeyHeld(Keys.A)) cameraPos += Vector2.UnitX * -moveSpeed;
				if (Input.KeyHeld(Keys.D)) cameraPos += Vector2.UnitX * moveSpeed;
			}
			cameraPos = cameraPos.Clamp(new VecRect(Vector2.Zero, GameState.worldSize)).Pixelize();
		}

		RenderTarget2D target;
		public override void Draw(SpriteBatch sb)
		{
			Rectangle db = drawBounds;
			Rectangle tbounds = db;
			if (target == null || target.Width != tbounds.Width || target.Height != tbounds.Height)
			{
				PresentationParameters pp = sb.GraphicsDevice.PresentationParameters;
				target = new RenderTarget2D(sb.GraphicsDevice, tbounds.Width, tbounds.Height, false, pp.BackBufferFormat, pp.DepthStencilFormat, 0, RenderTargetUsage.PreserveContents);
			}

			// initialize target drawing and draw backcolor
			UI.TargetPush(target);
			sb.End();
			sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, cameraTransform);

			sb.GraphicsDevice.Clear(GameState.backColor);

			VecRect cameraBoxCache = cameraBox;
			List<EntityPlacement> drawList = GameState.scene.currentMap.placements.Where((p) => p.DrawBounds.Intersects(cameraBoxCache)).OrderBy((p) => p.drawLayer).ToList();

			foreach (EntityPlacement p in drawList)
			{
				sb.DrawRect(p.DrawBounds.AsRectangle, Color.Blue);
			}

			// draw bounds
			const int buffer = 1024;
			Color colorOutBounds = new Color(0f, 0f, 0f, 0.5f);
			sb.DrawRect(new Rectangle(-buffer, -buffer, (int)GameState.worldSize.X + (buffer * 2), buffer), colorOutBounds);
			sb.DrawRect(new Rectangle(-buffer, (int)GameState.worldSize.Y, (int)GameState.worldSize.X + (buffer * 2), buffer), colorOutBounds);
			sb.DrawRect(new Rectangle(-buffer, 0, buffer, (int)GameState.worldSize.Y), colorOutBounds);
			sb.DrawRect(new Rectangle((int)GameState.worldSize.X, 0, buffer, (int)GameState.worldSize.Y), colorOutBounds);

			// finish up
			UI.TargetPop();
			sb.Draw(target, drawBounds, Color.White);
		}
	}
}
