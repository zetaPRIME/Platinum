using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum
{
	public class UIElement
	{
		public UIElement parent;

		public Rectangle bounds;
		public Point scroll;

		public Rectangle drawBounds
		{
			get
			{
				if (parent == null) return bounds;
				return new Rectangle(bounds.X - parent.scroll.X, bounds.Y - parent.scroll.Y, bounds.Width, bounds.Height);
			}
		}
		public virtual Rectangle innerBounds { get { return new Rectangle(0, 0, bounds.Width, bounds.Height); } }

		public virtual void Refresh() { }
		public virtual void Update() { }
		public virtual void Draw(SpriteBatch sb) { }


		// input
		public virtual void MouseOver(Point point) { }

		public virtual void MouseAction(bool left, bool leftP, bool leftR, bool right, bool rightP, bool rightR) { }
		public virtual void MouseScroll(int clicks) { }

		public virtual void TextInput() { }

		public virtual bool InterceptsMouse { get { return false; } }
		public virtual bool InterceptsScrollwheel { get { return false; } }
	}
}
