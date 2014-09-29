using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using LitJson;

using Platinum.UIKit;

namespace Platinum.Editor
{
	public class SceneDisplay : UIElement
	{
		// colors etc.
		Color colorSelection = new Color(0.8f, 0.9f, 1f, 1f);
		Color colorGrid = Color.White.MultiplyBy(0.5f);

		Color colorLinkParent = Color.Yellow;
		Color colorLinkChild = Color.LightGreen;

		// camera
		public Vector2 cameraPos;
		public float cameraZoom = 1f;
		public float cameraRot = 0f;

		public Matrix cameraTransform
		{
			get
			{
				Vector2 cPos = cameraPos;
				if (bounds.Width % 2 == 1) cPos.X += 0.5f;
				if (bounds.Height % 2 == 1) cPos.Y += 0.5f;
				return Matrix.CreateTranslation(-cPos.Upcast()) * Matrix.CreateRotationZ(cameraRot) * Matrix.CreateScale(cameraZoom) * Matrix.CreateTranslation((bounds.VecSize() * 0.5f).Upcast());
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
		public List<EntityPlacement> hidden = new List<EntityPlacement>();

		//
		public Vector2 mouseScreen { get { return Vector2.Transform(mouseWorld, cameraTransform); } set { mouseWorld = Vector2.Transform(value, cameraTransform.Invert()); } }
		public Vector2 mouseDownScreen { get { return Vector2.Transform(mouseDownWorld, cameraTransform); } set { mouseDownWorld = Vector2.Transform(value, cameraTransform.Invert()); } }
		public Vector2 mouseWorld;// { get { return Vector2.Transform(mouseScreen, cameraTransform.Invert()); } }
		public Vector2 mouseDownWorld;// { get { return Vector2.Transform(mouseDownScreen, cameraTransform.Invert()); } }
		public EntityPlacement mouseOverEntity;
		public override bool InterceptsMouse { get { return true; } }
		public override bool InterceptsScrollwheel { get { return true; } }

		public override void MouseOver(Point point)
		{
			UI.TakeFocus(this);
			mouseScreen = new Vector2(point.X, point.Y);
		}

		public override void MouseAction(bool left, bool leftP, bool leftR, bool right, bool rightP, bool rightR)
		{
			mouseOverEntity = FindClickEntity();
			if (leftP)
			{
				mouseDownScreen = mouseScreen;

				EntityPlacement p = mouseOverEntity;
				if (!Input.KeyHeld(Keys.LeftShift) && !selection.Contains(p)) selection.Clear();
				if (p != null)
				{
					selection.Remove(p);
					selection.Add(p);
					foreach (EntityPlacement s in selection) s.oldPosition = s.Position;
					//p.oldPosition = p.position;
				}
				EditorCore.SetPropertiesFromContext();
			}

			if (left)
			{
				Vector2 curDiff = mouseWorld - mouseDownWorld;
				if (curDiff.Length() > 2) // threshold so you don't accidentally snap
				{
					if (selection.Count > 0)
					{
						EntityPlacement master = selection[selection.Count - 1];
						float gridSize = GameDef.gridSize;
						if (Input.KeyHeld(Keys.LeftShift)) gridSize /= 2;
						master.Position = master.oldPosition + curDiff;
						master.Snap(gridSize);

						foreach (EntityPlacement p in selection)
						{
							if (p == master) continue;
							p.Position = master.Position + (p.oldPosition - master.oldPosition);
						}
					}
				}

				if (selection.Count == 0)
				{
					cameraPos -= curDiff;
					cameraPos = cameraPos.Clamp(new VecRect(Vector2.Zero, GameState.worldSize)).Pixelize();
				}
			}

			if (rightP)
			{
				if (selection.Count == 1)
				{
					EntityPlacement p = selection[0].Copy(mouseWorld);
					p.AddToMap(GameState.scene.currentMap);
					selection[0] = p;
					p.Snap(GameDef.gridSize);
				}
				else if (selection.Count > 1)
				{
					List<EntityPlacement> selc = new List<EntityPlacement>();

					EntityPlacement selMaster = selection[selection.Count - 1];
					EntityPlacement mp = selMaster.Copy(mouseWorld);
					mp.AddToMap(GameState.scene.currentMap);
					mp.Snap(GameDef.gridSize);
					
					foreach (EntityPlacement p in selection)
					{
						if (p == selMaster) continue;
						if (HasParentSelected(p)) continue;
						EntityPlacement np = p.Copy(mp.Position + (p.Position - selMaster.Position));
						np.AddToMap(GameState.scene.currentMap);
						selc.Add(np);
					}

					selection.Clear();
					selection.AddRange(selc);
					selection.Add(mp);
				}

				EditorCore.SetPropertiesFromContext();
			}
		}

		bool HasParentSelected(EntityPlacement p)
		{
			if (p.parent == null) return false;
			if (selection.Contains(p.parent)) return true;
			return HasParentSelected(p.parent);
		}

		void SelectChildren(EntityPlacement p)
		{
			foreach (EntityPlacement c in p.children)
			{
				if (!selection.Contains(c)) selection.Add(c);
				SelectChildren(c);
			}
		}

		public override void MouseScroll(int clicks)
		{
			if (mouseOverEntity == null) return; // have to operate on this, of course!

			if (Input.KeyHeld(Keys.LeftControl))
			{
				int atOnce = 10;
				if (Input.KeyHeld(Keys.LeftShift)) atOnce = 1;
				mouseOverEntity.drawLayer -= clicks * atOnce;
			}
		}

		public EntityPlacement FindClickEntity()
		{
			Vector2 point = mouseWorld;

			List<EntityPlacement> eligible = GameState.scene.currentMap.placements.Where((p) => p.SelectBounds.Contains(point) && !hidden.Contains(p)).OrderBy((p) => p.drawLayer).Reverse().ToList();
			if (eligible.Count == 0) return null;

			return eligible[0];
		}

		public override void Update()
		{
			if (UI.focusMouse != this) mouseOverEntity = null; // mouse isn't over an entity if it's not over this
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

			// ctrl hotkeys
			if (Input.KeyHeld(Keys.LeftControl))
			{
				// save
				if (Input.KeyPressed(Keys.S)) EditorCore.SaveScene();
			}
			else if (UI.focusText == null) // not-ctrl hotkeys
			{
				if (Input.KeyPressed(Keys.Delete)) // delete
				{
					foreach (EntityPlacement p in selection)
					{
						p.Kill(GameState.scene.currentMap);
					}

					EditorCore.SetPropertiesFromContext();
				}

				if (Input.KeyPressed(Keys.L)) // link
				{
					if (Input.KeyHeld(Keys.LeftShift))
					{
						foreach (EntityPlacement p in selection) p.Parent = null;
					}
					else
					{
						if (selection.Count > 1)
						{
							EntityPlacement master = selection[selection.Count - 1];
							foreach (EntityPlacement p in selection)
							{
								if (p == master) continue;
								p.Parent = master;
							}
						}
					}
				}

				if (Input.KeyPressed(Keys.X)) // hide/unhide
				{
					if (Input.KeyHeld(Keys.LeftShift))
					{
						hidden.Clear();
					}
					else if (mouseOverEntity != null)
					{
						selection.Remove(mouseOverEntity);
						hidden.Add(mouseOverEntity);
					}
					EditorCore.SetPropertiesFromContext();
				}

				if (Input.KeyPressed(Keys.C))
				{
					List<EntityPlacement> selc = new List<EntityPlacement>(selection);
					foreach (EntityPlacement s in selc) SelectChildren(s);
				}
			}
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

			int numGrid = (int)Math.Max(cameraBoxCache.Size.X, cameraBoxCache.Size.Y) / GameDef.gridSize;
			numGrid += 2;
			// draw grid
			for (int i = 0; i < numGrid; i++)
			{
				Vector2 gridStart = new Vector2((int)(cameraBoxCache.topLeft.X / GameDef.gridSize) * GameDef.gridSize, (int)(cameraBoxCache.topLeft.Y / GameDef.gridSize) * GameDef.gridSize) - Vector2.One * GameDef.gridSize;
				gridStart += Vector2.One;
				sb.DrawLine(new LineSegment(gridStart + new Vector2(GameDef.gridSize * i, 0), gridStart + new Vector2(GameDef.gridSize * i, 9999)), colorGrid);
				sb.DrawLine(new LineSegment(gridStart + new Vector2(0, GameDef.gridSize * i), gridStart + new Vector2(9999, GameDef.gridSize * i)), colorGrid);

				gridStart -= Vector2.One;
				Color colorGrid2 = colorGrid.MultiplyBy(0.5f);
				sb.DrawLine(new LineSegment(gridStart + new Vector2(GameDef.gridSize * i, 0), gridStart + new Vector2(GameDef.gridSize * i, 9999)), colorGrid2);
				sb.DrawLine(new LineSegment(gridStart + new Vector2(0, GameDef.gridSize * i), gridStart + new Vector2(9999, GameDef.gridSize * i)), colorGrid2);
			}

			// some stuff
			EntityPlacement selMaster = null;
			if (selection.Count > 0) selMaster = selection[selection.Count - 1];

			// draw entities
			List<EntityPlacement> drawList = GameState.scene.currentMap.placements.Where((p) => p.DrawBounds.Intersects(cameraBoxCache)).OrderBy((p) => p.drawLayer).ToList();

			foreach (EntityPlacement p in drawList)
			{
				//sb.DrawRect(p.DrawBounds.AsRectangle, Color.Blue);
				Color color = Color.White;
				if (hidden.Contains(p)) color = Color.White.MultiplyBy(0.1f);
				p.type.editorEntity.DrawInEditor(p, sb, color);
				if (selection.Contains(p))
				{
					VecRect edb = p.DrawBounds;
					sb.DrawRect(edb.AsRectangle, colorSelection.MultiplyBy(0.5f));

					float lineWidth = 1;
					if (p == selMaster && selection.Count > 1) lineWidth = 2;
					//edb = edb.ExpandOut(1.0f);
					sb.DrawLine(new LineSegment(edb.topLeft, edb.topRight), colorSelection, lineWidth);
					sb.DrawLine(new LineSegment(edb.bottomLeft, edb.bottomRight), colorSelection, lineWidth);
					sb.DrawLine(new LineSegment(edb.topLeft, edb.bottomLeft), colorSelection, lineWidth);
					sb.DrawLine(new LineSegment(edb.topRight, edb.bottomRight), colorSelection, lineWidth);
				}
			}

			// draw things relevant to mouseover entity
			EntityPlacement lineEntity = mouseOverEntity;
			if (lineEntity == null) lineEntity = selMaster;
			if (lineEntity != null)
			{
				lineEntity.DrawParentLine(sb, colorLinkParent, 2.5f);
				lineEntity.DrawChildLines(sb, colorLinkChild, 2.5f);
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
