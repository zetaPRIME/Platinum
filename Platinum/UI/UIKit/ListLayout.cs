using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum.UIKit
{
	public class ListLayout : UIElement
	{
		public int marginX = 8;
		public int marginY = 0;
		public int gap = 2;

		public bool autoExpand = true;
		public bool snapToTop = true;

		public List<UIElement> children = new List<UIElement>();
		public void AddElement(UIElement element) { children.Add(element); element.parent = this; UpdateLayout(); }

		Rectangle oldBounds;
		void Upkeep()
		{
			if (parent != null) scroll = parent.scroll;
			if (bounds != oldBounds)
			{
				oldBounds = bounds;
				UpdateLayout();
			}
		}

		public override void MouseOver(Point point)
		{
			Upkeep();
			point -= scroll; // bugderp?
			foreach (UIElement e in children) if (e.drawBounds.Contains(point)) e.MouseOver(e.drawBounds.PointWithin(point));
		}

		public override void Refresh()
		{
			UpdateLayout();
		}

		void UpdateLayout()
		{
			int level = marginY;

			if (parent != null)
			{
				if (snapToTop) bounds.Y = parent.innerBounds.Y;

				bounds.X = parent.innerBounds.X;
				bounds.Width = parent.innerBounds.Width;
			}

			foreach (UIElement e in children)
			{
				e.bounds.Y = bounds.Y + level;
				level = e.bounds.Bottom + gap;
				if (autoExpand)
				{
					e.bounds.X = marginX;
					e.bounds.Width = bounds.Width - (marginX * 2);
				}
			}

			bounds.Height = (level - gap) + marginY;

			if (parent != null) parent.Refresh();
		}

		public override void Update()
		{
			Upkeep();
			//UpdateLayout();
			foreach (UIElement e in children) e.Update();
		}

		public override void Draw(SpriteBatch sb)
		{
			Upkeep();
			Rectangle dBounds = bounds;
			if (parent != null) dBounds = parent.innerBounds;
			foreach (UIElement e in children) if (e.drawBounds.Intersects(dBounds)) e.Draw(sb);
		}
	}
}
