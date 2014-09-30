using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum.Editor
{
	public class ExplorerEntry : UIElement
	{
		public Predicate<string> pIsSelected;
		public Action<string> actionOnSelect;
		public Action<SpriteBatch, Rectangle, string> actionDraw;

		public string entryName = "";

		public ExplorerEntry(string name, Predicate<string> isSelected, Action<string> onSelect, Action<SpriteBatch, Rectangle, string> draw)
		{
			entryName = name;
			pIsSelected = isSelected;
			actionOnSelect = onSelect;
			actionDraw = draw;

			bounds = new Rectangle(0, 0, 66, 66);
		}

		public ExplorerEntry SetHeight(int height) { bounds.Height = height; return this; }

		public override bool InterceptsMouse { get { return true; } }
		public override void MouseOver(Point point) { UI.TakeFocus(this); }
		bool mouseOver = false;
		bool mouseClick = false;
		public override void MouseAction(bool left, bool leftP, bool leftR, bool right, bool rightP, bool rightR)
		{
			mouseOver = true;
			if (mouseClick && leftR && actionOnSelect != null) actionOnSelect(entryName);
			if (!left) mouseClick = false;
			if (leftP) mouseClick = true;
		}

		public override void Update()
		{
			if (!mouseOver) mouseClick = false;
			mouseOver = false;
		}

		public override void Draw(SpriteBatch sb)
		{
			bool selected = pIsSelected(entryName);
			Rectangle db = drawBounds;

			if (mouseOver || selected)
			{
				Color cBox = Color.LightGray;
				if (selected) cBox = Color.LightBlue;
				if (mouseClick) cBox = cBox.MultiplyBy(0.75f);

				sb.DrawRect(db.MarginTop(1), cBox);
				sb.DrawRect(db.MarginBottom(1), cBox);
				sb.DrawRect(db.MarginLeft(1), cBox);
				sb.DrawRect(db.MarginRight(1), cBox);

				sb.DrawRect(db, cBox.MultiplyBy(0.5f));
			}

			if (actionDraw != null) actionDraw(sb, db, entryName);
		}
	}
}
