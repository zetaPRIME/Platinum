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
			if (!jr.Has("editor") || !jr["editor"].IsObject) return; // yeah
			JsonData j = jr["editor"];

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

		public virtual void DrawInEditor(EntityPlacement ep, SpriteBatch sb)
		{
			ExtTexture tx = def.mainTexture;
			if (editorIcon != null) tx = editorIcon;
			sb.Draw(tx, ep.position, 0, Color.White, 0f, tx.center, 1f, SpriteEffects.None);
		}

		public virtual void DrawIcon(Rectangle rect, SpriteBatch sb)
		{
			ExtTexture tx = def.mainTexture;
			if (editorIcon != null) tx = editorIcon;
			Vector2 size = tx.center * 2;
			Vector2 maxSize = new Vector2(rect.Width, rect.Height);
			Vector2 pos = new Vector2(rect.X, rect.Y) + maxSize / 2f;

			float scale = Math.Min(Math.Max(maxSize.X / size.X, maxSize.Y / size.Y), 1);
			sb.Draw(tx, pos.Pixelize(), 0, Color.White, 0f, tx.center, scale, SpriteEffects.None);
		}
	}
}
