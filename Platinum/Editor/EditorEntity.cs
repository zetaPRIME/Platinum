using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using LitJson;

namespace Platinum.Editor
{
	public class EditorEntity
	{
		public EntityDef def;

		public virtual VecRect DrawBounds { get; set; }
		public virtual VecRect SelectBounds { get; set; }

		public ExtTexture editorIcon;

		public virtual void LoadJson()
		{
			JsonData jr = def.source.def;
			JsonData j;
			if (!jr.Has("editor") || !jr["editor"].IsObject) j = new JsonData(); // still set reasonable defaults
			else j = jr["editor"];

			VecRect drawBounds = VecRect.Radius * 16f;
			VecRect selectBounds = VecRect.Radius * 16f;
			j.Read("drawBounds", ref drawBounds);
			j.Read("selectBounds", ref selectBounds);
			DrawBounds = drawBounds;
			SelectBounds = selectBounds;

			string iconName = "";
			j.Read("icon", ref iconName);
			editorIcon = def.source.GetTexture(iconName);
		}

		public virtual void DrawInEditor(EntityPlacement ep, SpriteBatch sb, Color colorModifier)
		{
			ExtTexture tx = def.mainTexture;
			if (editorIcon != null) tx = editorIcon;
			if (tx == null)
			{
				sb.DrawRect(ep.DrawBounds.AsRectangle, Color.Magenta.MultiplyBy(colorModifier));
				const string str = "NO ?\nICON";
				Vector2 center = UI.Font.MeasureString(str) / 2f;
				sb.DrawString(UI.Font, str, ep.Position.Pixelize(), colorModifier, 0f, center.Pixelize(), 1f, SpriteEffects.None, 0f);
			}
			else sb.Draw(tx, ep.Position.Pixelize(), 0, colorModifier, ep.Rotation, tx.center, 1f, SpriteEffects.None);
		}

		public virtual void DrawIcon(Rectangle rect, SpriteBatch sb)
		{
			ExtTexture tx = def.mainTexture;
			if (editorIcon != null) tx = editorIcon;
			if (tx == null)
			{
				sb.DrawRect(rect, Color.Magenta);
				return;
			}

			Vector2 size = tx.center * 2;
			Vector2 maxSize = new Vector2(rect.Width, rect.Height);
			Vector2 pos = new Vector2(rect.X, rect.Y) + maxSize / 2f;

			float scale = Math.Min(Math.Max(size.X / maxSize.X, size.Y / maxSize.Y), 1);
			sb.Draw(tx, pos.Pixelize(), 0, Color.White, 0f, tx.center, scale, SpriteEffects.None);
		}

		public virtual Vector2 SnapOffset
		{
			get
			{
				if (SelectBounds.Size.X % GameDef.gridSize == 0 && SelectBounds.Size.Y % GameDef.gridSize == 0)
				{
					int tx = (int)SelectBounds.Size.X / GameDef.gridSize;
					int ty = (int)SelectBounds.Size.Y / GameDef.gridSize;
					int gh = GameDef.gridSize / 2;
					int x = 0; int y = 0;
					if (tx % 2 != 0) x = gh;
					if (ty % 2 != 0) y = gh;
					return new Vector2(x, y);
				}
				return Vector2.One * GameDef.gridSize / 2f;
			}
		}

		public virtual void BuildDefaultJson(JsonData j) { }
		public virtual List<UIElement> BuildProperties(EntityPlacement p)
		{
			return null;
		}
	}
}
