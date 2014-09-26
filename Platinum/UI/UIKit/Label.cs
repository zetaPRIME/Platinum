using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum.UIKit
{
	public class Label : UIElement
	{
		public string text = "";
		string oldText = "";

		const int marginSides = 8;

		public override void Update()
		{
			if (text != oldText)
			{
				oldText = text;
				Vector2 size = UI.Font.MeasureString(text);

				bounds.Height = (int)size.Y;
			}
		}

		public override void Draw(SpriteBatch sb)
		{
			Rectangle db = drawBounds;
			sb.DrawString(UI.Font, text, new Vector2(db.X + marginSides, db.Y), UI.colorText);
		}
	}
}
