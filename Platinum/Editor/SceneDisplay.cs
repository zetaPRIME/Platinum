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
		// colors etc.
		Color colorSelection = new Color(0.8f, 0.9f, 1f, 1f);

		// camera
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
		public Vector2 mouseScreen { get { return Vector2.Transform(mouseWorld, cameraTransform); } set { mouseWorld = Vector2.Transform(value, cameraTransform.Invert()); } }
		public Vector2 mouseDownScreen { get { return Vector2.Transform(mouseDownWorld, cameraTransform); } set { mouseDownWorld = Vector2.Transform(value, cameraTransform.Invert()); } }
		public Vector2 mouseWorld;// { get { return Vector2.Transform(mouseScreen, cameraTransform.Invert()); } }
		public Vector2 mouseDownWorld;// { get { return Vector2.Transform(mouseDownScreen, cameraTransform.Invert()); } }
		public override bool InterceptsMouse { get { return true; } }
		public override bool InterceptsScrollwheel { get { return true; } }

		public override void MouseOver(Point point)
		{
			UI.TakeFocus(this);
			mouseScreen = new Vector2(point.X, point.Y);
		}

		public override void MouseAction(bool left, bool leftP, bool leftR, bool right, bool rightP, bool rightR)
		{
			if (leftP)
			{
				mouseDownScreen = mouseScreen;

				selection.Clear();
				EntityPlacement p = FindClickEntity();
				if (p != null)
				{
					selection.Add(p);
					p.oldPosition = p.position;
				}
			}

			if (left)
			{
				Vector2 curDiff = mouseWorld - mouseDownWorld;
				foreach (EntityPlacement p in selection)
				{
					p.position = p.oldPosition + curDiff;

					float gridSize = GameDef.gridSize;
					Vector2 snap = p.type.editorEntity.SnapOffset;

					p.position += Vector2.One * gridSize / 2;
					p.position += snap;
					p.position = new Vector2((int)(p.position.X / gridSize) * gridSize, (int)(p.position.Y / gridSize) * gridSize);
					p.position -= snap;
					//p.position -= Vector2.One * gridSize;
				}

				if (selection.Count == 0)
				{
					cameraPos -= curDiff;
					cameraPos = cameraPos.Clamp(new VecRect(Vector2.Zero, GameState.worldSize)).Pixelize();
				}
			}
		}

		public EntityPlacement FindClickEntity()
		{
			Vector2 point = mouseWorld;

			List<EntityPlacement> eligible = GameState.scene.currentMap.placements.Where((p) => p.SelectBounds.Contains(point)).OrderBy((p) => p.drawLayer).Reverse().ToList();
			if (eligible.Count == 0) return null;

			return eligible[0];
		}

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
				//sb.DrawRect(p.DrawBounds.AsRectangle, Color.Blue);
				p.type.editorEntity.DrawInEditor(p, sb);
				if (selection.Contains(p))
				{
					Console.WriteLine("blah");
					sb.DrawRect(p.DrawBounds.AsRectangle, colorSelection.MultiplyBy(0.5f));
				}
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
