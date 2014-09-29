using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum.UIKit
{
	public class Separator : UIElement
	{
		public Separator()
		{
			bounds = new Rectangle(0, 0, 8, 2);
		}

		public override void Draw(SpriteBatch sb)
		{
			Rectangle db = drawBounds;
			db.Inflate(-3, 0);
			sb.DrawRect(db, UI.colorBacking);
		}
	}
}
