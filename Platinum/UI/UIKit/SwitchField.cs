using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum.UIKit
{
	public class SwitchField : UIElement
	{
		public bool autoSetBounds = true;
		public int currentPage = 0;

		public List<UIElement> pages = new List<UIElement>();

		public void AddPage(UIElement page)
		{
			pages.Add(page);
			page.parent = this;
		}

		void Upkeep()
		{
			if (currentPage >= pages.Count) currentPage = pages.Count - 1;
			if (autoSetBounds) pages[currentPage].bounds = this.bounds;
		}

		public override void Update()
		{
			Upkeep();
			pages[currentPage].Update();
		}

		public override void Draw(SpriteBatch sb)
		{
			Upkeep();
			pages[currentPage].Draw(sb);
		}

		public override void MouseOver(Point point)
		{
			Upkeep();
			//if (pages[currentPage].bounds.Contains(point)) pages[currentPage].MouseOver(pages[currentPage].bounds.PointWithin(point));
			pages[currentPage].MouseOver(point);
		}
	}
}
